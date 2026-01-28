using Microsoft.Data.SqlClient;
using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SMSWEBAPP.Views.Admin.Terms;

namespace SMSWEBAPP.Views.Admin
{
    public partial class AssessmentAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                SetButtonText();
                PopulateDropDownLists();
                if (Request.QueryString["AssessmentId"] != null)
                {
                    int AssessmentId = int.Parse(Request.QueryString["AssessmentId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteExam(AssessmentId);
                    }
                    else
                    {
                        LoadExamData(AssessmentId);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["AssessmentId"] != null)
            {
                btnSubmit.Text = "Update";
            }
            else
            {
                btnSubmit.Text = "Add";
            }
        }
        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string SubjectQry = @"Select 0 as SubjectId, '------- Select Subject ------' As SubjectName Union 
                                    select Sa.allocationid as SubjectId,s.subjectName +'-'+C.ClassName from SubjectAllocation sa
                                    Inner join Subject s on sa.subjectid=s.subjectid
                                    inner join Term T on sa.termId=T.TermId
                                    inner join users u on sa.TeacherId=u.userid
									inner join Class C on sa.Classid=C.ClassId
                                    Where u.username=@UserName and sa.Schoolid=@SchoolId and T.status=2";
                string ExamTypeQry = @"Select 0 as ExamTypeId, '------- Select Exam Type ------' As ExamType Union Select ExamTypeId,ExamType from ExamType";
                string TermQry = @"select T.TermId, TN.TermNumber+'('+y.FinancialYear+')'  as Term from term T
                                    inner Join TermNumber TN on T.Term=TN.Termid
                                    inner Join FinancialYear y on T.YearId=y.FinancialYearId where T.Status=2 and T.SchoolId=@SchoolId";
                string ExamLockQry = @"select 0 ReleaseExamId, '-- Select Assessment Lock --' as ReleaseStatus union  select ReleaseExamId,ReleaseStatus from ReleaseExam";
                string StatusQry = @"select 0 StatusId, '-- Select Assessment Status --' as Status union  select StatusId,Status from Status";

                Con.Open();
                PopulateDropDownList(Con, SubjectQry, ddlSubject, "SubjectName", "SubjectId");
                PopulateDropDownList(Con, ExamTypeQry, ddlAssessmentType, "ExamType", "ExamTypeId");
                PopulateDropDownList(Con, TermQry, ddlTerm, "Term", "TermId");
                PopulateDropDownList(Con, StatusQry, ddlStatus, "Status", "StatusId");
                PopulateDropDownList(Con, ExamLockQry, ddlAssessmentStatus, "ReleaseStatus", "ReleaseExamId");
            }
        }

        private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Username", Session["UserName"]);
            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
            SqlDataReader dr = cmd.ExecuteReader();
            ddl.DataSource = dr;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            dr.Close();
        }

        private void LoadExamData(int AssessmentId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Assessment WHERE AssessmentId = @AssessmentId", Con);
                cmd.Parameters.AddWithValue("@AssessmentId", AssessmentId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    txtAssessmentTitle.Text = dr["AssessmentTitle"].ToString();
                    txtAssessmentWeight.Text = dr["Contribution"].ToString();
                    ddlAssessmentType.SelectedValue = dr["AssessmentTypeId"].ToString();
                    ddlTerm.SelectedValue = dr["TermId"].ToString();
                    ddlSubject.SelectedValue = dr["SubjectId"].ToString();
                    ddlStatus.SelectedValue = dr["Status"].ToString();
                    ddlAssessmentStatus.SelectedValue = dr["AssessmentStatus"].ToString();
                }
                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["AssessmentId"] != null)
            {
                int AssessmentId = int.Parse(Request.QueryString["AssessmentId"]);
                UpdateExam(AssessmentId);
            }
            else
            {
                AddNewExam();
            }
            ClearControls();
        }

        private void AddNewExam()
        {
            // Validate dropdown selections
            if (ddlAssessmentType.SelectedIndex == 0)
            {
                lblErrorMessage.Text = "Please select Assessment Type.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return;
            }


            if (ddlSubject.SelectedIndex == 0)
            {
                lblErrorMessage.Text = "Please select Subject.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return;
            }

            if (ddlStatus.SelectedIndex == 0)
            {
                lblErrorMessage.Text = "Please select Status.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return;
            }

            if (ddlAssessmentStatus.SelectedIndex == 0)
            {
                lblErrorMessage.Text = "Please select Assessment Status.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return;
            }


            if (Session["Permissions"] == null || Session["RoleId"] == null)
            {
                lblErrorMessage.Text = "Session expired. Please log in again.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return;
            }

            List<string> userPermissions = (List<string>)Session["Permissions"];
            int roleId = Convert.ToInt32(Session["RoleId"]);

            if (!userPermissions.Contains("Manage_Assessments"))
            {
                lblErrorMessage.Text = "ACCESS DENIED! You do not have permission to perform this action.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return;
            }

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                try
                {
                    string query = @"INSERT INTO Assessment 
                            (Status, AssessmentTitle, Contribution, SubjectId, AssessmentTypeId, TermId, AssessmentStatus, CreatedBy, SchoolId)
                            VALUES 
                            (@Status, @AssessmentTitle, @Contribution, @SubjectId, @AssessmentTypeId, @TermId, @AssessmentStatus, @CreatedBy, @SchoolId)";

                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@AssessmentTitle", txtAssessmentTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@Contribution", txtAssessmentWeight.Text.Trim());
                    cmd.Parameters.AddWithValue("@SubjectId", ddlSubject.SelectedValue);
                    cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@AssessmentTypeId", ddlAssessmentType.SelectedValue);
                    cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                    cmd.Parameters.AddWithValue("@AssessmentStatus", ddlAssessmentStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                    Con.Open();
                    cmd.ExecuteNonQuery();

                    lblMessage.Text = "Assessment added successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                }
                catch (SqlException ex)
                {
                    string AssessmentType = ddlAssessmentType.SelectedItem.ToString();
                    string Term = ddlTerm.SelectedItem.ToString();
                    string Subject = ddlSubject.SelectedItem.ToString();

                    if (ex.Number == 2601) // Unique constraint error
                    {
                        lblErrorMessage.Text = $"{AssessmentType} Assessment already exists for {Subject} within Term {Term}. Please create an assessment for another subject.";
                    }
                    else
                    {
                        lblErrorMessage.Text = "An error occurred while adding the assessment. Error details: " + ex.Message;
                    }

                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }
        }
        private void UpdateExam(int AssessmentId)
        {
            string AssessmentType = ddlAssessmentType.SelectedItem.ToString();
            string Term = ddlTerm.SelectedItem.ToString();
            string Subject = ddlSubject.SelectedItem.ToString();

            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = "UPDATE Assessment SET SubjectId=@subjectId,Status=@Status," +
                        " AssessmentTitle=@AssessmentTitle, AssessmentStatus=@AssessmentStatus,Contribution=@Contribution WHERE AssessmentId=@AssessmentId";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@AssessmentStatus", ddlAssessmentStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@AssessmentTitle", txtAssessmentTitle.Text.ToString());
                    cmd.Parameters.AddWithValue("@SubjectId", ddlSubject.SelectedValue);
                    cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@Contribution", txtAssessmentWeight.Text.ToString());
                    cmd.Parameters.AddWithValue("@AssessmentId", AssessmentId);
                    cmd.ExecuteNonQuery();
                    ClearControls();
                    SetButtonText();
                }
                lblMessage.Text = "Assessment updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601) // Unique constraint error
                {
                    lblErrorMessage.Text = AssessmentType + " Assessment already exists for " + Subject + " Within Term " + Term + "Please Update  Assessment for another Subject";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
                else
                {
                    lblErrorMessage.Text = "An error occurred while updating the assessment. Error details: " + ex.Message;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }
        }
        private void DeleteExam(int AssessmentId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Assessment WHERE AssessmentId = @AssessmentId", Con);
                cmd.Parameters.AddWithValue("@AssessmentId", AssessmentId);
                cmd.ExecuteNonQuery();
                Response.Redirect("Assessment.aspx?deleteSuccess=true");
            }
        }

        private void ClearControls()
        {
            txtAssessmentTitle.Text = string.Empty;
            txtAssessmentWeight.Text = string.Empty;
            ddlAssessmentStatus.SelectedIndex = 0;
            ddlTerm.SelectedIndex = 0;
           ddlSubject.SelectedIndex = 0;
        }
    }
}
