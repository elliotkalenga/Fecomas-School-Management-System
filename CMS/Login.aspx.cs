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

namespace SMSWEBAPP.CMS
{
        public partial class Login : System.Web.UI.Page
        {
            protected void Page_Load(object sender, EventArgs e)
            {
                if (!IsPostBack)
                {
                    lblError.Text = "";
                }
            }

            protected void BtnLogin_Click(object sender, EventArgs e)
            {
                string username = TxtUsername.Text.Trim();
                string password = TxtPassword.Text.Trim();

                if (AuthenticateUser(username, password))
                {
                    DataRow dataRow = GetUserData(username, password);

                    if (dataRow != null)
                    {
                        // ✅ Store user-specific session data
                        Session["USER"] = username;
                        Session["SchoolName"] = dataRow["SchoolName"].ToString();
                        Session["SchoolCode"] = dataRow["SchoolCode"].ToString();
                        Session["RoleTitle"] = dataRow["RoleTitle"].ToString();
                        Session["FirstName"] = dataRow["FirstName"].ToString();
                        Session["LastName"] = dataRow["LastName"].ToString();
                        Session["SchoolId"] = dataRow["School"].ToString();
                        Session["Username"] = dataRow["Username"].ToString();
                        Session["Address"] = dataRow["Address"].ToString();
                        Session["UserId"] = dataRow["UserId"].ToString();
                        Session["SystemId"] = dataRow["SystemId"].ToString();
                        Session["LicenseStatus"] = dataRow["LicenseStatus"].ToString();
                        Session["LicensedDay"] = dataRow["LicensedDays"].ToString();
                        Session["UsedDays"] = dataRow["UsedDays"].ToString();
                        Session["RemainingDays"] = dataRow["RemainingDays"].ToString();
                        Session["EndDate"] = dataRow["EndDate"].ToString();

                        // Handling student-specific fields
                        if (dataRow.Table.Columns.Contains("StudentId"))
                            Session["StudentId"] = dataRow["StudentId"].ToString();

                        if (dataRow.Table.Columns.Contains("StudentNo"))
                            Session["StudentNo"] = dataRow["StudentNo"].ToString();

                        // Handling role ID and storing in session
                        int roleId = dataRow["RoleId"] != DBNull.Value ? Convert.ToInt32(dataRow["RoleId"]) : 0;
                        Session["RoleId"] = roleId;

                        // ✅ Fetch permissions
                        Session["Permissions"] = GetPermissions(roleId);

                        // ✅ Debugging (Remove after testing)
                        Response.Write($"Role ID: {roleId}<br>");
                        Response.Write("Permissions: " + string.Join(", ", (List<string>)Session["Permissions"]) + "<br>");

                        // ✅ Update logs
                        UpdateLogs(username, Session["SchoolName"].ToString());

                        // ✅ Redirect based on role
                        if (Session["RoleTitle"].ToString().ToLower().Contains("student") || roleId == 3)
                        {
                            // Redirecting to student dashboard if role is student
                            Response.Redirect("../students/StudentDashboard.aspx");
                        }
                        else
                        {
                            // Redirecting to admin dashboard if role is not student
                            string absoluteUrl = Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/CMS/Dashboard.aspx");
                            Response.Redirect(absoluteUrl);
                        }
                    }
                    else
                    {
                        ShowErrorMessage("Username or Password Invalid");
                    }
                }
                else
                {
                    ShowErrorMessage("Username or Password Invalid");
                }
            }

            private void ShowErrorMessage(string message)
            {
                lblError.Visible = true;
                lblError.Text = message;
                TxtUsername.Text = "";
                TxtPassword.Text = "";
                TxtUsername.Focus();
            }

            private List<string> GetPermissions(int roleId)
            {
                List<string> permissions = new List<string>();

                string sql = @"SELECT Permission FROM RolePermission rp 
                           INNER JOIN roles R ON rp.roleid = R.roleid
                           INNER JOIN Permission p ON rp.permissionid = p.permissionid 
                           WHERE rp.roleid = @RoleId";

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand(sql, Con);
                    cmd.Parameters.AddWithValue("@RoleId", roleId);

                    Con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        permissions.Add(dr["Permission"].ToString());
                    }

                    dr.Close();
                }

                return permissions;
            }

            private bool AuthenticateUser(string username, string password)
            {
                // First, check LoginView (admin users)
                string encryptedPassword = SecureData.EncryptData(password.Trim());

                bool isAdminValid = CheckLoginView(username, encryptedPassword);

                // If the user is not found in LoginView, check StudentLoginView (student users)
                if (!isAdminValid)
                {
                    return CheckStudentLoginView(username, password);
                }

                return isAdminValid;
            }

            private bool CheckLoginView(string username, string encryptedPassword)
            {
                string sql = @"SELECT COUNT(*) 
                           FROM LoginView 
                           WHERE Username = @Username 
                           AND Password = @EncryptedPassword 
                           AND Status = 2";

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand(sql, Con);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@EncryptedPassword", encryptedPassword);

                    Con.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }

            private bool CheckStudentLoginView(string username, string password)
            {
                string sql = @"SELECT COUNT(*) 
                           FROM StudentLoginView 
                           WHERE Username = @Username 
                           AND Password = @Password 
                           AND Status = 2";

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand(sql, Con);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password.Trim());

                    Con.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }

            private DataRow GetUserData(string username, string password)
            {
                string encryptedPassword = SecureData.EncryptData(password.Trim());

                // First, check LoginView (admin users)
                DataRow adminData = GetAdminUserData(username, encryptedPassword);
                if (adminData != null)
                {
                    return adminData;
                }

                // If user is not found in LoginView, check StudentLoginView (student users)
                return GetStudentUserData(username, password);
            }

            private DataRow GetAdminUserData(string username, string encryptedPassword)
            {
                string sql = @"SELECT UserId, Username, SchoolName, SchoolCode, RoleTitle, FirstName, LastName, 
                                  School, Address, LicenseStatus, StartDate, EndDate, RoleId, LicensedDays, UsedDays, RemainingDays,SystemId
                           FROM LoginView 
                           WHERE Username = @Username 
                           AND Password = @EncryptedPassword 
                           AND Status = 2 
                           AND SystemId=4";

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand(sql, Con);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@EncryptedPassword", encryptedPassword);

                    Con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dr);

                    return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
                }
            }

            private DataRow GetStudentUserData(string username, string password)
            {
                string sql = @"SELECT UserId, Username, SchoolName, SchoolCode, RoleTitle, FirstName, LastName, StudentId,StudentNo,
                                  School, Address, LicenseStatus, StartDate, EndDate, RoleId, LicensedDays, UsedDays, RemainingDays 
                           FROM StudentLoginView 
                           WHERE Username = @Username 
                           AND Password = @Password 
                           AND Status = 2";

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand(sql, Con);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password.Trim());

                    Con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dr);

                    return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
                }
            }

            private void UpdateLogs(string username, string schoolName)
            {
                string visitorIP = GetVisitorIP();

                string deviceType = HttpContext.Current?.Request?.UserAgent?.ToLower().Contains("mobile") ?? false ? "Mobile" : "Computer";

                string sql = @"INSERT INTO LoginAudits(Operator_Name, Operate_Role,IP_Address, DeviceType)
                           VALUES(@Operator_Name, @Operate_Role, @IP_Address, @DeviceType)";

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand(sql, Con);
                    cmd.Parameters.AddWithValue("@Operator_Name", username);
                    cmd.Parameters.AddWithValue("@Operate_Role", schoolName);
                    cmd.Parameters.AddWithValue("@IP_Address", visitorIP);
                    cmd.Parameters.AddWithValue("@DeviceType", deviceType);

                    Con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            private string GetVisitorIP()
            {
                return HttpContext.Current?.Request?.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                       HttpContext.Current?.Request?.ServerVariables["REMOTE_ADDR"] ?? "Unknown";
            }

        }
    }
