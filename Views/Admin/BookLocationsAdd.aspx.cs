using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SMSWEBAPP.Views.Admin.BookLocationsAdd;

namespace SMSWEBAPP.Views.Admin
{
    public partial class BookLocationsAdd : System.Web.UI.Page
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
                if (Request.QueryString["LocationID"] != null)
                {
                    int LocationID = int.Parse(Request.QueryString["LocationID"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteRecord(LocationID);
                    }
                    else
                    {
                        LoadRecordData(LocationID);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["LocationD"] != null)
            {
                btnSubmit.Text = "Update";
            }
            else
            {
                btnSubmit.Text = "Add";
            }
        }


        private void LoadRecordData(int ExamID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Location WHERE LocationID = @LocationID", Con);
                cmd.Parameters.AddWithValue("@LocationID", ExamID);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    txtLocationName.Text = dr["Location"].ToString();
                    txtDescription.Text = dr["DEscription"].ToString();
                }
                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["LocationID"] != null)
            {
                int ExamID = int.Parse(Request.QueryString["LocationID"]);
                UpdateRecord(ExamID);
            }
            else
            {
                AddNewRecord();
            }
            ClearControls();
        }

        private void AddNewRecord()
        {
            if (Session["Permissions"] != null && Session["RoleId"] != null)
            {
                List<string> userPermissions = (List<string>)Session["Permissions"];
                int roleId = Convert.ToInt32(Session["RoleId"]);


                if (userPermissions.Contains("Shelf_Create"))
                {
                    try
                    {
                        using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                        {
                            Con.Open();
                            string query = "INSERT INTO Location (Location,Description, CreatedBy,SchoolId) " +
                                           "VALUES (@Location,@Description, @CreatedBy,@SchoolId)";
                            SqlCommand cmd = new SqlCommand(query, Con);
                            cmd.Parameters.AddWithValue("@Location", txtLocationName.Text);
                            cmd.Parameters.AddWithValue("@Description", txtDescription.Text);
                            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                            cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                            cmd.ExecuteNonQuery();
                        }
                        lblMessage.Text = "Book Shelf added successfully!";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                    }
                    catch (SqlException ex)
                    {
                        lblErrorMessage.Text = "Error adding exam. Please try again." + ex.Message;
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                }
                else
                {
                    lblErrorMessage.Text = "ACCESS DENIED! YOU DO NOT HAVE PERMISSION TO PERFORM THIS ACTION ";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }
        }
        private void UpdateRecord(int LocationID)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = "UPDATE Location SET Location=@Location,Description=@Description WHERE LocationID=@LocationID";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@Location", txtLocationName.Text);
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text);
                    cmd.Parameters.AddWithValue("@LocationID", LocationID);
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

        private void DeleteRecord(int LocationID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Location WHERE LocationID = @LocationID", Con);
                cmd.Parameters.AddWithValue("@LocationID", LocationID);
                cmd.ExecuteNonQuery();
                Response.Redirect("BookLocations.aspx?deleteSuccess=true");
            }
        }

        private void ClearControls()
        {
            txtLocationName.Text = string.Empty;
            txtDescription.Text = string.Empty;
        }
    }
}
