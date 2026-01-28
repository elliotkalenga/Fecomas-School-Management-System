using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace SMSWEBAPP.Views.Admin
{
    public partial class LessonPlansAdd : System.Web.UI.Page
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
                if (Request.QueryString["LessonId"] != null)
                {
                    int LessonId = int.Parse(Request.QueryString["LessonId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteRecord(LessonId);
                    }
                    else
                    {
                        LoadRecordData(LessonId);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            btnSubmit.Text = Request.QueryString["LessonId"] != null ? "Update" : "Add";

            bool isEditMode = Request.QueryString["LessonId"] != null;

            txtLessonEvaluation.Enabled = isEditMode && HasPermissionToApproveLessonPlan();
            ddlCheckStatus.Enabled = isEditMode && HasPermissionToApproveLessonPlan();
            txtCheckdDate.Enabled = isEditMode && HasPermissionToApproveLessonPlan();

            if (!isEditMode)
            {
                txtLessonEvaluation.Enabled = false;
                txtCheckdDate.Enabled = false;
                ddlCheckStatus.Enabled = false;
            }
        }

        private bool HasPermissionToApproveLessonPlan()
        {
            return Session["Permissions"] != null
                && ((List<string>)Session["Permissions"]).Contains("Approve_Lesson_Plan");
        }
        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string SubjectQry = @"
    SELECT 
        Sc.Schemeid,
        sc.WeekNo +'. TERM : '+Tn.TermNumber + ' (' + F.FinancialYear + '). SUBJECT:  '+S.SubjectName + '-' + C.ClassName + '. TOPIC:  ' +sc.Topic as SchemeOfWork
    FROM 
        Schemesofwork sc WITH (NOLOCK) 
    INNER JOIN 
        Subjectallocation sa WITH (NOLOCK) on sc.subjectId = sa.AllocationId
    INNER JOIN 
        Class c on sa.classId = c.ClassId 
    INNER JOIN 
        Subject s WITH (NOLOCK) on sa.subjectid = s.subjectId 
    INNER JOIN 
        Term AS T WITH (NOLOCK) ON Sc.TermId = T.TermId
    INNER JOIN 
        TermNumber AS TN WITH (NOLOCK) ON T.Term = TN.TermId
    INNER JOIN 
        FinancialYear AS F WITH (NOLOCK) ON T.YearId = F.FinancialYearId
    WHERE 
        sc.Schoolid =@SchoolId and T.status = 2

";
              

                Con.Open();
                PopulateDropDownList(Con, SubjectQry, ddlScheme, "SchemeOfWork", "SchemeId");
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
            ddl.Items.Insert(0, new ListItem("-------Select Scheme of work you want to use for Lesson Plan Creation------", "")); // Use ddl instead of ddlScheme
            dr.Close();
        }
        private void LoadRecordData(int LessonId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM LessonPlan WHERE LessonId = @LessonId", Con);
                cmd.Parameters.AddWithValue("@LessonId", LessonId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    txtLessonObjectives.Text = dr["LessonObjectives"].ToString();
                    txtLessonOutcome.Text = dr["LessonOutcome"].ToString();
                    txtResources.Text = dr["Resources"].ToString();
                    txtLessonEvaluation.Text = dr["LessonEvaluation"].ToString();
                    txtLessonTopic.Text = dr["LessonTopic"].ToString();
                    txtAssessmentCriteria.Text = dr["AssessmentCriteria"].ToString();
                    txtIntroduction.Text = dr["Introduction"].ToString();
                    txtDuration.Text = dr["Duration"].ToString();
                    txtPlannedActivities.Text = dr["PlannedActivities"].ToString();
                    txtTeachingMethods.Text = dr["TeachingMethods"].ToString();
                    ddlScheme.SelectedValue = dr["SchemeId"].ToString();
                    ddlWeekNo.SelectedValue = dr["WeekNo"].ToString();
                    ddlCheckStatus.SelectedValue = dr["CheckStatus"].ToString();



                    // Check for DBNull before converting to string
                    if (!dr.IsDBNull(dr.GetOrdinal("CheckedDate")))
                    {
                        txtCheckdDate.Text = Convert.ToDateTime(dr["CheckedDate"]).ToString("yyyy-MM-ddTHH:mm:ss"); // format date as desired
                    }
                    else
                    {
                        txtCheckdDate.Text = string.Empty; // or some default value
                    }

                    if (!dr.IsDBNull(dr.GetOrdinal("DeliveryTime")))
                    {
                        txtDeliveryTime.Text = Convert.ToDateTime(dr["DeliveryTime"]).ToString("yyyy-MM-ddTHH:mm:ss"); // format date as desired
                    }
                    else
                    {
                        txtDeliveryTime.Text = string.Empty; // or some default value
                    }
                }
                dr.Close();
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["LessonId"] != null)
            {
                int LessonId = int.Parse(Request.QueryString["LessonId"]);
                UpdateRecord(LessonId);
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

                if (userPermissions.Contains("Manage_Lesson_Plan"))
                {
                    try
                    {
                        // Check if the user has selected a value from each dropdown
                        if (ddlScheme.SelectedValue == "0")
                        {
                            lblerror.Text = "Please select Scheme of Work.";
                            return;
                        }

                        using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                        {
                            con.Open();
                            string query = "INSERT INTO LessonPlan (schemeid,deliveryTime, duration, lessonTopic, LessonObjectives, TeachingMethods, Introduction, PlannedActivities, Resources, AssessmentCriteria, LessonOutcome, LessonEvaluation, CheckStatus, SchoolId, CreatedBy, CreatedDate, WeekNo) " +
                                            "VALUES (@SchemeId,@deliveryTime, @Duration, @Topic, @LearningObjectives, @TeachingMethods, @Introduction, @PlannedActivities, @Resources, @AssessmentCriteria, @LearningOutcome, @LessonEvaluation, @CheckStatus, @SchoolId, @CreatedBy, GETDATE(), @WeekNo)";
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.Parameters.AddWithValue("@SchemeId", ddlScheme.SelectedValue);
                                cmd.Parameters.AddWithValue("@Duration", int.Parse(txtDuration.Text));
                                cmd.Parameters.AddWithValue("@Topic", txtLessonTopic.Text);
                                cmd.Parameters.AddWithValue("@LearningObjectives", txtLessonObjectives.Text);
                                cmd.Parameters.AddWithValue("@TeachingMethods", txtTeachingMethods.Text);
                                cmd.Parameters.AddWithValue("@Introduction", txtIntroduction.Text);
                                cmd.Parameters.AddWithValue("@PlannedActivities", txtPlannedActivities.Text);
                                cmd.Parameters.AddWithValue("@Resources", txtResources.Text);
                                cmd.Parameters.AddWithValue("@AssessmentCriteria", txtAssessmentCriteria.Text);
                                cmd.Parameters.AddWithValue("@LearningOutcome", txtLessonOutcome.Text);
                                cmd.Parameters.AddWithValue("@LessonEvaluation", txtLessonEvaluation.Text);
                                cmd.Parameters.AddWithValue("@CheckStatus", ddlCheckStatus.SelectedValue);
                                cmd.Parameters.AddWithValue("@WeekNo", ddlWeekNo.SelectedValue);
                                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                                cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                                DateTime? checkedDate = null;
                                if (!string.IsNullOrEmpty(txtCheckdDate.Text) && DateTime.TryParse(txtCheckdDate.Text, out DateTime parsedCheckedDate))
                                {
                                    checkedDate = parsedCheckedDate;
                                }

                                DateTime? deliveryTime = null;
                                if (!string.IsNullOrEmpty(txtDeliveryTime.Text) && DateTime.TryParse(txtDeliveryTime.Text, out DateTime parsedDeliveryTime))
                                {
                                    deliveryTime = parsedDeliveryTime;
                                }

                                cmd.Parameters.Add("@CheckedDate", SqlDbType.DateTime).Value = (object)checkedDate ?? DBNull.Value;
                                cmd.Parameters.Add("@DeliveryTime", SqlDbType.DateTime).Value = (object)deliveryTime ?? DBNull.Value;



 
                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    lblMessage.Text = "Lesson Plan Record added successfully!";
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
                        lblErrorMessage.Text = "Error adding  Lesson Plan Please try again. " + ex.Message;
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

        private void UpdateRecord(int LessonId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"UPDATE LessonPlan SET schemeid = @SchemeId, duration = @Duration, 
                lessonTopic = @Topic, LessonObjectives = @LearningObjectives, TeachingMethods = @TeachingMethods,
                Introduction = @Introduction, PlannedActivities = @PlannedActivities, Resources = @Resources,
                AssessmentCriteria = @AssessmentCriteria, LessonOutcome = @LearningOutcome, LessonEvaluation = @LessonEvaluation,
                CheckStatus = @CheckStatus, WeekNo = @WeekNo,CheckedDate = @CheckedDate,DeliveryTime=@DeliveryTime
                WHERE LessonId = @LessonId";

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@SchemeId", ddlScheme.SelectedValue);
                        cmd.Parameters.AddWithValue("@Duration", int.Parse(txtDuration.Text));
                        cmd.Parameters.AddWithValue("@Topic", txtLessonTopic.Text);
                        cmd.Parameters.AddWithValue("@LearningObjectives", txtLessonObjectives.Text);
                        cmd.Parameters.AddWithValue("@TeachingMethods", txtTeachingMethods.Text);
                        cmd.Parameters.AddWithValue("@Introduction", txtIntroduction.Text);
                        cmd.Parameters.AddWithValue("@PlannedActivities", txtPlannedActivities.Text);
                        cmd.Parameters.AddWithValue("@Resources", txtResources.Text);
                        cmd.Parameters.AddWithValue("@AssessmentCriteria", txtAssessmentCriteria.Text);
                        cmd.Parameters.AddWithValue("@LearningOutcome", txtLessonOutcome.Text);
                        cmd.Parameters.AddWithValue("@LessonEvaluation", txtLessonEvaluation.Text);
                        cmd.Parameters.AddWithValue("@CheckStatus", ddlCheckStatus.SelectedValue);
                        cmd.Parameters.AddWithValue("@WeekNo", ddlWeekNo.SelectedValue);
                        cmd.Parameters.AddWithValue("@LessonId", LessonId);


                        DateTime ? checkedDate = null;
                        if (!string.IsNullOrEmpty(txtCheckdDate.Text) && DateTime.TryParse(txtCheckdDate.Text, out DateTime parsedCheckedDate))
                        {
                            checkedDate = parsedCheckedDate;
                        }

                        DateTime? deliveryTime = null;
                        if (!string.IsNullOrEmpty(txtDeliveryTime.Text) && DateTime.TryParse(txtDeliveryTime.Text, out DateTime parsedDeliveryTime))
                        {
                            deliveryTime = parsedDeliveryTime;
                        }

                        cmd.Parameters.Add("@CheckedDate", SqlDbType.DateTime).Value = (object)checkedDate ?? DBNull.Value;
                        cmd.Parameters.Add("@DeliveryTime", SqlDbType.DateTime).Value = (object)deliveryTime ?? DBNull.Value;                            

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            lblMessage.Text = "Lesson Plan Record updated successfully!";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                            ClearControls();
                            SetButtonText();
                        }
                        else
                        {
                            lblErrorMessage.Text = "No records found with the given LessonPlanId.";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating Lesson Plan. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "An error occurred. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }
        private void DeleteRecord(int LessonId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM LessonPlan WHERE LessonId = @LessonId", Con);
                cmd.Parameters.AddWithValue("@LessonId", LessonId);
                cmd.ExecuteNonQuery();
                Response.Redirect("LessonPlans.aspx?deleteSuccess=true");
            }
        }

        private void ClearControls()
        {
            txtLessonTopic.Text = string.Empty;
            txtLessonObjectives.Text = string.Empty;
            txtLessonOutcome.Text = string.Empty;
            txtLessonEvaluation.Text = string.Empty;
            txtIntroduction.Text = string.Empty;
            txtTeachingMethods.Text = string.Empty;
            txtPlannedActivities.Text = string.Empty;
            txtResources.Text = string.Empty;
            txtAssessmentCriteria.Text = string.Empty;
            txtDuration.Text = string.Empty;
            ddlScheme.SelectedIndex = 0;
            ddlWeekNo.SelectedIndex = 0;
            ddlCheckStatus.SelectedIndex = 0;
        }
    }
}
