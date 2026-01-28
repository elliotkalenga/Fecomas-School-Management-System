using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SMSWEBAPP.Services;
using SMSWEBAPP.DAL;
using Newtonsoft.Json.Linq;

namespace SMSWEBAPP.Views.Admin
{
    public partial class SMS : System.Web.UI.Page
    {
        private SmsService _smsService = new SmsService();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if the user is logged in
            if (Session["User"] == null)
            {
                // Redirect to login page
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                // Load the student data if needed
            }
        }

        private async Task InsertSmsLog(string phone,string Student,string message)
        {
            using (SqlConnection conn = new SqlConnection(AppConnection.GetConnectionString()))
            {
                await conn.OpenAsync(); // Open the connection

                // Get the TariffId with status = 1
                string tariffQuery = "SELECT TOP 1 TariffId FROM smsTariff WHERE Status = 1";
                int tariffId = 0;
                using (SqlCommand tariffCmd = new SqlCommand(tariffQuery, conn))
                {
                    object tariffResult = await tariffCmd.ExecuteScalarAsync();
                    if (tariffResult != null)
                    {
                        tariffId = Convert.ToInt32(tariffResult);
                    }
                    else
                    {
                        // Handle the case when no tariff with status = 1 is found
                        throw new Exception("No active tariff found.");
                    }
                }

                string query = "INSERT INTO SmsLog (Message,Phone, CreatedDate, CreatedBy, SchoolId, TariffId,Student) VALUES (@message,@Phone, GETDATE(), @CreatedBy, @SchoolId, @TariffId,@Student)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Message", message);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Student", Student);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    cmd.Parameters.AddWithValue("@TariffId", tariffId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        protected async void btnSendSms_Click(object sender, EventArgs e)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback += (sender2, cert, chain, sslPolicyErrors) => true;

                // Check if SchoolCode is available in session
                if (Session["SchoolCode"] == null)
                {
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    lblStatus.Text = "Session expired. Please log in again.";
                    return;
                }

                string schoolCode = Session["SchoolCode"].ToString();

                using (SqlConnection conn = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    await conn.OpenAsync();

                    // Step 1: Check SMSStatus for the current school
                    string smsStatusQuery = "SELECT SMSStatus FROM School WHERE SchoolCode = @SchoolCode";
                    using (SqlCommand smsStatusCmd = new SqlCommand(smsStatusQuery, conn))
                    {
                        smsStatusCmd.Parameters.AddWithValue("@SchoolCode", schoolCode);
                        object statusResult = await smsStatusCmd.ExecuteScalarAsync();

                        if (statusResult == null || Convert.ToInt32(statusResult) != 1)
                        {
                            lblStatus.ForeColor = System.Drawing.Color.Red;
                            lblStatus.Text = "SMS sending is disabled for your school. Please contact the System Administrator.";
                            return;
                        }
                    }

                    // Step 2: Proceed with sending SMS
                    string query = @"SELECT enrollmentID, Phone, Student, ClassNAme, Term, TotalFees, 
                                    TotalCollected, Balance, Guardian, schoolName 
                             FROM FeesSMS 
                             WHERE SchoolCode=@SchoolCode AND Balance > 0";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SchoolCode", schoolCode);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    string token = await _smsService.GetAccessTokenAsync();
                    if (string.IsNullOrEmpty(token))
                    {
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        lblStatus.Text = "Failed to get access token.";
                        return;
                    }

                    StringBuilder summary = new StringBuilder();
                    int messagesSent = 0;

                    while (await reader.ReadAsync())
                    {
                        string phone = reader["Phone"].ToString().Trim();

                        if (!phone.StartsWith("265"))
                        {
                            summary.AppendLine($"Skipped {reader["Student"]} ({phone}): Invalid phone number<br/>");
                            continue;
                        }

                        string student = reader["Student"].ToString();
                        string className = reader["ClassNAme"].ToString();
                        string term = reader["Term"].ToString();
                        string totalFees = Convert.ToDecimal(reader["TotalFees"]).ToString("N0");
                        string totalCollected = Convert.ToDecimal(reader["TotalCollected"]).ToString("N0");
                        string balance = Convert.ToDecimal(reader["Balance"]).ToString("N0");
                        string guardian = reader["Guardian"].ToString();
                        string school = reader["schoolName"].ToString();

                        string message = $"FROM: {school}\n" +
                                         $"FEES BALANCE REMINDER FOR {student} - {className}\n" +
                                         $"TOTAL: K{totalFees}\n" +
                                         $"PAID: K{totalCollected}\n" +
                                         $"BALANCE: K{balance}\n" +
                                         $"TERM: {term}";

                        string result = await _smsService.SendSmsAsync(token, new string[] { phone }, message);
                        summary.AppendLine($"Sent to {student} ({phone}): {result}<br/>");

                        try
                        {
                            var jsonData = JObject.Parse(result);
                            if (jsonData["payload"].First["status"].ToString() == "submitted")
                            {
                                await InsertSmsLog(phone,student, message);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Error parsing JSON: " + ex.Message);
                        }

                        messagesSent++;
                    }
                    reader.Close();

                    if (messagesSent == 0)
                    {
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        lblStatus.Text = "No students found with valid phone numbers or SMS not sent. Possible reasons:<br/>" +
                                         "- No students with outstanding balances<br/>" +
                                         "- No valid phone numbers (must start with '265')<br/>" +
                                         "- No credit for sending SMS. Please contact the System Administrator.";
                    }
                    else
                    {
                        lblStatus.ForeColor = System.Drawing.Color.Black;
                        lblStatus.Text = summary.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Error: " + ex.Message;
            }
        }

        protected async  void btnSendSmsInvoices_Click(object sender, EventArgs e)
        {

        }

        protected async void btnSendSmsExamRelease_Click(object sender, EventArgs e)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback += (sender2, cert, chain, sslPolicyErrors) => true;

                // Check if SchoolCode is available in session
                if (Session["SchoolCode"] == null)
                {
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    lblStatus.Text = "Session expired. Please log in again.";
                    return;
                }

                string schoolCode = Session["SchoolCode"].ToString();

                using (SqlConnection conn = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    await conn.OpenAsync();

                    // Step 1: Check SMSStatus for the current school
                    string smsStatusQuery = "SELECT SMSStatus FROM School WHERE SchoolCode = @SchoolCode";
                    using (SqlCommand smsStatusCmd = new SqlCommand(smsStatusQuery, conn))
                    {
                        smsStatusCmd.Parameters.AddWithValue("@SchoolCode", schoolCode);
                        object statusResult = await smsStatusCmd.ExecuteScalarAsync();

                        if (statusResult == null || Convert.ToInt32(statusResult) != 1)
                        {
                            lblStatus.ForeColor = System.Drawing.Color.Red;
                            lblStatus.Text = "SMS sending is disabled for your school. Please contact the System Administrator.";
                            return;
                        }
                    }

                    // Step 2: Proceed with sending SMS
                    string query = @"select distinct student,AssessmentTitle,ClassName,phone,status,schoolcode,SchoolName,username,Password from Vw_endofterm
                                    where schoolcode=@schoolCode and Status=2
                             ";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SchoolCode", schoolCode);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    string token = await _smsService.GetAccessTokenAsync();
                    if (string.IsNullOrEmpty(token))
                    {
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        lblStatus.Text = "Failed to get access token.";
                        return;
                    }

                    StringBuilder summary = new StringBuilder();
                    int messagesSent = 0;

                    while (await reader.ReadAsync())
                    {
                        string phone = reader["Phone"].ToString().Trim();

                        if (!phone.StartsWith("265"))
                        {
                            summary.AppendLine($"Skipped {reader["Student"]} ({phone}): Invalid phone number<br/>");
                            continue;
                        }

                        string student = reader["Student"].ToString();
                        string className = reader["ClassNAme"].ToString();
                        string Assessment = reader["AssessmentTitle"].ToString();
                        string school = reader["schoolName"].ToString();
                        string username = reader["Username"].ToString();
                        string password = reader["Password"].ToString();

                        string message = $"{school}\n" +
                                         $"HAS RELEASED {Assessment} FOR {student} - {className}\n" +
                                         $"Access Results on Portal https://fecomas.com/views/Admin/UserLogin.aspx \n" +
                                         $"Username: {username}\n" +
                                         $"Password: {password}\n" ;
                        string result = await _smsService.SendSmsAsync(token, new string[] { phone }, message);
                        summary.AppendLine($"Sent to {student} ({phone}): {result}<br/>");

                        try
                        {
                            var jsonData = JObject.Parse(result);
                            if (jsonData["payload"].First["status"].ToString() == "submitted")
                            {
                                await InsertSmsLog(phone, student,message);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Error parsing JSON: " + ex.Message);
                        }

                        messagesSent++;
                    }
                    reader.Close();

                    if (messagesSent == 0)
                    {
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        lblStatus.Text = "No students found with valid phone numbers or SMS not sent. Possible reasons:<br/>" +
                                         "- No students with outstanding balances<br/>" +
                                         "- No valid phone numbers (must start with '265')<br/>" +
                                         "- No credit for sending SMS. Please contact the System Administrator.";
                    }
                    else
                    {
                        lblStatus.ForeColor = System.Drawing.Color.Black;
                        lblStatus.Text = summary.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Error: " + ex.Message;
            }

        }

        protected async void btnSendAllParents_Click(object sender, EventArgs e)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback += (sender2, cert, chain, sslPolicyErrors) => true;

                // Check if SchoolCode is available in session
                if (Session["SchoolCode"] == null)
                {
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    lblStatus.Text = "Session expired. Please log in again.";
                    return;
                }

                string schoolCode = Session["SchoolCode"].ToString();

                using (SqlConnection conn = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    await conn.OpenAsync();

                    // Step 1: Check SMSStatus for the current school
                    string smsStatusQuery = "SELECT SMSStatus FROM School WHERE SchoolCode = @SchoolCode";
                    using (SqlCommand smsStatusCmd = new SqlCommand(smsStatusQuery, conn))
                    {
                        smsStatusCmd.Parameters.AddWithValue("@SchoolCode", schoolCode);
                        object statusResult = await smsStatusCmd.ExecuteScalarAsync();

                        if (statusResult == null || Convert.ToInt32(statusResult) != 1)
                        {
                            lblStatus.ForeColor = System.Drawing.Color.Red;
                            lblStatus.Text = "SMS sending is disabled for your school. Please contact the System Administrator.";
                            return;
                        }
                    }

                    // Step 2: Proceed with sending SMS
                    string query = @"Select S.FirstName+' '+ S.LastName as Student,
                            Phone,SchoolName,Sc.SchoolCode,
                            T.status from Enrollment E Inner join Student S on E.studentid=S.studentid
                            Inner join School Sc on S.school=sc.schoolid Inner join Term t on E.termid=T.TermId
                                    where sc.schoolcode=@schoolCode and T.Status=2
                             ";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SchoolCode", schoolCode);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    string token = await _smsService.GetAccessTokenAsync();
                    if (string.IsNullOrEmpty(token))
                    {
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        lblStatus.Text = "Failed to get access token.";
                        return;
                    }

                    StringBuilder summary = new StringBuilder();
                    int messagesSent = 0;

                    while (await reader.ReadAsync())
                    {
                        string phone = reader["Phone"].ToString().Trim();

                        if (!phone.StartsWith("265"))
                        {
                            summary.AppendLine($"Skipped {reader["Student"]} ({phone}): Invalid phone number<br/>");
                            continue;
                        }

                        string student = reader["Student"].ToString();
                        string school = reader["schoolName"].ToString();

                        string message = txtAllParentsMessage.Text.ToString();
                        string result = await _smsService.SendSmsAsync(token, new string[] { phone }, message);
                        summary.AppendLine($"Sent to {student} ({phone}): {result}<br/>");

                        try
                        {
                            var jsonData = JObject.Parse(result);
                            if (jsonData["payload"].First["status"].ToString() == "submitted")
                            {
                                await InsertSmsLog(phone, student, message);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Error parsing JSON: " + ex.Message);
                        }

                        messagesSent++;
                    }
                    reader.Close();

                    if (messagesSent == 0)
                    {
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        lblStatus.Text = "No students found with valid phone numbers or SMS not sent. Possible reasons:<br/>" +
                                         "- No students with outstanding balances<br/>" +
                                         "- No valid phone numbers (must start with '265')<br/>" +
                                         "- No credit for sending SMS. Please contact the System Administrator.";
                    }
                    else
                    {
                        lblStatus.ForeColor = System.Drawing.Color.Black;
                        lblStatus.Text = summary.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Error: " + ex.Message;
            }

        }
    }
}