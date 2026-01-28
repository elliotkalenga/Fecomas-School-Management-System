using SMSWEBAPP.DAL;
using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace SMSWEBAPP.Views.Admin
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null) // Ensure user is logged in
            {
                Response.Redirect("UserLogin.aspx"); // Redirect to login if not authenticated
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                lblMessage.Text = "Session expired. Please log in again.";
                return;
            }

            int UserId = Convert.ToInt32(Session["UserId"]);
            string currentPassword = txtCurrentPassword.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            // Validate that the new passwords match
            if (newPassword != confirmPassword)
            {
                lblMessage.Text = "New passwords do not match!";
                return;
            }

            // Connect to the database and validate current password
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();

                string query = "SELECT Password FROM Users WHERE UserID = @UserId";
                string dbPassword = null;

                using (SqlCommand cmd = new SqlCommand(query, Con))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        dbPassword = result.ToString();
                    }
                }

                // Ensure the password exists before comparison
                if (string.IsNullOrEmpty(dbPassword))
                {
                    lblMessage.Text = "User not found!";
                    return;
                }

                // Encrypt the user input and compare
                string encryptedCurrentPassword = SecureData.EncryptData(currentPassword);
                if (dbPassword != encryptedCurrentPassword)
                {
                    lblMessage.Text = "Current password is incorrect!";
                    return;
                }

                // Update the password in the database
                query = "UPDATE Users SET Password = @NewPassword WHERE UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(query, Con))
                {
                    cmd.Parameters.AddWithValue("@NewPassword", SecureData.EncryptData(newPassword));
                    cmd.Parameters.AddWithValue("@UserID", UserId);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        lblMessage.CssClass = "text-success";
                        lblMessage.Text = "Password changed successfully!";
                    }
                    else
                    {
                        lblMessage.Text = "Error updating password!";
                    }
                }
            }
        }
    }
}
