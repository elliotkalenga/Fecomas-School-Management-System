using System;
using System.Data.SqlClient;
using System.Web.UI;
using SMSWEBAPP.DAL;

namespace SMSWEBAPP.Views.Students
{
    public partial class StudentDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                // Redirect to login page
                Response.Redirect("~/Views/Admin/UserLogin.aspx");
            }
            if (!IsPostBack)
            {
                lblBrand.Text = $"WELCOME TO {Session["SchoolName"]} ({Session["SchoolCode"]})";

            }
        }




    }
}