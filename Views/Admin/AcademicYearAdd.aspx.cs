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
    public partial class AcademicYearAdd : System.Web.UI.Page
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

                if (Request.QueryString["FinancialYearId"] != null)
                {
                    int FinancialYearId = int.Parse(Request.QueryString["FinancialYearId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteRecord(FinancialYearId);
                    }
                    else
                    {
                        LoadRecordData(FinancialYearId);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["FinancialYearId"] != null)
            {
                btnSubmit.Text = "Update";
            }
            else
            {
                btnSubmit.Text = "Add";
            }
        }


        private void LoadRecordData(int FinancialYearId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM FinancialYear WHERE FinancialYearId = @FinancialYearId", Con);
                cmd.Parameters.AddWithValue("@FinancialYearId", FinancialYearId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    txtAcademicYear.Text = dr["FinancialYear"].ToString();
                    //if (dr["StartDate"] != DBNull.Value)
                    //{
                    //    txtStartDate.Text = Convert.ToDateTime(dr["StartDate"]).ToString("yyyy-MM-dd");
                    //}
                    //else
                    //{
                    //    txtStartDate.Text = ""; // Or handle it as you see fit
                    //}




                }
                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["FinancialYearId"] != null)
            {
                int FinancialYearId = int.Parse(Request.QueryString["FinancialYearId"]);
                UpdateRecord(FinancialYearId);
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
                    string query = @"INSERT INTO FinancialYear
                                (FinancialYear, SchoolId, CreatedBy)
                             VALUES
                                (@FinancialYear, @SchoolId, @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@FinancialYear", txtAcademicYear.Text.Trim());
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Academic Year added successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                return; // Ensure no further execution

            }
            catch (SqlException ex)
            {
                // Handle unique constraint violation (duplicate entry)
                if (ex.Number == 2627)
                {
                    lblErrorMessage.Text = "A Academic Year with the same name already exists.";
                }
                // Handle error caused by the CHECK constraint violation
                else if (ex.Message.Contains("CHECK constraint"))
                {
                    lblErrorMessage.Text = "Invalid grading system selected. Please choose either 'Average Score' or 'Aggregate Points'.";
                }
                else
                {
                    lblErrorMessage.Text = "Error adding class. Please try again. " + ex.Message;
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
            catch (Exception ex) // Catch non-SQL exceptions
            {
                lblErrorMessage.Text = "An unexpected error occurred: " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void UpdateRecord(int FinancialYearId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"UPDATE FinancialYear SET 
                                    FinancialYear=@FinancialYear
                                    WHERE FinancialYearId=@FinancialYearId";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@FinancialYear", txtAcademicYear.Text.Trim());
                    cmd.Parameters.AddWithValue("@FinancialYearId", FinancialYearId);
                    cmd.ExecuteNonQuery();
                    ClearControls();
                    SetButtonText();
                }
                lblMessage.Text = "Class updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating Record. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteRecord(int FinancialYearId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM FinancialYear WHERE FinancialYearId = @FinancialYearId", Con);
                cmd.Parameters.AddWithValue("@FinancialYearId", FinancialYearId);
                cmd.ExecuteNonQuery();
                Response.Redirect("AcademicYear.aspx?deleteSuccess=true");
            }
        }

        private void ClearControls()
        {
            txtAcademicYear.Text = "";
        }
    }
}
