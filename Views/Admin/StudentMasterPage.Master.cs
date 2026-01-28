using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class StudentMasterPage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER"] != null)
            {
                loggedinUser.InnerText = "Hi " + Session["FirstName"] + " " + Session["LastName"].ToString();
            }
            else
            {
                Response.Redirect("UserLogin.aspx");
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Redirect("../Admin/UserLogin.aspx");
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {

        }
    }
}
