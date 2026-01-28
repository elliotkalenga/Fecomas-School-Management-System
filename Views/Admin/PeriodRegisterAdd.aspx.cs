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
    public partial class PeriodRegisterAdd : System.Web.UI.Page
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
                if (Request.QueryString["RegisterId"] != null)
                {
                    int RegisterId = int.Parse(Request.QueryString["RegisterId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteRecord(RegisterId);
                    }
                    else
                    {
                        LoadRecordData(RegisterId);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            btnSubmit.Text = Request.QueryString["RegisterId"] != null ? "Update" : "Add";

            bool isEditMode = Request.QueryString["RegisterId"] != null;

           // txtLessonEvaluation.Enabled = isEditMode && HasPermissionToApproveLessonPlan();
          //  ddlCheckStatus.Enabled = isEditMode && HasPermissionToApproveLessonPlan();
           // txtCheckdDate.Enabled = isEditMode && HasPermissionToApproveLessonPlan();

            if (!isEditMode)
            {
         //       txtLessonEvaluation.Enabled = false;
         //       txtCheckdDate.Enabled = false;
        //        ddlCheckStatus.Enabled = false;
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
                string LessonQry = @" 
            SELECT 
                Lp.LessonId,
                LP.WeekNo + ' '+Tn.TermNumber + ' (' + F.FinancialYear + ' ' +S.SubjectName + '-' +u.FirstName+' '+LastName+' - '+C.ClassName + ' ' +sc.Topic as Lesson
            FROM  
                LessonPlan lp  WITH (NOLOCK) 
            INNER JOIN 
                Schemesofwork sc WITH (NOLOCK) on lp.schemeid=sc.schemeid 
            INNER JOIN 
                Subjectallocation sa WITH (NOLOCK) on sc.subjectId = sa.allocationId 
            Inner join users u on sa.teacherid=u.userid
            INNER JOIN 
                Class c on sa.classId = c.ClassId  Inner join ClassStream cs on cs.ClassId=c.Classid
            INNER JOIN 
                Subject s WITH (NOLOCK) on sa.subjectid = s.subjectId 
            INNER JOIN 
                Term AS T WITH (NOLOCK) ON Sc.TermId = T.TermId
            INNER JOIN 
                TermNumber AS TN WITH (NOLOCK) ON T.Term = TN.TermId
            INNER JOIN 
                FinancialYear AS F WITH (NOLOCK) ON T.YearId = F.FinancialYearId
            WHERE 
                sc.Schoolid =@SchoolId  and T.status = 2
        ";

                string classstream = @"  
            select cs.streamId, StreamName+' ('+c.className+')' as ClassStream 
            from Classstream cs with (Nolock) 
            inner join class c on cs.classid=c.classid
            where cs.schoolid=@SchoolId
        ";

                string TeacherQry = @"  
            SELECT 
                sa.AllocationId as TeacherId,
                u.FirstName+' '+LastName+'-' +S.SubjectName + ' - '+C.ClassName +'-' +Tn.TermNumber + ' (' + F.FinancialYear +')'as Teacher
            FROM 
                Subjectallocation sa WITH (NOLOCK) 
            Inner join users u on sa.teacherid=u.userid
            INNER JOIN 
                Class c on sa.classId = c.ClassId  Inner join ClassStream cs on cs.ClassId=c.Classid
            INNER JOIN 
                Subject s WITH (NOLOCK) on sa.subjectid = s.subjectId 
            INNER JOIN 
                Term AS T WITH (NOLOCK) ON Sa.TermId = T.TermId
            INNER JOIN 
                TermNumber AS TN WITH (NOLOCK) ON T.Term = TN.TermId
            INNER JOIN 
                FinancialYear AS F WITH (NOLOCK) ON T.YearId = F.FinancialYearId
            WHERE 
                sa.Schoolid =@SchoolId  and T.status = 2
        ";

                Con.Open();
                PopulateDropDownList(Con, LessonQry, ddlLessonPlan, "Lesson", "LessonId", "-------Select Lesson Plan ------");
                PopulateDropDownList(Con, classstream, ddlClassStream, "ClassStream", "streamId", "-------Select Class Stream------");
                PopulateDropDownList(Con, TeacherQry, ddlTeacher, "Teacher", "TeacherId", "-------Select Teacher------");
            }
        }

        private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField, string defaultText)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddl.DataSource = dt;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem(defaultText, ""));
        }
        private void LoadRecordData(int RegisterId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM PeriodRegister WHERE RegisterId = @RegisterId", Con);
                cmd.Parameters.AddWithValue("@RegisterId", RegisterId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    txtClassAttendance.Text = dr["Attendance"].ToString();
                    txtPeriodNumber.Text = dr["PeriodNumber"].ToString();
                    ddlClassStream.SelectedValue = dr["StreamId"].ToString();
                    ddlLessonPlan.SelectedValue = dr["LessonPlanId"].ToString();
                    ddlTeacher.SelectedValue = dr["TeacherId"].ToString();



                    // Check for DBNull before converting to string
                    if (!dr.IsDBNull(dr.GetOrdinal("DeliveryFrom")))
                    {
                        dtpDeliveryTimeFrom.Text = Convert.ToDateTime(dr["DeliveryFrom"]).ToString("yyyy-MM-ddTHH:mm:ss"); // format date as desired
                    }
                    else
                    {
                        dtpDeliveryTimeFrom.Text = string.Empty; // or some default value
                    }

                    if (!dr.IsDBNull(dr.GetOrdinal("DeliveryTo")))
                    {
                        dtpDeliveryTimeTo.Text = Convert.ToDateTime(dr["DeliveryTo"]).ToString("yyyy-MM-ddTHH:mm:ss"); // format date as desired
                    }
                    else
                    {
                        dtpDeliveryTimeTo.Text = string.Empty; // or some default value
                    }
                }
                dr.Close();
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["RegisterId"] != null)
            {
                int RegisterId = int.Parse(Request.QueryString["RegisterId"]);
                UpdateRecord(RegisterId);
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

                if (userPermissions.Contains("Manage_Period_Register"))
                {
                    try
                    {
                        // Check if the user has selected a value from each dropdown
                        if (ddlClassStream.SelectedValue == "0")
                        {
                            lblerror.Text = "Please select Class Stream.";
                            return;
                        }
                        if (ddlLessonPlan.SelectedValue == "0")
                        {
                            lblerror.Text = "Please select Lesson Plan.";
                            return;
                        }
                        if (ddlTeacher.SelectedValue == "0")
                        {
                            lblerror.Text = "Please select Teacher";
                            return;
                        }

                        using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                        {
                            con.Open();
                            string query = @"INSERT INTO periodRegister (Teacherid,StreamId,LessonPlanId, PeriodNumber, Attendance,Deliveryfrom,Deliveryto,SchoolId, CreatedBy)" +
                                            "VALUES " +
                                            "(@Teacherid,@StreamId,@LessonPlanId, @PeriodNumber, @Attendance,@Deliveryfrom,@Deliveryto,@SchoolId, @CreatedBy)";
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.Parameters.AddWithValue("@TeacherId", ddlTeacher.SelectedValue);
                                cmd.Parameters.AddWithValue("@PeriodNumber", int.Parse(txtPeriodNumber.Text));
                                cmd.Parameters.AddWithValue("@Attendance", int.Parse(txtClassAttendance.Text));
                                cmd.Parameters.AddWithValue("@StreamId", ddlClassStream.SelectedValue);
                                cmd.Parameters.AddWithValue("@LessonPlanId", ddlLessonPlan.SelectedValue);
                                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                                cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                                DateTime? deliveryfrom = null;
                                if (!string.IsNullOrEmpty(dtpDeliveryTimeFrom.Text) && DateTime.TryParse(dtpDeliveryTimeFrom.Text, out DateTime parsedCheckedDate))
                                {
                                    deliveryfrom = parsedCheckedDate;
                                }

                                DateTime? deliveryto = null;
                                if (!string.IsNullOrEmpty(dtpDeliveryTimeTo.Text) && DateTime.TryParse(dtpDeliveryTimeTo.Text, out DateTime parsedDeliveryTime))
                                {
                                    deliveryto = parsedDeliveryTime;
                                }

                                cmd.Parameters.Add("@Deliveryfrom", SqlDbType.DateTime).Value = (object)deliveryfrom ?? DBNull.Value;
                                cmd.Parameters.Add("@Deliveryto", SqlDbType.DateTime).Value = (object)deliveryto ?? DBNull.Value;




                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    lblMessage.Text = "Period Register Record added successfully!";
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

        private void UpdateRecord(int RegisterId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"UPDATE PeriodRegister 
Set TeacherId=@TeacherId, 
    PeriodNumber=@PeriodNumber, 
    Attendance=@Attendance,
    StreamId=@StreamId,
    LessonPlanId=@LessonPlanId,
    Deliveryfrom=@Deliveryfrom,
    Deliveryto=@Deliveryto 
Where RegisterId = @RegisterId";

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@TeacherId", ddlTeacher.SelectedValue);
                        cmd.Parameters.AddWithValue("@PeriodNumber", int.Parse(txtPeriodNumber.Text));
                        cmd.Parameters.AddWithValue("@Attendance", int.Parse(txtClassAttendance.Text));
                        cmd.Parameters.AddWithValue("@StreamId", ddlClassStream.SelectedValue);
                        cmd.Parameters.AddWithValue("@LessonPlanId", ddlLessonPlan.SelectedValue);
                        cmd.Parameters.AddWithValue("@RegisterId", RegisterId);


                        DateTime? deliveryfrom = null;
                        if (!string.IsNullOrEmpty(dtpDeliveryTimeFrom.Text) && DateTime.TryParse(dtpDeliveryTimeFrom.Text, out DateTime parsedCheckedDate))
                        {
                            deliveryfrom = parsedCheckedDate;
                        }

                        DateTime? deliveryto = null;
                        if (!string.IsNullOrEmpty(dtpDeliveryTimeTo.Text) && DateTime.TryParse(dtpDeliveryTimeTo.Text, out DateTime parsedDeliveryTime))
                        {
                            deliveryto = parsedDeliveryTime;
                        }

                        cmd.Parameters.Add("@Deliveryfrom", SqlDbType.DateTime).Value = (object)deliveryfrom ?? DBNull.Value;
                        cmd.Parameters.Add("@Deliveryto", SqlDbType.DateTime).Value = (object)deliveryto ?? DBNull.Value;




                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            lblMessage.Text = "Period Register Record updated successfully!";
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
        private void DeleteRecord(int RegisterId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM PeriodRegister WHERE RegisterId = @RegisterId", Con);
                    cmd.Parameters.AddWithValue("@RegisterId", RegisterId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        lblMessage.Text = "Record deleted successfully!";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                        Response.Redirect("PeriodRegisster.aspx?deleteSuccess=true");
                    }
                    else
                    {
                        lblErrorMessage.Text = "No records found with the given RegisterId or record deletion failed.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                }
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error deleting record. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "An error occurred. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }
        private void ClearControls()
        {
            dtpDeliveryTimeTo.Text = string.Empty;
            dtpDeliveryTimeFrom.Text = string.Empty;
            txtClassAttendance.Text = string.Empty;
            txtPeriodNumber.Text = string.Empty;
            ddlClassStream.SelectedIndex = 0;
            ddlLessonPlan.SelectedIndex = 0;
            ddlTeacher.SelectedIndex = 0;
        }
    }
}
