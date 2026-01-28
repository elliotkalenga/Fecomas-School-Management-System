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
    public partial class FeesStructureAdd : System.Web.UI.Page
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
                if (Request.QueryString["FeesId"] != null)
                {
                    int FeesId = int.Parse(Request.QueryString["FeesId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteExam(FeesId);
                    }
                    else
                    {
                        LoadRecordData(FeesId);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["FeesId"] != null)
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
                string StatusQry = @"select 0 StatusId, '-- Select Status --' as Status union  select StatusId,Status from Status";

                Con.Open();
                PopulateDropDownList(Con, StatusQry, ddlStatus, "Status", "StatusId");
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

        private void LoadRecordData(int FeesId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM FeesConfiguration WHERE FeesId = @FeesId", Con);
                cmd.Parameters.AddWithValue("@FeesId", FeesId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    txtFeesName.Text = dr["FeesName"].ToString();
                    txtDescription.Text = dr["Description"].ToString();
                    ddlStatus.SelectedValue = dr["Status"].ToString();
                }
                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["FeesId"] != null)
            {
                int FeesId = int.Parse(Request.QueryString["FeesId"]);
                UpdateExam(FeesId);
            }
            else
            {
                AddNewExam();
            }
            ClearControls();
        }

        private void AddNewExam()
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = "INSERT INTO FeesConfiguration  (FeesName, Description, Amount, Status, CreatedBy,SchoolId) " +
                                   "VALUES (@FeesName, @Description, @Amount, @Status, @CreatedBy,@SchoolId)";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@FeesName", txtFeesName.Text);
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text);
                    cmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Trim());
                    cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                    cmd.ExecuteNonQuery();
                }
                lblMessage.Text = "Fees Structure added successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error adding fees Structure. Please try again." + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void UpdateExam(int FeesId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = "UPDATE FeesConfiguration SET Status=@Status,FeesName=@FeesName,Description=@Description WHERE FeesId=@FeesId";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@FeesName", txtFeesName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description",txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@FeesId", FeesId);
                    cmd.ExecuteNonQuery();
                    ClearControls();
                    SetButtonText();
                }
                lblMessage.Text = "Record updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating exam. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteExam(int FeesId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM FeesConfiguration WHERE FeesId = @FeesId", Con);
                cmd.Parameters.AddWithValue("@FeesId", FeesId);
                cmd.ExecuteNonQuery();
                Response.Redirect("FeesStructure.aspx?deleteSuccess=true");
            }
        }

        private void ClearControls()
        {
            txtFeesName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtAmount.Text = string.Empty;
            ddlStatus.SelectedIndex = 0;
        }
    }
}
