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
    public partial class ResultSheetsEOTReports : System.Web.UI.Page
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
                string MSCEPerStudentQry = @"SELECT Distinct AssessmentTitle+'-'+Classname AS AssessmentId, AssessmentTitle+'-'+Classname as Assessment  
                            FROM MSCEEndofterm 
                            WHERE Schoolcode = @SchoolCode";

                string JCEPerStudentQry = @"SELECT Distinct AssessmentTitle+'-'+Classname AS AssessmentId, AssessmentTitle+'-'+Classname as Assessment 
                            FROM JCEEndofterm 
                            WHERE Schoolcode = @SchoolCode";

                string PRIMARYPerStudentQry = @"SELECT Distinct AssessmentTitle+'-'+Classname AS AssessmentId, AssessmentTitle+'-'+Classname as Assessment  
                                FROM PRIEndofterm 
                            WHERE Schoolcode = @SchoolCode";
                Con.Open();
                PopulateDropDownList(Con, MSCEPerStudentQry, ddlAssessmentStudent, "Assessment", "AssessmentId", "----Please Select Assessment----");
                PopulateDropDownList(Con, JCEPerStudentQry, ddlAssessment, "Assessment", "AssessmentId", "----Please Select Assessment----");
                PopulateDropDownList(Con, PRIMARYPerStudentQry, ddlAggregate, "Assessment", "AssessmentId", "----Please Select Assessment ---");
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
            txtAssessment.Text = ddlAssessment.SelectedValue;
            JCESCHOOLREPORT();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);

        }
        protected void ddlAssessmentStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtAssessmentStudent.Text = ddlAssessmentStudent.SelectedValue;
            MSCESCHOOLREPORT();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);

        }

        protected void ddlAggregate_SelectedIndexChanged(object sender, EventArgs e)
        {

            txtAggregate.Text = ddlAggregate.SelectedValue;
            PRIMARYSCHOOLREPORT();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);


        }



        public void PRIMARYSCHOOLREPORT()
        {
            string query = @"SELECT Distinct  [AssessmentTitle] 
      ,[StudentNo]
      ,[Student]
      ,[Phone]
      ,[ClassName]
      ,[StudentName]
    ,LEFT([SubjectName], 3) AS SubjectName
      ,[Term]
      ,[TermId]
,CAST(FLOOR([Score]) AS VARCHAR(10)) 
    + '  ' + CAST(ScoreRank AS VARCHAR(10)) 
    + '/' + CAST(TotalScoreCount AS VARCHAR(10)) 
    + ' (' + Grade +')' AS Score
      ,[Grade],Score as Score1
      ,[GradeDescription]
      ,[Remark]
      ,[SubjectTeacher]
      ,[ScaletypeId]
      ,[scaledescription]
      ,[SchoolName]
      ,[Schoolcode]
      ,[Address]
      ,[Logo]
      ,[TotalScoreCount]
      ,[ScoreRank]
      ,[AverageScore]
      ,[TotalMarks]
      ,[EnglishScore]
      ,[SubjectsPassed]
      ,[Result]
      ,[Position]
      ,[ClassCount]
  FROM [FECOMASDESK].[dbo].[PRIEndofTerm]


  
WHERE  (SchoolCode = @SchoolCode) AND  (AssessmentTitle+'-'+ClassName=@Assessment) 
Order By position";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    command.Parameters.AddWithValue("@Assessment", txtAggregate.Text.ToString());
                    command.CommandTimeout = 130;
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }
            string logoName2 = string.Empty;

            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT L.LogoName2 FROM School S inner Join Logo L on S.Logoid=L.id WHERE SchoolCode = @SchoolCode", con))
                {
                    cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    con.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        logoName2 = result.ToString();
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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/ResultSheetPRI.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("PRIEndofterm", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);

                // Generate comments based on student performance
                DataRow firstRow = dataTable.Rows[0];
                string studentName = firstRow["Student"].ToString();
                string phone = firstRow["Phone"].ToString();
                string assessment = firstRow["AssessmentTitle"].ToString();
                string classname = firstRow["ClassName"].ToString();
                string schoolname = firstRow["SchoolName"].ToString();

                string overallComment = GenerateOverallComment(dataTable, studentName, assessment);
                txtPhone.Text = phone;
                txtPhone.Text = phone;
                txtAssessment.Text = assessment;
                txtClass.Text = classname;
                txtSchool.Text = schoolname;
                txtStudent.Text = studentName;

                // Pass the overallComment as a parameter
                ReportParameter logo2Parameter = new ReportParameter("LogoName2", logoName2);
                ReportParameter rpOverallComment = new ReportParameter("OverallComment", overallComment);
                ReportParameter imagePathParameter = new ReportParameter("ImagePath", "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/");
                ReportParameter usernameParameter = new ReportParameter("UserName", Session["Username"] != null ? Session["Username"].ToString() : string.Empty);
                ReportViewer1.ShowToolBar = true;
                ReportViewer1.ShowExportControls = true;
                ReportViewer1.ShowPrintButton = true;

                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpOverallComment, imagePathParameter, logo2Parameter, usernameParameter });
                ReportViewer1.LocalReport.DisplayName = (classname + assessment).Replace(" ", string.Empty);
                string studentfilename = (assessment + classname)
                    .Replace(" ", string.Empty)
                    .Replace("/", "_")
                    + DateTime.Now.ToString("yyyyMMddHHmmss")
                    + ".pdf";
                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
                SaveReportAsPDF(studentfilename);
            }

        }


        public void JCESCHOOLREPORT()
        {
            string query = @"SELECT Distinct [AssessmentTitle]
      ,[StudentNo]
      ,[Student]
      ,[ClassName]
      ,[StudentName]
      ,[phone]
    ,LEFT([SubjectName], 3) AS SubjectName
      ,[Term]
      ,[TermId]
,CAST(FLOOR([Score]) AS VARCHAR(10)) 
    + '  ' + CAST(ScoreRank AS VARCHAR(10)) 
    + '/' + CAST(TotalScoreCount AS VARCHAR(10)) 
    + ' (' + Grade +')' AS Score
      ,[Grade],Score as Score1
      ,[GradeDescription]
      ,[Remark]
      ,[SubjectTeacher]
      ,[ScaletypeId]
      ,[scaledescription]
      ,[SchoolName]
      ,[Schoolcode]
      ,[Address]
      ,[Logo]
      ,[TotalScoreCount]
      ,[ScoreRank]
      ,[AverageScore]
      ,[EnglishScore]
      ,[SubjectsPassed]
      ,[Result]
      ,[Position]
      ,[ClassCount]
  FROM [FECOMASDESK].[dbo].[JCEEndofTerm]

WHERE  (SchoolCode = @SchoolCode) AND 

(AssessmentTitle+'-'+ClassName=@Assessment) Order By position";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    command.Parameters.AddWithValue("@Assessment", txtAssessment.Text.ToString());
                    command.CommandTimeout = 130;
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }
            string logoName2 = string.Empty;

            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT L.LogoName2 FROM School S inner Join Logo L on S.Logoid=L.id WHERE SchoolCode = @SchoolCode", con))
                {
                    cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    con.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        logoName2 = result.ToString();
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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/ResultSheetJCE.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("JCEEndofterm", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);

                // Generate comments based on student performance
                DataRow firstRow = dataTable.Rows[0];
                string studentName = firstRow["Student"].ToString();
                string phone = firstRow["Phone"].ToString();
                string assessment = firstRow["AssessmentTitle"].ToString();
                string classname = firstRow["ClassName"].ToString();
                string schoolname = firstRow["SchoolName"].ToString();

                string overallComment = GenerateOverallComment(dataTable, studentName, assessment);
                txtPhone.Text = phone;
                txtPhone.Text = phone;
                txtAssessment.Text = assessment;
                txtClass.Text = classname;
                txtSchool.Text = schoolname;
                txtStudent.Text = studentName;

                // Pass the overallComment as a parameter
                ReportParameter logo2Parameter = new ReportParameter("LogoName2", logoName2);
                ReportParameter rpOverallComment = new ReportParameter("OverallComment", overallComment);
                ReportParameter imagePathParameter = new ReportParameter("ImagePath", "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/");
                ReportParameter usernameParameter = new ReportParameter("UserName", Session["Username"] != null ? Session["Username"].ToString() : string.Empty);
                ReportViewer1.ShowToolBar = true;
                ReportViewer1.ShowExportControls = true;
                ReportViewer1.ShowPrintButton = true;

                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpOverallComment, imagePathParameter, logo2Parameter, usernameParameter });
                ReportViewer1.LocalReport.DisplayName = (classname + assessment).Replace(" ", string.Empty);
                string studentfilename = (assessment + classname)
                    .Replace(" ", string.Empty)
                    .Replace("/", "_")
                    + DateTime.Now.ToString("yyyyMMddHHmmss")
                    + ".pdf";
                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
                SaveReportAsPDF(studentfilename);
            }

        }


        public void MSCESCHOOLREPORT()
        {
            string query = @"SELECT Distinct
    [AssessmentTitle],
    [StudentNo],
    [Student],
    LEFT([SubjectName], 3) AS SubjectName,  
    [ClassName],
    [StudentName],
    [Term],
    [Phone],
    [TermId]
,CAST(FLOOR([Score]) AS NVARCHAR(10)) 
    + '  ' + CAST(ScoreRank AS NVARCHAR(10)) 
    + '/' + CAST(TotalScoreCount AS NVARCHAR(10)) 
    + ' (' + CAST(Grade AS NVARCHAR(10)) + ')' AS Score,
    [Grade],Score as Score1,
    [GradeDescription],
    [Remark],
    [SubjectTeacher],
    [ScaleTypeId],
    [ScaleDescription],
    [SchoolName],
    [SchoolCode],
    [Address],
    [Logo],
    [TotalScoreCount],
    [ScoreRank],
    [Points],
    [Result],
    [AverageScore],
    [Position],
    [ClassCount]
FROM [FECOMASDESK].[dbo].[MSCEEndofterm]
WHERE 
    (SchoolCode = @SchoolCode) 
    AND (AssessmentTitle + '-' + ClassName = @Assessment)
ORDER BY 
    Position;
";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    command.Parameters.AddWithValue("@Assessment", txtAssessmentStudent.Text.ToString());
                    command.CommandTimeout = 130;
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }
            string logoName2 = string.Empty;

            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT L.LogoName2 FROM School S inner Join Logo L on S.Logoid=L.id WHERE SchoolCode = @SchoolCode", con))
                {
                    cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    con.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        logoName2 = result.ToString();
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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/ResultSheetMSCE.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("MSCEEndofterm", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);

                // Generate comments based on student performance
                DataRow firstRow = dataTable.Rows[0];
                string studentName = firstRow["Student"].ToString();
                string phone = firstRow["Phone"].ToString();
                string assessment = firstRow["AssessmentTitle"].ToString();
                string classname = firstRow["ClassName"].ToString();
                string schoolname = firstRow["SchoolName"].ToString();

                string overallComment = GenerateOverallComment(dataTable, studentName, assessment);
                txtPhone.Text = phone;
                txtPhone.Text = phone;
                txtAssessment.Text = assessment;
                txtClass.Text = classname;
                txtSchool.Text = schoolname;
                txtStudent.Text = studentName;

                // Pass the overallComment as a parameter
                ReportParameter logo2Parameter = new ReportParameter("LogoName2", logoName2);
                ReportParameter rpOverallComment = new ReportParameter("OverallComment", overallComment);
                ReportParameter imagePathParameter = new ReportParameter("ImagePath", "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/");
                ReportParameter usernameParameter = new ReportParameter("UserName", Session["Username"] != null ? Session["Username"].ToString() : string.Empty);
                ReportViewer1.ShowToolBar = true;
ReportViewer1.ShowExportControls = true;
ReportViewer1.ShowPrintButton = true;

                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpOverallComment, imagePathParameter, logo2Parameter, usernameParameter });
                ReportViewer1.LocalReport.DisplayName = (classname + assessment).Replace(" ", string.Empty);
                string studentfilename = (assessment + classname)
                    .Replace(" ", string.Empty)
                    .Replace("/", "_")
                    + DateTime.Now.ToString("yyyyMMddHHmmss")
                    + ".pdf";
                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
                SaveReportAsPDF(studentfilename);
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

        private string GenerateOverallComment(DataTable performanceTable, string studentName, string assessment)
        {
            StringBuilder commentBuilder = new StringBuilder();

            // Subjects where the student needs improvement
            List<string> subjectsToImprove = new List<string>();

            bool isPassed = true;

            foreach (DataRow row in performanceTable.Rows)
            {
                string subject = row["SubjectName"].ToString();
                decimal score = Convert.ToDecimal(row["Score1"]);
                string resultStatus = row["Result"].ToString();

                if (resultStatus.ToUpper() == "FAIL")
                {
                    isPassed = false;
                }

                // Assuming a score below 60 is considered needing improvement
                if (score < 60)
                {
                    subjectsToImprove.Add(subject);
                }
            }

            if (!isPassed)
            {
                commentBuilder.Append($"Unfortunately, {studentName} has failed {assessment}. \n");
                if (subjectsToImprove.Count > 0)
                {
                    commentBuilder.Append($"Improvement is needed in the following subjects: \n {string.Join(", ", subjectsToImprove)}. \n");
                    commentBuilder.Append("With dedication and the right support, improvement is definitely achievable.");
                }
                else
                {
                    commentBuilder.Append($"Because {studentName} wrote fewer subjects than the recommended minimum.\n");
                    commentBuilder.Append($"Need to write enough subjects next time. Otherwise, {studentName} has potential.");
                }
            }
            else
            {
                commentBuilder.Append($"Congratulations, {studentName} has passed {assessment}.\n");
                if (subjectsToImprove.Count > 0)
                {
                    commentBuilder.Append($"Improvement is needed in the following subjects:\n {string.Join(", ", subjectsToImprove)}. \n");
                    commentBuilder.Append("With dedication and the right support, improvement is definitely achievable.");
                }
                else
                {
                    commentBuilder.Append($"and has consistently demonstrated a balanced performance across all subjects. \n");
                    commentBuilder.Append("Keep up the good work and strive for excellence across all subjects.");
                }
            }

            return commentBuilder.ToString();
        }

    }
}



