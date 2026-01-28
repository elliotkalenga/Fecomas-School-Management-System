using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
	public partial class PayInvoice : System.Web.UI.Page
	{
            protected void Page_Load(object sender, EventArgs e)
            {
                if (Session["User"] == null)
                {
                    Response.Redirect("UserLogin.aspx");
                }

                if (!IsPostBack)
                {
                    SetButtonText();

                    if (Request.QueryString["InvoiceId"] != null)
                    {
                        int InvoiceId;
                        if (int.TryParse(Request.QueryString["InvoiceId"], out InvoiceId))
                        {
                            string mode = Request.QueryString["mode"];
                            if (mode == "delete")
                            {
                                DeleteRecord(InvoiceId);
                            }
                            else
                            {
                                LoadRecordData(InvoiceId);
                            }
                        }
                    }
                }
            }

            protected void SetButtonText()
            {
                btnSubmit.Text = Request.QueryString["InvoiceId"] != null ? "PAY" : "Add";
            }



            private void LoadRecordData(int InvoiceId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    using (SqlCommand cmd = new SqlCommand(@"SELECT 
    TN.TermNumber + ' (' + FY.FinancialYear + ')' AS Term,
    i.InvoiceId,
    i.InvoiceNumber,
    ISNULL(SUM(it.Subtotal), 0) AS Amount,  -- Ensure 0 if no items exist
    i.InvoiceDescription, 
    i.status, 
    S.SchoolName 
FROM Invoice i
LEFT JOIN InvoiceItems it ON it.InvoiceID = i.InvoiceId  -- Change INNER JOIN to LEFT JOIN
INNER JOIN School S ON i.CustomerId = S.SchoolID
INNER JOIN Term T ON i.TermID = T.TermId
INNER JOIN TermNumber TN ON T.Term = TN.TermId
INNER JOIN FinancialYear FY ON T.YearId = FY.FinancialYearId
WHERE i.InvoiceId = @InvoiceId
GROUP BY 
    i.InvoiceId, 
    i.InvoiceNumber, 
    i.InvoiceDescription, 
    i.status, 
    S.SchoolName, 
    TN.TermNumber, 
    FY.FinancialYear;


 ", Con))
                    {
                        cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows && dr.Read())
                            {
                                txtAmount.Text = dr["Amount"].ToString();
                            }
                        }
                    }
                }
            }

            protected void btnSubmit_Click(object sender, EventArgs e)
            {
                if (Request.QueryString["InvoiceId"] != null)
                {
                    int InvoiceId;
                    if (int.TryParse(Request.QueryString["InvoiceId"], out InvoiceId))
                    {
                        UpdateRecord(InvoiceId);
                    }
                }
                else
                {
                    AddNewRecord();
                }

                ClearControls();
            }

            private void AddNewRecord()
            {
            }

            private void UpdateRecord(int InvoiceId)
            {
                try
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();

                        // Step 1: Check if the Requisition is Approved
                        string checkStatusQuery = "SELECT Status FROM Invoice WHERE InvoiceId = @InvoiceId";
                        using (SqlCommand checkCmd = new SqlCommand(checkStatusQuery, Con))
                        {
                            checkCmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
                            string status = checkCmd.ExecuteScalar()?.ToString();

                            if (status == "Paid")
                            {
                                lblErrorMessage.Text = "Update failed! Invoice is already Paid.";
                                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                                return; // Exit the method to prevent update
                            }
                        }

                        // Step 2: Proceed with the update if not Approved
                        string updateQuery = @"UPDATE Invoice 
                                   SET PaidAmount = @PaidAmount,
                                    Status = 'Paid'
                                       
                                   WHERE InvoiceId = @InvoiceId";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, Con))
                        {
                            cmd.Parameters.AddWithValue("@PaidAmount", txtAmount.Text.Trim());

                            cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    lblMessage.Text = "Invoice has been Paid successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                }
                catch (SqlException ex)
                {
                    lblErrorMessage.Text = "Error updating record. Please try again. " + ex.Message;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }

            private void DeleteRecord(int InvoiceId)
            {
                try
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Invoice WHERE InvoiceId = @InvoiceId", Con))
                        {
                            cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Response.Redirect("Invoice.aspx?deleteSuccess=true");
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547) // Foreign key constraint violation error
                    {
                        lblErrorMessage.Text = "Error: This Invoice cannot be deleted because it is linked to other records.";
                    }
                    else
                    {
                        lblErrorMessage.Text = "An unexpected error occurred while deleting the requisition. Please try again.";
                    }

                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }

            private void ClearControls()
            {
            }
        }
    }
