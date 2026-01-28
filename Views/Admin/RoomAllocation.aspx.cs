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
    public partial class RoomAllocation : System.Web.UI.Page
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

        private List<RoomAllocations> GetRecordsList()
        {
            List<RoomAllocations> rooms = new List<RoomAllocations>();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"
                                                SELECT 
    ra.AllocationId, ra.CreatedDate,
    r.RoomNumber,
    h.HostelName, 
    s.FirstName + ' ' + s.LastName AS Student,
    ra.Condition,
    Tn.TermNumber + ' (' + F.FinancialYear + ')' AS Term,
    ra.CreatedBy
FROM 
    RoomAllocations ra 
INNER JOIN 
    Rooms r ON ra.RoomId = r.RoomId 
INNER JOIN 
    Hostels h ON r.HostelId = h.HostelId 
INNER JOIN 
    Enrollment E ON ra.StudentId = E.StudentId 
INNER JOIN 
    Student s ON E.StudentId = S.StudentId 
INNER JOIN 
    Term T ON ra.TermID = T.TermId
INNER JOIN 
    TermNumber Tn ON T.Term = Tn.TermId
INNER JOIN 
    FinancialYear F ON t.YearId = F.FinancialYearId
WHERE 
    ra.SchoolId = @SchoolId
ORDER BY 
    r.RoomNumber, H.HostelName";

                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            rooms.Add(new RoomAllocations
                            {
                                AllocationId = dr["AllocationId"].ToString(),
                                RoomNumber = dr["RoomNumber"].ToString(),
                                HostelName = dr["HostelName"].ToString(),
                                Student = dr["Student"].ToString(),
                                Term = dr["Term"].ToString(),
                                Condition = dr["Condition"].ToString(),
                                CreatedBy = dr["CreatedBy"].ToString(),
                                CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
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
                return new List<RoomAllocations>();
            }
            return rooms;
        }
        private void BindRecordsRepeater()
        {
            List<RoomAllocations> rooms = GetRecordsList();
            RecordsRepeater.DataSource = rooms;
            RecordsRepeater.DataBind();
        }
    }

    // Moved the class outside to maintain better structure
    public class RoomAllocations
    {
        public string AllocationId { get; set; }
        public string RoomNumber { get; set; }
        public string HostelName { get; set; }
        public string Student { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Condition { get; set; }
        public string Term { get; set; }
    }
}
