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
    public partial class BookCategory : System.Web.UI.Page
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

        private List<Bookategories> GetRecordsList()
        {
            List<Bookategories> Bookategoriess = new List<Bookategories>();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"
                SELECT * FROM BookCategory 
WHERE 
    SchoolId = @SchoolId";

                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Bookategoriess.Add(new Bookategories
                            {
                                BookCategoryId = dr["BookCategoryId"].ToString(),
                                Category = dr["Category"].ToString(),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // Logger.LogError(ex);
                // Return an empty list or null
                return new List<Bookategories>();
            }
            return Bookategoriess;
        }
        private void BindRecordsRepeater()
        {
            List<Bookategories> Bookategoriess = GetRecordsList();
            RecordsRepeater.DataSource = Bookategoriess;
            RecordsRepeater.DataBind();
        }
    }

    // Moved the class outside to maintain better structure
    public class Bookategories
    {
        public string BookCategoryId { get; set; }
        public string  Category { get; set; }
    }
}
