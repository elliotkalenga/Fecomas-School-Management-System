using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SMSWEBAPP.Views.Admin.Exams;
using static SMSWEBAPP.Views.Admin.Scores;

namespace SMSWEBAPP.Views.Admin
{
    public partial class SubjectAllocationAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                BindTeacherDropdown();
                BindClassDropdown();
                BindSubjectDropdown();
                BindTermDropdown();
                SetButtonText();

                if (Request.QueryString["AllocationId"] != null)
                {
                    int AllocationId = int.Parse(Request.QueryString["AllocationId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteAllocation(AllocationId);
                    }
                    else
                    {
                        LoadAllocationData(AllocationId);
                    }
                }
            }
        }

        private void BindTermDropdown()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"
                select T.TermId, TN.TermNumber+'('+y.FinancialYear+')'  as Term from term T
                                    inner Join TermNumber TN on T.Term=TN.Termid
                                    inner Join FinancialYear y on T.YearId=y.FinancialYearId where T.Status=2 and T.SchoolId=@SchoolId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@SchoolId", SqlDbType.Int).Value = Session["SchoolId"];

                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            ddlTerm.DataSource = dr;
                            ddlTerm.DataTextField = "Term";
                            ddlTerm.DataValueField = "TermId";
                            ddlTerm.DataBind();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle exception (log it, show message to user, etc.)
                // Optionally, you can log the exception details here.
                Console.WriteLine(ex.Message);
            }
        }

        private void BindTeacherDropdown()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"
                Select UserId as TeacherId,FirstName+' '+LastName+' '+'('+'Teacher'+')' as Teacher
                                    from Users u inner join roles r on u.roleid=r.roleid
									Where Status=2  and u.School=@SchoolId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@SchoolId", SqlDbType.Int).Value = Session["SchoolId"];

                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            ddlTeacher.DataSource = dr;
                            ddlTeacher.DataTextField = "Teacher";
                            ddlTeacher.DataValueField = "TeacherId";
                            ddlTeacher.DataBind();
                        }
                    }
                }

                ddlTeacher.Items.Insert(0, new ListItem("-- Select Teacher --", "0"));
            }
            catch (Exception ex)
            {
                // Handle exception (log it, show message to user, etc.)
                // Optionally, you can log the exception details here.
                Console.WriteLine(ex.Message);
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["AllocationId"] != null)
            {
                btnSubmit.Text = "Update";
            }
            else
            {
                btnSubmit.Text = "Add";
            }
        }
        private void DeleteAllocation(int AllocationId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM SubjectAllocation WHERE AllocationId = @AllocationId", Con);
                cmd.Parameters.AddWithValue("@AllocationId", AllocationId);
                cmd.ExecuteNonQuery();
                Response.Redirect("SubjectAllocation.aspx?deleteSuccess=true");
            }
        }
   
        private void LoadAllocationData(int AllocationId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM SubjectAllocation WHERE AllocationId = @AllocationId", Con);
                cmd.Parameters.AddWithValue("@AllocationId", AllocationId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    ddlSubject.SelectedValue = dr["SubjectId"].ToString();
                    ddlClass.SelectedValue = dr["ClassId"].ToString();
                    ddlTeacher.SelectedValue = dr["TeacherId"].ToString();
                    ddlTerm.SelectedValue = dr["TermId"].ToString();
                }
                dr.Close();
            }
        }

        private void BindClassDropdown()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"Select ClassId,ClassNAme From Class Where SchoolId=@SchoolId";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                Con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                ddlClass.DataSource = dr;
                ddlClass.DataTextField = "ClassName";
                ddlClass.DataValueField = "ClassId";
                ddlClass.DataBind();
                dr.Close();
            }
            ddlClass.Items.Insert(0, new ListItem("-- Select Class --", "0"));

        }

        private void BindSubjectDropdown()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"
                SELECT SubjectId, SubjectName  From Subject Where SchoolId=@SchoolId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@SchoolId", SqlDbType.Int).Value = Session["SchoolId"];

                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            ddlSubject.DataSource = dr;
                            ddlSubject.DataTextField = "SubjectName";
                            ddlSubject.DataValueField = "SubjectId";
                            ddlSubject.DataBind();
                        }
                    }
                }

                ddlSubject.Items.Insert(0, new ListItem("-- Select Subject --", "0"));
            }
            catch (Exception ex)
            {
                // Handle exception (log it, show message to user, etc.)
                // Optionally, you can log the exception details here.
                Console.WriteLine(ex.Message);
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["AllocationId"] != null)
            {
                int AllocationId = int.Parse(Request.QueryString["AllocationId"]);
                UpdateAllocation(AllocationId);
            }
            else
            {
                AddNewAllocation();
            }
            ClearControls();


        }

        private void ClearControls()
        {
            ddlTeacher.SelectedIndex = 0;
            ddlSubject.SelectedIndex = 0;
        }

        private void AddNewAllocation()
        {

            if (ddlClass.SelectedValue == "0")
            {
                lblErrorMessage.Text = "Please select a Class";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return;
            }


            if (ddlSubject.SelectedValue == "0")
            {
                lblErrorMessage.Text = "Please select a Subject";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return;
            }


            if (ddlTeacher.SelectedValue == "0")
            {
                lblErrorMessage.Text = "Please select a Teacher";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return;
            }

            if (ddlTerm.SelectedValue == "0")
            {
                lblErrorMessage.Text = "Please select a Term";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return;
            }

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                try
                {
                    string query = @"INSERT INTO SubjectAllocation
                            (SubjectId, TeacherId, TermId, ClassId, CreatedBy, SchoolId) 
                            VALUES(@SubjectId, @TeacherId, @TermId, @ClassId, @CreatedBy, @SchoolId)";

                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@SubjectId", ddlSubject.SelectedValue);
                    cmd.Parameters.AddWithValue("@TeacherId", ddlTeacher.SelectedValue);
                    cmd.Parameters.AddWithValue("@Termid", ddlTerm.SelectedValue);
                    cmd.Parameters.AddWithValue("@Classid", ddlClass.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                    Con.Open();
                    cmd.ExecuteNonQuery();

                    lblMessage.Text = "Allocation saved successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);

                    // Clear fields after submission
                    ClearControls();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627) // Unique constraint error
                    {
                        lblErrorMessage.Text = "Duplicate entry detected. Subject has already been Allocated! Please allocate another Subject";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                    else
                    {
                        lblErrorMessage.Text = "An error occurred while saving the score. Please try again later.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                }
            }
        }

        private void UpdateAllocation(int AllocationId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"UPDATE SubjectAllocation SET
                                ClassId = @ClassId,
                                TeacherId=@TeacherId,
                                SubjectId = @SubjectId
                             WHERE AllocationId = @AllocationId";

                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@AllocationId", AllocationId);
                    cmd.Parameters.AddWithValue("@SubjectId", ddlSubject.SelectedValue);
                    cmd.Parameters.AddWithValue("@TeacherId", ddlTeacher.SelectedValue);
                    cmd.Parameters.AddWithValue("@Classid", ddlClass.SelectedValue);

                    Con.Open();
                    cmd.ExecuteNonQuery();

                    ClearControls();
                    SetButtonText();
                    lblMessage.Text = "Allocation Updated successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Unique constraint violation (duplicate entry)
                {
                    lblErrorMessage.Text = "Duplicate entry detected. Subject has already been allocated! Please allocate another Subject.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
                else
                {
                    lblErrorMessage.Text = "An error occurred while saving the allocation. Please try again later.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }
        }

    }
}
