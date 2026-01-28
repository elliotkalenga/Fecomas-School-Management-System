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
	public partial class InvoiceItems : System.Web.UI.Page
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
                BindRecordsRepeater();
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
                            BindRecordsRepeater();

                        }
                    }
                    }
                }
            }

            protected void SetButtonText()
            {
                btnSubmit.Text = Request.QueryString["InvoiceId"] != null ? "Add Item" : "Add";
            }


            private void LoadRecordData(int InvoiceId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Invoice WHERE InvoiceId = @InvoiceId", Con))
                    {
                        cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows && dr.Read())
                            {
                            txtInvoiceId.Text = dr["InvoiceId"].ToString();
                            txtInvoiceNumber.Text = dr["InvoiceNumber"].ToString();
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
                int quantity;
                if (!int.TryParse(txtQuantity.Text, out quantity) || quantity < 1)
                {
                    lblErrorMessage.Text = "This should be a positive integer";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                decimal UnitPrice;
                if (!decimal.TryParse(txtUnitPrice.Text, out UnitPrice) || UnitPrice < 1)
                {
                    lblErrorMessage.Text = "Invalid Input! Unit Price should be positive number";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"INSERT INTO InvoiceItems (InvoiceId,Item,Quantity, UnitPrice,SchoolId) 
                                     VALUES (@InvoiceId,@Item,@Quantity, @UnitPrice,@SchoolId)";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@Item", txtItem.Text.Trim());
                        cmd.Parameters.AddWithValue("@InvoiceId", txtInvoiceId.Text.Trim());
                        cmd.Parameters.AddWithValue("@Quantity", txtQuantity.Text.Trim());
                        cmd.Parameters.AddWithValue("@UnitPrice", txtUnitPrice.Text.Trim());
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                        BindRecordsRepeater();

                    }
                }

                lblMessage.Text = "Item Added successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error adding Record. Please try again. " + ex.Message;
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
            txtItem.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            txtUnitPrice.Text = string.Empty;
            }

        protected decimal grandTotal = 0;

        protected void RecordsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                decimal subTotal = Convert.ToDecimal(DataBinder.Eval(e.Item.DataItem, "SubTotal"));
                grandTotal += subTotal;
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                Label lblGrandTotal = (Label)e.Item.FindControl("lblGrandTotal");
                lblGrandTotal.Text = grandTotal.ToString("N2");
            }
        }

        private List<InvoiceDetail> GetRecordsList()
        {
            List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>();

            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"Select * from InvoiceItems where InvoiceId=@InvoiceId
 ";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@InvoiceId", txtInvoiceId.Text.Trim());

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                invoiceDetails.Add(new InvoiceDetail
                                {
                                    InvoiceId = dr["InvoiceId"].ToString(),
                                    Item = dr["Item"].ToString(),
                                    Quantity= dr["Quantity"].ToString(),
                                    UnitPrice = dr["UnitPrice"].ToString(),
                                    SubTotal = dr["SUbTotal"].ToString(),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error instead of Response.Write
                System.Diagnostics.Debug.WriteLine($"Error fetching Invoice records: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert",
         $"alert('An error occurred: {ex.Message}. Check logs for details.');", true);
            }

            return invoiceDetails;
        }

        private void BindRecordsRepeater()
        {
            List<InvoiceDetail> invoiceDetails = GetRecordsList();
            RecordsRepeater.DataSource = invoiceDetails;
            RecordsRepeater.DataBind();
        }

    }
}
