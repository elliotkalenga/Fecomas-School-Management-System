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
    public partial class BorrowBook : System.Web.UI.Page
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
                    string ShowData = @"SELECT 
    BB.BorrowingId, 
    Li.Barcode, 
    B.BookTitle, 
    LM.Member,      
    BB.BorrowedDate,
    BB.NumberofDays,
    BB.DateToReturn,
    BB.BookStatus,
    BB.ActualReturnDate,
    BB.SchoolId,
    BB.CreatedBy
FROM  Books B Inner Join LibraryInventory LI ON LI.BookId = B.BookId 
Inner JOIN  BorrowBook BB ON BB.BookNo = Li.Barcode
Inner Join LibraryMember LM on BB.MemberId=LM.MemberId
                                            Where BB.SchoolId=@SchoolId order by BB.BookStatus";

                    Con.Open();
                    SqlCommand cmd = new SqlCommand(ShowData, Con);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        DateTime BorrowedDate;
                        DateTime.TryParse(dr["BorrowedDate"].ToString(), out BorrowedDate);


                    DateTime DateToReturn;
                    DateTime.TryParse(dr["DateToReturn"].ToString(), out DateToReturn);


                    DateTime ActualReturnDate;
                    DateTime.TryParse(dr["ActualReturnDate"].ToString(), out ActualReturnDate);



                    inventory.Add(new Inventory
                        {
                            BorrowingId = dr["BorrowingId"].ToString(),
                            BookTitle = dr["BookTitle"].ToString(),
                            Barcode = dr["Barcode"].ToString(),
                            Member= dr["Member"].ToString(),
                            NumberofDays = dr["NumberofDays"].ToString(),
                            CreatedBy = dr["CreatedBy"].ToString(),
                            BookStatus = dr["BookStatus"].ToString(),
                        ActualReturnDate = ActualReturnDate,
                        BorrowedDate = BorrowedDate,
                        DateReturn = DateToReturn,
                    });
                    }
                    dr.Close();
                }
                return inventory;  // Return the list of exams
            }

            public class Inventory
            {
                public string BorrowingId { get; set; }
                public string BookTitle { get; set; }
                public string Barcode { get; set; }
                public string BookStatus { get; set; }
            public string NumberofDays { get; set; }
            public string Member { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? ActualReturnDate { get; set; }  // Nullable DateTime

            public string ActualReturnDateString =>
                ActualReturnDate.HasValue
                    ? ActualReturnDate.Value.ToString("dd-MMMM yyyy")
                    : string.Empty;
            public DateTime DateReturn { get; set; }  // Renamed to start with an uppercase letter
            public string DateReturnString => DateReturn.ToString("dd-MMMM yyyy");  // Use a string property for formatted date

            public DateTime BorrowedDate { get; set; }  // Renamed to start with an uppercase letter
            public string BorrowedDateString => BorrowedDate.ToString("dd-MMMM yyyy");  // Use a string property for formatted date

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
