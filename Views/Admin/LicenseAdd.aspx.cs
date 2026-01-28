using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class LicenseAdd : System.Web.UI.Page
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

                if (Request.QueryString["LicenseId"] != null)
                {
                    int LicenseId = int.Parse(Request.QueryString["LicenseId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteRecord(LicenseId);
                    }
                    else
                    {
                        LoadRecordData(LicenseId);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["LicenseId"] != null)
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
                string ScaleTypeQry = @"select 0 as SchoolId, '--- Select School ---' as SchoolName UNION 
                                        Select SchoolId, SchoolName from School";



                Con.Open();
                PopulateDropDownList(Con, ScaleTypeQry, ddlSchool, "SchoolName", "SchoolId");
            }
        }

        private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            ddl.DataSource = dr;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            dr.Close();
        }

        private void LoadRecordData(int LicenseId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM License WHERE LicenseId = @LicenseId", Con);
                cmd.Parameters.AddWithValue("@LicenseId", LicenseId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    ddlSchool.SelectedValue = dr["SchoolId"].ToString();
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
            if (Request.QueryString["LicenseId"] != null)
            {
                int LicenseId = int.Parse(Request.QueryString["LicenseId"]);
                UpdateRecord(LicenseId);
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
                    lblErrorMessage.Text = "Please select a School.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"INSERT INTO License
                                (LicenseKey, SchoolId, LicenseType, CreatedBy)
                             VALUES
                                (@LicenseKey, @SchoolId, 2, @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@SchoolId", ddlSchool.SelectedValue);
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                        cmd.Parameters.AddWithValue("@LicenseKey", "LIC/" + ddlSchool.SelectedValue + "/" + DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "License added successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                return; // Ensure no further execution

            }
            catch (SqlException ex)
            {
                // Handle unique constraint violation (duplicate entry)
                if (ex.Number == 2627)
                {
                    lblErrorMessage.Text = "A class with the same name already exists.";
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

        private void UpdateRecord(int LicenseId)
        {
            DateTime today = DateTime.Today;

            DateTime currentDate = DateTime.Today; // get the current date
            int daysToAdd = ValidDays(LicenseId); // number of days to add

            DateTime newDate = currentDate.AddDays(daysToAdd);

            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"UPDATE License
                                    SET 
                                   StartDate=@StartDate,
                                    EndDate=@EndDate,
                                    VerifiedStatus=@VerifiedStatus,
                                    AppliedBy=@VerifiedBy
                                    WHERE LicenseId=@LicenseId";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@StartDate", DateTime.Today);
                    cmd.Parameters.AddWithValue("@EndDate", newDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@VerifiedStatus", "Verified");
                    cmd.Parameters.AddWithValue("@VerifiedBy", Session["Username"]);
                    cmd.Parameters.AddWithValue("@LicenseId", LicenseId);
                    cmd.ExecuteNonQuery();
                    ClearControls();
                    SetButtonText();
                }
                lblMessage.Text = "License updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating Record. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private int ValidDays(int LicenseId)
        {
            string query = @"SELECT 
                              	LT.ValidDays+ DATEDIFF(day, CONVERT(DATE, GETDATE()), CONVERT(DATE, L.EndDate)) AS RemainingDays
                            
                            FROM 
                                License AS L
                            INNER JOIN 
                                School AS S ON L.SchoolId = S.SchoolID
                            INNER JOIN 
                                LicenseType AS LT ON L.LicenseType = LT.LicenseTypeId 
                             where LicenseId=@LicenseId";
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(query, Con))
                {
                    cmd.Parameters.AddWithValue("@LicenseId", LicenseId);
                    Con.Open();
                    int days = (int)cmd.ExecuteScalar();
                    Con.Close();
                    return days;
                }
            }
        }

        private void DeleteRecord(int LicenseId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM License WHERE LicenseId = @LicenseId", Con);
                cmd.Parameters.AddWithValue("@LicenseId", LicenseId);
                cmd.ExecuteNonQuery();
                Response.Redirect("Licenses.aspx?deleteSuccess=true");
            }
        }

        private void ClearControls()
        {
            ddlSchool.SelectedIndex = 0;
        }
    }
}
