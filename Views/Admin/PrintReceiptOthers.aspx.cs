using Microsoft.Reporting.WebForms;
using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class PrintReceiptOthers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["FeesCollectionId"] != null)
                {
                    int feesCollectionId = int.Parse(Request.QueryString["FeesCollectionId"]);
                    string mode = Request.QueryString["mode"];

                    if (mode == "delete")
                    {


                    }
                    else
                    {
                        LoadFeesCollectionData(feesCollectionId);
                        GenerateReceipt(feesCollectionId);
                    }
                }
            }
        }

        private void LoadFeesCollectionData(int feesCollectionId)
        {
            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                con.Open();

                string query = @"
                    SELECT FeesCollectionId, ReferenceNo, StudentNo, Student, FeesName, Description, AmountCollected, ClassName, Term, PaymentMethod, CreatedBy, CreatedDate, SchoolName, SchoolCode, Logo, address
FROM   FeesCollectionSummaryOthers
WHERE (FeesCollectionId = @FeescollectionId)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FeesCollectionId", feesCollectionId);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        txtFeesCollectionId.Text = dr["FeesCollectionId"].ToString();
                    }
                    else
                    {
                        lblMessage.Text = "Transaction not found.";
                        lblMessage.CssClass = "alert alert-warning";
                    }
                }
            }
        }

        private void GenerateReceipt(int feesCollectionId)
        {
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.Refresh();

            if (int.TryParse(txtFeesCollectionId.Text, out int id))
            {
                LoadReport(id);
            }
            else
            {
                lblMessage.Text = "Invalid Fees Collection ID!";
                lblMessage.CssClass = "alert alert-warning";
            }
        }

        private void LoadReport(int feesCollectionId)
        {
            string query = @"
                SELECT FeesCollectionId, ReferenceNo, StudentNo, Student, FeesName, Description, AmountCollected, ClassName, Term, PaymentMethod, CreatedBy, CreatedDate, SchoolName, SchoolCode, Logo, address
FROM   FeesCollectionSummaryOthers
WHERE (FeesCollectionId = @FeescollectionId)";

            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FeesCollectionId", feesCollectionId);
                    cmd.CommandTimeout = 130;

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                if (dataTable.Rows.Count == 0)
                {
                    lblMessage.Text = "No results found for selected transaction!";
                    lblMessage.CssClass = "alert alert-warning";
                    ReportViewer1.Visible = false;
                }
                else
                {
                    lblMessage.CssClass = "d-none";
                    ReportViewer1.Visible = true;

                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/PrintOtherReceipt.rdlc");
                    ReportViewer1.LocalReport.EnableExternalImages = true;

                    ReportDataSource reportDataSource = new ReportDataSource("PrintOtherReceipt", dataTable);
                    ReportViewer1.LocalReport.DataSources.Add(reportDataSource);

                    string imagePath = "file:///C:/Logo/";
                    ReportParameter param = new ReportParameter("ImagePath3", imagePath);
                    ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { param });

                    ReportViewer1.LocalReport.Refresh();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger";
            }
        }

        protected void btnPrintPdf_Click(object sender, EventArgs e)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            byte[] bytes = ReportViewer1.LocalReport.Render(
                "PDF", null, out mimeType, out encoding, out extension,
                out streamids, out warnings);

            // Send to browser
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("Content-disposition", "inline; filename=Receipt.pdf");
            Response.BinaryWrite(bytes);
            Response.End();
        }

        private void ClearFormFields()
        {
            txtFeesCollectionId.Text = "";
        }

        protected void txtFeesCollectionId_TextChanged(object sender, EventArgs e)
        {
            if (Request.QueryString["FeesCollectionId"] != null)
            {
                int feesCollectionId = int.Parse(Request.QueryString["FeesCollectionId"]);
                GenerateReceipt(feesCollectionId);
                ClearFormFields();
            }
        }
    }
}
