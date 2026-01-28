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
	public partial class InvoiceReport : System.Web.UI.Page
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
                    // Check if the query string parameters "Term" and "RequisitionId" are available
                    if (!string.IsNullOrEmpty(Request.QueryString["Term"]))
                    {
                        string term = Request.QueryString["Term"];
                        txtTerm.Text = term;
                    }

                    if (!string.IsNullOrEmpty(Request.QueryString["InvoiceId"]))
                    {
                        string requisitionId = Request.QueryString["InvoiceId"];
                        txtInvoiceId.Text = requisitionId; // Assuming you have a textbox for RequisitionId
                    }
                }
            }


            protected void btnBudgetMonitoring_Click(object sender, EventArgs e)
            {

            }

            protected void btnAllExpenseReport_Click(object sender, EventArgs e)
            {
                string query = @"SELECT Term, InvoiceId, InvoiceNumber, Amount, InvoiceDescription, status, Item, Quantity, UnitPrice, SubTotal, SchoolName, Logo, SchoolCode, Customer, Address
FROM   InvoiceReport
WHERE (InvoiceId = @InvoiceId) AND (SchoolCode = @SchoolCode)";

                // Create a DataTable to hold the data
                DataTable dataTable = new DataTable();

                // Fetch the data
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    using (SqlCommand command = new SqlCommand(query, Con))
                    {
                        command.Parameters.AddWithValue("@InvoiceId", txtInvoiceId.Text.ToString());
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
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/ExpenseReportDetailed.rdlc");

                    // Enable external images
                    ReportViewer1.LocalReport.EnableExternalImages = true;
                    // Add the data source to the report
                    ReportDataSource reportDataSource = new ReportDataSource("ExpenseReportDetailed", dataTable);
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


            protected void btnExpenseDetailedReport_Click(object sender, EventArgs e)
            {

                string query = @" SELECT Term, InvoiceId, InvoiceNumber, Amount, InvoiceDescription, status, Item, Quantity, UnitPrice, SubTotal, SchoolName, Logo, SchoolCode, Customer, Address
FROM   InvoiceReport
WHERE (InvoiceId = @InvoiceId) AND (SchoolCode = @SchoolCode)";

                // Create a DataTable to hold the data
                DataTable dataTable = new DataTable();

                // Fetch the data
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    using (SqlCommand command = new SqlCommand(query, Con))
                    {
                        command.Parameters.AddWithValue("@InvoiceId", txtInvoiceId.Text.ToString());
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
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/InvoiceReport.rdlc");

                    // Enable external images
                    ReportViewer1.LocalReport.EnableExternalImages = true;
                    // Add the data source to the report
                    ReportDataSource reportDataSource = new ReportDataSource("InvoiceReport", dataTable);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    // Set the external images path
                    string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                string UserName = (Session["FirstName"] != null ? Session["FirstName"].ToString() : string.Empty) +
                                  (Session["LastName"] != null ? " " + Session["LastName"].ToString() : string.Empty);
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                    ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                    ReportParameter Username = new ReportParameter("UserName", UserName);
                    ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { Username });



                    // Refresh the report
                    ReportViewer1.LocalReport.Refresh();
                SaveReportAsPDF();
                }

            }

        private void SaveReportAsPDF()
        {
            string fileName = "INVOICE" + txtInvoiceId.Text + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
            string folderPath = Server.MapPath("~/Reports/GeneratedReports/"); // Local path
            string filePath = Path.Combine(folderPath, fileName);

            byte[] bytes;
            string mimeType, encoding, fileNameExtension;
            string[] streams;
            Microsoft.Reporting.WebForms.Warning[] warnings;

            bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            // Ensure directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.WriteAllBytes(filePath, bytes);

            // Ensure file exists before storing session variable
            if (File.Exists(filePath))
            {
                // Replace "localhost" with your actual domain or VPS IP
                string baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string publicUrl = baseUrl + "/Reports/GeneratedReports/" + fileName;

                Session["ReportPDF"] = publicUrl; // Store the publicly accessible URL
            }
            else
            {
                Session["ReportPDF"] = null;
            }
        }


        protected void btnIncomeStatement_Click(object sender, EventArgs e)
            {

            }
        }
    }



