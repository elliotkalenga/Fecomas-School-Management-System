using Microsoft.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class AssessmentScoresReports : System.Web.UI.Page
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
                string PerStudentQry = @"SELECT Distinct
    St.FirstName + ' ' + St.LastName +'-'+ C.ClassName+'-'+TN.TermNumber + ' (' + F.FinancialYear + ')'  AS StudentId,
    St.FirstName + ' ' + St.LastName +'-'+ C.ClassName+'-'+TN.TermNumber + ' (' + F.FinancialYear + ')'  AS StudentName
FROM 
    AssessmentScores Sc WITH (NOLOCK)
INNER JOIN 
    Enrollment E WITH (NOLOCK) ON sc.StudentId = E.EnrollmentId
INNER JOIN 
    Student St WITH (NOLOCK) ON E.StudentId = St.StudentId
INNER JOIN 
    Term T WITH (NOLOCK) ON E.TermId = T.TermId
INNER JOIN 
    TermNumber TN WITH (NOLOCK) ON T.Term = TN.TermId
INNER JOIN 
    FinancialYear F WITH (NOLOCK) ON T.YearId = F.FinancialYearId
INNER JOIN 
    Class C WITH (NOLOCK) ON E.ClassId = C.ClassId 

	Where sc.createdby=@Username   ";



                string PerClassQry = @"SELECT distinct
a.AssessmentId,
a.AssessmentTitle+' '+ET.ExamType+'-'+C.ClassName+'-'+TN.TermNumber + ' (' + F.FinancialYear + ')'  +' '+sub.SubjectName AS Assessment
 FROM 
    AssessmentScores Sc WITH (NOLOCK)
INNER JOIN 
Assessment a on sc.AssessmentId=a.AssessmentId 
Inner Join
ExamType ET on a.AssessmentTypeId=ET.ExamTypeId
Inner Join
    Enrollment E WITH (NOLOCK) ON sc.StudentId = E.EnrollmentId
INNER JOIN 
SubjectAllocation sa on a.SubjectId=sa.AllocationId
Inner join 
Subject Sub on sa.SUbjectId=sub.SubjectId
INNER JOIN 
    Student St WITH (NOLOCK) ON E.StudentId = St.StudentId
INNER JOIN 
    Term T WITH (NOLOCK) ON E.TermId = T.TermId
INNER JOIN 
    TermNumber TN WITH (NOLOCK) ON T.Term = TN.TermId
INNER JOIN 
    FinancialYear F WITH (NOLOCK) ON T.YearId = F.FinancialYearId
INNER JOIN 
    Class C WITH (NOLOCK) ON E.ClassId = C.ClassId
	where sc.createdby=@username

        ";

                string PerClassEOFQry = @"
select distinct Assessment as AssessmentId,Assessment from Vw_endofterm where subjectTeacher=@username
        ";

                Con.Open();
                PopulateDropDownList(Con, PerStudentQry, ddlAssessmentStudent, "StudentName", "StudentId", "----Please Select Student----");
                PopulateDropDownList(Con, PerClassQry, ddlAssessment, "Assessment", "AssessmentId", "----Please Select Assessment----");
                PopulateDropDownList(Con, PerClassEOFQry, ddlAggregate, "AssessmentId", "AssessmentId", "----Please Select Assessment----");
            }
        }


        private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField, string defaultText)
        {
            if (Session["SchoolId"] != null)
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
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
            LoadReportbyClassAssessment();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);

        }



        protected void ddlAssessmentStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtAssessmentStudent.Text = ddlAssessmentStudent.SelectedValue;
            LoadReportbyStudent();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);

        }

        protected void ddlAggregate_SelectedIndexChanged(object sender, EventArgs e)
        {

            txtAggregate.Text = ddlAggregate.SelectedValue;
            LoadReportbyClassEOF();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);


        }


        public void LoadReportbyStudent()
        {
            string query = @"SELECT [ScoreRank]
      ,[ScoreId]
      ,[AssessmentTitle]
      ,[Student]
      ,[ClassName]
      ,[UserName]
      ,[StudentName]
      ,[Class]
      ,[SubjectName]
      ,[Score]
      ,[Contribution]
      ,[Grade]
      ,[GradeDescription]
      ,[Remark]
      ,[SubjectTeacher]
      ,[Term]
      ,[ScoreCount]
      ,[SchoolCode]
      ,[SchoolName]
      ,[Address]
      ,[Logo]
  FROM [dbo].[Vw_AssessmentScores]

                    WHERE  (SchoolCode = @SchoolCode) AND (StudentName = @StudentName) Order By Score Desc";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@StudentName", txtAssessmentStudent.Text.ToString());
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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/AssessmentsPerStudent.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("Assessmentperstudent", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                // Set the external images path
                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                string UserName = Session["Username"] != null ? Session["Username"].ToString() : string.Empty;
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                ReportParameter Username = new ReportParameter("UserName", UserName);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { Username });

                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
            }

        }


        public void LoadReportbyClassAssessment()
        {
            string query = @"SELECT [ScoreRank]
      ,[ScoreId],AssessmentId
      ,[AssessmentTitle] 
      ,[Student]
      ,[ClassName]
      ,[UserName]
      ,[StudentName]
      ,[Assessment]
      ,[SubjectName]
      ,[Score]
      ,[Contribution]
      ,[Grade]
      ,[GradeDescription]
      ,[Remark]
      ,[SubjectTeacher]
      ,[Term]
      ,[ScoreCount]
      ,[SchoolCode]
      ,[SchoolName]
      ,[Address]
      ,[Logo]
  FROM [dbo].[Vw_AssessmentScores]

WHERE  (SchoolCode = @SchoolCode) AND (AssessmentId  = @Assessment) Order By Score Desc";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@Assessment", txtAssessment.Text.ToString());
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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/AssessmentScoresReport.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("AssessmentScoresReport", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                // Set the external images path
                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                string UserName = Session["Username"] != null ? Session["Username"].ToString() : string.Empty;
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                ReportParameter Username = new ReportParameter("UserName", UserName);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { Username });



                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
            }

        }

        public void LoadReportbyClassEOF()
        {
            string query = @"SELECT [AssessmentTitle]
      ,[Assessment]
      ,[StudentNo]
      ,[Student]
      ,[SubjectName]
      ,[ClassName]
      ,[Term]
      ,[TermId]
      ,[Score]
      ,[Grade]
      ,[GradeDescription]
      ,[Remark]
      ,[Assessment]
      ,[SubjectTeacher]
      ,[ScaletypeId]
      ,[scaledescription]
      ,[SchoolName]
      ,[Schoolcode]
      ,[Address]
      ,[Logo]
      ,[TotalScoreCount]
      ,[ScoreRank]
  FROM [dbo].[Vw_endofterm]

WHERE  (SchoolCode = @SchoolCode) AND (Assessment = @Assessment) Order By Score Desc";

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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/EndofTermAssessments.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("Endofterm", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                // Set the external images path
                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                string UserName = Session["Username"] != null ? Session["Username"].ToString() : string.Empty;
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                ReportParameter Username = new ReportParameter("UserName", UserName);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { Username });



                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
            }

        }

    }
}



