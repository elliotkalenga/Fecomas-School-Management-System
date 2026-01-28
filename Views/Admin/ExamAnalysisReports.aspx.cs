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
    public partial class ExamAnalysisReports : System.Web.UI.Page
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
                string MSCEPerStudentQry = @"SELECT Distinct AssessmentTitle AS AssessmentId, AssessmentTitle as Assessment  
                            FROM MSCEAnalysis
                            WHERE Schoolcode = @SchoolCode";

                string JCEPerStudentQry = @"SELECT Distinct AssessmentTitle AS AssessmentId, AssessmentTitle as Assessment 
                            FROM JCEAnalysis 
                            WHERE Schoolcode = @SchoolCode";

                Con.Open();
                PopulateDropDownList(Con, MSCEPerStudentQry, ddlAssessmentStudent, "Assessment", "AssessmentId", "----Please Select Assessment----");
                PopulateDropDownList(Con, JCEPerStudentQry, ddlAssessment, "Assessment", "AssessmentId", "----Please Select Assessment----");
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





        public void JCESCHOOLREPORT()
        {
            string query = @"SELECT [AssessmentTitle]
      ,[SubjectName]
      ,[ClassName]
      ,[SchoolName]
      ,[SchoolCode]
      ,[Address]
      ,[Logo]
      ,[Grade_A]
      ,[Grade_B]
      ,[Grade_C]
      ,[Grade_D]
      ,[Grade_F]
      ,[TotalStudents]
  FROM [dbo].[JCEANALYSIS]

WHERE  (SchoolCode = @SchoolCode) AND 

(AssessmentTitle=@Assessment) Order By SubjectName";

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

            // Check if the DataTable is empty
            if (dataTable.Rows.Count == 0)
            {
                ReportViewer1.Visible = false;
            }
            else
            {
                ReportViewer1.Visible = true;
                ReportViewer1.Reset();

                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath =
                    Server.MapPath("~/Reports/JCEExamAnalysis.rdlc");

                ReportViewer1.LocalReport.EnableExternalImages = true;

                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(
                    new ReportDataSource("DataSet1", dataTable)
                );

                ReportViewer1.LocalReport.SetParameters(
                    new ReportParameter("ImagePath",
                    "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/")
                );

                ReportViewer1.LocalReport.Refresh();

            }

        }


        public void MSCESCHOOLREPORT()
        {
            string query = @"SELECT [AssessmentTitle]
      ,[SubjectName]
      ,[SchoolName]
      ,[SchoolCode]
      ,[Address]
      ,[Logo]
      ,[ClassName]
      ,[Grade_1]
      ,[Grade_2]
      ,[Grade_3]
      ,[Grade_4]
      ,[Grade_5]
      ,[Grade_6]
      ,[Grade_7]
      ,[Grade_8]
      ,[Grade_9]
      ,[TotalStudents]
  FROM [dbo].[MSCEANALYSIS]
WHERE 
    (SchoolCode = @SchoolCode) 
    AND (AssessmentTitle = @Assessment)
ORDER BY 
    SubjectName;
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

            // Check if the DataTable is empty
            if (dataTable.Rows.Count == 0)
            {
                ReportViewer1.Visible = false;
            }
            else
            {
                ReportViewer1.Visible = true;
                ReportViewer1.Reset();

                ReportViewer1.ProcessingMode = ProcessingMode.Local;

                // ✅ MUST come BEFORE ReportPath
                ReportViewer1.LocalReport.EnableExternalImages = true;

                ReportViewer1.LocalReport.ReportPath =
                    Server.MapPath("~/Reports/MSCEExamAnalysis.rdlc");

                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(
                    new ReportDataSource("DataSet1", dataTable)
                );

                if (dataTable.Rows.Count == 0)
                {
                    return;
                }

                ReportParameter imagePathParameter =
                    new ReportParameter(
                        "ImagePath",
                        "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/"
                    );

                // ✅ MUST be set BEFORE Refresh
                ReportViewer1.LocalReport.SetParameters(
                    new[] { imagePathParameter }
                );

                ReportViewer1.LocalReport.Refresh();
            }

        }



    }
}



