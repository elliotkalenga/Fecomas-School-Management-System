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
    public partial class ExpenseAdd : System.Web.UI.Page
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
                            }
                        }
                    }
                }
            }

            protected void SetButtonText()
            {
                btnSubmit.Text = Request.QueryString["ExpenseId"] != null ? "Update" : "Add";
            }

            private void PopulateDropDownLists()
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string TermQry = @"select T.TermId, TN.TermNumber+'('+y.FinancialYear+')'  as Term from term T
                                    inner Join TermNumber TN on T.Term=TN.Termid
                                    inner Join FinancialYear y on T.YearId=y.FinancialYearId where T.Status=2 and T.SchoolId=@SchoolId";

                    string RequisitionQry = @"SELECT 0 AS RequisitionId, '-- Select Target Requisition --' AS Requisition
                                      UNION  
                                      SELECT RequisitionId, Purpose as Requisition FROM Requisition where SchoolId=@SchoolId";
                    string CategoryQry = @"SELECT 0 AS ExpenseCategoryId, '-- Select Expense Category --' AS ExpenseCategory
                                      UNION  
                                      SELECT ExpenseCategoryId, ExpenseCategory FROM ExpenseCategory ";

                    Con.Open();
                    PopulateDropDownList(Con, TermQry, ddlTerm, "Term", "TermId");
                    PopulateDropDownList(Con, RequisitionQry, ddlRequisition, "Requisition", "RequisitionId");
                    PopulateDropDownList(Con, CategoryQry, ddlCategory, "ExpenseCategory", "ExpenseCategoryId");
                }
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
                                txtDescription.Text = dr["Description"].ToString();
                                ddlRequisition.SelectedValue = dr["RequisitionId"].ToString();
                                ddlCategory.SelectedValue = dr["ExpenseCategoryId"].ToString();
                            ddlTerm.SelectedValue = dr["TermId"].ToString();
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
                try

                {
                    if (ddlCategory.SelectedValue == "0")
                    {
                        lblErrorMessage.Text = "Please select Expense Category.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                        return;
                    }

                    if (ddlRequisition.SelectedValue == "0")
                    {
                        lblErrorMessage.Text = "Please select Target Requisition";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                        return;
                    }

                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        string query = @"INSERT INTO Expense (ExpenseStatus,Description, TermId, ExpenseCategoryId, RequisitionId, CreatedBy, SchoolId) 
                                     VALUES (@ExpenseStatus,@Description, @TermId, @ExpenseCategoryId, @RequisitionId, @CreatedBy, @SchoolId)";
                        using (SqlCommand cmd = new SqlCommand(query, Con))
                        {
                            cmd.Parameters.AddWithValue("@ExpenseStatus", "Pending");
                            cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                            cmd.Parameters.AddWithValue("@RequisitionId", ddlRequisition.SelectedValue);
                            cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                            cmd.Parameters.AddWithValue("@ExpenseCategoryId", ddlCategory.SelectedValue);
                            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"] ?? DBNull.Value);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    lblMessage.Text = "Expense  Created successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                }
                catch (SqlException ex)
                {
                    lblErrorMessage.Text = "Error adding Record. Please try again. " + ex.Message;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
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
                            checkCmd.Parameters.AddWithValue("@ExpenseId", ExpenseId);
                            string status = checkCmd.ExecuteScalar()?.ToString();

                            if (status == "Approved")
                            {
                                lblErrorMessage.Text = "Update failed! Expense is already approved.";
                                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                                return; // Exit the method to prevent update
                            }
                        }

                        // Step 2: Proceed with the update if not Approved
                        string updateQuery = @"UPDATE Expense 
                                   SET Description = @Description, 
                                       ExpenseCategoryId = @ExpenseCategoryId,
                                       RequisitionId = @RequisitionId
                                   WHERE ExpenseId = @ExpenseId";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, Con))
                        {
                             cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                             cmd.Parameters.AddWithValue("@RequisitionId", ddlRequisition.SelectedValue);
                             cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                             cmd.Parameters.AddWithValue("@ExpenseCategoryId", ddlCategory.SelectedValue);

                            cmd.Parameters.AddWithValue("@ExpenseId", ExpenseId);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    lblMessage.Text = "Expense Transaction updated successfully!";
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
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Expenseitem WHERE ExpenseId = @ExpenseId", Con))
                    {
                        cmd.Parameters.AddWithValue("@ExpenseId", ExpenseId);
                        cmd.ExecuteNonQuery();
                    }
                }

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Expense WHERE ExpenseId = @ExpenseId", Con))
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
                        lblErrorMessage.Text = "Error: This Expense cannot be deleted because it is linked to other records.";
                    }
                    else
                    {
                        lblErrorMessage.Text = "An unexpected error occurred while deleting the expense. Please try again.";
                    }

                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }

            private void ClearControls()
            {
                txtDescription.Text = string.Empty;
                ddlRequisition.SelectedIndex = 0;
                ddlCategory.SelectedIndex = 0;
            }
        }
    }
