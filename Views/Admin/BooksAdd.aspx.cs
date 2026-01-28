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
    public partial class BooksAdd : System.Web.UI.Page
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

                if (Request.QueryString["BookId"] != null)
                {
                    int BookId;
                    if (int.TryParse(Request.QueryString["BookId"], out BookId))
                    {
                        string mode = Request.QueryString["mode"];
                        if (mode == "delete")
                        {
                            DeleteBook(BookId);
                        }
                        else
                        {
                            LoadRecordData(BookId);
                        }
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            btnSubmit.Text = Request.QueryString["BookId"] != null ? "Update" : "Add";
        }

        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string BookCategoryQry = @"SELECT 0 AS BookCategoryId, '-- Select BookCategory --' AS Category 
                                           UNION  
                                           SELECT BookCategoryId, Category FROM BookCategory 
                                            Where SchoolId = @SchoolId";

                string SubjectQry = @"SELECT 0 AS SubjectId, '-- Select Subject --' AS SubjectName
                                      UNION  
                                      SELECT SubjectId, SubjectName FROM Subject WHERE SchoolId = @SchoolId";

                Con.Open();
                PopulateDropDownList(Con, BookCategoryQry, ddlBookCategory, "Category", "BookCategoryId");
                PopulateDropDownList(Con, SubjectQry, ddlSubject, "SubjectName", "SubjectId");
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

        private void LoadRecordData(int BookId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Books WHERE BookId = @BookId", Con))
                {
                    cmd.Parameters.AddWithValue("@BookId", BookId);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows && dr.Read())
                        {
                            txtBookTitle.Text = dr["BookTitle"].ToString();
                            txtISBN.Text = dr["ISBN"].ToString();
                            txtPublisher.Text = dr["Publisher"].ToString();
                            txtAuthor.Text = dr["Author"].ToString();
                            ddlBookCategory.SelectedValue = dr["CategoryId"].ToString();
                            ddlSubject.SelectedValue = dr["SubjectId"].ToString();
                        }
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["BookId"] != null)
            {
                int BookId;
                if (int.TryParse(Request.QueryString["BookId"], out BookId))
                {
                    UpdateBook(BookId);
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
                    string query = @"INSERT INTO Books (BookTitle, Author, Publisher, ISBN, CategoryId, SubjectId, CreatedBy, SchoolId) 
                                     VALUES (@BookTitle, @Author, @Publisher, @ISBN, @CategoryId, @SubjectId, @CreatedBy, @SchoolId)";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@BookTitle", txtBookTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@Author", txtAuthor.Text.Trim());
                        cmd.Parameters.AddWithValue("@Publisher", txtPublisher.Text.Trim());
                        cmd.Parameters.AddWithValue("@ISBN", txtISBN.Text.Trim());
                        cmd.Parameters.AddWithValue("@CategoryId", ddlBookCategory.SelectedValue);
                        cmd.Parameters.AddWithValue("@SubjectId", ddlSubject.SelectedValue);
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"] ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Book added successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error adding book. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void UpdateBook(int BookId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"UPDATE Books 
                                     SET BookTitle = @BookTitle, 
                                         Author = @Author, 
                                         Publisher = @Publisher, 
                                         ISBN = @ISBN, 
                                         CategoryId = @CategoryId, 
                                         SubjectId = @SubjectId 
                                     WHERE BookId = @BookId";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@BookTitle", txtBookTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@Author", txtAuthor.Text.Trim());
                        cmd.Parameters.AddWithValue("@Publisher", txtPublisher.Text.Trim());
                        cmd.Parameters.AddWithValue("@ISBN", txtISBN.Text.Trim());
                        cmd.Parameters.AddWithValue("@CategoryId", ddlBookCategory.SelectedValue);
                        cmd.Parameters.AddWithValue("@SubjectId", ddlSubject.SelectedValue);
                        cmd.Parameters.AddWithValue("@BookId", BookId);

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

        private void DeleteBook(int BookId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Books WHERE BookId = @BookId", Con))
                {
                    cmd.Parameters.AddWithValue("@BookId", BookId);
                    cmd.ExecuteNonQuery();
                }
            }
            Response.Redirect("Books.aspx?deleteSuccess=true");
        }

        private void ClearControls()
        {
            txtBookTitle.Text = string.Empty;
            txtAuthor.Text = string.Empty;
            txtISBN.Text = string.Empty;
            txtPublisher.Text = string.Empty;
            ddlBookCategory.SelectedIndex = 0;
            ddlSubject.SelectedIndex = 0;
        }
    }
}
