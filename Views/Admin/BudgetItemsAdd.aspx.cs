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
    public partial class BudgetItemsAdd : System.Web.UI.Page
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

                if (Request.QueryString["BudgetItemId"] != null)
                {
                    int BudgetItemId;
                    if (int.TryParse(Request.QueryString["BudgetItemId"], out BudgetItemId))
                    {
                        string mode = Request.QueryString["mode"];
                        if (mode == "delete")
                        {
                            DeleteBook(BudgetItemId);
                        }
                        else
                        {
                            LoadRecordData(BudgetItemId);
                        }
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            btnSubmit.Text = Request.QueryString["BudgetItemId"] != null ? "Update" : "Add";
        }

        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string BudgetQry = @"									select BudgetID,B.BudgetName + '-'+TN.TermNumber + '('+FY.FinancialYear+')' As Budget from Budget B inner join Term T on B.TermID=T.TermId
                                        Inner Join TermNumber TN on T.Term=TN.TermId
                                        Inner Join FinancialYear FY on T.Yearid=FY.FinancialYearid Where 
                                        B.SchoolId=@SchoolId and T.Status=2";

                string CategoryQry = @"Select 0 as BudgetCategoryId,'Select Budget Category--' as CategoryName Union  select BudgetCategoryId,CategoryName  from BudgetCategory";


                Con.Open();
                PopulateDropDownList(Con, BudgetQry, ddlBudget, "Budget", "BudgetId");
                PopulateDropDownList(Con, CategoryQry, ddlCategory, "CategoryName", "BudgetCategoryId");
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

        private void LoadRecordData(int BudgetItemId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM  BudgetItems WHERE BudgetItemId = @BudgetItemId", Con))
                {
                    cmd.Parameters.AddWithValue("@BudgetItemId", BudgetItemId);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows && dr.Read())
                        {
                            txtItemName.Text = dr["ItemName"].ToString();
                            txtAmount.Text = dr["Amount"].ToString();
                            txtDescription.Text = dr["Description"].ToString();
                            ddlBudget.SelectedValue = dr["BudgetId"].ToString();
                            ddlCategory.SelectedValue = dr["BudgetCategoryID"].ToString();
                        }
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["BudgetItemId"] != null)
            {
                int BudgetItemId;
                if (int.TryParse(Request.QueryString["BudgetItemId"], out BudgetItemId))
                {
                    UpdateRecord(BudgetItemId);
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
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"INSERT INTO BudgetItems (BudgetId,BudgetCategoryId,Amount,Description,ItemName,CreatedBy,SchoolId) 
                                     VALUES (@BudgetId,@BudgetCategoryId,@Amount,@Descriptionn,@ItemName,@CreatedBy,@SchoolId)";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Trim());
                        cmd.Parameters.AddWithValue("@ItemName", txtItemName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Descriptionn", txtDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@BudgetId", ddlBudget.SelectedValue);
                        cmd.Parameters.AddWithValue("@BudgetCategoryId", ddlCategory.SelectedValue);
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"] ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Budget Item added successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error adding  Record. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void UpdateRecord(int BudgetItemId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"UPDATE BudgetItems 
                             SET ItemName = @ItemName, 
                                 BudgetCategoryId = @BudgetCategoryId, 
                                 Description = @Description, 
                                 BudgetId = @BudgetId, 
                                 Amount = @Amount, 
                                 UpdatedBy = @UpdatedBy 
                             WHERE BudgetItemId = @BudgetItemId";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        // Add parameters ensuring they are unique
                        cmd.Parameters.AddWithValue("@ItemName", txtItemName.Text.Trim());
                        cmd.Parameters.AddWithValue("@BudgetCategoryId", ddlCategory.SelectedValue);
                        cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@BudgetId", ddlBudget.SelectedValue);
                        cmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Trim());
                        cmd.Parameters.AddWithValue("@UpdatedBy", Session["Username"] ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BudgetItemId", BudgetItemId);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Record updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating Record. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteBook(int BudgetItemId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM BudgetItems WHERE BudgetItemId = @BudgetItemId", Con))
                {
                    cmd.Parameters.AddWithValue("@BudgetItemId", BudgetItemId);
                    cmd.ExecuteNonQuery();
                }
            }
            Response.Redirect("Budget.aspx?deleteSuccess=true");
        }

        private void ClearControls()
        {
            txtItemName.Text = string.Empty;
            txtAmount.Text = string.Empty;
            txtDescription.Text = string.Empty;
            ddlBudget.SelectedIndex = 0;
            ddlCategory.SelectedIndex = 0;
        }

    }
}