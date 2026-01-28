using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class PasswordReset : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            string systemId = Session["SystemId"] as string;

            if (!string.IsNullOrEmpty(systemId))
            {
                switch (systemId)
                {
                    case "1":
                        this.MasterPageFile = "~/Views/Admin/AdminMaster.Master";
                        break;
                    case "2":
                        this.MasterPageFile = "~/POS/AdminMaster.Master";
                        break;
                    case "3":
                        this.MasterPageFile = "~/AMS/AdminMaster.Master";
                        break;
                    case "4":
                        this.MasterPageFile = "~/CMS/AdminMaster.Master";
                        break;
                    case "6":
                        this.MasterPageFile = "~/UBN/AdminMaster.Master";
                        break;

                    default:
                        this.MasterPageFile = "~/POS/AdminMaster.Master";
                        break;
                }
            }
            else
            {
                // Optionally redirect to login page if session is missing
                this.MasterPageFile = "~/POS/AdminMaster.Master";
            }

        }


        private const string USER_SESSION_KEY = "User";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[USER_SESSION_KEY] == null)
            {
                // Redirect to login page
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                BindScoresRepeater();
                if (Request.QueryString["UserId"] != null)
                {
                    int examId = int.Parse(Request.QueryString["UserId"]);
                }
            }
        }


        private List<User> GetUsersList()
        {
            List<User> users = new List<User>();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"SELECT u.UserId,
       u.RoleId,
       u.FirstName,
       u.LastName,
       u.UserName,
       u.Password,
       u.Status,
       u.School,
       r.RoleTitle,
       s.SchoolName,
       s.SchoolCode,
	   sta.Status as UserStatus,
	   s.Address,
       ps.ParentSchoolName,
       ps.ParentSchoolCode,
       ps.ParentSchoolId,
	   S.SchoolId,
	   D.LicenseStatus,D.StartDate,D.Enddate,D.LicensedDays,D.USedDays,D.RemainingDays

FROM   users AS u
       INNER JOIN
       Roles AS r
       ON u.Roleid = r.RoleId
       INNER JOIN
       School AS S
       ON u.School = s.SchoolID
       INNER JOIN status Sta on U.Status=sta.StatusId Inner join
       ParentSchool AS ps
       ON S.ParentSchoolId = ps.ParentSchoolId
	   Inner Join DisplyLicensedata D on S.SchoolID=D.SchoolId Where u.School=@SchoolId";

                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        users.Add(new User
                        {
                            UserId = dr["UserId"].ToString(),
                            FirstName = dr["FirstName"].ToString(),
                            LastName = dr["LastName"].ToString(),
                            UserName = dr["UserName"].ToString(),
                            RoleTitle = dr["RoleTitle"].ToString(),
                            UserStatus = dr["UserStatus"].ToString(),
                        });
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                // Log the exception and handle it gracefully
                // Example: log.Error(ex);
                Response.Write("An error occurred: " + ex.Message);
            }
            return users;
        }

        public class User
        {
            public string UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string UserName { get; set; }
            public string RoleTitle { get; set; }
            public string UserStatus { get; set; }
        }

        private void BindScoresRepeater()
        {
            List<User> users = GetUsersList();
            RepeaterControl.DataSource = users;
            RepeaterControl.DataBind();
        }
    }
}
