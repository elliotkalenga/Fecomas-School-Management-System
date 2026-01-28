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
    public partial class PasswordResetAdd : System.Web.UI.Page
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

                if (Request.QueryString["UserId"] != null)
                {
                    int UserId = int.Parse(Request.QueryString["UserId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                    }
                    else
                    {
                        LoadRecordData(UserId);
                    }
                }
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
                    txtUserName.Text = dr["UserName"].ToString();

                }
                dr.Close();
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text) ||
                string.IsNullOrWhiteSpace(TxtNewPassword.Text) ||
                string.IsNullOrWhiteSpace(TxtConfirmPassword.Text))
            {
                lblErrorMessage.Text = "All fields are required.";
                lblErrorMessage.Visible = true;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return;
            }

            if (TxtNewPassword.Text != TxtConfirmPassword.Text)
            {
                lblErrorMessage.Text = "Passwords do not match.";
                lblErrorMessage.Visible = true;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return;
            }

            // Encrypt the password before passing it
            string encryptedPassword = SecureData.EncryptData(TxtNewPassword.Text.Trim());

            ResetPassword(txtUserName.Text.Trim(), encryptedPassword);
        }

        private void ResetPassword(string username, string newPassword)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                try
                {
                    string query = "UPDATE Users SET Password=@Password WHERE UserName=@UserName";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@Password", SecureData.EncryptData(TxtNewPassword.Text.Trim()));

                    Con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        lblMessage.Text = "Password reset successfully!";
                        lblMessage.Visible = true;
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                    }
                    else
                    {
                        lblErrorMessage.Text = "Username not found.";
                        lblErrorMessage.Visible = true;
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                }
                catch (Exception ex)
                {
                    lblErrorMessage.Text = "An error occurred: " + ex.Message;
                    lblErrorMessage.Visible = true;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }
        }
        private void ClearControls()
        {
            txtUserName.Text = "";
        }




    }
}
