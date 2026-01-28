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
    public partial class Hostels : System.Web.UI.Page
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

        private List<Hostel> GetRecordsList()
        {
            List<Hostel> hostels = new List<Hostel>();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"
                SELECT 
    h.HostelId,
    h.HostelName,
    h.HostelDescription,
    h.CreatedBy,
    COALESCE(Tn.TermNumber + ' (' + F.FinancialYear + ')', 'No Term') AS Term,
    COALESCE(SUM(r.Capacity), 0) AS HostelCapacity,
    COALESCE(COUNT(r.StudentID), 0) AS Allocated
FROM 
    Hostels h
LEFT JOIN 
    (SELECT 
         r.HostelID, 
         r.RoomID, 
         ra.TermID, 
         r.Capacity, 
         ra.StudentID 
     FROM 
         Rooms r 
     LEFT JOIN 
         RoomAllocations ra ON r.RoomID = ra.RoomID) AS r ON h.HostelId = r.HostelID
LEFT JOIN 
    Term T ON r.TermID = t.TermId AND T.Status = 2
LEFT JOIN 
    TermNumber Tn ON T.Term = Tn.TermId
LEFT JOIN 
    FinancialYear F ON t.YearId = F.FinancialYearId
WHERE 
    h.SchoolId = @SchoolId
GROUP BY 
    h.HostelId, h.HostelName, h.HostelDescription, h.CreatedBy, Tn.TermNumber, F.FinancialYear, T.Term";

                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            hostels.Add(new Hostel
                            {
                                HostelId = dr["HostelId"].ToString(),
                                HostelName = dr["HostelName"].ToString(),
                                HostelDescription = dr["HostelDescription"].ToString(),
                                CreatedBy = dr["CreatedBy"].ToString(),
                                Capacity = dr["HostelCapacity"].ToString(),
                                Allocated = dr["Allocated"].ToString(),
                                Term = dr["Term"].ToString(),
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
                return new List<Hostel>();
            }
            return hostels;
        }
        private void BindRecordsRepeater()
        {
            List<Hostel> hostels = GetRecordsList();
            RecordsRepeater.DataSource = hostels;
            RecordsRepeater.DataBind();
        }
    }

    // Moved the class outside to maintain better structure
    public class Hostel
    {
        public string HostelId { get; set; }
        public string HostelName { get; set; }
        public string HostelDescription { get; set; }
        public string CreatedBy { get; set; }
        public string Capacity { get; set; }
        public string Allocated { get; set; }
        public string Term { get; set; }
    }
}
