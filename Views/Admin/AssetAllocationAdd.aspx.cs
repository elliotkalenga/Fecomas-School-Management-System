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
    public partial class AssetAllocationAdd : System.Web.UI.Page
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
                if (Request.QueryString["AllocationId"] != null)
                {
                    int AllocationId = int.Parse(Request.QueryString["AllocationId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        ReturnAsset(AllocationId);
                    }
                    else
                    {
                        LoadRecordData(AllocationId);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["AllocationId"] != null)
            {
                btnSubmit.Text = "Update";
            }
            else
            {
                btnSubmit.Text = "Add";
            }
        }
        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string AssetQry = @"Select 0 as AssetId, '------- Select Asset Asset ------' As Asset Union Select AssetId,Barcode+' '+AssetName+' ('+AllocatedStatus+')'  as Asset
                                    from Asset Where AllocatedStatus='Not Allocated' and SchoolId=@SchoolId";
                string UserQry = @"Select 0 as UserId, '---- Select Asset Asset Holder ----' As AssetHolder Union Select UserId,FirstName+' '+LastName as AssetHolder from Users Where School=@SchoolId";

                Con.Open();
                PopulateDropDownList(Con, AssetQry, ddlAsset, "Asset", "AssetId");
                PopulateDropDownList(Con, UserQry, ddlAssetHolder, "AssetHolder", "UserId");
            }
        }

        private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
            SqlDataReader dr = cmd.ExecuteReader();
            ddl.DataSource = dr;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            dr.Close();
        }

        private void LoadRecordData(int AllocationId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM AssetAllocation WHERE AllocationId = @AllocationId", Con);
                cmd.Parameters.AddWithValue("@AllocationId", AllocationId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                   txtAssetLocation.Text = dr["AssetLocation"].ToString();
                   ddlAsset.SelectedValue= dr["AssetId"].ToString();
                    ddlAssetHolder.SelectedValue = dr["UserId"].ToString();

                }
                dr.Close();
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["AllocationId"] != null)
            {
                int AllocationId = int.Parse(Request.QueryString["AllocationId"]);
                UpdateRecord(AllocationId);
            }
            else
            {
                AddNewRecord();
            }
            ClearControls();
        }

        private void AddNewRecord()
        {
            if (Session["Permissions"] != null && Session["RoleId"] != null && Session["SchoolId"] != null && Session["Username"] != null)
            {
                List<string> userPermissions = (List<string>)Session["Permissions"];
                int roleId = Convert.ToInt32(Session["RoleId"]);

                if (userPermissions.Contains("Manage_Assets"))
                {
                    try
                    {
                        // Check if the user has selected a value from each dropdown
                        if (ddlAsset.SelectedValue == "0")
                        {
                            ErrorMessage.Text = "Please select Asset.";
                            return;
                        }

                        if (ddlAssetHolder.SelectedValue == "0")
                        {
                            ErrorMessage.Text = "Please select Asset Holder";
                            return;
                        }


                        using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                        {
                            con.Open();
                            string query = "INSERT INTO AssetAllocation (AssetId, UserId, AssetLocation, CreatedBy, SchoolId) " +
                                           "VALUES (@AssetId,@UserId, @AssetLocation, @CreatedBy, @SchoolId)";
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.Parameters.AddWithValue("@AssetId", ddlAsset.SelectedValue);
                                cmd.Parameters.AddWithValue("@UserId", ddlAssetHolder.SelectedValue);
                                cmd.Parameters.AddWithValue("@AssetLocation", txtAssetLocation.Text);
                                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                                cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                                // Log the query and parameters
                                string logMessage = $"Executing query: {query} with parameters: ";
                                foreach (SqlParameter param in cmd.Parameters)
                                {
                                    logMessage += $"{param.ParameterName} = {param.Value}, ";
                                }
                                // Log the message

                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    lblMessage.Text = "AssetcAllocation added successfully!";
                                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                                }
                                else
                                {
                                    lblErrorMessage.Text = "No rows were affected.";
                                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                                }
                            }
                        }

                        using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                        {
                            Con.Open();
                            SqlCommand cmd = new SqlCommand(@"UPDATE Asset
                                SET AllocatedStatus = 'Allocated' 
                                FROM Asset Where AssetId=@AssetId
                              ", Con);
                            cmd.Parameters.AddWithValue("@AssetId", ddlAsset.SelectedValue);
                            cmd.ExecuteNonQuery();
                        }


                    }
                    catch (SqlException ex)
                    {
                        lblErrorMessage.Text = "Error adding Asset. Please try again. " + ex.Message;
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                    catch (Exception ex)
                    {
                        lblErrorMessage.Text = "An error occurred. " + ex.Message;
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                }
                else
                {
                    lblErrorMessage.Text = "Access denied! You do not have permission to perform this action.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }
            else
            {
                lblErrorMessage.Text = "Session expired or invalid. Please login again.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }
        private void UpdateRecord(int AllocationId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = "UPDATE Asset SET " +
                        "AssetId=@AssetId,UserId=@UserId,AssetLocation=@AssetLocation WHERE AllocationId=@AllocationId";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@AssetId", ddlAsset.SelectedValue);
                    cmd.Parameters.AddWithValue("@UserId", ddlAssetHolder.SelectedValue);
                    cmd.Parameters.AddWithValue("@AssetLocation", txtAssetLocation.Text);
                    cmd.Parameters.AddWithValue("@AllocationId", AllocationId);
                    cmd.ExecuteNonQuery();
                    ClearControls();
                    SetButtonText();
                }
                lblMessage.Text = "Asset ALlocation updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating Asset. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void ReturnAsset(int AllocationId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("Update AssetAllocation Set AllocatedStatus='Returned',ReturnedDate=Getdate() Where AllocationId = @AllocationId", Con);
                cmd.Parameters.AddWithValue("@AllocationId", AllocationId);
                cmd.ExecuteNonQuery();
            }


            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand(@"UPDATE a 
SET AllocatedStatus = 'Not Allocated' 
FROM Asset a 
INNER JOIN AssetAllocation aa ON a.assetID = aa.assetID 
WHERE aa.AllocationId = @AllocationId", Con);
                cmd.Parameters.AddWithValue("@AllocationId", AllocationId);
                cmd.ExecuteNonQuery();
            }

            Response.Redirect("AssetAllocation.aspx?deleteSuccess=true");

        }

        private void ClearControls()
        {
            txtAssetLocation.Text = string.Empty;
            ddlAsset.SelectedIndex = 0;
            ddlAssetHolder.SelectedIndex = 0;
        }
    }
}
