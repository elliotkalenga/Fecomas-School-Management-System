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
    public partial class HostelsAdd : System.Web.UI.Page
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

                if (Request.QueryString["HostelId"] != null)
                {
                    int HostelId;
                    if (int.TryParse(Request.QueryString["HostelId"], out HostelId))
                    {
                        string mode = Request.QueryString["mode"];
                        if (mode == "delete")
                        {
                            DeleteBook(HostelId);
                        }
                        else
                        {
                            LoadRecordData(HostelId);
                        }
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            btnSubmit.Text = Request.QueryString["HostelId"] != null ? "Update" : "Add";
        }



        private void LoadRecordData(int HostelId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Hostels WHERE HostelId = @HostelId", Con))
                {
                    cmd.Parameters.AddWithValue("@HostelId", HostelId);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows && dr.Read())
                        {
                            txtHostelName.Text = dr["HostelName"].ToString();
                            txtHostelDescription.Text = dr["HostelDescription"].ToString();
                        }
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["HostelId"] != null)
            {
                int HostelId;
                if (int.TryParse(Request.QueryString["HostelId"], out HostelId))
                {
                    UpdateBook(HostelId);
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
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"insert into Hostels (HostelName,HostelDescription,SchoolId,CreatedBy) 
                                     VALUES (@HostelName,@HostelDescription,@SchoolId,@CreatedBy)";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@HostelName", txtHostelName.Text.Trim());
                        cmd.Parameters.AddWithValue("@HostelDescription", txtHostelDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"] ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Hostel added successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error adding Hostel. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void UpdateBook(int HostelId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"UPDATE Hostels 
                                     SET HostelName = @HostelName, 
                                         HostelDescription= @HostelDescription 
                                        
                                     WHERE HostelId = @HostelId";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@HostelName", txtHostelName.Text.Trim());
                        cmd.Parameters.AddWithValue("@HostelDescription", txtHostelDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@HostelId", HostelId);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Record updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating Hostel. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteBook(int HostelId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Hostels WHERE HostelId = @HostelId", Con))
                {
                    cmd.Parameters.AddWithValue("@HostelId", HostelId);
                    cmd.ExecuteNonQuery();
                }
            }
            Response.Redirect("Hostels.aspx?deleteSuccess=true");
        }

        private void ClearControls()
        {
            txtHostelName.Text = string.Empty;
            txtHostelDescription.Text = string.Empty;
        }
    }
}
