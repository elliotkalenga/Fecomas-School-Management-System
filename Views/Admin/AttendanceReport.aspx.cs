using SMSWEBAPP.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class AttendanceReport : System.Web.UI.Page
    {
        private const string USER_SESSION_KEY = "User";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[USER_SESSION_KEY] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                BindTermDropdown();
                BindClassDropdown();
                BindAttendanceReport();
            }
        }

        private void BindTermDropdown()
        {
            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT Term FROM vw_StudentAttendanceReport WHERE SchoolCode = @SchoolCode", con))
            {
                cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ddlTerm.DataSource = reader;
                ddlTerm.DataTextField = "Term";
                ddlTerm.DataValueField = "Term";
                ddlTerm.DataBind();
                ddlTerm.Items.Insert(0, new ListItem("Select Term", ""));
            }
        }

        private void BindClassDropdown()
        {
            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT Class FROM vw_StudentAttendanceReport WHERE SchoolCode = @SchoolCode", con))
            {
                cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ddlClass.DataSource = reader;
                ddlClass.DataTextField = "Class";
                ddlClass.DataValueField = "Class";
                ddlClass.DataBind();
                ddlClass.Items.Insert(0, new ListItem("Select Class", ""));
            }
        }

        private void BindAttendanceReport()
        {
            string schoolCode = Session["SchoolCode"].ToString();
            string termId = ddlTerm.SelectedValue;
            string className = ddlClass.SelectedValue;

            DataTable dt = GetAttendancePivotData(schoolCode, termId, className);
            gvAttendanceReport.DataSource = dt;
            gvAttendanceReport.DataBind();
        }

        private DataTable GetAttendancePivotData(string schoolCode, string termId, string className)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand("sp_GetAttendancePivot", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SchoolCode", schoolCode);
                cmd.Parameters.AddWithValue("@Term", string.IsNullOrEmpty(termId) ? (object)DBNull.Value : termId);
                cmd.Parameters.AddWithValue("@Class", string.IsNullOrEmpty(className) ? (object)DBNull.Value : className);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        protected void ddlTerm_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindAttendanceReport();
        }

        protected void ddlClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindAttendanceReport();
        }
    }
}
