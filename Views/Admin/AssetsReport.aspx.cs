using Microsoft.Reporting.WebForms;
using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class AssetsReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                // Redirect to login page
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {

            }


        }










        protected void btnAssetAllocation_Click(object sender, EventArgs e)
        {
            string query = @"SELECT AllocationId, Barcode, AllocatedStatus, AssetName, AssetDescription, AssetCategory, ReturnedDate, SchoolId, CreatedBy, CreatedDate, LifeSpan, PurchasedDate, LifespanInDays, UsedSpan, RemainingSpan, UsagePercent, Status, 
                  AssetHolder, SchoolName, Logo, Address, SchoolCode
FROM     AssetsAllocationReport
WHERE  (SchoolCode = @SchoolCode) and AllocatedStatus != 'Returned' Order by AssetCategory";

            // Create a DataTable to hold the data
            DataTable dataTable = new  DataTable();

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

            if (dataTable.Rows.Count == 0)
            {
                ReportViewer1.Visible = true;

            }
            else
            {
                ReportViewer1.Visible = true;

                // Set the ReportViewer properties
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/AssetAllocationReport.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("AssetAllocationReport", dataTable);
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

        protected void BtnAllocationHistory_Click(object sender, EventArgs e)
        {
            string query = @"SELECT AllocationId, Barcode, AllocatedStatus, AssetName, AssetDescription, AssetCategory, ReturnedDate, SchoolId, CreatedBy, CreatedDate, LifeSpan, PurchasedDate, LifespanInDays, UsedSpan, RemainingSpan, UsagePercent, Status, 
                  AssetHolder, SchoolName, Logo, Address, SchoolCode
FROM     AssetsAllocationReport
WHERE  (SchoolCode = @SchoolCode) Order by AllocationId";

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

            if (dataTable.Rows.Count == 0)
            {
                ReportViewer1.Visible = true;

            }
            else
            {
                ReportViewer1.Visible = true;

                // Set the ReportViewer properties
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/AssetHistoryReport.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("AssetAllocationReport", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                // Set the external images path
                // string imagePath = "file:///C:/Logo/";
                //string imagePath = "file:///C:/SMSWEBAPP/SMSWEBAPP/StudentImages/";
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

        protected void btnAssetInventory_Click(object sender, EventArgs e)
        {
            string query = @"SELECT AssetId, Barcode, AllocatedStatus, AssetName, AssetDescription, AssetCategory, SchoolId, CreatedBy, CreatedDate, LifeSpan, PurchasedDate, PurchaseCost, LifespanInDays, UsedSpan, RemainingSpan, AssetValue, UsagePercent, 
                  Status, SchoolName, Logo, Address, SchoolCode
FROM     AssetInventoryReport
WHERE  (SchoolCode = @SchoolCode) Order by AssetCategory";

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

            if (dataTable.Rows.Count == 0)
            {
                ReportViewer1.Visible = true;

            }
            else
            {
                ReportViewer1.Visible = true;

                // Set the ReportViewer properties
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/AssetsInventory.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("AssetInventory", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                // Set the external images path
                // string imagePath = "file:///C:/Logo/";
                //string imagePath = "file:///C:/SMSWEBAPP/SMSWEBAPP/StudentImages/";
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



