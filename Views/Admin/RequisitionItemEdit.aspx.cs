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
    public partial class RequisitionItemEdit : System.Web.UI.Page
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

                    if (Request.QueryString["RequisitionItemId"] != null)
                    {
                        int RequisitionItemId;
                        if (int.TryParse(Request.QueryString["RequisitionItemId"], out RequisitionItemId))
                        {
                            string mode = Request.QueryString["mode"];
                            if (mode == "delete")
                            {
                                DeleteRecord(RequisitionItemId);
                            }
                            else
                            {
                                LoadRecordData(RequisitionItemId);
                            }
                        }
                    }
                }
            }

            protected void SetButtonText()
            {
                btnSubmit.Text = Request.QueryString["RequisitionItemId"] != null ? "Update" : "Add";
            }

            private void LoadRecordData(int RequisitionItemId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM RequisitionItem WHERE RequisitionItemId = @RequisitionItemId", Con))
                    {
                        cmd.Parameters.AddWithValue("@RequisitionItemId", RequisitionItemId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows && dr.Read())
                            {
                                txtItem.Text = dr["RequisitionItemName"].ToString();
                                txtAmount.Text = dr["Amount"].ToString();
                            }
                        }
                    }
                }
            }

            protected void btnSubmit_Click(object sender, EventArgs e)
            {
                if (Request.QueryString["RequisitionItemId"] != null)
                {
                    int RequisitionItemId;
                    if (int.TryParse(Request.QueryString["RequisitionItemId"], out RequisitionItemId))
                    {
                        UpdateRecord(RequisitionItemId);
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

            private void UpdateRecord(int RequisitionItemId)
            {
                try
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();

                        // Step 1: Check if the Requisition is Approved
                        string checkStatusQuery = @"
                             select R.RequisitionStatus from RequisitionItem RI Inner join Requisition R on RI.RequisitionId=R.RequisitionId where RI.RequisitionItemID in
                             (select RequisitionItemID from RequisitionItem where RequisitionItemID=@RequisitionItemId)";
                        using (SqlCommand checkCmd = new SqlCommand(checkStatusQuery, Con))
                        {
                            checkCmd.Parameters.AddWithValue("@RequisitionItemId", RequisitionItemId);
                            string status = checkCmd.ExecuteScalar()?.ToString();

                            if (status == "Approved")
                            {
                                lblErrorMessage.Text = "Update failed! Requisition is already approved.";
                                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                                return; // Exit the method to prevent update
                            }
                        }

                        // Step 2: Proceed with the update if not Approved
                        string updateQuery = @"UPDATE RequisitionItem 
                                   SET RequisitionItemName = @RequisitionItemName, 
                                       Amount = @Amount
                                   WHERE RequisitionItemId = @RequisitionItemId";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, Con))
                        {
                        cmd.Parameters.AddWithValue("@RequisitionItemName", txtItem.Text.Trim());
                        cmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Trim());
                            cmd.Parameters.AddWithValue("@RequisitionItemId", RequisitionItemId);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    lblMessage.Text = "Requisition Item updated successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                }
                catch (SqlException ex)
                {
                    lblErrorMessage.Text = "Error updating record. Please try again. " + ex.Message;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }

            private void DeleteRecord(int RequisitionItemId)
            {
                try
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM RequisitionItem WHERE RequisitionItemId = @RequisitionItemId", Con))
                        {
                            cmd.Parameters.AddWithValue("@RequisitionItemId", RequisitionItemId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547) // Foreign key constraint violation error
                    {
                        lblErrorMessage.Text = "Error: This requisition cannot be deleted because it is linked to other records.";
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
            txtAmount.Text = string.Empty;
            txtItem.Text = string.Empty;

        }
    }
    }
