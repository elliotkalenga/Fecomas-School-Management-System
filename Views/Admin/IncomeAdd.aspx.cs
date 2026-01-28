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
    public partial class IncomeAdd : System.Web.UI.Page
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

                    if (Request.QueryString["IncomeId"] != null)
                    {
                        int IncomeId;
                        if (int.TryParse(Request.QueryString["IncomeId"], out IncomeId))
                        {
                            string mode = Request.QueryString["mode"];
                            if (mode == "delete")
                            {
                                DeleteBook(IncomeId);
                            }
                            else
                            {
                                LoadRecordData(IncomeId);
                            }
                        }
                    }
                }
            }

            protected void SetButtonText()
            {
                btnSubmit.Text = Request.QueryString["IncomeId"] != null ? "Update" : "Add";
            }

            private void PopulateDropDownLists()
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                string TermQry = @"select T.TermId, TN.TermNumber+'('+y.FinancialYear+')'  as Term from term T
                                    inner Join TermNumber TN on T.Term=TN.Termid
                                    inner Join FinancialYear y on T.YearId=y.FinancialYearId where T.Status=2 and T.SchoolId=@SchoolId";

                string SourceQry = @"SELECT 0 AS SourceId, '-- Select Source of Income --' AS Source
                                      UNION  
                                      SELECT SourceId, Source FROM Source";

                    Con.Open();
                    PopulateDropDownList(Con, TermQry, ddlTerm, "Term", "TermId");
                    PopulateDropDownList(Con, SourceQry, ddlSource, "Source", "SourceId");
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

            private void LoadRecordData(int IncomeId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Income WHERE IncomeId = @IncomeId", Con))
                    {
                        cmd.Parameters.AddWithValue("@IncomeId", IncomeId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows && dr.Read())
                            {
                                txtDescription.Text = dr["Description"].ToString();
                                txtAmount.Text = dr["Amount"].ToString();
                                txtPurpose.Text = dr["Purpose"].ToString();
                                ddlSource.SelectedValue = dr["Source"].ToString();
                                ddlTerm.SelectedValue = dr["TermId"].ToString();
                            }
                        }
                    }
                }
            }

            protected void btnSubmit_Click(object sender, EventArgs e)
            {
                if (Request.QueryString["IncomeId"] != null)
                {
                    int IncomeId;
                    if (int.TryParse(Request.QueryString["IncomeId"], out IncomeId))
                    {
                        UpdateBook(IncomeId);
                    }
                }
                else
                {
                    AddNewBook();
                }

                ClearControls();
            }

            private void AddNewBook()
            {
                try

                {
                if (ddlSource.SelectedValue == "0")
                {
                    lblErrorMessage.Text = "Please select Source of Income.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        string query = @"INSERT INTO Income (Purpose, Description, TermId, Amount, Source, CreatedBy, SchoolId) 
                                     VALUES (@Purpose, @Description, @TermId, @Amount, @Source, @CreatedBy, @SchoolId)";
                        using (SqlCommand cmd = new SqlCommand(query, Con))
                        {
                            cmd.Parameters.AddWithValue("@Purpose", txtPurpose.Text.Trim());
                            cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Trim());
                            cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                            cmd.Parameters.AddWithValue("@Source", ddlSource.SelectedValue);
                            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"] ?? DBNull.Value);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    lblMessage.Text = "Income Transaction created successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                }
                catch (SqlException ex)
                {
                    lblErrorMessage.Text = "Error adding Record. Please try again. " + ex.Message;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }

            private void UpdateBook(int IncomeId)
            {
                try
                {
              

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        string query = @"UPDATE Income 
                                     SET Purpose = @Purpose, 
                                         Description = @Description, 
                                         Amount = @Amount, 
                                         Source = @Source
                                     WHERE IncomeId = @IncomeId";
                        using (SqlCommand cmd = new SqlCommand(query, Con))
                        {
                        cmd.Parameters.AddWithValue("@Purpose", txtPurpose.Text.Trim());
                        cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Trim());
                        cmd.Parameters.AddWithValue("@Source", ddlSource.SelectedValue);
                        cmd.Parameters.AddWithValue("@IncomeId", IncomeId);

                            cmd.ExecuteNonQuery();
                        }
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

            private void DeleteBook(int IncomeId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Income WHERE IncomeId = @IncomeId", Con))
                    {
                        cmd.Parameters.AddWithValue("@IncomeId", IncomeId);
                        cmd.ExecuteNonQuery();
                    }
                }
                Response.Redirect("Income.aspx?deleteSuccess=true");
            }

            private void ClearControls()
            {
                txtAmount.Text = string.Empty;
                txtDescription.Text = string.Empty;
                txtPurpose.Text = string.Empty;
                ddlSource.SelectedIndex = 0;
                ddlTerm.SelectedIndex = 0;
            }
        }
    }
