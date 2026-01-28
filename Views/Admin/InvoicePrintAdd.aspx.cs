using Microsoft.Reporting.WebForms;
using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SMSWEBAPP.Views.Admin.Enrollment;
using static SMSWEBAPP.Views.Admin.FeesCollection;

namespace SMSWEBAPP.Views.Admin
{
    public partial class InvoicePrintAdd : System.Web.UI.Page
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
                SetUserPermissions();


                if (Request.QueryString["StudentNo"] != null)
                {
                    string StudentNo = Request.QueryString["StudentNo"]; // No need to parse as int if it's a string
                    string mode = Request.QueryString["mode"];

                    if (mode == "delete")
                    {
                        DeleteReverseTransactionData(StudentNo); // Pass StudentNo as string
                    }
                    else
                    {
                        LoadRecordData(StudentNo); // Pass StudentNo as string
                                                           // Load the student data if needed

                    }
                }
            }

        }


        private void SetUserPermissions()
        {
            if (Session["Permissions"] == null || Session["RoleId"] == null)
            {
                // Redirect or display an error message
                Response.Redirect("UserLogin.aspx"); // Or handle appropriately
                return;
            }

            List<string> userPermissions = Session["Permissions"] as List<string>;
            int roleId = Convert.ToInt32(Session["RoleId"]);

            if (userPermissions != null && userPermissions.Contains("SendWhatsUp_Message"))
            {
                pnlWhatsAppButtons.Visible = true;
            }
        }

        private void GenerateReport()
        {

            // Check if the text box is not empty (optional validation for better handling)
            if (!string.IsNullOrWhiteSpace(txtStudentNo.Text))
            {
                string studentNo = txtStudentNo.Text; // Directly assign the text value (string) from the TextBox

                // Call LoadReport method with studentNo as a string
                LoadReport(studentNo); // Assuming LoadReport is updated to take a string as a parameter
            }
            else
            {
                // Handle empty or invalid StudentNo, maybe show an error or prompt to enter a valid StudentNo
                // Example: lblError.Text = "Please enter a valid Student Number.";
            }
        }

        private void DeleteReverseTransactionData(string FeesCollectionId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();

                string query = @"
                             INSERT INTO feescollection
                             (referenceNo, InvoiceId, Amount, PaymentMethod, CreatedBy, SchoolId)
                            Select referenceNo+'(Rvs)', InvoiceId, (-1)*Amount, PaymentMethod, @CreatedBy, SchoolId from feescollection where FeescollectionId=@FeescollectionId";

                SqlCommand cmd = new SqlCommand(query, Con);
                string RandomString = RandomStringGenerator.GenerateRandomString();
                cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                cmd.Parameters.AddWithValue("@feescollectionId", FeesCollectionId);

                cmd.ExecuteNonQuery();

                ClearFormFields();
                lblMessage.Text = "Transaction has been reversed successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }


            // Redirect back to the students page after deletion
            Response.Redirect("FeesCollection.aspx");

        }



        private void LoadRecordData(string StudentNo)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();

                string receiptquery = @"SELECT 
    StudentNo,
    REPLACE(Student, ' ', '') AS Student,  -- Remove spaces from Student name
    ClassName,
    schoolid,
    Phone,
    SchoolName,
    Term,
    'MK' + FORMAT(SUM(CAST(TotalFees AS DECIMAL(18, 2))), 'N0') AS TotalFees,
    'MK' + FORMAT(SUM(CAST(TotalCollected AS DECIMAL(18, 2))), 'N0') AS TotalCollected,
    'MK' + FORMAT(SUM(CAST(TotalFees AS DECIMAL(18, 2))) - SUM(CAST(TotalCollected AS DECIMAL(18, 2))), 'N0') AS Balance,
    CASE 
        WHEN (SUM(CAST(TotalFees AS DECIMAL(18, 2))) - SUM(CAST(TotalCollected AS DECIMAL(18, 2)))) = 0 THEN 'Fully Paid'
        WHEN SUM(CAST(TotalCollected AS DECIMAL(18, 2))) = 0 THEN 'Not Paid'
        ELSE 'Partly Paid'
    END AS PaidStatus
FROM 
    PrintInvoice 
WHERE 
    StudentNo = @StudentNo and status=2 
GROUP BY 
    StudentNo, Student, ClassName, Term, schoolid, Phone, SchoolName
ORDER BY 
    Student;

";
                SqlCommand cmd = new SqlCommand(receiptquery, Con);
                cmd.Parameters.AddWithValue("@StudentNo", StudentNo);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        // Debug: Check values being retrieved
                        var StudentNO = dr["StudentNo"].ToString();
                        txtPhone.Text = dr["Phone"].ToString();
                        txtStudentName.Text = dr["Student"].ToString();
                        txtTerm.Text = dr["Term"].ToString();
                        txtBalance.Text = dr["Balance"].ToString();
                        txtSchoolName.Text = dr["SchoolName"].ToString();


                        // Set values to dropdown lists
                        txtStudentNo.Text = StudentNo;

                        // Debug: Log or break to verify values

                        // Handle other fields if any
                    }
                }
                else
                {
                    // Debug: Log if no rows found
                    System.Diagnostics.Debug.WriteLine("No rows found for the given FeesCollectionId.");
                }

                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            // Check if the text box is not empty (optional validation for better handling)
            if (!string.IsNullOrWhiteSpace(txtStudentNo.Text))
            {
                string studentNo = txtStudentNo.Text; // Directly assign the text value (string) from the TextBox

                // Call LoadReport method with studentNo as a string
                LoadReport(studentNo); // Assuming LoadReport is updated to take a string as a parameter
            }
            else
            {
                // Handle empty or invalid StudentNo, maybe show an error or prompt to enter a valid StudentNo
                // Example: lblError.Text = "Please enter a valid Student Number.";
            }
        }





        private void ClearFormFields()
        {
            txtStudentNo.Text = "";
        }


        private void LoadReport(string StudentNo)
        {
            // Define the query
            string query = @"SELECT [EnrollmentID]
      ,[Guardian]
      ,[InvoiceId]
      ,[InvoiceNo]
      ,[Student]
      ,[ClassName]
      ,[StreamName]
      ,[StudentNO]
      ,[FeesName]
      ,[ThisTermTotal]
      ,[PreviousTermBalance]
      ,[TotalFees]
      ,[TotalCollected]
      ,[Balance]
      ,[PaidStatus]
      ,[CollectionPercentage]
      ,[Phone]
      ,[TermId]
      ,[PreviousTerm]
      ,[NextTerm]
      ,[Status]
      ,[Term]
      ,[SchoolName]
      ,[SchoolCode]
      ,[SchoolId]
      ,[Logo]
      ,[Address]
  FROM [dbo].[PrintInvoice]

                     WHERE (StudentNO = @StudentNo) and (Status=2)";

            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    using (SqlCommand command = new SqlCommand(query, Con))
                    {
                        command.Parameters.AddWithValue("@StudentNo", StudentNo);
                        command.CommandTimeout = 130;
                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                        {
                            dataAdapter.Fill(dataTable);
                        }
                    }
                }

                if (dataTable.Rows.Count == 0)
                {
                    lblMessage.Text = "No Results found for selected transaction!";
                    lblMessage.CssClass = "alert alert-warning";
                }
                else
                {
                    lblMessage.CssClass = "d-none"; // Hide the message if data is found
                    ReportViewer1.Visible = true;

                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/Invoiceprint.rdlc");

                    ReportViewer1.LocalReport.EnableExternalImages = true;

                    ReportDataSource reportDataSource = new ReportDataSource("PrintInvoice", dataTable);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(reportDataSource);

                    // Set image path for external images if required
                   // string imagePath = "file:///C:/Logo/";
                    string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                    ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                    ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });

                    // Refresh the report
                    ReportViewer1.LocalReport.Refresh();
                    SaveReportAsPDF();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger";
            }
        }

        private string SanitizeFileName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Invoice";

            // Remove all invalid file name characters
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c.ToString(), ""); // replace with nothing
            }

            input = input.Trim();
            return string.IsNullOrWhiteSpace(input) ? "Invoice" : input;
        }

        private void SaveReportAsPDF()
        {
            string safeStudentName = SanitizeFileName(txtStudentName.Text);

            string fileName = $"Invoice_{safeStudentName}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string folderPath = Server.MapPath("~/Reports/Invoices/"); // Keep your existing path
            string filePath = Path.Combine(folderPath, fileName);

            byte[] bytes = ReportViewer1.LocalReport.Render("PDF");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            File.WriteAllBytes(filePath, bytes);

            if (File.Exists(filePath))
            {
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                Session["ReportPDF"] = $"{baseUrl}/Reports/Invoices/{fileName}";
            }
            else
            {
                Session["ReportPDF"] = null;
            }
        }

        protected void btnGenerateInvoice_Click(object sender, EventArgs e)
        {
            GenerateReport();

        }
    }

}

