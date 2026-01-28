using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace SMSWEBAPP.Views.Admin
{
    public partial class ExpenseItemAdd : System.Web.UI.Page
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
                    PopulateDropDownLists();

                    if (Request.QueryString["ExpenseId"] != null)
                    {
                        int ExpenseId;
                        if (int.TryParse(Request.QueryString["ExpenseId"], out ExpenseId))
                        {
                            string mode = Request.QueryString["mode"];
                            if (mode == "delete")
                            {
                                DeleteRecord(ExpenseId);
                            }
                            else
                            {
                                LoadRecordData(ExpenseId);
                                BindRecordsRepeater(); // Moved here
                            }
                        }
                    }
                }
            }

            protected void SetButtonText()
            {
                btnSubmit.Text = Request.QueryString["ExpenseId"] != null ? "Submit" : "Add";
            }

            private void PopulateDropDownLists()
            {
            }

            private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField)
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        ddl.DataSource = dr;
                        ddl.DataTextField = textField;
                        ddl.DataValueField = valueField;
                        ddl.DataBind();
                    }
                }
            }

            private void LoadRecordData(int ExpenseId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Expense WHERE ExpenseId = @ExpenseId", Con))
                    {
                        cmd.Parameters.AddWithValue("@ExpenseId", ExpenseId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows && dr.Read())
                            {
                                txtExpenseId.Text = dr["ExpenseId"].ToString();
                            }
                        }
                    }
                }
            }

            protected void btnSubmit_Click(object sender, EventArgs e)
            {
                if (Request.QueryString["ExpenseId"] != null)
                {
                    int ExpenseId;
                    if (int.TryParse(Request.QueryString["ExpenseId"], out ExpenseId))
                    {
                        UpdateRecord(ExpenseId);
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

            private void UpdateRecord(int ExpenseId)
            {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();

                    // Step 1: Check if the Requisition is Approved
                    string checkStatusQuery = "SELECT ExpenseStatus FROM Expense WHERE ExpenseId = @ExpenseId";
                    using (SqlCommand checkCmd = new SqlCommand(checkStatusQuery, Con))
                    {
                        checkCmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = Convert.ToInt32(txtExpenseId.Text.Trim());
                        object statusObj = checkCmd.ExecuteScalar();
                        string status = statusObj != null ? statusObj.ToString() : string.Empty;

                        if (status == "Approved")
                        {
                            lblErrorMessage.Text = "Update failed! This Expense Transaction is already approved.";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                            return; // Exit the method to prevent update
                        }
                    }

                    // Step 2: Proceed with the update if not Approved
                    string updateQuery = @"INSERT INTO ExpenseItem (ItemName, Amount, ExpenseId, CreatedBy, SchoolId,Notes)
                               VALUES (@ItemName, @Amount, @ExpenseId, @CreatedBy, @SchoolId,@Notes)";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, Con))
                    {
                        cmd.Parameters.Add("@Notes", SqlDbType.NVarChar, 255).Value = "Pending";
                        cmd.Parameters.Add("@ItemName", SqlDbType.NVarChar, 255).Value = txtItem.Text.Trim();
                        cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Convert.ToDecimal(txtAmount.Text.Trim());
                        cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = Convert.ToInt32(txtExpenseId.Text.Trim());
                        cmd.Parameters.Add("@SchoolId", SqlDbType.Int).Value = Session["SchoolId"] != null ? Convert.ToInt32(Session["SchoolId"]) : (object)DBNull.Value;
                        cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100).Value = Session["Username"] != null ? Session["Username"].ToString() : (object)DBNull.Value;

                        cmd.ExecuteNonQuery();
                    }
                }

                BindRecordsRepeater(); // Refresh records after insertion

                lblMessage.Text = "Expense Item added successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating record. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteRecord(int ExpenseId)
            {
                try
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM expense WHERE ExpenseId = @ExpenseId", Con))
                        {
                            cmd.Parameters.AddWithValue("@ExpenseId", ExpenseId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Response.Redirect("Expenses.aspx?deleteSuccess=true");
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


            private List<ExpenseModel> GetRecordsList()
            {
                List<ExpenseModel> ExpenseList = new List<ExpenseModel>();

                try
                {
                    using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        string query = @"
                SELECT ExpenseItemId, ItemName, Format(Amount,'N0') As Amount, Notes 
 FROM ExpenseItem 
                WHERE ExpenseId = @ExpenseId;
            ";

                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            // Ensure txtExpenseId.Text is not null or empty
                            string ExpenseId = string.IsNullOrWhiteSpace(txtExpenseId.Text) ? DBNull.Value.ToString() : txtExpenseId.Text;

                            cmd.Parameters.AddWithValue("@ExpenseId", ExpenseId);

                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    ExpenseList.Add(new ExpenseModel
                                    {
                                        ExpenseItemId = dr["ExpenseItemId"].ToString(),
                                        ExpenseItemName = dr["ItemName"].ToString(), // Fixed casing
                                        Amount = dr["Amount"].ToString(),
                                        Notes = dr["Notes"].ToString(),
                                    });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error properly
                    System.Diagnostics.Debug.WriteLine($"[GetRecordsList] Error: {ex}");

                    // Display a user-friendly alert
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert",
                        $"alert('An error occurred while fetching records. Please try again.');", true);
                }

                return ExpenseList;
            }

            private void BindRecordsRepeater()
            {
                List<ExpenseModel> expenses = GetRecordsList();
                RecordsRepeater.DataSource = expenses;
                RecordsRepeater.DataBind();
            }

        }
    }
