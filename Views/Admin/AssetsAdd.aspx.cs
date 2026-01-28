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
    public partial class AssetsAdd : System.Web.UI.Page
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
                if (Request.QueryString["AssetId"] != null)
                {
                    int AssetId = int.Parse(Request.QueryString["AssetId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteExam(AssetId);
                    }
                    else
                    {
                        LoadRecordData(AssetId);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["AssetId"] != null)
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
                string AssetCategoryQry = @"Select 0 as AssetCategoryId, '------- Select Asset Category ------' As AssetCategory Union Select AssetCategoryId,AssetCategory from AssetCategory where SchoolId=@SchoolId";

                Con.Open();
                PopulateDropDownList(Con, AssetCategoryQry, ddlAssetCategory, "AssetCategory", "AssetCategoryId");
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

        private void LoadRecordData(int AssetId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Asset WHERE AssetId = @AssetId", Con);
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    txtAssetCode.Text = dr["Barcode"].ToString();
                    txtAssetName.Text = dr["AssetName"].ToString();
                    txtAssetDescription.Text = dr["AssetDescription"].ToString();
                    txtPurchasedCost.Text = dr["PurchaseCost"].ToString();

                    // Check for DBNull before converting to string
                    if (!dr.IsDBNull(dr.GetOrdinal("PurchasedDate")))
                    {
                        txtPurchasedDate.Text = Convert.ToDateTime(dr["PurchasedDate"]).ToString("yyyy-MM-dd"); // format date as desired
                    }
                    else
                    {
                        txtPurchasedDate.Text = string.Empty; // or some default value
                    }

                    txtLifespan.Text = dr["LifeSpan"].ToString();
                    ddlAssetCategory.SelectedValue = dr["AssetCategoryId"].ToString();
                }
                dr.Close();
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["AssetId"] != null)
            {
                int AssetId = int.Parse(Request.QueryString["AssetId"]);
                UpdateExam(AssetId);
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
                        if (ddlAssetCategory.SelectedValue == "0")
                        {
                            lblErrorMessage.Text = "Please select Asset Category.";
                            return;
                        }



                        using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                        {
                            con.Open();
                            string query = "INSERT INTO Asset (Barcode, AssetName, AssetDescription, LifeSpan, PurchasedDate, PurchaseCost, CreatedBy, SchoolId,AssetCategoryId) " +
                                           "VALUES (@Barcode, @AssetName, @AssetDescription, @LifeSpan, @PurchasedDate, @PurchaseCost, @CreatedBy, @SchoolId,@AssetCategoryId)";
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.Parameters.AddWithValue("@Barcode", txtAssetCode.Text);
                                cmd.Parameters.AddWithValue("@AssetName", txtAssetName.Text);
                                cmd.Parameters.AddWithValue("@AssetDescription", txtAssetDescription.Text);
                                cmd.Parameters.AddWithValue("@AssetCategoryId", ddlAssetCategory.SelectedValue);
                                cmd.Parameters.AddWithValue("@LifeSpan", int.Parse(txtLifespan.Text));
                                cmd.Parameters.AddWithValue("@PurchasedDate", DateTime.Parse(txtPurchasedDate.Text));
                                cmd.Parameters.AddWithValue("@PurchaseCost", decimal.Parse(txtPurchasedCost.Text));
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
                                    lblMessage.Text = "Asset added successfully!";
                                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                                }
                                else
                                {
                                    lblErrorMessage.Text = "No rows were affected.";
                                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                                }
                            }
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
        private void UpdateExam(int AssetId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = "UPDATE Asset SET " +
                        "AssetName=@AssetName,Barcode=@Barcode,AssetDescription=@AssetDescription,AssetCategoryId=@AssetCategoryId," +
                        "PurchaseCost=@PurchaseCost,PurchasedDate=@PurchasedDate,LifeSpan=@Lifespan WHERE AssetId=@AssetId";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@Barcode", txtAssetCode.Text);
                    cmd.Parameters.AddWithValue("@AssetName", txtAssetName.Text);
                    cmd.Parameters.AddWithValue("@AssetDescription", txtAssetDescription.Text);
                    cmd.Parameters.AddWithValue("@AssetCategoryId", ddlAssetCategory.SelectedValue);
                    cmd.Parameters.AddWithValue("@LifeSpan", txtLifespan.Text.ToString());
                    cmd.Parameters.AddWithValue("@PurchasedDate", txtPurchasedDate.Text.ToString());
                    cmd.Parameters.AddWithValue("@PurchaseCost", txtPurchasedCost.Text.ToString());
                    cmd.Parameters.AddWithValue("@AssetId", AssetId);
                    cmd.ExecuteNonQuery();
                    ClearControls();
                    SetButtonText();
                }
                lblMessage.Text = "Asset updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating Asset. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteExam(int AssetId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Asset WHERE AssetId = @AssetId", Con);
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                cmd.ExecuteNonQuery();
                Response.Redirect("Assets.aspx?deleteSuccess=true");
            }
        }

        private void ClearControls()
        {
            txtAssetCode.Text = string.Empty;
            txtAssetName.Text = string.Empty;
            txtAssetDescription.Text = string.Empty;
            txtPurchasedCost.Text = string.Empty;
            txtPurchasedDate.Text = string.Empty;
            ddlAssetCategory.SelectedIndex = 0;
        }
    }
}
