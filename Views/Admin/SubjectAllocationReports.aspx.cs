using Microsoft.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class SubjectAllocationReports : System.Web.UI.Page
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
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);

                PopulateDropDownLists();
                txtAssessment.Enabled = false;
                txtAssessmentStudent.Enabled = false;
                txtAggregate.Enabled = false;
                // Check if both query string parameters are available
                if (Request.QueryString["Term"] != null)
                {
                    string Term = Request.QueryString["Term"]; // Get Term from query string

                    if (Term == "delete")
                    {
                        // Handle the delete logic if the exam mode is "delete"
                        // Example: DeleteStudentRecord(StudentNo); 
                    }
                    else
                    {
                        // Load the student data using the StudentNo

                        // Handle the Exam (mode) - you can load different data based on exam
                        // Add any other "exam" modes you need to handle
                    }
                }
            }


        }



        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string MSCEPerStudentQry = @"SELECT Distinct  StudentName AS StudentNameId, StudentName 
                            FROM MSCEOthers 
                            WHERE Schoolcode = @SchoolCode";

                string JCEPerStudentQry = @"SELECT distinct Term as TermId, Term, SchoolCode
FROM   Vw_SubjectAllocationReport WHERE (SchoolCode = @SchoolCode)";

                string PRIMARYPerStudentQry = @"SELECT Distinct  StudentName  AS StudentNameId, StudentName 
                                FROM PRIOthers 
                            WHERE Schoolcode = @SchoolCode";
                Con.Open();
            //    PopulateDropDownList(Con, MSCEPerStudentQry, ddlAssessmentStudent, "StudentName", "StudentNameId", "----Please Select Student----");
                PopulateDropDownList(Con, JCEPerStudentQry, ddlsubjectAllocation, "Term", "TermId", "----Please Select Term----");
            //    PopulateDropDownList(Con, PRIMARYPerStudentQry, ddlAggregate, "StudentName", "StudentNameId", "----Please Select Student ---");
            }
        }


        private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField, string defaultText)
        {
            if (Session["SchoolId"] != null)
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                cmd.Parameters.AddWithValue("@Username", Session["Username"]);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddl.DataSource = dt;
                ddl.DataTextField = textField;
                ddl.DataValueField = valueField;
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem(defaultText, ""));
            }
            else
            {
                // Handle the case when Session["SchoolId"] is null
            }
        }

        protected void ddlAssessment_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtAssessment.Text = ddlsubjectAllocation.SelectedValue;
            SubjectAllocationFull();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);

        }





        public void SubjectAllocationFull()
        {
            string query = @"SELECT SubjectName, SubjectCode, Teacher, ClassName, Term, SchoolCode, SchoolId, SchoolName, Logo, Address
FROM   Vw_SubjectAllocationReport
WHERE (SchoolCode = @SchoolCode) AND (Term = @Term)
ORDER BY SubjectName";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    command.Parameters.AddWithValue("@Term", txtAssessment.Text.ToString());
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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/SubjectAllocationReport.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("SubjectAllocationReport", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);

                //Generate comments based on student performance


              //  string overallComment = GenerateOverallComment(dataTable, studentName, assessment);
          
               
                // Pass the overallComment as a parameter
                //ReportParameter rpOverallComment = new ReportParameter("OverallComment", overallComment);
                ReportParameter imagePathParameter = new ReportParameter("ImagePath", "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/");
                ReportParameter usernameParameter = new ReportParameter("UserName", Session["Username"] != null ? Session["Username"].ToString() : string.Empty);

                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { imagePathParameter, usernameParameter });
                string studentfilename = ("SubjectAllocationReport")
                 .Replace(" ", string.Empty)
                 .Replace("/", "_")
                 + DateTime.Now.ToString("yyyyMMddHHmmss")
                 + ".pdf";
                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
                //SaveReportAsPDF(studentfilename);
            }

        }



        private void SaveReportAsPDF(string student)
        {
            string fileName = "SchoolReport" + student + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
            string folderPath = Server.MapPath("~/Reports/SchoolReports/"); // Local path
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
                string publicUrl = baseUrl + "/Reports/SchoolReports/" + fileName;

                Session["ReportPDF"] = publicUrl; // Store the publicly accessible URL
            }
            else
            {
                Session["ReportPDF"] = null;
            }
        }


    }
}



