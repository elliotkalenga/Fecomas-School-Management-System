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
    public partial class RequisitionItemAdd : System.Web.UI.Page
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
                            BindRecordsRepeater(); // Moved here
                        }
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            btnSubmit.Text = Request.QueryString["RequisitionId"] != null ? "Submit" : "Add";
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
                            txtRequisitionId.Text = dr["RequisitionId"].ToString();
                        }                    }
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
                    string updateQuery = @"Insert Into RequisitionItem
                                               (RequisitionItemName,Amount,RequisitionId,CreatedBy,SchoolId)
                                            Values (@RequisitionItemName,@Amount,@RequisitionId,@CreatedBy,@SchoolId)";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, Con))
                    {
                        cmd.Parameters.AddWithValue("@RequisitionItemName", txtItem.Text.Trim());
                        cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                        cmd.Parameters.AddWithValue("@RequisitionId", txtRequisitionId.Text);
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"] ?? DBNull.Value);


                        cmd.ExecuteNonQuery();
                    }
                }
                BindRecordsRepeater(); // Moved here

                lblMessage.Text = "Requisition Item Added successfully!";
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
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Requisition WHERE RequisitionId = @RequisitionId", Con))
                    {
                        cmd.Parameters.AddWithValue("@RequisitionId", RequisitionId);
                        cmd.ExecuteNonQuery();
                    }
                }
                Response.Redirect("Income.aspx?deleteSuccess=true");
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


        private List<RequisitionModel> GetRecordsList()
        {
            List<RequisitionModel> requisitionList = new List<RequisitionModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"
                SELECT RequisitionItemId, RequisitionItemName, Format(Amount,'N0') As Amount, Notes 
 FROM RequisitionItem 
                WHERE RequisitionId = @RequisitionId;
            ";

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Ensure txtRequisitionId.Text is not null or empty
                        string requisitionId = string.IsNullOrWhiteSpace(txtRequisitionId.Text) ? DBNull.Value.ToString() : txtRequisitionId.Text;

                        cmd.Parameters.AddWithValue("@RequisitionId", requisitionId);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                requisitionList.Add(new RequisitionModel
                                {
                                    RequisitionItemId = dr["RequisitionItemId"].ToString(),
                                    RequisitionItemName = dr["RequisitionItemName"].ToString(), // Fixed casing
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

            return requisitionList;
        }

        private void BindRecordsRepeater()
        {
            List<RequisitionModel> requisitions = GetRecordsList();
            RecordsRepeater.DataSource = requisitions;
            RecordsRepeater.DataBind();
        }

    }
}
