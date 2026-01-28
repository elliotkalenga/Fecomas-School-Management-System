using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SMSWEBAPP.DAL;
using System.Net;
using Newtonsoft.Json.Linq;

namespace SMSWEBAPP.Views.Students
{

    public partial class Login : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                                    lblError.Text = "Username or Password Invalid";

            }
        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            // Authenticate the user
            if (AuthenticateUser(TxtUsername.Text.Trim(), TxtPassword.Text.Trim()))
            {
                // Get user data
                DataRow dataRow = GetUserData(TxtUsername.Text.Trim(), TxtPassword.Text.Trim());

                if (dataRow != null)
                {
                    // Set user data in session
                    LoggedInUser.SchoolName = dataRow["SchoolName"].ToString();
                    Session["SchoolCode"] = dataRow["SchoolCode"].ToString();
                    Session["StudentId"] = dataRow["StudentId"].ToString();
                    Session["StudentNo"] = dataRow["StudentNo"].ToString();
                    LoggedInUser.FirstName = dataRow["FirstName"].ToString();
                    LoggedInUser.LastName = dataRow["LastName"].ToString();
                    Session["SchoolId"] = dataRow["SchoolId"].ToString();
                    Session["Username"] = dataRow["username"].ToString();
                    LoggedInUser.Address = dataRow["Address"].ToString();
                    LoggedInUser.RoleTitle = dataRow["RoleTitle"].ToString();
                    Session["UserId"] = dataRow["userId"].ToString();
                    LoggedInUser.RoleId = Convert.ToInt32(dataRow["RoleId"]);
                    Session["USER"] = LoggedInUser.FirstName.ToString() + " " + LoggedInUser.LastName.ToString();


                    // Update logs
                    UpdateLogs(TxtUsername.Text.Trim(), LoggedInUser.SchoolName);

                    // Redirect to protected page
                    Response.Redirect("StudentDashboard.aspx");
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Username or Password Invalid";
                    TxtUsername.Text = "";
                    TxtPassword.Text = "";
                    TxtUsername.Focus();
                }
            }
            else
            {
                lblError.Visible = true;
                lblError.Text = "Username or Password Invalid";
                TxtUsername.Text = "";
                TxtPassword.Text = "";
                TxtUsername.Focus();
            }

        }

        private bool AuthenticateUser(string username, string password)
        {
            username = TxtUsername.Text;
            password = TxtPassword.Text;
            // TODO: Implement your authentication logic here

            // For example, you can check the username and password against a database
            string sql = @"SELECT Count (*)
                   FROM StudentLoginView 
                   WHERE Username = @Username AND Password = @Password AND Status = 2";

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(sql, Con);
                cmd.Parameters.Add(new SqlParameter("@Username", username));
                cmd.Parameters.Add(new SqlParameter("@Password", password));

                Con.Open();
                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }
        }
        private DataRow GetUserData(string username, string password)
        {
            username = TxtUsername.Text;
            password = TxtPassword.Text;
            string sql = @"SELECT * 
                   FROM StudentLoginView 
                   WHERE Username = @Username AND Password = @Password AND Status = 2";

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(sql, Con);
                cmd.Parameters.Add(new SqlParameter("@Username", username));
                cmd.Parameters.Add(new SqlParameter("@Password", password));

                Con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(dr);

                if (dataTable.Rows.Count > 0)
                {
                    return dataTable.Rows[0];
                }
                else
                {
                    return null;
                }
            }
        }



        private void UpdateLogs(string username, string schoolName)
        {
            string visitorIP = GetVisitorIP();
            var location = GetLocationDetails(visitorIP);

            // Determine Device Type
            string deviceType = "Unknown"; // Default if UserAgent is unavailable

            if (HttpContext.Current?.Request?.UserAgent != null)
            {
                string userAgent = HttpContext.Current.Request.UserAgent.ToLower();

                if (userAgent.Contains("mobile") || userAgent.Contains("android") || userAgent.Contains("iphone"))
                {
                    deviceType = "Mobile";
                }
                else
                {
                    deviceType = "Computer";
                }
            }

            string sql = @"INSERT INTO LoginAudits(
               Operator_Name, Operate_Role, IP_Address, DeviceType, Country, City, Region
           ) 
           VALUES(
                @Operator_Name, @Operate_Role, @IP_Address, @DeviceType, @Country, @City, @Region)";

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(sql, Con);
                cmd.Parameters.Add(new SqlParameter("@Operator_Name", username));
                cmd.Parameters.Add(new SqlParameter("@Operate_Role", schoolName));
                cmd.Parameters.Add(new SqlParameter("@IP_Address", visitorIP));
                cmd.Parameters.Add(new SqlParameter("@DeviceType", deviceType));
                cmd.Parameters.Add(new SqlParameter("@Country", location.Country));
                cmd.Parameters.Add(new SqlParameter("@City", location.City));
                cmd.Parameters.Add(new SqlParameter("@Region", location.Region));

                Con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Function to get the visitor's IP address
        private string GetVisitorIP()
        {
            string ip = HttpContext.Current?.Request?.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current?.Request?.ServerVariables["REMOTE_ADDR"];
            }

            return ip ?? "Unknown";
        }

        // Function to get location details from a GeoIP API
        private (string Country, string City, string Region) GetLocationDetails(string ip)
        {
            string url = $"http://ip-api.com/json/{ip}";
            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(url);
                JObject obj = JObject.Parse(json);
                return (obj["country"]?.ToString() ?? "Unknown",
                        obj["city"]?.ToString() ?? "Unknown",
                        obj["regionName"]?.ToString() ?? "Unknown");
            }
        }

        protected void BtnLogin_Click1(object sender, EventArgs e)
        {

        }
    }
}