using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using SMSWEBAPP.DAL;
using SMSWEBAPP.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace SMSWEBAPP.Views.Admin
{
    public partial class ReleaseExam : System.Web.UI.Page
    {
        string releaseStatus = null;
        private SmsService _smsService = new SmsService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ddlExam.Enabled = false;
                BindExamDropdown();
                SetUserPermissions();
                GetReleaseStatus();
                BindClassDropdown();

                string assessmentId = Request.QueryString["AssessmentID"];
                string releaseStatus = Request.QueryString["ReleaseStatus"];

                if (!string.IsNullOrEmpty(assessmentId))
                {
                    // Pass both parameters when loading
                    LoadScoreData(assessmentId, releaseStatus);
                }

                SetButtonText();
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["AssessmentId"] != null)
            {
                btnSubmit.Text = "Release Exam";
            }
            else
            {
                btnSubmit.Text = "Add";
            }
        }

        private void LoadScoreData(string AssessmentId, string releaseStatus)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand(@"
select Distinct 
    SchoolCode,
    TermId,
    NULL as ReleaseStatus,
    AssessmentTitle as AssessmentTitleId,
    AssessmentTitle 
from JCEEndofterm 
Where SchoolCode = @SchoolCode and AssessmentTitle = @AssessmentTitle 
union 
select Distinct 
    SchoolCode,
    TermId,
    ReleaseStatus,
    AssessmentTitle as AssessmentTitleId,
    AssessmentTitle 
from MSCEEndofterm  
Where SchoolCode = @SchoolCode and AssessmentTitle = @AssessmentTitle 
union 
select Distinct 
    SchoolCode,
    TermId,
    ReleaseStatus,
    AssessmentTitle as AssessmentTitleId,
    AssessmentTitle 
from PRIEndofterm 
Where SchoolCode = @SchoolCode and AssessmentTitle = @AssessmentTitle 
union 
select Distinct 
    SchoolCode,
    TermId,
    ReleaseStatus,
    AssessmentTitle as AssessmentTitleId,
    AssessmentTitle 
from JCEOTHERs	 
Where SchoolCode = @SchoolCode and AssessmentTitle = @AssessmentTitle 
union 
select Distinct 
    SchoolCode,
    TermId,
    ReleaseStatus,
    AssessmentTitle as AssessmentTitleId,
    AssessmentTitle 
from MSCEOTHERs	 
Where SchoolCode = @SchoolCode and AssessmentTitle = @AssessmentTitle 
union 
select Distinct 
    SchoolCode,
    TermId,
    ReleaseStatus,
    AssessmentTitle as AssessmentTitleId,
    AssessmentTitle 
from PRIOTHERs 
Where SchoolCode = @SchoolCode and AssessmentTitle = @AssessmentTitle
", Con);

                cmd.Parameters.AddWithValue("@AssessmentTitle", AssessmentId);
                cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows && dr.Read())
                    {
                        ddlExam.SelectedValue = dr["AssessmentTitleId"].ToString();
                        ddlStatus.SelectedValue = dr["ReleaseStatus"] == DBNull.Value ? string.Empty : dr["ReleaseStatus"].ToString();
                    }
                }
            }
        }

        private void BindExamDropdown()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"select Distinct 
    SchoolCode,
    TermId,
    NULL as ReleaseStatus,
    NULL as ReleasedTime,
    NULL as ReleasedBy,
    AssessmentTitle as AssessmentId,
    AssessmentTitle 
from JCEEndofterm 
Where SchoolCode = @SchoolCode 
union 
select Distinct 
    SchoolCode,
    TermId,
    ReleaseStatus,
    ReleasedTime,
    ReleasedBy,
    AssessmentTitle as AssessmentId,
    AssessmentTitle 
from MSCEEndofterm  
Where SchoolCode = @SchoolCode 
union 
select Distinct 
    SchoolCode,
    TermId,
    ReleaseStatus,
    ReleasedTime,
    ReleasedBy,
    AssessmentTitle as AssessmentId,
    AssessmentTitle 
from PRIEndofterm 
Where SchoolCode = @SchoolCode  
union 
select Distinct 
    SchoolCode,
    TermId,
    ReleaseStatus,
    ReleasedTime,
    ReleasedBy,
    AssessmentTitle as AssessmentId,
    AssessmentTitle 
from JCEOTHERs	 
Where SchoolCode = @SchoolCode   
union 
select Distinct 
    SchoolCode,
    TermId,
    ReleaseStatus,
    ReleasedTime,
    ReleasedBy,
    AssessmentTitle as AssessmentId,
    AssessmentTitle 
from MSCEOTHERs	 
Where SchoolCode = @SchoolCode   
union 
select Distinct 
    SchoolCode,
    TermId,
    ReleaseStatus,
    ReleasedTime,
    ReleasedBy,
    AssessmentTitle as AssessmentId,
    AssessmentTitle 
from PRIOTHERs 
Where SchoolCode = @SchoolCode
Order by TermId Desc";

                using (SqlCommand cmd = new SqlCommand(query, Con))
                {
                    cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    Con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        ddlExam.DataSource = dr;
                        ddlExam.DataTextField = "AssessmentTitle";
                        ddlExam.DataValueField = "AssessmentId";
                        ddlExam.DataBind();
                    }
                }
            }
        }


        private void BindClassDropdown()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"
Select Distinct '0' as ClassId, 'Select Class Name ' as ClassName Union 
Select Distinct ClassName as ClassId, ClassName from JCEEndofterm  where Schoolcode=@SchoolCode Union 
Select Distinct ClassName as ClassId, ClassName from MSCEEndofterm  where Schoolcode=@SchoolCode union 
Select Distinct ClassName as ClassId, ClassName from PRIEndofterm  where Schoolcode=@SchoolCode Union
Select Distinct ClassName as ClassId, ClassName from JCEOthers  where Schoolcode=@SchoolCode Union 
Select Distinct ClassName as ClassId, ClassName from MSCEOthers  where Schoolcode=@SchoolCode union 
Select Distinct ClassName as ClassId, ClassName from PRIOthers  where Schoolcode=@SchoolCode";

                using (SqlCommand cmd = new SqlCommand(query, Con))
                {
                    cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    Con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        ddlClass.DataSource = dr;
                        ddlClass.DataTextField = "ClassName";
                        ddlClass.DataValueField = "ClassId";
                        ddlClass.DataBind();
                    }
                }
            }
        }

        // Release_Exam_Results

        // ********** IMPORTANT: make this async so we await UpdateScoreAsync **********
        protected async void btnSubmit_Click(object sender, EventArgs e)
        {
            bool isValid = true;

            if (ddlStatus.SelectedIndex == 0)
            {
                lblStatus.ForeColor = System.Drawing.Color.Green;
                lblStatus.Text = "Please select Release Status Option";
                isValid = false;
            }

            if (ddlClass.SelectedIndex == 0)
            {
                lblSMSError.ForeColor = System.Drawing.Color.Green;
                lblSMSError.Text = "Please select Class";
                isValid = false;
            }
            if (ddlSMS.SelectedIndex == 0)
            {
                lblSMSError.ForeColor = System.Drawing.Color.Green;
                lblSMSError.Text = "Please select Send SMS Option";
                isValid = false;
            }

            if (!isValid) return;

            lblStatus.ForeColor = System.Drawing.Color.Black;
            lblStatus.Text = "Processing...";

            if (Request.QueryString["AssessmentId"] != null)
            {
                string AssessmentId = Request.QueryString["AssessmentId"].ToString();
                await UpdateScoreAsync(AssessmentId);
            }
            else
            {
                lblStatus.ForeColor = System.Drawing.Color.Green;
                lblStatus.Text = "AssessmentId is required.";
            }

            ClearControls();
        }


        private void GetReleaseStatus()
        {
            string assessmentTitle = ddlExam.SelectedValue.ToString();

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();

                string query = "SELECT ReleaseStatus FROM CheckReleaseStatus WHERE AssessmentTitle = @AssessmentTitle";

                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@AssessmentTitle", assessmentTitle);

                    object result = command.ExecuteScalar();
                    releaseStatus = result != null ? result.ToString() : null;
                }
            }
        }
        private void ClearControls()
        {
            if (ddlExam.Items.Count > 0) ddlExam.SelectedIndex = 0;
            if (ddlStatus.Items.Count > 0) ddlStatus.SelectedIndex = 0;
        }

        private async Task UpdateScoreAsync(string AssessmentId)
        {
            GetReleaseStatus();
            string sendsms = ddlSMS.SelectedValue.ToString();
            string status = ddlStatus.SelectedValue ?? string.Empty;
            string ClassName = ddlClass.SelectedValue ?? string.Empty;
            string exam = ddlExam.SelectedItem?.Text ?? AssessmentId;
            string schoolCode = Session["SchoolCode"]?.ToString();
            string releasedBy = Session["Username"]?.ToString();

            if (string.IsNullOrEmpty(schoolCode) || string.IsNullOrEmpty(releasedBy))
            {
                lblStatus.ForeColor = System.Drawing.Color.Green;
                lblStatus.Text = "Session expired or missing user info. Please log in again.";
                return;
            }

            string[] tables =
            {
                "MSCEOthers",
                "JCEOthers",
                "PRIOthers",
                "MSCEEndofterm",
                "PRIEndofterm",
                "JCEEndofterm"
            };

            try
            {
                int totalUpdated = 0;

                foreach (string tbl in tables)
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        string query = $@"
                UPDATE {tbl}
                SET ReleaseStatus = @Status,
                    ReleasedTime = @ReleasedTime,
                    ReleasedBy = @ReleasedBy
                WHERE AssessmentTitle = @AssessmentId
                AND Schoolcode = @SchoolCode and ClassName=@ClassName";

                        using (SqlCommand cmd = new SqlCommand(query, Con))
                        {
                            cmd.Parameters.AddWithValue("@Status", status);
                            cmd.Parameters.AddWithValue("@ReleasedTime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@ReleasedBy", releasedBy);
                            cmd.Parameters.AddWithValue("@SchoolCode", schoolCode);
                            cmd.Parameters.AddWithValue("@ClassName", ClassName);
                            cmd.Parameters.AddWithValue("@AssessmentId", AssessmentId);

                            await Con.OpenAsync();
                            int affected = await cmd.ExecuteNonQueryAsync();
                            totalUpdated += affected;
                        }
                    }
                }

                lblStatus.ForeColor = System.Drawing.Color.Green;
                lblStatus.Text = $"Update done. Rows updated across tables: {totalUpdated}.";

                if (status == "Released" && sendsms == "Yes")
                {
                    try
                    {
                        lblStatus.Text += "<br/>Releasing SMS to recipients...";
                        await SendSmsExamRelease();
                    }
                    catch (Exception ex)
                    {
                        lblStatus.ForeColor = System.Drawing.Color.Green;
                        lblStatus.Text += "<br/>SMS Error: " + ex.Message;
                        System.Diagnostics.Debug.WriteLine("SMS Error (UpdateScoreAsync): " + ex.ToString());
                    }
                }

                SetButtonText();

                lblStatus.Text = exam + " has been " + status + " successfully!";
                lblStatus.ForeColor = System.Drawing.Color.Green;
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = System.Drawing.Color.Green;
                lblStatus.Text = "Update Error: " + ex.ToString();
                System.Diagnostics.Debug.WriteLine("UpdateScoreAsync ERROR: " + ex.ToString());
            }
        }

        private async Task SendSmsExamRelease()
        { 
            try
            {
                ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback += (sender2, cert, chain, sslPolicyErrors) => true;

                if (Session["SchoolCode"] == null)
                {
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    lblStatus.Text += "<br/>Session expired. Please log in again.";
                    return;
                }

                string schoolCode = Session["SchoolCode"].ToString();

                using (SqlConnection conn = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    await conn.OpenAsync();

                    // Check SMS setting
                    string smsStatusQuery = "SELECT SMSStatus FROM School WHERE SchoolCode = @SchoolCode";
                    using (SqlCommand smsStatusCmd = new SqlCommand(smsStatusQuery, conn))
                    {
                        smsStatusCmd.Parameters.AddWithValue("@SchoolCode", schoolCode);
                        object statusResult = await smsStatusCmd.ExecuteScalarAsync();

                        if (statusResult == null || Convert.ToInt32(statusResult) != 1)
                        {
                            lblStatus.ForeColor = System.Drawing.Color.Green;
                            lblStatus.Text += "<br/>SMS sending disabled for this school. Contact admin.";
                            return;
                        }
                    }

                    string query = @"
            SELECT DISTINCT student, AssessmentTitle, ClassName, M.phone, status, M.schoolcode,
                            SchoolName, username, M.StudentNo, Password
            FROM PRIOthers M INNER JOIN Student S ON M.StudentNo = S.StudentNo  
            WHERE M.schoolcode = @SchoolCode AND AssessmentTitle = @AssessmentTitle and ClassName=@ClassName
            UNION  
            SELECT DISTINCT student, AssessmentTitle, ClassName, M.phone, status, M.schoolcode,
                            SchoolName, username, M.StudentNo, Password
            FROM JCEOthers M INNER JOIN Student S ON M.StudentNo = S.StudentNo  
            WHERE M.schoolcode = @SchoolCode AND AssessmentTitle = @AssessmentTitle and ClassName=@ClassName
            UNION  
            SELECT DISTINCT student, AssessmentTitle, ClassName, M.phone, status, M.schoolcode,
                            SchoolName, username, M.StudentNo, Password
            FROM MSCEOthers M INNER JOIN Student S ON M.StudentNo = S.StudentNo  
            WHERE M.schoolcode = @SchoolCode AND AssessmentTitle = @AssessmentTitle and ClassName=@ClassName
            UNION  
            SELECT DISTINCT student, AssessmentTitle, ClassName, M.phone, status, M.schoolcode,
                            SchoolName, username, M.StudentNo, Password
            FROM PRIEndofterm M INNER JOIN Student S ON M.StudentNo = S.StudentNo  
            WHERE M.schoolcode = @SchoolCode AND AssessmentTitle = @AssessmentTitle and ClassName=@ClassName
            UNION  
            SELECT DISTINCT student, AssessmentTitle, ClassName, M.phone, status, M.schoolcode,
                            SchoolName, username, M.StudentNo, Password
            FROM JCEEndofterm M INNER JOIN Student S ON M.StudentNo = S.StudentNo  
            WHERE M.schoolcode = @SchoolCode AND AssessmentTitle = @AssessmentTitle and ClassName=@ClassName
            UNION  
            SELECT DISTINCT student, AssessmentTitle, ClassName, M.phone, status, M.schoolcode,
                            SchoolName, username, M.StudentNo, Password
            FROM MSCEEndofterm M INNER JOIN Student S ON M.StudentNo = S.StudentNo  
            WHERE M.schoolcode = @SchoolCode AND AssessmentTitle = @AssessmentTitle and ClassName=@ClassName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AssessmentTitle", ddlExam.SelectedValue);
                        cmd.Parameters.AddWithValue("@SchoolCode", schoolCode);
                        cmd.Parameters.AddWithValue("@ClassName", ddlClass.SelectedValue);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            // Count number of recipients
                            int recipientCount = 0;
                            StringBuilder summary = new StringBuilder();
                            int messagesSent = 0;

                            // Get token once
                            string token = await _smsService.GetAccessTokenAsync();
                            if (string.IsNullOrEmpty(token))
                            {
                                lblStatus.ForeColor = System.Drawing.Color.Green;
                                lblStatus.Text += "<br/>Failed to get SMS Token. Check SMS service credentials.";
                                return;
                            }

                            // Move through results
                            while (await reader.ReadAsync())
                            {
                                recipientCount++;

                                string phone = (reader["Phone"] ?? string.Empty).ToString().Trim();

                                // phone fix: 0XXXXXXXX -> 265XXXXXXXX
                                if (phone.StartsWith("0") && phone.Length == 10)
                                    phone = "265" + phone.Substring(1);

                                if (!phone.StartsWith("265") || phone.Length < 12)
                                {
                                    summary.AppendLine($"Skipped {reader["Student"]} - Invalid/Empty Phone: {phone}<br/>");
                                    continue;
                                }

                                string student = reader["Student"]?.ToString();
                                string className = reader["ClassName"]?.ToString();
                                string assessment = ddlExam.SelectedValue?.ToString();
                                string school = reader["SchoolName"]?.ToString();
                                string username = reader["Username"]?.ToString();
                                string password = reader["Password"]?.ToString();

                                string message =
                                    $"{school}\n" +
                                    $"HAS RELEASED {assessment} FOR {student} - {className}\n" +
                                    $"Portal: https://fecomas.com/views/Admin/UserLogin.aspx\n" +
                                    $"Username: {username}\n" +
                                    $"Password: {password}";

                                // Send SMS via service
                                string result = await _smsService.SendSmsAsync(token, new string[] { phone }, message);

                                // Log API response
                                summary.AppendLine($"API Response ({student} - {phone}): {result}<br/>");

                                // Try parse returned JSON for 'submitted' status and log to DB
                                try
                                {
                                    var jsonData = JObject.Parse(result);
                                    // safe navigation - check payload present
                                    if (jsonData["payload"] != null && jsonData["payload"].HasValues)
                                    {
                                        var first = jsonData["payload"].First;
                                        if (first != null && first["status"] != null && first["status"].ToString().Equals("submitted", StringComparison.OrdinalIgnoreCase))
                                        {
                                            await InsertSmsLog(phone, student, message);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // don't crash if response isn't valid json, but log it
                                    summary.AppendLine($"Failed to parse API response for {phone}: {ex.Message}<br/>");
                                    System.Diagnostics.Debug.WriteLine("JSON parse error: " + ex.ToString());
                                }

                                messagesSent++;
                            } // end reader loop

                            // close reader before updating UI
                            reader.Close();

                            lblStatus.ForeColor = messagesSent > 0 ? System.Drawing.Color.Black : System.Drawing.Color.Green;
                            lblStatus.Text += $"<br/>Recipients found: {recipientCount}, Messages submitted: {messagesSent}.<br/>{summary.ToString()}";
                        } // end using reader
                    } // end using cmd
                } // end using conn
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = System.Drawing.Color.Green;
                lblStatus.Text += "<br/>SendSms Error: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("SendSmsExamRelease ERROR: " + ex.ToString());
            }
        }

        private async Task InsertSmsLog(string phone, string student, string message)
        {
            using (SqlConnection conn = new SqlConnection(AppConnection.GetConnectionString()))
            {
                await conn.OpenAsync();

                string tariffQuery = "SELECT TOP 1 TariffId FROM smsTariff WHERE Status = 1";
                object tid = await new SqlCommand(tariffQuery, conn).ExecuteScalarAsync();
                int tariffId = tid == null ? 0 : Convert.ToInt32(tid);

                string query = @"INSERT INTO SmsLog 
                        (Message, Phone, CreatedDate, CreatedBy, SchoolId, TariffId, Student)
                        VALUES (@Message, @Phone, GETDATE(), @CreatedBy, @SchoolId, @TariffId, @Student)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Message", message);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Student", student ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"] ?? string.Empty);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? string.Empty);
                    cmd.Parameters.AddWithValue("@TariffId", tariffId);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        protected void ddlExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            // optional: you can call LoadScoreData here for selected exam
        }

        protected void ddlSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
        }


        private void SetUserPermissions()
        {
            if (Session["Permissions"] != null && Session["RoleId"] != null)
            {
                List<string> userPermissions = (List<string>)Session["Permissions"];
                int roleId = Convert.ToInt32(Session["RoleId"]);


                if (userPermissions.Contains("Release_Exam_Results"))
                {

                    btnSubmit.Enabled = true;


                }
                else
                {
                    btnSubmit.Enabled = true;

                }
            }
        }
    }
}
