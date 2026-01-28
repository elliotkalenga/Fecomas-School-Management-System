using DocumentFormat.OpenXml.Spreadsheet;
using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class SchemesOfWorkAdd : System.Web.UI.Page
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
                if (Request.QueryString["SchemeId"] != null)
                {
                    int SchemeId = int.Parse(Request.QueryString["SchemeId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteRecord(SchemeId);
                    }
                    else
                    {
                        LoadRecordData(SchemeId);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["SchemeId"] != null)
            {
                btnSubmit.Text = "Update";
                ddlTerm.Enabled = false;
                ddlCompleteStatus.Enabled = true;
                txtSchemeEvaluation.Enabled = true;
                txtCompletedDate.Enabled = true;
             



            }
            else
            {
                btnSubmit.Text = "Add";
                txtSchemeEvaluation.Enabled= false;
                txtCompletedDate.Enabled = false;
                ddlTerm.Enabled = false;
                ddlCompleteStatus.Enabled = false;

            }
        }
        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string SubjectQry = @"Select 0 as SubjectId, '------- Select Subject ------' As SubjectName Union SELECT 
    sa.AllocationId,
    s.SubjectName + '-' + C.ClassName AS Subjectname
FROM 
    SubjectAllocation sa WITH (NOLOCK)
INNER JOIN 
    Subject s WITH (NOLOCK) ON sa.SubjectId = s.SubjectId
INNER JOIN 
    Class c WITH (NOLOCK) ON sa.ClassId = c.ClassId
INNER JOIN 
    Term t WITH (NOLOCK) ON sa.termId = t.TermId AND t.status = 2 AND t.SchoolId = sa.SchoolId
WHERE 
    sa.SchoolId =@SchoolId";
                string TermQry = @"Select T.TermId,Tn.TermNumber+' ('+F.FinancialYear+')' as Term
from  Term AS T INNER JOIN TermNumber AS TN ON T.Term = TN.TermId INNER JOIN FinancialYear AS F ON T.YearId = F.FinancialYearId
 where T.SchoolId=@SchoolId and T.status=2";

                Con.Open();
                PopulateDropDownList(Con, SubjectQry, ddlSubject, "SubjectName", "SubjectId");
                PopulateDropDownList(Con, TermQry, ddlTerm, "Term", "TermId");
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

        private void LoadRecordData(int SchemeId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM SchemesOfWork WHERE SchemeId = @SchemeId", Con);
                cmd.Parameters.AddWithValue("@SchemeId", SchemeId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    txtRemarks.Text = dr["Remarks"].ToString();
                    txtReferences.Text = dr["SchemeReferences"].ToString();
                    txtLearningObjectives.Text = dr["LearningObjectives"].ToString();
                    txtLearningOutcome.Text = dr["LearningOutcome"].ToString();
                    txtResources.Text = dr["Resources"].ToString();
                    txtSchemeEvaluation.Text = dr["SchemeEvaluation"].ToString();
                    txtTopic.Text = dr["Topic"].ToString();
                    txtLessons.Text = dr["Lessons"].ToString();
                    ddlTerm.SelectedValue = dr["TermId"].ToString();
                   ddlSubject.SelectedValue = dr["SubjectId"].ToString();
                    ddlWeekNo.SelectedValue = dr["WeekNo"].ToString();
                    ddlCompleteStatus.SelectedValue = dr["CompleteStatus"].ToString();



                    // Check for DBNull before converting to string
                    if (!dr.IsDBNull(dr.GetOrdinal("DateCompleted")))
                    {
                        txtCompletedDate.Text = Convert.ToDateTime(dr["DateCompleted"]).ToString("yyyy-MM-dd"); // format date as desired
                    }
                    else
                    {
                        txtCompletedDate.Text = string.Empty; // or some default value
                    }
                }
                dr.Close();
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["SchemeId"] != null)
            {
                int SchemeId = int.Parse(Request.QueryString["SchemeId"]);
                UpdateRecord(SchemeId);
            }
            else
            {
                AddNewRecord();
            }
            ClearControls();
        }

        private void AddNewRecord()
        {
            if (Session["Permissions"] != null && Session["RoleId"] != null && Session["SchoolId"] != null && Session["Username"] != null)
            {
                List<string> userPermissions = (List<string>)Session["Permissions"];
                int roleId = Convert.ToInt32(Session["RoleId"]);

                if (userPermissions.Contains("Manage_Schemes_Of_Work"))
                {
                    try
                    {
                        // Check if the user has selected a value from each dropdown
                        if (ddlSubject.SelectedValue == "0")
                        {
                            lblerror.Text = "Please select Subject.";
                            return;
                        }



                        using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                        {
                            con.Open();
                            string query = "INSERT INTO SchemesOfWork (Remarks,SchemeReferences,SubjectId,TermId,WeekNo,Topic,Lessons,Resources,LearningObjectives,LearningOutcome,SchemeEvaluation,SchoolId,Createdby) " +
                                            "VALUES (@Remarks,@SchemeReferences,@SubjectId,@TermId,@WeekNo,@Topic,@Lessons,@Resources,@LearningObjectives,@LearningOutcome,@SchemeEvaluation,@SchoolId,@Createdby)";
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text);
                                cmd.Parameters.AddWithValue("@Schemereferences", txtReferences.Text);
                                cmd.Parameters.AddWithValue("@LearningObjectives", txtLearningObjectives.Text);
                                cmd.Parameters.AddWithValue("@LearningOutcome", txtLearningOutcome.Text);
                                cmd.Parameters.AddWithValue("@SchemeEvaluation", txtSchemeEvaluation.Text);
                                cmd.Parameters.AddWithValue("@Topic", txtTopic.Text);
                                cmd.Parameters.AddWithValue("@Resources", txtResources.Text);
                                cmd.Parameters.AddWithValue("@WeekNo", ddlWeekNo.SelectedValue);
                                cmd.Parameters.AddWithValue("@SubjectId", ddlSubject.SelectedValue);
                                cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                                cmd.Parameters.AddWithValue("@Lessons", int.Parse(txtLessons.Text));
                                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                                cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                                // Log the query and parameters
                                string logMessage = $"Executing query: {query} with parameters: ";
                                foreach (SqlParameter param in cmd.Parameters)
                                {
                                    logMessage += $"{param.ParameterName} = {param.Value}, ";
                                }
                                // Log the message

                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    lblMessage.Text = "Scheme of Work Record added successfully!";
                                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                                }
                                else
                                {
                                    lblErrorMessage.Text = "No rows were affected.";
                                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                                }
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        lblErrorMessage.Text = "Error adding  Schemes of work Please try again. " + ex.Message;
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                    catch (Exception ex)
                    {
                        lblErrorMessage.Text = "An error occurred. " + ex.Message;
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                }
                else
                {
                    lblErrorMessage.Text = "Access denied! You do not have permission to perform this action.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }
            else
            {
                lblErrorMessage.Text = "Session expired or invalid. Please login again.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }
        private void UpdateRecord(int SchemeId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"UPDATE SchemesOfWork SET SubjectId = @SubjectId, WeekNo = @WeekNo, 
                                            Remarks=@Remarks,schemereferences=@SchemeReferences,
                                            Topic = @Topic, Lessons = @Lessons, Resources = @Resources, LearningObjectives = @LearningObjectives,
                                            LearningOutcome = @LearningOutcome, SchemeEvaluation = @SchemeEvaluation,DateCompleted = @DateCompleted,CompleteStatus=@CompleteStatus
                                            WHERE SchemeId = @SchemeId";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text);
                    cmd.Parameters.AddWithValue("@Schemereferences", txtReferences.Text);
                    cmd.Parameters.AddWithValue("@LearningObjectives", txtLearningObjectives.Text);
                    cmd.Parameters.AddWithValue("@LearningOutcome", txtLearningOutcome.Text);
                    cmd.Parameters.AddWithValue("@SchemeEvaluation", txtSchemeEvaluation.Text);
                    cmd.Parameters.AddWithValue("@Topic", txtTopic.Text);
                    cmd.Parameters.AddWithValue("@Resources", txtResources.Text);
                    cmd.Parameters.AddWithValue("@WeekNo", ddlWeekNo.SelectedValue);
                    cmd.Parameters.AddWithValue("@SubjectId", ddlSubject.SelectedValue);
                    cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                    cmd.Parameters.AddWithValue("@CompleteStatus", ddlCompleteStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@Lessons", int.Parse(txtLessons.Text));
                    cmd.Parameters.AddWithValue("@DateCompleted", DateTime.Parse(txtCompletedDate.Text));
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                    cmd.Parameters.AddWithValue("@SchemeId", SchemeId);
                    cmd.ExecuteNonQuery();
                    ClearControls();
                    SetButtonText();
                }
                lblMessage.Text = "Schemes of work Record updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating Asset. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteRecord(int SchemeId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Schemesofwork WHERE SchemeId = @SchemeId", Con);
                cmd.Parameters.AddWithValue("@SchemeId", SchemeId);
                cmd.ExecuteNonQuery();
                Response.Redirect("Schemesofwork.aspx?deleteSuccess=true");
            }
        }

        private void ClearControls()
        {
            txtCompletedDate.Text = string.Empty;
           txtLearningObjectives.Text = string.Empty;
            txtLearningOutcome.Text = string.Empty;
            txtLessons.Text = string.Empty;
            txtResources.Text = string.Empty;
            txtSchemeEvaluation.Text = string.Empty;
            txtTopic.Text = string.Empty;
            ddlTerm.SelectedIndex = 0;
            ddlWeekNo.SelectedIndex = 0;
           ddlSubject.SelectedIndex = 0;
            ddlCompleteStatus.SelectedIndex = 0;
        }
    }
}
