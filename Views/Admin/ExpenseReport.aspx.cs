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
using System.Windows.Forms;

namespace SMSWEBAPP.Views.Admin
{
	public partial class ExpenseReport : System.Web.UI.Page
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

                if (!string.IsNullOrEmpty(Request.QueryString["ExpenseId"]))
                {
                    string requisitionId = Request.QueryString["ExpenseId"];
                    txtExpenseId.Text = requisitionId; // Assuming you have a textbox for RequisitionId
                }
            }
        }


        protected void btnBudgetMonitoring_Click(object sender, EventArgs e)
            {
           
            }

        protected void btnAllExpenseReport_Click(object sender, EventArgs e)
        {
                string query = @"SELECT ExpenseID, ExpenseName, ExpenseCategory, ItemName, Term, Amount, SchoolName, Address, SchoolCode, Logo
FROM   ExpenseReport
WHERE (SchoolCode = @SchoolCode) AND (Term = @Term)";

                // Create a DataTable to hold the data
                DataTable dataTable = new DataTable();

                // Fetch the data
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    using (SqlCommand command = new SqlCommand(query, Con))
                    {
                        command.Parameters.AddWithValue("@Term", txtTerm.Text.ToString());
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

            string query = @" SELECT ExpenseID, ExpenseName, ExpenseCategory, ItemName, Term, Amount, SchoolName, Address, SchoolCode, Logo, ExpenseItemId, Notes
FROM ExpenseReport
WHERE(ExpenseID = @ExpenseId) AND(SchoolCode = @SchoolCode)";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@ExpenseId", txtExpenseId.Text.ToString());
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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/ExpenseReport.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("ExpenseReport", dataTable);
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

        protected void btnIncomeStatement_Click(object sender, EventArgs e)
        {

        }
    }
    }



