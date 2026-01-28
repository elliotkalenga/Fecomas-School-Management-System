using Microsoft.Reporting.WebForms;
using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class SchemesofworkReport : System.Web.UI.Page
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
                txtTeacher.Enabled = false;
                txtSubject.Enabled = false;
                txtWeek.Enabled = false;
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
                string WeekQry = @" SELECT Distinct
        Sc.SchemeId as WeekId, 
        sc.WeekNo +'-'+ S.SubjectName + '-' + C.ClassName +'-'+ Tn.TermNumber + ' (' + F.FinancialYear + ')'  WeekNo
    FROM 
        SchemesOfWork sc WITH (NOLOCK) 
    INNER JOIN 
        Subjectallocation sa WITH (NOLOCK) on sc.subjectId = sa.AllocationId
    INNER JOIN Users u with (nolock) on sa.teacherId=u.userid inner join 
        Class c on sa.classId = c.ClassId 
    INNER JOIN 
        Subject s WITH (NOLOCK) on sa.subjectid = s.subjectId 
    INNER JOIN 
        Term AS T WITH (NOLOCK) ON Sc.TermId = T.TermId
    INNER JOIN 
        TermNumber AS TN WITH (NOLOCK) ON T.Term = TN.TermId
    INNER JOIN 
        FinancialYear AS F WITH (NOLOCK) ON T.YearId = F.FinancialYearId Where Sc.SchoolId=@SchoolId and T.status=2 and u.username=@Username     ";

                string SubjectQry = @" SELECT Distinct
        sa.AllocationId as SUbjectId, 
         S.SubjectName + '-' + C.ClassName +'-'+ Tn.TermNumber + ' (' + F.FinancialYear + ')'  as SubjectNAme
    FROM 
        SchemesOfWork sc WITH (NOLOCK) 
    INNER JOIN 
        Subjectallocation sa WITH (NOLOCK) on sc.subjectId = sa.AllocationId
    INNER JOIN Users u with (nolock) on sa.teacherId=u.userid inner join 
        Class c on sa.classId = c.ClassId 
    INNER JOIN 
        Subject s WITH (NOLOCK) on sa.subjectid = s.subjectId 
    INNER JOIN 
        Term AS T WITH (NOLOCK) ON Sc.TermId = T.TermId
    INNER JOIN 
        TermNumber AS TN WITH (NOLOCK) ON T.Term = TN.TermId
    INNER JOIN 
        FinancialYear AS F WITH (NOLOCK) ON T.YearId = F.FinancialYearId 
          Where Sc.SchoolId=@SchoolId and T.status=2 and u.username=@Username
        ";

                string TeacherQry = @"SELECT 
        u.username as TeacherId, 
        u.firstName+' '+ u.LastName +' ('+ u.username +')' as Teacher
    FROM 
        SchemesOfWork sc WITH (NOLOCK) 
    INNER JOIN 
        Subjectallocation sa WITH (NOLOCK) on sc.subjectId = sa.AllocationId
    INNER JOIN Users u with (nolock) on sa.teacherId=u.userid inner join 
        Class c on sa.classId = c.ClassId 
    INNER JOIN 
        Subject s WITH (NOLOCK) on sa.subjectid = s.subjectId 
    INNER JOIN 
        Term AS T WITH (NOLOCK) ON Sc.TermId = T.TermId
    INNER JOIN 
        TermNumber AS TN WITH (NOLOCK) ON T.Term = TN.TermId
    INNER JOIN 
        FinancialYear AS F WITH (NOLOCK) ON T.YearId = F.FinancialYearId Where Sc.SchoolId=@SChoolid and T.status=2   and u.username=@Username
            
        ";

                Con.Open();
                PopulateDropDownList(Con, WeekQry, ddlWeeks, "WeekNo", "WeekId", "----Please Select Week & Subject----");
                PopulateDropDownList(Con, SubjectQry, ddlSubjects, "SubjectName", "SubjectId", "----Please Select Subject----");
                PopulateDropDownList(Con, TeacherQry, ddlTeachers, "Teacher", "TeacherId", "----Please Select a Teacher----");
            }
        }

        private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField, string defaultText)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
            cmd.Parameters.AddWithValue("@Username", Session["UserName"]);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddl.DataSource = dt;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem(defaultText, ""));
        }




        protected void ddlSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSubject.Text = ddlSubjects.SelectedValue;
            LoadReportbySubject();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);
        
        }

        protected void ddlWeeks_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtWeek.Text = ddlWeeks.SelectedValue;
            LoadReportbyWeekSUbject();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);


        }

        protected void ddlTeachers_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtTeacher.Text = ddlTeachers.SelectedValue;
            LoadReportbyTeacher();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);

        }


     public void LoadReportbySubject()
        {
            string query = @"SELECT SchemeId, allocationId, WeekNo, Term, SubjectName, Topic, LessonsCount, LearningObjectives, LearningOutcome, SchemeEvaluation, Resources, Teacher, DateCompleted, CompleteStatus, SchoolName, SchoolCode, Logo, SchoolID, 
                  Address
                FROM     Vw_SchemesOfWOrkReport
                WHERE  (SchoolCode = @SchoolCode) AND (AllocationId = @SubjectName) Order By WeekNo";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@SubjectName", txtSubject.Text.ToString());
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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/SchemesofWorkBySubject.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("Schemesofworkreport", dataTable);
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


    public void LoadReportbyWeekSUbject()
        {
            string query = @"SELECT SchemeId, allocationId, WeekNo, Term, SubjectName, Topic, LessonsCount, LearningObjectives, LearningOutcome, SchemeEvaluation, Resources, Teacher, DateCompleted, CompleteStatus, SchoolName, SchoolCode, Logo, SchoolID, 
                  Address
                FROM     Vw_SchemesOfWOrkReport
                WHERE  (SchoolCode = @SchoolCode) AND (SchemeId = @WeekNo) Order By WeekNo";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@WeekNo", txtWeek.Text.ToString());
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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/SchemesofWorkByWeekSubject.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("Schemesofworkreport", dataTable);
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

        public void LoadReportbyTeacher()
        {
            string query = @"SELECT SchemeId,username, allocationId, WeekNo, Term, SubjectName, Topic, LessonsCount, LearningObjectives, LearningOutcome, SchemeEvaluation, Resources, Teacher, DateCompleted, CompleteStatus, SchoolName, SchoolCode, Logo, SchoolID, 
                  Address
                FROM     Vw_SchemesOfWOrkReport
                WHERE  (SchoolCode = @SchoolCode) AND (Username = @Teacher) Order By Teacher,WeekNo";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    command.Parameters.AddWithValue("@Teacher", txtTeacher.Text.ToString());
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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/SchemesofWorkByTeacher.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("Schemesofworkreport", dataTable);
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
        protected void btnReportBySubject_Click(object sender, EventArgs e)
        {

         }


        
        protected void btnReportByTeacher_Click(object sender, EventArgs e)
        {

        }
    }
}



