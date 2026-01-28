using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SMSWEBAPP.Views.Admin.Exams;

namespace SMSWEBAPP.Views.Admin
{
    public partial class ExamAdd : System.Web.UI.Page
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
                if (Request.QueryString["ExamID"] != null)
                {
                    int ExamID = int.Parse(Request.QueryString["ExamID"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteExam(ExamID);
                    }
                    else
                    {
                        LoadExamData(ExamID);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["ExamID"] != null)
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
                string ExamTypeQry = @"Select 0 as ExamTypeId, '------- Select Exam Type ------' As ExamType Union Select ExamTypeId,ExamType from ExamType";
                string TermQry = @"select T.TermId, TN.TermNumber+'('+y.FinancialYear+')'  as Term from term T
                                    inner Join TermNumber TN on T.Term=TN.Termid
                                    inner Join FinancialYear y on T.YearId=y.FinancialYearId where T.Status=2 and T.SchoolId=@SchoolId";
                string ExamLockQry = @"select 0 ReleaseExamId, '-- Select Lock Status --' as ReleaseStatus union  select ReleaseExamId,ReleaseStatus from ReleaseExam";

                Con.Open();
                PopulateDropDownList(Con, ExamTypeQry, ddlExamType, "ExamType", "ExamTypeId");
                PopulateDropDownList(Con, TermQry, ddlTerm, "Term", "TermId");
                PopulateDropDownList(Con, ExamLockQry, ddlExamLock, "ReleaseStatus", "ReleaseExamId");
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

        private void LoadExamData(int ExamID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Exam WHERE ExamID = @ExamID", Con);
                cmd.Parameters.AddWithValue("@ExamID", ExamID);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    txtExamCode.Text = dr["ExamCode"].ToString();
                    txtExamWeight.Text = dr["ExamWeight"].ToString();
                    txtExamName.Text = dr["ExamTitle"].ToString();
                    ddlExamType.SelectedValue = dr["ExamTypeId"].ToString();
                    ddlTerm.SelectedValue = dr["TermId"].ToString();
                    ddlExamLock.SelectedValue = dr["ReleasedStatus"].ToString();
                }
                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["ExamID"] != null)
            {
                int ExamID = int.Parse(Request.QueryString["ExamID"]);
                UpdateExam(ExamID);
            }
            else
            {
                AddNewExam();
            }
            ClearControls();
        }

        private void AddNewExam()
        {
            if (Session["Permissions"] != null && Session["RoleId"] != null)
            {
                List<string> userPermissions = (List<string>)Session["Permissions"];
                int roleId = Convert.ToInt32(Session["RoleId"]);


                if (userPermissions.Contains("Exam_Create"))
                {
                    try
                    {
                        using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                        {
                            Con.Open();
                            string query = "INSERT INTO Exam (ExamCode,ExamWeight, ExamTitle, ExamTypeId, TermId, ReleasedStatus, CreatedBy,SchoolId) " +
                                           "VALUES (@ExamCode,@ExamWeight, @ExamTitle, @ExamTypeId, @TermId, @ReleasedStatus, @CreatedBy,@SchoolId)";
                            SqlCommand cmd = new SqlCommand(query, Con);
                            cmd.Parameters.AddWithValue("@ExamCode", txtExamCode.Text);
                            cmd.Parameters.AddWithValue("@ExamTitle", txtExamName.Text);
                            cmd.Parameters.AddWithValue("@ExamWeight", txtExamWeight.Text);
                            cmd.Parameters.AddWithValue("@ExamTypeId", ddlExamType.SelectedValue);
                            cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                            cmd.Parameters.AddWithValue("@ReleasedStatus", ddlExamLock.SelectedValue);
                            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                            cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                            cmd.ExecuteNonQuery();
                        }
                        lblMessage.Text = "Exam added successfully!";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                    }
                    catch (SqlException ex)
                    {
                        lblErrorMessage.Text = "Error adding exam. Please try again." + ex.Message;
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                }
                else
                {
                    lblErrorMessage.Text = "ACCESS DENIED! YOU DO NOT HAVE PERMISSION TO PERFORM THIS ACTION " ;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }
        }
        private void UpdateExam(int ExamID)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = "UPDATE Exam SET ReleasedStatus=@ReleasedStatus,ExamWeight=@ExamWeight,ExamTitle=@ExamTitle," +
                        " WHERE ExamID=@ExamID";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@ReleasedStatus", ddlExamLock.SelectedValue);
                    cmd.Parameters.AddWithValue("@ExamTitle", txtExamName.Text.ToString());
                    cmd.Parameters.AddWithValue("@ExamWeight", txtExamWeight.Text.ToString());
                    cmd.Parameters.AddWithValue("@ExamID", ExamID);
                    cmd.ExecuteNonQuery();
                    ClearControls();
                    SetButtonText();
                }
                lblMessage.Text = "Exam updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating exam. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteExam(int ExamID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Exam WHERE ExamID = @ExamID", Con);
                cmd.Parameters.AddWithValue("@ExamID", ExamID);
                cmd.ExecuteNonQuery();
                Response.Redirect("Exams.aspx?deleteSuccess=true");
            }
        }

        private void ClearControls()
        {
            txtExamCode.Text = string.Empty;
            txtExamName.Text = string.Empty;
            ddlExamType.SelectedIndex = 0;
            ddlTerm.SelectedIndex = 0;
            ddlExamLock.SelectedIndex = 0;
        }
    }
}
