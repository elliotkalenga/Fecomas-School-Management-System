using Microsoft.Data.SqlClient;
using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class BookCategoryAdd : System.Web.UI.Page
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

                if (Request.QueryString["BookCategoryId"] != null)
                {
                    int BookCategoryId;
                    if (int.TryParse(Request.QueryString["BookCategoryId"], out BookCategoryId))
                    {
                        string mode = Request.QueryString["mode"];
                        if (mode == "delete")
                        {
                            DeleteBook(BookCategoryId);
                        }
                        else
                        {
                            LoadRecordData(BookCategoryId);
                        }
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            btnSubmit.Text = Request.QueryString["BookCategoryId"] != null ? "Update" : "Add";
        }



        private void LoadRecordData(int BookCategoryId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM BookCategory WHERE BookCategoryId = @BookCategoryId", Con))
                {
                    cmd.Parameters.AddWithValue("@BookCategoryId", BookCategoryId);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows && dr.Read())
                        {
                            txtAssetCategory.Text = dr["Category"].ToString();
                        }
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["BookCategoryId"] != null)
            {
                int BookCategoryId;
                if (int.TryParse(Request.QueryString["BookCategoryId"], out BookCategoryId))
                {
                    UpdateBook(BookCategoryId);
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
                    string query = @"insert into BookCategory (Category,SchoolId) 
                                     VALUES (@Category,@SchoolId)";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@Category", txtAssetCategory.Text.Trim());
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Book Category added successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error adding  Category. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void UpdateBook(int BookCategoryId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"UPDATE BookCategory 
                                     SET Category = @Category 
                                        
                                     WHERE BookCategoryId = @BookCategoryId";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@Category ", txtAssetCategory.Text.Trim());
                        cmd.Parameters.AddWithValue("@BookCategoryId", BookCategoryId);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Record updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating  Category. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteBook(int BookCategoryId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM BookCategory WHERE BookCategoryId = @BookCategoryId", Con))
                {
                    cmd.Parameters.AddWithValue("@BookCategoryId", BookCategoryId);
                    cmd.ExecuteNonQuery();
                }
            }
            Response.Redirect("BookCategory.aspx?deleteSuccess=true");
        }

        private void ClearControls()
        {
            txtAssetCategory.Text = string.Empty;
        }
    }
}
