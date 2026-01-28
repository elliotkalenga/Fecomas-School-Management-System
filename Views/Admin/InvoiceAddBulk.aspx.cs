using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SMSWEBAPP.Views.Admin.Enrollment;

namespace SMSWEBAPP.Views.Admin
{
    public partial class InvoiceAddBulk : System.Web.UI.Page
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
                string ClassQry = @"SELECT 0 as ClassId,'---- Select  Class-----' AS  ClassName
                                    UNION Select ClassId,ClassNAme  from Class where SchoolId=@SchoolId
                                    Order by ClassName";

                string FeesNametQry = @"SELECT 0 as FeesId,'---- Select Fees Details-----' AS  Fees UNION SELECT FeesId, FeesName + ' ' +  'MK'+FORMAT(Amount, 'N0') AS Fees FROM FeesConfiguration WHERE Status = 2 and SchoolId=@SchoolId";


                string TermtQry = @"Select T.TermId, TN.TermNumber + ' ('+F.FinancialYear + ')' As Term  from Term T
                INNER JOIN TermNumber TN on T.Term=TN.TermId
                INNER JOIN FinancialYear F on T.Yearid=F.FinancialYearid where T.Status=2 and T.SchoolId=@SchoolId";


                Con.Open();

                PopulateDropDownList(Con, ClassQry, ddlClass, "ClassName", "ClassId");
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
                        ddlClass.SelectedValue = studentId;
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
                AddBulkInvoices();

            }
        }


        private void AddBulkInvoices()
        {
            try
            {
                // Check if the user has selected a value from each dropdown
                if (ddlClass.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select a Class.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlFeesName.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select a Fees Category.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlTerm.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select a Term.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();

                    // Retrieve all students in the selected class
                    string getStudentsQuery = @"SELECT StudentId FROM dbo.Enrollment E
                                        INNER JOIN Term T ON E.Termid = T.TermId
                                        WHERE ClassId = @ClassId AND T.Status = 2";
                    SqlCommand getStudentsCmd = new SqlCommand(getStudentsQuery, Con);
                    getStudentsCmd.Parameters.AddWithValue("@ClassId", ddlClass.SelectedValue);
                    SqlDataReader reader = getStudentsCmd.ExecuteReader();

                    List<string> studentIds = new List<string>();
                    while (reader.Read())
                    {
                        studentIds.Add(reader["StudentId"].ToString());
                    }
                    reader.Close();

                    if (studentIds.Count == 0)
                    {
                        ErrorMessage.Text = "There are no students in the selected class. Please enroll students in this class before generating invoices.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                        return;
                    }

                    // Loop through each student and generate an invoice
                    foreach (string studentId in studentIds)
                    {
                        // Retrieve the EnrollmentId using StudentId
                        string getEnrollmentQuery = @"SELECT EnrollmentId FROM dbo.Enrollment E
                                              INNER JOIN Term T ON E.Termid = T.TermId 
                                              WHERE StudentId = @StudentId AND T.Status = 2";
                        SqlCommand getEnrollmentCmd = new SqlCommand(getEnrollmentQuery, Con);
                        getEnrollmentCmd.Parameters.AddWithValue("@StudentId", studentId);
                        object enrollmentIdObj = getEnrollmentCmd.ExecuteScalar();

                        if (enrollmentIdObj != null)
                        {
                            string enrollmentId = enrollmentIdObj.ToString();

                            // Check if the record already exists
                            string checkExistingQuery = @"SELECT COUNT(1) 
                                                  FROM dbo.StudentInvoice 
                                                  WHERE EnrollmentId = @EnrollmentId 
                                                  AND FeesId = @FeesId 
                                                  AND TermId = @TermId";

                            SqlCommand checkExistingCmd = new SqlCommand(checkExistingQuery, Con);
                            checkExistingCmd.Parameters.AddWithValue("@EnrollmentId", enrollmentId);
                            checkExistingCmd.Parameters.AddWithValue("@FeesId", ddlFeesName.SelectedValue);
                            checkExistingCmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                            int recordCount = (int)checkExistingCmd.ExecuteScalar();

                            if (recordCount == 0)
                            {
                                // Use this EnrollmentId in your INSERT statement
                                string query = @"
                            INSERT INTO dbo.StudentInvoice
                            (InvoiceNo, StudentId, FeesId, SchoolId,
                             PaidStatus, CreatedBy, EnrollmentId, TermId)
                            VALUES
                            (@InvoiceNo, @StudentId, @FeesId, @SchoolId,
                             @PaidStatus, @CreatedBy, @EnrollmentId, @TermId)";

                                SqlCommand cmd = new SqlCommand(query, Con);
                                cmd.Parameters.AddWithValue("@InvoiceNo", "STU/" + studentId + "/" + ddlFeesName.SelectedValue + "/" + ddlTerm.SelectedValue + "/" + Session["SchoolCode"]);
                                cmd.Parameters.AddWithValue("@StudentId", studentId);
                                cmd.Parameters.AddWithValue("@FeesId", ddlFeesName.SelectedValue);
                                cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                                cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                                cmd.Parameters.AddWithValue("@PaidStatus", "NOT PAID");
                                cmd.Parameters.AddWithValue("@EnrollmentId", enrollmentId);

                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                // If record exists, log a message or handle accordingly
                                lblMessage.Text = $"Duplicate Entry: A student with the same enrollment details already exists for StudentId: {studentId}.";
                                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                            }
                        }
                        else
                        {
                            lblMessage.Text = $"EnrollmentId not found for the StudentId: {studentId}.";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                        }
                    }

                    lblMessage.Text = "Invoices generated successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
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
                    ErrorMessage.Text = $"An error occurred while generating invoices. SQL Error: {ex.Number}. Please try again.";
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
            ddlClass.SelectedIndex = 0;
            ddlTerm.SelectedIndex = 0;
            ddlFeesName.SelectedIndex = 0;
        }
    }
}
