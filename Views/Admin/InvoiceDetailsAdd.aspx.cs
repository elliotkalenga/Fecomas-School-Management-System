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
	public partial class InvoiceDetailsAdd : System.Web.UI.Page
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

                    if (Request.QueryString["InvoiceId"] != null)
                    {
                        int InvoiceId;
                        if (int.TryParse(Request.QueryString["InvoiceId"], out InvoiceId))
                        {
                            string mode = Request.QueryString["mode"];
                            if (mode == "delete")
                            {
                                DeleteRecord(InvoiceId);
                            }
                            else
                            {
                                LoadRecordData(InvoiceId);
                            }
                        }
                    }
                }
            }

            protected void SetButtonText()
            {
                btnSubmit.Text = Request.QueryString["InvoiceId"] != null ? "Update" : "Add";
            }

            private void PopulateDropDownLists()
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string TermQry = @"select T.TermId, TN.TermNumber+'('+y.FinancialYear+')'  as Term from term T
                                    inner Join TermNumber TN on T.Term=TN.Termid
                                    inner Join FinancialYear y on T.YearId=y.FinancialYearId where T.Status=2 and T.SchoolId=@SchoolId";

                    string SchoolQry = @"SELECT 0 AS SchoolId, '-- Select School --' AS SchoolName
                                      UNION  
                                      SELECT SchoolId, SchoolName FROM School ";

                    Con.Open();
                    PopulateDropDownList(Con, TermQry, ddlTerm, "Term", "TermId");
                    PopulateDropDownList(Con, SchoolQry, ddlSchool, "SchoolName", "SchoolId");
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

            private void LoadRecordData(int InvoiceId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Invoice WHERE InvoiceId = @InvoiceId", Con))
                    {
                        cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows && dr.Read())
                            {
                                txtDescription.Text = dr["InvoiceDescription"].ToString();
                                ddlSchool.SelectedValue = dr["CustomerId"].ToString();
                                ddlTerm.SelectedValue = dr["TermId"].ToString();
                            }
                        }
                    }
                }
            }

            protected void btnSubmit_Click(object sender, EventArgs e)
            {
                if (Request.QueryString["InvoiceId"] != null)
                {
                    int InvoiceId;
                    if (int.TryParse(Request.QueryString["InvoiceId"], out InvoiceId))
                    {
                        UpdateRecord(InvoiceId);
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
                    if (ddlSchool.SelectedValue == "0")
                    {
                        lblErrorMessage.Text = "Please select School.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                        return;
                    }


                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        string query = @"INSERT INTO Invoice(CustomerId,Status,InvoiceDescription, TermId, CreatedBy, SchoolId) 
                                     VALUES (@CustomerId,@Status,@InvoiceDescription, @TermId, @CreatedBy, @SchoolId)";
                        using (SqlCommand cmd = new SqlCommand(query, Con))
                        {
                            cmd.Parameters.AddWithValue("@Status", "Pending");
                            cmd.Parameters.AddWithValue("@InvoiceDescription", txtDescription.Text.Trim());
                            cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                            cmd.Parameters.AddWithValue("@CustomerId", ddlSchool.SelectedValue);
                            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"] ?? DBNull.Value);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    lblMessage.Text = "Invoice  Created successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                }
                catch (SqlException ex)
                {
                    lblErrorMessage.Text = "Error adding Record. Please try again. " + ex.Message;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }

            private void UpdateRecord(int InvoiceId)
            {
                try
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();

                        // Step 1: Check if the Requisition is Approved
                        string checkStatusQuery = "SELECT Status FROM  Invoice WHERE InvoiceId = @InvoiceId";
                        using (SqlCommand checkCmd = new SqlCommand(checkStatusQuery, Con))
                        {
                            checkCmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
                            string status = checkCmd.ExecuteScalar()?.ToString();

                            if (status == "Paid")
                            {
                                lblErrorMessage.Text = "Update failed! Invoice is already Paid.";
                                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                                return; // Exit the method to prevent update
                            }
                        }

                        // Step 2: Proceed with the update if not Approved
                        string updateQuery = @"UPDATE Invoice 
                                   SET InvoiceDescription = @InvoiceDescription
                                       
                                   WHERE InvoiceId = @InvoiceId";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, Con))
                        {
                        cmd.Parameters.AddWithValue("@InvoiceDescription", txtDescription.Text.Trim());

                            cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    lblMessage.Text = "Invoice Transaction updated successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                }
                catch (SqlException ex)
                {
                    lblErrorMessage.Text = "Error updating record. Please try again. " + ex.Message;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }

            private void DeleteRecord(int InvoiceId)
            {
                try
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Invoice WHERE InvoiceId = @InvoiceId", Con))
                        {
                            cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Response.Redirect("Invoice.aspx?deleteSuccess=true");
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547) // Foreign key constraint violation error
                    {
                        lblErrorMessage.Text = "Error: This Invoice cannot be deleted because it is linked to other records.";
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
                txtDescription.Text = string.Empty;
                ddlSchool.SelectedIndex = 0;
            }
        }
    }
