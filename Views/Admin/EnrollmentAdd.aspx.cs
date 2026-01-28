using SMSWEBAPP.DAL;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SMSWEBAPP.Views.Admin.Enrollment;

namespace SMSWEBAPP.Views.Admin
{
    public partial class EnrollmentAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           if (Session["User"] == null)
           {
               // Redirect to login page
               Response.Redirect("UserLogin.aspx");
           }


            if (!IsPostBack)
            {
                SetButtonText();
                PopulateDropDownLists();
                if (Request.QueryString["EnrollmentID"] != null)
                {
                    int EnrollmentID = int.Parse(Request.QueryString["EnrollmentID"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteStudentData(EnrollmentID);
                    }
                    else
                    {
                        LoadStudentData(EnrollmentID);
                        // Load the student data if needed
                    }
                }


            }

        }

        protected void SetButtonText()
        {
            if (Request.QueryString["EnrollmentID"] != null)
            {
                btnSubmit.Text = "Update";
            }
            else
            {
                btnSubmit.Text = "Add";
            }
        }
        private void DeleteStudentData(int EnrollmentID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Enrollment WHERE EnrollmentID = @EnrollmentID", Con);
                cmd.Parameters.AddWithValue("@EnrollmentID", EnrollmentID);
                cmd.ExecuteNonQuery();
                // Set a query parameter to indicate successful deletion
                Response.Redirect("Enrollment.aspx?deleteSuccess=true");
            }

            // Redirect back to the students page after deletion
            Response.Redirect("Enrollment.aspx");
        }

        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string StudentQry = @"SELECT 0 as StudentId,'---- Select Student-----' AS  Student
                                        UNION 
                                        Select StudentId, FirstName+ ' ' +LastName+ ' ' + StudentNo as Student from Student Where Status=2 and School=@SchoolId";

                
                    string ClasstQry = @"SELECT 0 as ClassId,'---- Select  Class-----' AS  ClassName
                                    UNION Select ClassId,ClassNAme  from Class where SchoolId=@SchoolId
                                    Order by ClassName";

                string ClasstStreamQry = @"SELECT 0 as StreamId,'---- Select  Stream-----' AS  StreamName
                                    UNION Select StreamId,StreamName from ClassStream where SchoolId=@SchoolId
                                    Order by StreamName";

                string TermtQry = @"Select T.TermId, TN.TermNumber + ' ('+F.FinancialYear + ')' As Term  from Term T
                INNER JOIN TermNumber TN on T.Term=TN.TermId
                INNER JOIN FinancialYear F on T.Yearid=F.FinancialYearid where T.Status=2 and T.SchoolId=@SchoolId";


                Con.Open();

                PopulateDropDownList(Con, StudentQry, ddlStudent, "Student", "StudentId");
                PopulateDropDownList(Con, ClasstStreamQry, ddlStream, "StreamName", "StreamId");
                PopulateDropDownList(Con, ClasstQry, ddlClass, "ClassName", "ClassId");
                PopulateDropDownList(Con, TermtQry, ddlTerm, "Term", "TermId");
            }
        }

        private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@SchoolId",Session["SchoolId"]);
            SqlDataReader dr = cmd.ExecuteReader();
            ddl.DataSource = dr;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            dr.Close();
        }

        private void LoadStudentData(int EnrollmentID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Enrollment WHERE EnrollmentID = @EnrollmentID", Con);
                cmd.Parameters.AddWithValue("@EnrollmentID", EnrollmentID);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        // Debug: Check values being retrieved
                        var studentId = dr["StudentId"].ToString();
                        var classId = dr["ClassId"].ToString();
                        var termId = dr["TermId"].ToString();
                        var streamId = dr["StreamId"].ToString();

                        // Set values to dropdown lists
                        ddlStudent.SelectedValue = studentId;
                        ddlClass.SelectedValue = classId;
                        ddlTerm.SelectedValue = termId;
                        ddlStream.SelectedValue = streamId;

                        // Debug: Log or break to verify values
                        System.Diagnostics.Debug.WriteLine($"StudentId: {studentId}, ClassId: {classId}, TermId: {termId}, StreamId: {streamId}");

                        // Handle other fields if any
                    }
                }
                else
                {
                    // Debug: Log if no rows found
                    System.Diagnostics.Debug.WriteLine("No rows found for the given EnrollmentID.");
                }

                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            if (Request.QueryString["EnrollmentID"] != null)
            {
                int EnrollmentID = int.Parse(Request.QueryString["EnrollmentID"]);
                UpdateStudentData(EnrollmentID);
                ClearFormFields();
            }
            else
            {
                AddNewStudent();

            }
        }


        private void AddNewStudent()
        {
            try
            {
                // Check if the user has selected a value from each dropdown
                if (ddlStudent.SelectedValue == "0")
                {
                     ErrorMessage.Text = "Please select a student.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlClass.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select a class.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }
                if (ddlStream.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select a Stream.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlTerm.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select a term.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = "INSERT INTO Enrollment (StudentId, ClassId, TermId, CreatedBy, SchoolId,StreamId) " +
                                   "VALUES (@StudentId, @ClassId, @TermId, @CreatedBy, @SchoolId,@StreamId)";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@StudentId", ddlStudent.SelectedValue);
                    cmd.Parameters.AddWithValue("@ClassId", ddlClass.SelectedValue);
                    cmd.Parameters.AddWithValue("@StreamId", ddlStream.SelectedValue);
                    cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    cmd.ExecuteNonQuery();

                    ClearFormFields();
                    lblMessage.Text = "Student Enrolled successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // SQL error number for unique key violation
                {
                    ErrorMessage.Text = "A student with the same details has already been enrolled.";
                }
                else
                {
                    ErrorMessage.Text = "An error occurred while enrolling the student. Please try again.";
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }


        private void UpdateStudentData(int EnrollmentID)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = "UPDATE Enrollment SET ClassId = @ClassId, StreamId = @StreamId WHERE EnrollmentID = @EnrollmentID";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@ClassId", ddlClass.SelectedValue);
                    cmd.Parameters.AddWithValue("@StreamId", ddlStream.SelectedValue);
                    cmd.Parameters.AddWithValue("@EnrollmentID", EnrollmentID);

                    // Debug: Check values being passed to the query
                    System.Diagnostics.Debug.WriteLine($"ClassId: {ddlClass.SelectedValue}, StreamId: {ddlStream.SelectedValue}, EnrollmentID: {EnrollmentID}");

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ClearFormFields();
                        SetButtonText();
                        lblMessage.Text = "Student Enrollment updated successfully!";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                    }
                    else
                    {
                        ErrorMessage.Text = "No record was updated. Please check the EnrollmentID.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                }
            }
            catch (SqlException ex)
            {
                // Detailed error logging
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Number} - {ex.Message}");
                ErrorMessage.Text = $"An error occurred while updating the student enrollment: {ex.Message}";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void ClearFormFields()
        {
            ddlStudent.SelectedIndex = 0;
            ddlTerm.SelectedIndex = 0;
            ddlClass.SelectedIndex = 0;
        }
    }
}
