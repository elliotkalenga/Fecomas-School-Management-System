using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace SMSWEBAPP.Views.Admin
{
    public partial class ExpenseItemApprove : System.Web.UI.Page
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

                    if (Request.QueryString["ExpenseItemId"] != null)
                    {
                        int ExpenseItemId;
                        if (int.TryParse(Request.QueryString["ExpenseItemId"], out ExpenseItemId))
                        {
                            string mode = Request.QueryString["mode"];
                            if (mode == "delete")
                            {
                                DeleteRecord(ExpenseItemId);
                            }
                            else
                            {
                                LoadRecordData(ExpenseItemId);
                            }
                        }
                    }
                }
            }

            protected void SetButtonText()
            {
                btnSubmit.Text = Request.QueryString["ExpenseItemId"] != null ? "Update" : "Add";
            }

            private void LoadRecordData(int ExpenseItemId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM ExpenseItem WHERE ExpenseItemId = @ExpenseItemId", Con))
                    {
                        cmd.Parameters.AddWithValue("@ExpenseItemId", ExpenseItemId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows && dr.Read())
                            {
                            }
                        }
                    }
                }
            }

            protected void btnSubmit_Click(object sender, EventArgs e)
            {
                if (Request.QueryString["ExpenseItemId"] != null)
                {
                    int ExpenseItemId;
                    if (int.TryParse(Request.QueryString["ExpenseItemId"], out ExpenseItemId))
                    {
                        UpdateRecord(ExpenseItemId);
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

            private void UpdateRecord(int ExpenseItemId)
            {
                try
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();

                        // Step 1: Check if the Requisition is Approved
                        string checkStatusQuery = @"
                             select E.ExpenseStatus from ExpenseItem EI Inner join Expense E on EI.ExpenseId=E.ExpenseId where EI.ExpenseItemId in
                             (select ExpenseItemId from ExpenseItem where ExpenseItemId=@ExpenseItemId)";
                        using (SqlCommand checkCmd = new SqlCommand(checkStatusQuery, Con))
                        {
                            checkCmd.Parameters.AddWithValue("@ExpenseItemId", ExpenseItemId);
                            string status = checkCmd.ExecuteScalar()?.ToString();

                            if (status == "Approved")
                            {
                                lblErrorMessage.Text = "Update failed! Expense Transaction is already approved.";
                                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                                return; // Exit the method to prevent update
                            }
                        }

                        // Step 2: Proceed with the update if not Approved
                        string updateQuery = @"UPDATE ExpenseItem 
                                   SET Notes = @Notes 
                                   WHERE ExpenseItemId = @ExpenseItemId";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, Con))
                        {
                            cmd.Parameters.AddWithValue("@Notes", txtDescription.Text.Trim());
                            cmd.Parameters.AddWithValue("@ExpenseItemId", ExpenseItemId);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    lblMessage.Text = "Expense Item Approved successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                }
                catch (SqlException ex)
                {
                    lblErrorMessage.Text = "Error updating record. Please try again. " + ex.Message;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }

            private void DeleteRecord(int ExpenseItemId)
            {
                try
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM ExpenseItem WHERE ExpenseItemId = @ExpenseItemId", Con))
                        {
                            cmd.Parameters.AddWithValue("@ExpenseItemId", ExpenseItemId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547) // Foreign key constraint violation error
                    {
                        lblErrorMessage.Text = "Error: This Expense cannot be deleted because it is linked to other records.";
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
                txtDescription.Text = string.Empty;

            }
        }
    }
