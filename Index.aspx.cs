using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            string ipAddress = GetIPAddress();
            string userAgent = HttpContext.Current.Request.UserAgent;
            string deviceType = GetDeviceType(userAgent);
            string[] locationData = GetLocation(ipAddress);

            SaveVisitorIP(ipAddress, locationData[0], locationData[1], locationData[2], deviceType);
        }


        private string GetIPAddress()
        {
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

        private string[] GetLocation(string ipAddress)
        {
            try
            {
                string url = "http://ip-api.com/json/" + ipAddress;
                using (WebClient client = new WebClient())
                {
                    string json = client.DownloadString(url);
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    dynamic jsonObject = serializer.Deserialize<dynamic>(json);

                    // Check if keys exist before accessing
                    string country = jsonObject.ContainsKey("country") ? jsonObject["country"] : "Unknown";
                    string city = jsonObject.ContainsKey("city") ? jsonObject["city"] : "Unknown";
                    string region = jsonObject.ContainsKey("regionName") ? jsonObject["regionName"] : "Unknown";

                    return new string[] { country, city, region };
                }
            }
            catch (Exception ex)
            {
                // Log error and return default values
                return new string[] { "Unknown", "Unknown", "Unknown" };
            }
        }

        private string GetDeviceType(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Unknown";

            string mobilePattern = "(android|bb\\d+|meego|avantgo|bada\\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|iphone|ipod|ipad|kindle|lge |mmp|mobile|nokia|opera mobi|opera mini|palm|phone|p(ixi|re)\\/|plucker|pocket|psp|series40|series60|symbian|treo|up\\.browser|up\\.link|vodafone|wap|windows ce|xda|xiino)";

            if (Regex.IsMatch(userAgent.ToLower(), mobilePattern))
                return "Mobile";
            else
                return "Computer";
        }

        private void SaveVisitorIP(string ipAddress, string country, string city, string region, string deviceType)
        {
            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                con.Open();

                // Check if this IP has already been recorded today
                string checkQuery = "SELECT COUNT(*) FROM VisitorLogs WHERE IPAddress = @IPAddress AND CAST(VisitTime AS DATE) = CAST(GETDATE() AS DATE)";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                    int visitCount = (int)checkCmd.ExecuteScalar();

                    // If no visit found, insert new record
                    if (visitCount == 0)
                    {
                        string insertQuery = "INSERT INTO VisitorLogs (IPAddress, Country, City, Region, DeviceType) VALUES (@IPAddress, @Country, @City, @Region, @DeviceType)";
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                        {
                            insertCmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                            insertCmd.Parameters.AddWithValue("@Country", country);
                            insertCmd.Parameters.AddWithValue("@City", city);
                            insertCmd.Parameters.AddWithValue("@Region", region);
                            insertCmd.Parameters.AddWithValue("@DeviceType", deviceType);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }




        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = "INSERT INTO Inquiry (Category, FullName, Email, Phone, Message,systeminterest) " +
                               "VALUES (@Category, @FullName, @Email, @Phone, @Message,@systeminterest)";

                using (SqlCommand cmd = new SqlCommand(query, Con))
                {
                    cmd.Parameters.AddWithValue("@systeminterest", ddlSystemInterest.SelectedValue);
                    cmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue);
                    cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@Message", txtMessage.Text.Trim());

                    Con.Open();
                    cmd.ExecuteNonQuery();
                    Con.Close();
                }
            }

            // Clear fields after submission
            ddlCategory.SelectedIndex = 0;
            txtFullName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtMessage.Text = "";

            // Show confirmation message

            lblMessage.Text = "Thank you for submitting your inquiry. Our team will reach out to you soon.";

            // Trigger Bootstrap modal
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showModal",
                "setTimeout(function() { $('#successModal').modal('show'); }, 500);", true);
        }
    }
}
