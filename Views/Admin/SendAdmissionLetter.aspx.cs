using Microsoft.Reporting.Map.WebForms.BingMaps;
using Microsoft.Reporting.WebForms;
using SMSWEBAPP.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class SendAdmissionLetter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["CandidateID"] != null)
                {
                    int studentID;
                    if (int.TryParse(Request.QueryString["CandidateID"], out studentID))
                    {
                        LoadStudentData(studentID);
                    }
                }
            }
        }

        private void LoadStudentData(int studentID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand(@"select C.CandidateId, C.Phone,C.FirstName,C.LastName,S.SchoolName,S.Address,C.ImagePath,L.LogoName as Logo,s.Address as StudentLocation from Candidate C 
Inner Join School S on C.School=S.SchoolId
Inner join Logo L on S.Logoid=L.Id WHERE C.CandidateID = @CandidateID", Con);
                cmd.Parameters.AddWithValue("@CandidateID", studentID);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    txtCandidateId.Text = dr["CandidateId"].ToString();
                    txtFirstName.Text = dr["SchoolName"].ToString();
                    txtLastName.Text = dr["FirstName"].ToString() + dr["LastName"].ToString();
                    txtPhone.Text = dr["Phone"].ToString();

                    // Generate WhatsApp link directly when student data is loaded
                }
                dr.Close();
            }
        }


        protected void btnBudgetMonitoring_Click(object sender, EventArgs e)
        {
            string query = @"SELECT CandidateId, FirstName, LastName, SchoolName, Address, ImagePath, StudentLocation, Logo, SchoolCode
FROM   AdmissionLetter
WHERE (CandidateId = @CandidateId) AND (SchoolCode = @SchoolCode)";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@CandidateId", txtCandidateId.Text.ToString());
                    command.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    command.CommandTimeout = 130;

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }

            // Check if the DataTable is empty
            if (dataTable.Rows.Count == 0)
            {
                ReportViewer1.Visible = false;
            }
            else
            {
                ReportViewer1.Visible = true;

                // Set the ReportViewer properties
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/AdmissionLetter.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;

                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("AdmissionLetter", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);

                // Set external images path
                string UserName = (Session["FirstName"] != null ? Session["FirstName"].ToString() : string.Empty) + " " + (Session["LastName"] != null ? Session["LastName"].ToString() : string.Empty);
                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                ReportParameter Username = new ReportParameter("UserName", UserName);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { Username });

                // Refresh the report
                ReportViewer1.LocalReport.Refresh();

                // Export to PDF and Save on Server
                SaveReportAsPDF();
            }
        }

        private void SaveReportAsPDF()
        {
            string fileName = "AdmissionLetter_" + txtLastName.Text  + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
            string folderPath = Server.MapPath("~/Reports/AdmissionLetters/"); // Local path
            string filePath = Path.Combine(folderPath, fileName);

            byte[] bytes;
            string mimeType, encoding, fileNameExtension;
            string[] streams;
            Microsoft.Reporting.WebForms.Warning[] warnings;

            bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            // Ensure directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.WriteAllBytes(filePath, bytes);

            // Ensure file exists before storing session variable
            if (File.Exists(filePath))
            {
                // Replace "localhost" with your actual domain or VPS IP
                string baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string publicUrl = baseUrl + "/Reports/GeneratedReports/" + fileName;

                Session["ReportPDF"] = publicUrl; // Store the publicly accessible URL
            }
            else
            {
                Session["ReportPDF"] = null;
            }
        }


    }
}
