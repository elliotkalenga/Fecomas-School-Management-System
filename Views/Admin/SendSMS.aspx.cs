using System;
using System.Data.SqlClient;
using System.Net;
using System.Drawing;
using SMSWEBAPP.Services;
using SMSWEBAPP.DAL; // Assuming your DB connection class is here

namespace SMSWEBAPP.Views.Admin
{
    public partial class SendSMS : System.Web.UI.Page
    {
        private SmsService _smsService = new SmsService();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if the user is logged in
            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }
        }

        protected async void btnSendSms_Click(object sender, EventArgs e)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback += (sender2, cert, chain, sslPolicyErrors) => true;

                // Check if SchoolCode is present
                if (Session["SchoolCode"] == null)
                {
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text = "Session expired. Please log in again.";
                    return;
                }

                string schoolCode = Session["SchoolCode"].ToString();

                using (SqlConnection conn = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    await conn.OpenAsync();

                    // Step 1: Check SMSStatus for the school
                    string smsStatusQuery = "SELECT SMSStatus FROM School WHERE SchoolCode = @SchoolCode";
                    using (SqlCommand smsStatusCmd = new SqlCommand(smsStatusQuery, conn))
                    {
                        smsStatusCmd.Parameters.AddWithValue("@SchoolCode", schoolCode);
                        object statusResult = await smsStatusCmd.ExecuteScalarAsync();

                        if (statusResult == null || Convert.ToInt32(statusResult) != 1)
                        {
                            lblStatus.ForeColor = Color.Red;
                            lblStatus.Text = "SMS sending is disabled for your school. Please contact the System Administrator.";
                            return;
                        }
                    }
                }

                // Continue with sending SMS
                string rawNumbers = txtPhoneNumber.Text.Trim();
                string message = txtMessage.Text.Trim();

                string[] phoneNumbers = rawNumbers.Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                if (phoneNumbers.Length == 0)
                {
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text = "No valid phone numbers provided.";
                    return;
                }

                string token = await _smsService.GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    string result = await _smsService.SendSmsAsync(token, phoneNumbers, message);
                    lblStatus.ForeColor = Color.Black;
                    lblStatus.Text = "SMS Sent! Response: " + result;
                }
                else
                {
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text = "Failed to get access token.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = "Error: " + ex.Message;
            }
        }
    }
}
