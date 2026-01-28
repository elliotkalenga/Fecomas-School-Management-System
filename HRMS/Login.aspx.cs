using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;

namespace SMSWEBAPP.HRMS
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
            if (AuthenticateUser(TxtUsername.Text.Trim(), TxtPassword.Text.Trim()))
            {
                DataRow dataRow = GetUserData(TxtUsername.Text.Trim(), TxtPassword.Text.Trim());

                if (dataRow != null)
                {
                    Session["USER"] = TxtUsername.Text;
                    LoggedInUser.SchoolName = dataRow["SchoolName"].ToString();
                   Session["SchoolCode"] = dataRow["SchoolCode"].ToString();
                   Session["RoleTitle"] = dataRow["RoleTitle"].ToString();
                    LoggedInUser.FirstName = dataRow["FirstName"].ToString();
                    LoggedInUser.LastName = dataRow["LastName"].ToString();
                   Session["SchoolId"] = dataRow["School"].ToString();
                    Session["Username"] = dataRow["username"].ToString();
                    LoggedInUser.Address = dataRow["Address"].ToString();
                    Session["UserId"] = dataRow["userId"].ToString();
                    LoggedInUser.LicenseStatus = dataRow["LicenseStatus"].ToString();
                    LoggedInUser.StartDate = Convert.ToDateTime(dataRow["StartDate"]);
                    LoggedInUser.EndDate = Convert.ToDateTime(dataRow["EndDate"]);
                   Session["RoleId"] = Convert.ToInt32(dataRow["RoleId"]);
                    LoggedInUser.LicensedDay = Convert.ToInt32(dataRow["LicensedDays"]);
                    LoggedInUser.UsedDays = Convert.ToInt32(dataRow["UsedDays"]);
                    LoggedInUser.RemainingDays = Convert.ToInt32(dataRow["RemainingDays"]);

                    // Fetch permissions
                    LoggedInUser.Permissions = GetPermissions();

                    // Update logs
                    UpdateLogs(TxtUsername.Text.Trim(), LoggedInUser.SchoolName);

                    // Show modal instead of redirecting
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowComingSoonModal", "$('#comingSoonModal').modal('show');", true);
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

        private List<string> GetPermissions()
        {
            List<string> permissions = new List<string>();

            string sql = @"SELECT Permission FROM RolePermission rp 
                    INNER JOIN roles R ON rp.roleid = R.roleid
                    INNER JOIN Permission p ON rp.permissionid = p.permissionid 
                    WHERE rp.roleid = @RoleId";

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(sql, Con);
                cmd.Parameters.Add(new SqlParameter("@RoleId",Session["RoleId"]));

                Con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    permissions.Add(dr["Permission"].ToString());
                }

                dr.Close(); // Close reader after use
            }

            return permissions;
        }

        private bool AuthenticateUser(string username, string password)
        {
            username = TxtUsername.Text;
            password = SecureData.EncryptData(TxtPassword.Text.Trim()); // Encrypt the password

            // TODO: Implement your authentication logic here

            // For example, you can check the username and password against a database
            string sql = @"SELECT Count (*)
                   FROM LoginView 
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
            password = SecureData.EncryptData(TxtPassword.Text.Trim()); // Encrypt the password

            string sql = @"SELECT * 
                   FROM LoginView 
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
            string publicIP = GetPublicIP();
            var location = GetLocationDetails(publicIP);

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
                cmd.Parameters.Add(new SqlParameter("@IP_Address", publicIP));
                cmd.Parameters.Add(new SqlParameter("@DeviceType", deviceType));
                cmd.Parameters.Add(new SqlParameter("@Country", location.Country));
                cmd.Parameters.Add(new SqlParameter("@City", location.City));
                cmd.Parameters.Add(new SqlParameter("@Region", location.Region));

                Con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Function to get the public IP address
        private string GetPublicIP()
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString("https://api.ipify.org").Trim();
            }
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

    }
}

