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
    public partial class BudgetAdd : System.Web.UI.Page
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

                if (Request.QueryString["BudgetId"] != null)
                {
                    int BudgetId;
                    if (int.TryParse(Request.QueryString["BudgetId"], out BudgetId))
                    {
                        string mode = Request.QueryString["mode"];
                        if (mode == "delete")
                        {
                            DeleteBook(BudgetId);
                        }
                        else
                        {
                            LoadRecordData(BudgetId);
                        }
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            btnSubmit.Text = Request.QueryString["BudgetId"] != null ? "Update" : "Add";
        }

        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string TermQry = @"select T.TermId, TN.TermNumber+'('+y.FinancialYear+')'  as Term from term T
                                    inner Join TermNumber TN on T.Term=TN.Termid
                                    inner Join FinancialYear y on T.YearId=y.FinancialYearId where T.Status=2 and T.SchoolId=@SchoolId";

                string StatusQry = @"Select 0 as StatusId, '---Select Status---' as Status Union
                                    Select StatusId, Status from Status";


                Con.Open();
                PopulateDropDownList(Con, TermQry, ddlTerm, "Term", "TermId");
                PopulateDropDownList(Con, StatusQry, ddlStatus, "Status", "StatusId");
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

        private void LoadRecordData(int BudgetId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM  Budget WHERE BudgetId = @BudgetId", Con))
                {
                    cmd.Parameters.AddWithValue("@BudgetId", BudgetId);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows && dr.Read())
                        {
                            txtBudgetName.Text = dr["BudgetName"].ToString();
                           txtContingency.Text = dr["Contingency"].ToString();
                            txtDescription.Text = dr["Description"].ToString();
                            ddlTerm.SelectedValue = dr["TermId"].ToString();
                            ddlStatus.SelectedValue = dr["Status"].ToString(); 
                        }
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["BudgetId"] != null)
            {
                int BudgetId;
                if (int.TryParse(Request.QueryString["BudgetId"], out BudgetId))
                {
                    UpdateRecord(BudgetId);
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

                    // Proceed with the insertion of the new budget
                    string query = @"INSERT INTO Budget (BudgetName, TermId, Description, CreatedBy, SchoolId, Status, Contingency) 
                             VALUES (@BudgetName, @TermId, @Description, @CreatedBy, @SchoolId, @Status, @Contingency)";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@Contingency", txtContingency.Text.Trim());
                        cmd.Parameters.AddWithValue("@BudgetName", txtBudgetName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                        cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"] ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }

                    int schoolId = Session["SchoolId"] == DBNull.Value ? 0 : Convert.ToInt32(Session["SchoolId"]);
                    int status = Convert.ToInt32(ddlStatus.SelectedValue);
                    CheckBudgetStatusForNewRecord(schoolId, status);
                }

                lblMessage.Text = "Budget added successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                // If a unique constraint violation occurs, show an error message
                if (ex.Number == 2627 || ex.Number == 2601)  // Unique constraint violation error numbers
                {
                    lblErrorMessage.Text = "A Budget was already created for this Term! Please start preparing the Budget items as expected";
                }
                else
                {
                    lblErrorMessage.Text = "Error adding budget. Please try again. " + ex.Message;
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void CheckBudgetStatusForNewRecord(int schoolId, int status)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();

                // If the new status is active (2), deactivate other active budgets for the same school
                if (status == 2)
                {
                    string updateQuery = @"UPDATE Budget SET Status = 1 WHERE SchoolId = @SchoolId AND Status = 2";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, Con))
                    {
                        updateCmd.Parameters.AddWithValue("@SchoolId", schoolId);
                        updateCmd.ExecuteNonQuery();
                    }

                    lblMessage.Text = "A budget with status '2' already exists. The previous budget has been deactivated.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showWarningModal", "$('#warningModal').modal('show');", true);
                }
            }
        }

        private void UpdateRecord(int BudgetId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"UPDATE Budget 
                             SET BudgetName = @BudgetName, 
                                 TermId = @TermId, 
                                 Contingency=@Contingency,
                                 Description= @Description ,Status=@Status
                             WHERE BudgetId = @BudgetId";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@BudgetName", txtBudgetName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                        cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                        cmd.Parameters.AddWithValue("@BudgetId", BudgetId);
                        cmd.Parameters.AddWithValue("@Contingency", txtContingency.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }

                    CheckBudgetStatus(BudgetId);
                }

                lblMessage.Text = "Record updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating book. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void CheckBudgetStatus(int budgetId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();

                // Get the school ID and new status of the updated budget
                string getBudgetQuery = @"SELECT SchoolId, Status FROM Budget WHERE BudgetId = @BudgetId";
                using (SqlCommand getBudgetCmd = new SqlCommand(getBudgetQuery, Con))
                {
                    getBudgetCmd.Parameters.AddWithValue("@BudgetId", budgetId);
                    using (SqlDataReader reader = getBudgetCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int schoolId = reader["SchoolId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SchoolId"]);
                            int status = Convert.ToInt32(reader["Status"]);

                            reader.Close();

                            // If the new status is active (2), deactivate other active budgets for the same school
                            if (status == 2)
                            {
                                string updateQuery = @"UPDATE Budget SET Status = 1 WHERE SchoolId = @SchoolId AND BudgetId != @BudgetId AND Status = 2";
                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, Con))
                                {
                                    updateCmd.Parameters.AddWithValue("@SchoolId", schoolId);
                                    updateCmd.Parameters.AddWithValue("@BudgetId", budgetId);
                                    updateCmd.ExecuteNonQuery();
                                }

                                lblMessage.Text = "A budget with status '2' already exists. The previous budget has been deactivated.";
                                ScriptManager.RegisterStartupScript(this, GetType(), "showWarningModal", "$('#warningModal').modal('show');", true);
                            }
                        }
                    }
                }
            }
        }
        private void DeleteBook(int BudgetId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Budget WHERE BudgetId = @BudgetId", Con))
                {
                    cmd.Parameters.AddWithValue("@BudgetId", BudgetId);
                    cmd.ExecuteNonQuery();
                }
            }
            Response.Redirect("Budget.aspx?deleteSuccess=true");
        }

        private void ClearControls()
        {
            txtBudgetName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            ddlTerm.SelectedIndex = 0;
        }

    }
}