using Microsoft.Reporting.WebForms;
using SMSWEBAPP.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace SMSWEBAPP.Views.Admin
{
    public partial class AttendanceReportAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["Term"]))
                {
                    txtTerm.Text = Request.QueryString["Term"];
                }
            }
        }

        protected void btnGenerateAttendanceReport_Click(object sender, EventArgs e)
        {
            string query = @" SELECT StudentBarcode, AttendanceDate, Status, AttendanceWeek, Logo, SchoolName, SchoolCode, Term
        FROM vw_StudentAttendanceReport
        WHERE (SchoolCode = @SchoolCode) 
          AND (Term = @Term) 
          ";

            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@Term", txtTerm.Text.ToString());
                    command.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }

            // Check if the DataTable is empty
            if (dataTable.Rows.Count == 0)
            {
                ReportViewer1.Visible = false;
            }
            else
            {
                ReportViewer1.Visible = true;

                // Set the ReportViewer properties
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/AttendanceReport.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("StudentAttendanceReport", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                // Set the external images path
                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                string UserName = Session["Username"] != null ? Session["Username"].ToString() : string.Empty;
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                ReportParameter Username = new ReportParameter("UserName", UserName);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { Username });



                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
            }

        }
    }
}