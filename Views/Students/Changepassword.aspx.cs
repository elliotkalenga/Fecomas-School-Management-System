using SMSWEBAPP.DAL;
using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace SMSWEBAPP.Views.Students
{
    public partial class Changepassword : System.Web.UI.Page
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
            int studentId = Convert.ToInt32(Session["UserId"]);
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

                string query = "SELECT Password FROM Student WHERE StudentID = @StudentID";
                using (SqlCommand cmd = new SqlCommand(query, Con))
                {
                    cmd.Parameters.Add("@StudentID", Session["StudentId"]);
                    string dbPassword = cmd.ExecuteScalar()?.ToString();

                    if (dbPassword != currentPassword) // Compare passwords
                    {
                        lblMessage.Text = "Current password is incorrect!";
                        return;
                    }
                }

                // Update the password in the database
                query = "UPDATE Student SET Password = @NewPassword WHERE StudentID = @StudentID";
                using (SqlCommand cmd = new SqlCommand(query, Con))
                {
                    cmd.Parameters.Add("@NewPassword", System.Data.SqlDbType.NVarChar).Value = newPassword;
                    cmd.Parameters.Add("@StudentID", Session["StudentId"]);

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
