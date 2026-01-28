using SMSWEBAPP.DAL;
using System;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SMSWEBAPP.Views.Admin.Exams;

namespace SMSWEBAPP.Views.Admin
{
    public partial class TermsAdd : System.Web.UI.Page
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
                if (Request.QueryString["TermId"] != null)
                {
                    int TermId = int.Parse(Request.QueryString["TermId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteRecord(TermId);
                    }
                    else
                    {
                        LoadRecordData(TermId);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["TermId"] != null)
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
                string YearTypeQry = @"Select 0 as FinancialYearId, '------- Select Academic Year ------' As FinancialYear Union Select FinancialYearId,FinancialYear from FinancialYear where SchoolId=@SchoolId";
                string TermQry = @" Select 0 as TermId, '--- Select Term ----' as TermNumber Union Select TermId,TermNumber from TermNumber Where SchoolId=@SchoolId";
                string TermStatusQry = @"select 0 StatusId, '-- Select Term Status --' as Status union  select StatusId,Status from Status";

                Con.Open();
                PopulateDropDownList(Con, YearTypeQry, ddlYear, "FinancialYear", "FinancialYearId");
                PopulateDropDownList(Con, TermQry, ddlTerm, "TermNumber", "TermId");
                PopulateDropDownList(Con, TermStatusQry, ddlStatus, "Status", "StatusId");
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

        private void LoadRecordData(int TermId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Term WHERE TermId = @TermId", Con);
                cmd.Parameters.AddWithValue("@TermId", TermId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    ddlStatus.Text = dr["Status"].ToString();
                    ddlTerm.Text = dr["Term"].ToString();
                    ddlYear.SelectedValue = dr["Yearid"].ToString();
                    if (dr["StartDate"] != DBNull.Value)
                    {
                        txtStartDate.Text = Convert.ToDateTime(dr["StartDate"]).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        txtStartDate.Text = ""; // Or handle it as you see fit
                    }

                    if (dr["StartDate"] != DBNull.Value)
                    {
                        txtEndDate.Text = Convert.ToDateTime(dr["EndDate"]).ToString("yyyy-MM-dd");
                    }
                    else
                    {txtEndDate.Text = ""; // Or handle it as you see fit
                    }



                }
                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["TermId"] != null)
            {
                int TermId = int.Parse(Request.QueryString["TermId"]);
                UpdateRecord(TermId);
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


                if (ddlTerm.SelectedValue == "0")
                {
                    lblErrorMessage.Text = "Please select Term.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlYear.SelectedValue == "0")
                {
                    lblErrorMessage.Text = "Please select Academic Year.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlStatus.SelectedValue == "0")
                {
                    lblErrorMessage.Text = "Please select a Status";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"INSERT INTO Term
           (
               Term,
               StartDate,
               EndDate,
               Status,
               Yearid,
               CreatedBy,
               SchoolId
           )
           VALUES
           (
               @Term,
               @StartDate,
               @EndDate,
               @Status,
               @Yearid,
               @CreatedBy,
               @SchoolId
           )";

                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@Term", ddlTerm.SelectedValue);
                    cmd.Parameters.AddWithValue("@StartDate", DateTime.Parse(txtStartDate.Text));
                    cmd.Parameters.AddWithValue("@EndDate", DateTime.Parse(txtEndDate.Text));
                    cmd.Parameters.AddWithValue("@Yearid", ddlYear.SelectedValue);
                    cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                    cmd.ExecuteNonQuery();
                }

                lblMessage.Text = "Term added successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Check for unique constraint violation (duplicate)
                {
                    lblErrorMessage.Text = "A record with the same Term, Start Date, and End Date already exists.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
                else
                {
                    lblErrorMessage.Text = "Error adding exam. Please try again." + ex.Message;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }
        }

        private void UpdateRecord(int TermId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = "UPDATE Term SET Status=@Status WHERE TermId=@TermId";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@TermId", TermId);
                    cmd.ExecuteNonQuery();
                    ClearControls();
                    SetButtonText();
                }
                lblMessage.Text = "Term updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating exam. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteRecord(int TermId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Term WHERE TermId = @TermId", Con);
                cmd.Parameters.AddWithValue("@TermId", TermId);
                cmd.ExecuteNonQuery();
                Response.Redirect("Terms.aspx?deleteSuccess=true");
            }
        }

        private void ClearControls()
        {
            ddlStatus.SelectedIndex = 0;
            ddlYear.SelectedIndex = 0;
            ddlTerm.SelectedIndex = 0;
        }
    }
}
