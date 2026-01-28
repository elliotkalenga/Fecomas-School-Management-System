using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class Books : System.Web.UI.Page
    {
        private const string USER_SESSION_KEY = "User";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[USER_SESSION_KEY] == null)
            {
                // Redirect to login page
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                BindRecordsRepeater();
            }
        }

        private List<Book> GetRecordsList()
        {
            List<Book> books = new List<Book>();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"SELECT B.BookId, B.BookTitle, B.Author, B.Publisher, B.ISBN, 
                                            C.Category, S.SubjectName 
                                     FROM Books B
                                     INNER JOIN BookCategory C ON B.CategoryId = C.BookCategoryId
                                     INNER JOIN Subject S ON B.SubjectId = S.SubjectId
                                     WHERE B.SchoolId = @SchoolId";

                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            books.Add(new Book
                            {
                                BookId = dr["BookId"].ToString(),
                                BookTitle = dr["BookTitle"].ToString(),
                                SubjectName = dr["SubjectName"].ToString(),
                                Author = dr["Author"].ToString(),
                                Publisher = dr["Publisher"].ToString(),
                                ISBN = dr["ISBN"].ToString(),
                                Category = dr["Category"].ToString(),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Properly log the exception instead of using Response.Write
                // Example: Logger.LogError(ex);
                Response.Write("<script>alert('An error occurred while fetching data. Please try again later.');</script>");
            }
            return books;
        }

        private void BindRecordsRepeater()
        {
            List<Book> books = GetRecordsList();
            RecordsRepeater.DataSource = books;
            RecordsRepeater.DataBind();
        }
    }

    // Moved the class outside to maintain better structure
    public class Book
    {
        public string BookId { get; set; }
        public string BookTitle { get; set; }
        public string SubjectName { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Category { get; set; }
        public string ISBN { get; set; }
    }
}
