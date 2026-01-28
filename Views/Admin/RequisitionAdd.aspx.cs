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
    public partial class RequisitionAdd : System.Web.UI.Page
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

                    if (Request.QueryString["RequisitionId"] != null)
                    {
                        int RequisitionId;
                        if (int.TryParse(Request.QueryString["RequisitionId"], out RequisitionId))
                        {
                            string mode = Request.QueryString["mode"];
                            if (mode == "delete")
                            {
                                DeleteRecord(RequisitionId);
                            }
                            else
                            {
                                LoadRecordData(RequisitionId);
                            }
                        }
                    }
                }
            }

            protected void SetButtonText()
            {
                btnSubmit.Text = Request.QueryString["RequisitionId"] != null ? "Update" : "Add";
            }

            private void PopulateDropDownLists()
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string TermQry = @"select T.TermId, TN.TermNumber+'('+y.FinancialYear+')'  as Term from term T
                                    inner Join TermNumber TN on T.Term=TN.Termid
                                    inner Join FinancialYear y on T.YearId=y.FinancialYearId where T.Status=2 and T.SchoolId=@SchoolId";

                string BudgetQry = @"SELECT 0 AS BudgetItemId, '-- Select Cost Center (Budget) --' AS ItemName
                                      UNION  
                                      SELECT BudgetItemId, ItemName FROM BudgetItems where SchoolId=@SchoolId";
                string CategoryQry = @"SELECT 0 AS RequisitionCategoryId, '-- Select Requisition Category --' AS RequisitionCategory
                                      UNION  
                                      SELECT RequisitionCategoryId, RequisitionCategory FROM RequisitionCategory ";

                Con.Open();
                    PopulateDropDownList(Con, TermQry, ddlTerm, "Term", "TermId");
                PopulateDropDownList(Con, BudgetQry, ddlBudget, "ItemName", "BudgetItemId");
                PopulateDropDownList(Con, CategoryQry, ddlCategory, "RequisitionCategory", "RequisitionCategoryId");
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

            private void LoadRecordData(int RequisitionId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Requisition WHERE RequisitionId = @RequisitionId", Con))
                    {
                        cmd.Parameters.AddWithValue("@RequisitionId", RequisitionId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows && dr.Read())
                            {
                                txtPurpose.Text = dr["Purpose"].ToString();
                                ddlBudget.SelectedValue = dr["BudgetItemId"].ToString();
                            ddlCategory.SelectedValue = dr["RequisitionCategoryId"].ToString();
                            ddlTerm.SelectedValue = dr["TermId"].ToString();
                        }
                    }
                    }
                }
            }

            protected void btnSubmit_Click(object sender, EventArgs e)
            {
                if (Request.QueryString["RequisitionId"] != null)
                {
                    int RequisitionId;
                    if (int.TryParse(Request.QueryString["RequisitionId"], out RequisitionId))
                    {
                        UpdateRecord(RequisitionId);
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
                        lblErrorMessage.Text = "Please select Requsition Category.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                        return;
                    }

                if (ddlBudget.SelectedValue == "0")
                {
                    lblErrorMessage.Text = "Please select Cost Centre (Budget)";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        string query = @"INSERT INTO Requisition (RequisitionStatus,Purpose, TermId, RequisitionCategoryId, BudgetItemId, CreatedBy, SchoolId) 
                                     VALUES (@RequisitionStatus,@Purpose, @TermId, @RequisitionCategoryId, @BudgetItemId, @CreatedBy, @SchoolId)";
                        using (SqlCommand cmd = new SqlCommand(query, Con))
                        {
                            cmd.Parameters.AddWithValue("@RequisitionStatus", "Pending");
                            cmd.Parameters.AddWithValue("@Purpose", txtPurpose.Text.Trim());
                            cmd.Parameters.AddWithValue("@BudgetItemId", ddlBudget.SelectedValue);
                            cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                            cmd.Parameters.AddWithValue("@RequisitionCategoryId", ddlCategory.SelectedValue);
                            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"] ?? DBNull.Value);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    lblMessage.Text = "Requisition  created successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                }
                catch (SqlException ex)
                {
                    lblErrorMessage.Text = "Error adding Record. Please try again. " + ex.Message;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }

        private void UpdateRecord(int RequisitionId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();

                    // Step 1: Check if the Requisition is Approved
                    string checkStatusQuery = "SELECT RequisitionStatus FROM Requisition WHERE RequisitionId = @RequisitionId";
                    using (SqlCommand checkCmd = new SqlCommand(checkStatusQuery, Con))
                    {
                        checkCmd.Parameters.AddWithValue("@RequisitionId", RequisitionId);
                        string status = checkCmd.ExecuteScalar()?.ToString();

                        if (status == "Approved")
                        {
                            lblErrorMessage.Text = "Update failed! Requisition is already approved.";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                            return; // Exit the method to prevent update
                        }
                    }

                    // Step 2: Proceed with the update if not Approved
                    string updateQuery = @"UPDATE Requisition 
                                   SET Purpose = @Purpose, 
                                       RequisitionCategoryId = @RequisitionCategoryId,
                                       BudgetItemId = @BudgetItemId
                                   WHERE RequisitionId = @RequisitionId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, Con))
                    {
                        cmd.Parameters.AddWithValue("@Purpose", txtPurpose.Text.Trim());
                        cmd.Parameters.AddWithValue("@BudgetItemId", ddlBudget.SelectedValue);
                        cmd.Parameters.AddWithValue("@RequisitionCategoryId", ddlCategory.SelectedValue);
                        cmd.Parameters.AddWithValue("@RequisitionId", RequisitionId);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Requisition updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating record. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteRecord(int RequisitionId)
        {
            try
            {

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM requisitionitem WHERE RequisitionId = @RequisitionId", Con))
                    {
                        cmd.Parameters.AddWithValue("@RequisitionId", RequisitionId);
                        cmd.ExecuteNonQuery();
                    }
                }

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM requisition WHERE RequisitionId = @RequisitionId", Con))
                    {
                        cmd.Parameters.AddWithValue("@RequisitionId", RequisitionId);
                        cmd.ExecuteNonQuery();
                    }
                }
                Response.Redirect("Requisition.aspx?deleteSuccess=true");

            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) // Foreign key constraint violation error
                {
                    lblErrorMessage.Text = "Error: This Requisition cannot be deleted because it is linked to other records.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);

                }
                else
                {
                    lblErrorMessage.Text = "An unexpected error occurred while deleting the Requisition. Please try again.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);

                }

                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void ClearControls()
            {
                txtPurpose.Text = string.Empty;
                ddlBudget.SelectedIndex = 0;
                ddlCategory.SelectedIndex = 0;
            }
        }
    }
