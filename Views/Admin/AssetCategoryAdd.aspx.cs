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
    public partial class AssetCategoryAdd : System.Web.UI.Page
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

                    if (Request.QueryString["AssetCategoryId"] != null)
                    {
                        int AssetCategoryId;
                        if (int.TryParse(Request.QueryString["AssetCategoryId"], out AssetCategoryId))
                        {
                            string mode = Request.QueryString["mode"];
                            if (mode == "delete")
                            {
                                DeleteBook(AssetCategoryId);
                            }
                            else
                            {
                                LoadRecordData(AssetCategoryId);
                            }
                        }
                    }
                }
            }

            protected void SetButtonText()
            {
                btnSubmit.Text = Request.QueryString["AssetCategoryId"] != null ? "Update" : "Add";
            }



            private void LoadRecordData(int AssetCategoryId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM AssetCategory WHERE AssetCategoryId = @AssetCategoryId", Con))
                    {
                        cmd.Parameters.AddWithValue("@AssetCategoryId", AssetCategoryId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows && dr.Read())
                            {
                                txtAssetCategory.Text = dr["AssetCategory"].ToString();
                            }
                        }
                    }
                }
            }

            protected void btnSubmit_Click(object sender, EventArgs e)
            {
                if (Request.QueryString["AssetCategoryId"] != null)
                {
                    int AssetCategoryId;
                    if (int.TryParse(Request.QueryString["AssetCategoryId"], out AssetCategoryId))
                    {
                        UpdateBook(AssetCategoryId);
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
                        string query = @"insert into AssetCategory (AssetCategory,SchoolId,CreatedBy) 
                                     VALUES (@AssetCategory,@SchoolId,@CreatedBy)";
                        using (SqlCommand cmd = new SqlCommand(query, Con))
                        {
                            cmd.Parameters.AddWithValue("@AssetCategory", txtAssetCategory.Text.Trim());
                            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"] ?? DBNull.Value);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    lblMessage.Text = "Asset Category added successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                }
                catch (SqlException ex)
                {
                    lblErrorMessage.Text = "Error adding Asset Category. Please try again. " + ex.Message;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }

            private void UpdateBook(int AssetCategoryId)
            {
                try
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        string query = @"UPDATE AssetCategory 
                                     SET AssetCategory = @AssetCategory 
                                        
                                     WHERE AssetCategoryId = @AssetCategoryId";
                        using (SqlCommand cmd = new SqlCommand(query, Con))
                        {
                            cmd.Parameters.AddWithValue("@AssetCategory ", txtAssetCategory.Text.Trim());
                            cmd.Parameters.AddWithValue("@AssetCategoryId", AssetCategoryId);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    lblMessage.Text = "Record updated successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                }
                catch (SqlException ex)
                {
                    lblErrorMessage.Text = "Error updating Asset Category. Please try again. " + ex.Message;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }

            private void DeleteBook(int AssetCategoryId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM AssetCategory WHERE AssetCategoryId = @AssetCategoryId", Con))
                    {
                        cmd.Parameters.AddWithValue("@AssetCategoryId", AssetCategoryId);
                        cmd.ExecuteNonQuery();
                    }
                }
                Response.Redirect("AssetCategory.aspx?deleteSuccess=true");
            }

            private void ClearControls()
            {
                txtAssetCategory.Text = string.Empty;
            }
        }
    }
