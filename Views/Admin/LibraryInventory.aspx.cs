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
    public partial class LibraryInventory : System.Web.UI.Page
    {
            protected void Page_Load(object sender, EventArgs e)
            {// Check if the user is logged in
                if (Session["User"] == null)
                {
                    // Redirect to login page
                    Response.Redirect("UserLogin.aspx");
                }

                if (!IsPostBack)
                {

                    BindRecordsRepeater();
                    // Load the student data if needed
                }
            }

            private List<Inventory> GetRecordList()
            {
                List<Inventory> inventory = new List<Inventory>();
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string ShowData = @"select InventoryId,Barcode,B.BookTitle,B.Author,B.Publisher,C.Category,B.ISBN,L.Location,S.SubjectName,I.BookStatus from LibraryInventory I inner Join Books B on I.BookId=B.BookId
                                        Inner Join Location L on I.LocationId=L.LocationId 
                                           inner join Subject S on B.SUbjectId=S.SubjectId
                                            Inner Join BookCategory C on B.CategoryId=C.BookcategoryId
                                            Where I.SchoolId=@SchoolId order by I.BookStatus Desc ";

                    Con.Open();
                    SqlCommand cmd = new SqlCommand(ShowData, Con);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        //DateTime CreatedDate;
                        //DateTime.TryParse(dr["CreatedDate"].ToString(), out CreatedDate);  // Use StartDate from the reader

                        inventory.Add(new Inventory
                        {
                            InventoryId = dr["InventoryId"].ToString(),
                            BookTitle = dr["BookTitle"].ToString(),
                            BookNo = dr["Barcode"].ToString(),
                            Author = dr["Author"].ToString(),
                            Publisher = dr["Publisher"].ToString(),
                            ISBN = dr["ISBN"].ToString(),
                            Location = dr["Location"].ToString(),
                            Category = dr["Category"].ToString(),
                            SubjctName = dr["SubjectName"].ToString(),
                            BookStatus = dr["BookStatus"].ToString(),
                            //CreatedDate = CreatedDate,
                        });
                    }
                    dr.Close();
                }
                return inventory;  // Return the list of exams
            }

            public class Inventory
            {
                public string InventoryId { get; set; }
            public string BookTitle { get; set; }
            public string BookNo { get; set; }
            public string Author { get; set; }
                public string Publisher { get; set; }
                public string ISBN{ get; set; }
                public string Location { get; set; }
            public string Category { get; set; }
            public string BookStatus { get; set; }
            public string SubjctName { get; set; }
            //public DateTime CreatedDate { get; set; }  // Renamed to start with an uppercase letter
            //public string CreatedDateString => CreatedDate.ToString("dd-MMMM yyyy");  // Use a string property for formatted date

        }

        private void BindRecordsRepeater()
            {
                List<Inventory> inventory = GetRecordList();
                RecordRepeater.DataSource = inventory;
                RecordRepeater.DataBind();
            }

            protected void Page_PreRender(object sender, EventArgs e)
            {
                BindRecordsRepeater();
            }


        }

    }
