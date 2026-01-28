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

        public partial class ViewReceipt : System.Web.UI.Page
        {
            protected void Page_Load(object sender, EventArgs e)
            {
                if (!IsPostBack && Request.QueryString["FeesCollectionId"] != null)
                {
                    int feesCollectionId = int.Parse(Request.QueryString["FeesCollectionId"]);
                    LoadReport(feesCollectionId);
                }
            }

            private void LoadReport(int feescollectionid)
            {
                string query = @"SELECT feescollectionId, ReferenceNo, Student, InvoiceId, PaymentMethod, Fees, ReceiptAmount, Cum_Collection, balance, InvoiceStatus, SchoolName, Logo, createdDate, Address, Term, CreatedBy
                            FROM Receipt
                            WHERE feescollectionId = @feescollectionId";

                DataTable dataTable = new DataTable();
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@feescollectionid", feescollectionid);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                    }
                }

                if (dataTable.Rows.Count > 0)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/Receipt.rdlc");

                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("ReceiptData", dataTable));

                    ReportViewer1.LocalReport.EnableExternalImages = true;
                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("ImagePath3", "file:///C:/Logo/"));

                    ReportViewer1.LocalReport.Refresh();
                }
            }
        }
    }
