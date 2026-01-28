using Microsoft.Reporting.Map.WebForms.BingMaps;
using Microsoft.Reporting.WebForms;
using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
	public partial class LibraryInventoryReport : System.Web.UI.Page
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                // Redirect to login page
                Response.Redirect("UserLogin.aspx");
                return; // Prevent further execution
            }

            if (!IsPostBack)
            {
            }
        }



        protected void btnLibraryInventory_Click(object sender, EventArgs e)
        {
            string query = @"SELECT BookTitle, BookNo, Author, Publisher, ISBN, Category, SubjectName, Location, SchoolCode, SchoolName, BookStatus, Logo, BookId, Address
FROM   LibraryInventoryReport
WHERE (SchoolCode = @SChoolCode)";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    command.CommandTimeout = 130;

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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/LibraryInventoryReport.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;

                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("LibraryInventoryReport", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);

                // Set external images path
                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                string UserName = Session["Username"] != null ? Session["Username"].ToString() : string.Empty;
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                ReportParameter Username = new ReportParameter("UserName", UserName);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { Username });

                // Refresh the report
                ReportViewer1.LocalReport.Refresh();

                // Export to PDF and Save on Server
            }

        }

        protected void btnBorrowed_Click(object sender, EventArgs e)
        {
            string query = @"SELECT Member, BookTitle, BookNo, Author, Publisher, ISBN, Category, SubjectName, Location, SchoolCode, SchoolName, BookStatus, Logo, BookId, Address
FROM   BorrowedBooks
WHERE (SchoolCode = @SchoolCode)";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    command.CommandTimeout = 130;

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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/BorrowedBooks.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;

                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("BorrowedBooks", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);

                // Set external images path
                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                string UserName = Session["Username"] != null ? Session["Username"].ToString() : string.Empty;
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                ReportParameter Username = new ReportParameter("UserName", UserName);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { Username });

                // Refresh the report
                ReportViewer1.LocalReport.Refresh();

                // Export to PDF and Save on Server
            }

        }

        protected void btnLMissing_Click(object sender, EventArgs e)
        {
            string query = @"SELECT Member, BookTitle, BookNo, Author, Publisher, ISBN, Category, SubjectName, Location, SchoolCode, SchoolName, BookStatus, Logo, BookId, Address
                            FROM   BorrowedBooks
                            WHERE (SchoolCode = @SchoolCode)";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    command.CommandTimeout = 130;

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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/BorrowedBooks.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;

                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("BorrowedBooks", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);

                // Set external images path
                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                string UserName = Session["Username"] != null ? Session["Username"].ToString() : string.Empty;
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                ReportParameter Username = new ReportParameter("UserName", UserName);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { Username });

                // Refresh the report
                ReportViewer1.LocalReport.Refresh();

                // Export to PDF and Save on Server
            }

        }
    }
}
