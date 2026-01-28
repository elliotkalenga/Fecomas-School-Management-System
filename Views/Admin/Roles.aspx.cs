using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class Roles : System.Web.UI.Page
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
                BindRecordRepeater();
                if (Request.QueryString["RoleId"] != null)
                {
                    int examId = int.Parse(Request.QueryString["RoleId"]);
                }
            }
        }


        private List<Role> GetRolesList()
        {
            List<Role> roles = new List<Role>();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"Select * from Roles Where School=@SchoolId";

                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        roles.Add(new Role
                        {
                            RoleId = dr["RoleId"].ToString(),
                            RoleTitle= dr["RoleTitle"].ToString(),
                            Description = dr["Description"].ToString(),
                            CreatedBy = dr["CreatedBy"].ToString(),
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
            return roles;
        }

        public class Role
        {
            public string RoleId { get; set; }
            public string RoleTitle { get; set; }
            public string Description { get; set; }
            public string CreatedBy { get; set; }
        }

        private void BindRecordRepeater()
        {
            List<Role> roles = GetRolesList();
            RepeaterControl.DataSource = roles;
            RepeaterControl.DataBind();
        }
    }
}
