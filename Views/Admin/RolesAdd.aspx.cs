using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SMSWEBAPP.Views.Admin.Exams;

namespace SMSWEBAPP.Views.Admin
{
    public partial class RolesAdd : System.Web.UI.Page
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

                if (Request.QueryString["RoleId"] != null)
                {
                    int RoleId = int.Parse(Request.QueryString["RoleId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteRecord(RoleId);
                    }
                    else
                    {
                        LoadRecordData(RoleId);
                    }
                }
            }
        }


        protected void SetButtonText()
        {
            if (Request.QueryString["RoleId"] != null)
            {
                btnSubmit.Text = "Update";
            }
            else
            {
                btnSubmit.Text = "Add";
            }
        }
        private void DeleteRecord(int RoleId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Roles WHERE RoleId = @RoleId", Con);
                cmd.Parameters.AddWithValue("@RoleId", RoleId);
                cmd.ExecuteNonQuery();
                Response.Redirect("Roles.aspx?deleteSuccess=true");
            }
        }

        private void LoadRecordData(int RoleId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Roles WHERE RoleId = @RoleId", Con);
                cmd.Parameters.AddWithValue("@RoleId", RoleId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    txtRoleName.Text = dr["RoleTitle"].ToString();
                    txtRoleDescription.Text = dr["Description"].ToString();

                }
                dr.Close();
            }
        }

        //private void BindRoleDropdown()
        //{
        //    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
        //    {
        //        string query = @"Select 0 as RoleId, '--- Select Role ---' as RoleTitle Union  Select RoleId,RoleTitle from Roles Where School=@SchoolId";
        //        SqlCommand cmd = new SqlCommand(query, Con);
        //        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

        //        Con.Open();
        //        SqlDataReader dr = cmd.ExecuteReader();
        //        ddlRole.DataSource = dr;
        //        ddlRole.DataTextField = "RoleTitle";
        //        ddlRole.DataValueField = "RoleId";
        //        ddlRole.DataBind();
        //        dr.Close();
        //    }
        //}

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["RoleId"] != null)
            {
                int RoleId = int.Parse(Request.QueryString["RoleId"]);
                UpdateScore(RoleId);
            }
            else
            {
                AddNewRecord();
            }
            ClearControls();


        }

        private void ClearControls()
        {
            txtRoleDescription.Text = "";
            txtRoleName.Text = "";
        }

        private void AddNewRecord()
        {
            //// Validate dropdown selections
            //if (ddlRole.SelectedValue == "0")
            //{
            //    lblErrorMessage.Text = "Please select a valid Role.";
            //    lblErrorMessage.Visible = true;
            //    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            //    return; // Stop execution here to prevent clearing fields
            //}

            //if (ddlStatus.SelectedValue == "0")
            //{
            //    lblErrorMessage.Text = "Please select a valid Status.";
            //    lblErrorMessage.Visible = true;
            //    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            //    return; // Stop execution here to prevent clearing fields
            //}


            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                try
                {
                    string query = @"INSERT INTO Roles
                                            (RoleTitle, Description, Createdby, School)
                                         VALUES
                                            (@RoleTitle, @Description, @Createdby, @School)";

                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@RoleTitle", txtRoleName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description", txtRoleDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@School", Session["SchoolId"]);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                    Con.Open();
                    cmd.ExecuteNonQuery();

                    lblMessage.Text = "Record saved successfully!";
                    lblMessage.Visible = true;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);

                    // Clear fields only after successful submission
                    ClearControls();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627) // Unique constraint error
                    {
                        lblErrorMessage.Text = "Duplicate entry detected. UserName already exists.";
                    }
                    else
                    {
                        lblErrorMessage.Text = "An error occurred while saving the record. Please try again later. " + ex.Message;
                    }
                    lblErrorMessage.Visible = true;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }
        }

        private void UpdateScore(int RoleId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"UPDATE Roles SET 
                                    RoleTitle=@RoleTitle,
                                    Description=@Description
                                   WHERE RoleId=@RoleId";

                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@RoleTitle", txtRoleName.Text);
                cmd.Parameters.AddWithValue("@Description", txtRoleDescription.Text);
                cmd.Parameters.AddWithValue("@RoleId", RoleId);

                Con.Open();
                cmd.ExecuteNonQuery();
                ClearControls();
                SetButtonText();
                lblMessage.Text = "Record Updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);

            }
        }


    }
}
