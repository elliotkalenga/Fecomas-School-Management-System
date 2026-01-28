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
    public partial class Rooms : System.Web.UI.Page
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

        private List<Room> GetRecordsList()
        {
            List<Room> rooms = new List<Room>();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"
                                                SELECT 
    r.RoomID,
    r.RoomNumber,
    r.Capacity,
    COALESCE(SUM(CASE WHEN T.Status = 2 THEN 1 ELSE 0 END), 0) AS Allocated,
    r.Capacity - COALESCE(SUM(CASE WHEN T.Status = 2 THEN 1 ELSE 0 END), 0) AS AvailableSpace,
    COALESCE(Tn.TermNumber + ' (' + F.FinancialYear + ')', 'No Term') AS Term,
    h.HostelName,
    r.RoomDescription,
    CASE 
        WHEN SUM(CASE WHEN T.Status = 2 THEN 1 ELSE 0 END) IS NULL OR SUM(CASE WHEN T.Status = 2 THEN 1 ELSE 0 END) = 0 
        THEN 0 
        ELSE COALESCE(SUM(CASE WHEN T.Status = 2 THEN 1 ELSE 0 END), 0) 
    END AS AllocatedStudents
FROM 
    Rooms r
LEFT JOIN 
    RoomAllocations ra ON r.RoomID = ra.RoomID
LEFT JOIN Hostels h ON r.HostelID = h.HostelId
LEFT JOIN 
    Term T ON ra.TermID = T.TermId
LEFT JOIN 
    TermNumber Tn ON T.Term = Tn.TermId
LEFT JOIN 
    FinancialYear F ON t.YearId = F.FinancialYearId
where r.schoolId=@SchoolId
GROUP BY 
    r.RoomID, r.RoomNumber, r.Capacity, Tn.TermNumber, F.FinancialYear, h.HostelName, r.RoomDescription

order by h.hostelName,r.RoomNumber



";

                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            rooms.Add(new Room
                            {
                                RoomId = dr["RoomId"].ToString(),
                                RoomNumber = dr["RoomNumber"].ToString(),
                                HostelName = dr["HostelName"].ToString(),
                                Capacity = dr["Capacity"].ToString(),
                                Allocated = dr["Allocated"].ToString(),
                                AvailableSpace= dr["AvailableSpace"].ToString(),
                                Term = dr["Term"].ToString(),
                                RoomDescription = dr["RoomDescription"].ToString(),
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
                return new List<Room>();
            }
            return rooms;
        }
        private void BindRecordsRepeater()
        {
            List<Room> rooms = GetRecordsList();
            RecordsRepeater.DataSource = rooms;
            RecordsRepeater.DataBind();
        }
    }

    // Moved the class outside to maintain better structure
    public class Room
    {
        public string RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string HostelName { get; set; }
        public string Capacity { get; set; }
        public string Allocated { get; set; }
        public string RoomDescription { get; set; }
        public string AvailableSpace{ get; set; }
        public string Term { get; set; }
    }
}
