using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SMSWEBAPP.Views.Admin.Exams;

namespace SMSWEBAPP.Views.Admin
{
    public partial class UsersAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                BindRoleDropdown();
                BindStatusDropdown();
                SetButtonText();

                if (Request.QueryString["UserId"] != null)
                {
                    int UserId = int.Parse(Request.QueryString["UserId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteRecord(UserId);
                    }
                    else
                    {
                        LoadRecordData(UserId);
                    }
                }
            }
        }

        private void BindStatusDropdown()
        {

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"Select 0 as StatusID, '---Select Status ---' as Status Union Select StatusId,Status from Status ";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                Con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                ddlStatus.DataSource = dr;
                ddlStatus.DataTextField = "Status";
                ddlStatus.DataValueField = "StatusId";
                ddlStatus.DataBind();
                dr.Close();
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["UserId"] != null)
            {
                btnSubmit.Text = "Update";
            }
            else
            {
                btnSubmit.Text = "Add";
            }
        }
        private void DeleteRecord(int UserId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE UserId = @UserId", Con);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.ExecuteNonQuery();
                Response.Redirect("Users.aspx?deleteSuccess=true");
            }
        }

        private void LoadRecordData(int UserId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM users WHERE UserId = @UserId", Con);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    txtFirstName.Text = dr["FirstName"].ToString();
                    txtLastName.Text = dr["LastName"].ToString();
                    txtUserName.Text = dr["UserName"].ToString();
                    ddlRole.SelectedValue = dr["RoleId"].ToString();
                    ddlStatus.SelectedValue = dr["Status"].ToString();
                    TxtPassword.Text = dr["Password"].ToString();

                }
                dr.Close();
            }
        }

        private void BindRoleDropdown()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"Select 0 as RoleId, '--- Select Role ---' as RoleTitle Union  Select RoleId,RoleTitle from Roles Where School=@SchoolId";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                Con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                ddlRole.DataSource = dr;
                ddlRole.DataTextField = "RoleTitle";
                ddlRole.DataValueField = "RoleId";
                ddlRole.DataBind();
                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["UserId"] != null)
            {
                int UserId = int.Parse(Request.QueryString["UserId"]);
                UpdateScore(UserId);
            }
            else
            {
                AddNewRecord();
            }
            ClearControls();


        }

        private void ClearControls()
        {
            ddlRole.SelectedIndex = 0;
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtUserName.Text = "";
            TxtPassword.Text = "";
        }

private void AddNewRecord()
{
    // Validate dropdown selections
    if (ddlRole.SelectedValue == "0")
    {
        lblErrorMessage.Text = "Please select a valid Role.";
        lblErrorMessage.Visible = true;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return; // Stop execution here to prevent clearing fields
    }

    if (ddlStatus.SelectedValue == "0")
    {
        lblErrorMessage.Text = "Please select a valid Status.";
        lblErrorMessage.Visible = true;
        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return; // Stop execution here to prevent clearing fields
    }

    int RoleId = int.Parse(ddlRole.SelectedValue);
    int status = int.Parse(ddlStatus.SelectedValue);
    string FirstName = txtFirstName.Text.Trim();
    string LastName = txtLastName.Text.Trim();
    string UserName = txtUserName.Text.Trim();
    string Password = SecureData.EncryptData(TxtPassword.Text.Trim()); 

    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
    {
        try
        {
            string query = @"INSERT INTO Users
                                            (Firstname, Lastname, UserName, Password, RoleId, Createdby, School, Status,SystemId)
                                         VALUES
                                            (@Firstname, @Lastname, @UserName, @Password, @RoleId, @Createdby, @School, @Status,@SystemId)";

            SqlCommand cmd = new SqlCommand(query, Con);
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@FirstName", FirstName);
            cmd.Parameters.AddWithValue("@LastName", LastName);
            cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@Password", SecureData.EncryptData(TxtPassword.Text.Trim()));
                    cmd.Parameters.AddWithValue("@School", Session["SchoolId"]);
                    cmd.Parameters.AddWithValue("@SYstemId", Session["SystemId"]);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

            Con.Open();
            cmd.ExecuteNonQuery();

            lblMessage.Text = "Record saved successfully!";
            lblMessage.Visible = true;
            ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);

            // Clear fields only after successful submission
            ClearControls();
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2627) // Unique constraint error
            {
                lblErrorMessage.Text = "Duplicate entry detected. UserName already exists.";
            }
            else
            {
                lblErrorMessage.Text = "An error occurred while saving the record. Please try again later. " + ex.Message;
            }
            lblErrorMessage.Visible = true;
            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
        }
    }
}

        private void UpdateScore(int UserId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"UPDATE Users SET 
                                    Firstname=@Firstname,
                                    Lastname=@Lastname,
                                    Username=@Username,
                                    Status=@Status,
                                    RoleId=@Roleid
                                   WHERE UserId=@UserId";

                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                cmd.Parameters.AddWithValue("@UserName", txtUserName.Text);
                cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@Roleid", ddlRole.SelectedValue);
                cmd.Parameters.AddWithValue("@UserId", UserId);

                Con.Open();
                cmd.ExecuteNonQuery();
                ClearControls();
                SetButtonText();
                lblMessage.Text = "Record Updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);

            }
        }


    }
}
