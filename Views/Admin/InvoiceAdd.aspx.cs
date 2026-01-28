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
    public partial class InvoiceAdd : System.Web.UI.Page
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
                if (Request.QueryString["InvoiceID"] != null)
                {
                    int InvoiceID = int.Parse(Request.QueryString["InvoiceID"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteStudentData(InvoiceID);
                    }
                    else
                    {
                        LoadInvoiceData(InvoiceID);
                        // Load the student data if needed
                    }
                }


            }

        }

        protected void SetButtonText()
        {
            if (Request.QueryString["InvoiceID"] != null)
            {
                btnSubmit.Text = "Update";
            }
            else
            {
                btnSubmit.Text = "Add";
            }
        }
        private void DeleteStudentData(int InvoiceID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM StudentInvoice WHERE InvoiceID = @InvoiceID", Con);
                cmd.Parameters.AddWithValue("@InvoiceID", InvoiceID);
                cmd.ExecuteNonQuery();
                // Set a query parameter to indicate successful deletion
                Response.Redirect("Invoices.aspx?deleteSuccess=true");
            }

            // Redirect back to the students page after deletion
            Response.Redirect("Invoices.aspx");
        }

        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string StudentQry = @"SELECT 0 as StudentId,'---- Select Student Details -----' AS  Student, 0 as EnrollmentID UNION Select E.StudentID,S.FirstName + ' ' + S.LastName + ' '+S.StudentNo + ' ('+C.ClassName + ' )' as Student,EnrollmentID from Enrollment E
                                    INNER JOIN Student S on E.StudentId=S.StudentID
                                    INNER JOIN Term T on E.Termid=T.TermId
                                    INNER JOIN Class C on E.ClassId=C.ClassId
                                    INNER JOIN TermNumber TN on T.Term=TN.TermId
                                    INNER JOIN FinancialYear F on T.Yearid=F.FinancialYearid
                                    
                                    Where T.Status=2 and E.SchoolId=@SchoolId order by Student";


                string FeesNametQry = @"SELECT 0 as FeesId,'---- Select Fees Details-----' AS  Fees UNION SELECT FeesId, FeesName + ' ' +  'MK'+FORMAT(Amount, 'N0') AS Fees FROM FeesConfiguration WHERE Status = 2 and SchoolId=@SchoolId";


                string TermtQry = @"Select T.TermId, TN.TermNumber + ' ('+F.FinancialYear + ')' As Term  from Term T
                INNER JOIN TermNumber TN on T.Term=TN.TermId
                INNER JOIN FinancialYear F on T.Yearid=F.FinancialYearid where T.Status=2 and T.SchoolId=@SchoolId";


                Con.Open();

                PopulateDropDownList(Con, StudentQry, ddlStudent, "Student", "StudentId");
                PopulateDropDownList(Con, FeesNametQry, ddlFeesName, "Fees", "FeesId");
                PopulateDropDownList(Con, TermtQry, ddlTerm, "Term", "TermId");
            }
        }

        private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
            SqlDataReader dr = cmd.ExecuteReader();
            ddl.DataSource = dr;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            dr.Close();
        }

        private void LoadInvoiceData(int InvoiceID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM StudentInvoice WHERE InvoiceID = @InvoiceID", Con);
                cmd.Parameters.AddWithValue("@InvoiceID", InvoiceID);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        // Debug: Check values being retrieved
                        var studentId = dr["StudentId"].ToString();
                        var feesId = dr["FeesId"].ToString();
                        var termId = dr["TermId"].ToString();

                        // Set values to dropdown lists
                        ddlStudent.SelectedValue = studentId;
                        ddlFeesName.SelectedValue = feesId;
                        ddlTerm.SelectedValue = termId;

                        // Debug: Log or break to verify values
                        System.Diagnostics.Debug.WriteLine($"StudentId: {studentId}, feesId: {feesId}, TermId: {termId}");

                        // Handle other fields if any
                    }
                }
                else
                {
                    // Debug: Log if no rows found
                    System.Diagnostics.Debug.WriteLine("No rows found for the given InvoiceID.");
                }

                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            if (Request.QueryString["InvoiceID"] != null)
            {
                int InvoiceID = int.Parse(Request.QueryString["InvoiceID"]);
                UpdateStudentData(InvoiceID);
                ClearFormFields();
            }
            else
            {
                AddNewInvoice();

            }
        }


        private void AddNewInvoice()
        {
            try
            {
                // Check if the user has selected a value from each dropdown
                if (ddlStudent.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select Student.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlFeesName.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select Fees Category.";
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

                    // First, retrieve the EnrollmentId using StudentId
                    string getEnrollmentQuery = "SELECT EnrollmentId FROM dbo.Enrollment  WHERE StudentId = @StudentId";
                    SqlCommand getEnrollmentCmd = new SqlCommand(getEnrollmentQuery, Con);
                    getEnrollmentCmd.Parameters.AddWithValue("@StudentId", ddlStudent.SelectedValue);
                    object enrollmentIdObj = getEnrollmentCmd.ExecuteScalar();

                    if (enrollmentIdObj != null)
                    {
                        string enrollmentId = enrollmentIdObj.ToString();

                        // Now, you can use this EnrollmentId in your INSERT statement
                        string query = @"
                                 INSERT INTO dbo.StudentInvoice
                                 (InvoiceNo, StudentId, FeesId, SchoolId,
                                  PaidStatus, CreatedBy, EnrollmentId, TermId)
                                 VALUES
                                 (@InvoiceNo, @StudentId, @FeesId, @SchoolId,
                                  @PaidStatus, @CreatedBy, @EnrollmentId, @TermId)";

                        SqlCommand cmd = new SqlCommand(query, Con);
                        cmd.Parameters.AddWithValue("@InvoiceNo", "STU/" + ddlStudent.SelectedValue + "." + ddlFeesName.SelectedValue + "." + ddlTerm.SelectedValue + "/" + Session["SchoolCode"]);
                        cmd.Parameters.AddWithValue("@StudentId", ddlStudent.SelectedValue);
                        cmd.Parameters.AddWithValue("@FeesId", ddlFeesName.SelectedValue);
                        cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                        cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                        cmd.Parameters.AddWithValue("@PaidStatus", "NOT PAID");
                        cmd.Parameters.AddWithValue("@EnrollmentId", enrollmentId);

                        cmd.ExecuteNonQuery();

                        ClearFormFields();
                        lblMessage.Text = "Invoice Generated successfully!";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                    }
                    else
                    {
                        lblMessage.Text = "EnrollmentId not found for the selected StudentId.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // SQL error number for unique key violation
                {
                    ErrorMessage.Text = "Duplicate Entry: A student with the same enrollment details already exists.";
                }
                else
                {
                    ErrorMessage.Text = $"An error occurred while enrolling the student. SQL Error: {ex.Number}. Please try again.";
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }


        private void UpdateStudentData(int InvoiceID)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = "UPDATE StudentInvoice SET FeesId = @FeesId " +
                                   "WHERE InvoiceID = @InvoiceID";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@FeesId", ddlFeesName.SelectedValue);
                    cmd.Parameters.AddWithValue("@InvoiceID", InvoiceID);

                    // Debug: Check values being passed to the query
                    System.Diagnostics.Debug.WriteLine($"FeesId: {ddlFeesName.SelectedValue}, InvoiceID: {InvoiceID}");

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ClearFormFields();
                        SetButtonText();
                        lblMessage.Text = "Invoice updated successfully!";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                    }
                    else
                    {
                        ErrorMessage.Text = "No record was updated. Please check the InvoiceID.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                }
            }
            catch (SqlException ex)
            {
                // Detailed error logging
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Number} - {ex.Message}");
                ErrorMessage.Text = "An error occurred while updating Invoice. Please try again.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }


        private void ClearFormFields()
        {
            ddlStudent.SelectedIndex = 0;
            ddlTerm.SelectedIndex = 0;
            ddlFeesName.SelectedIndex = 0;
        }
    }
}
