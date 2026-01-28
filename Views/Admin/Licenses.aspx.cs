using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SMSWEBAPP.Views.Admin.Schools;

namespace SMSWEBAPP.Views.Admin
{
    public partial class Licenses : System.Web.UI.Page
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

        private List<SchoolRecord> GetRecordList()
        {
            List<SchoolRecord> records = new List<SchoolRecord>();
            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string showData = @"
            Select LicenseId,LicenseKey,SchoolName,LicenseStatus,LicensedDays,UsedDays,RemainingDays, EndDate as ExpiryDate 
            from DisplyLicensedata";  // Added WHERE clause for parameter usage

                con.Open();
                SqlCommand cmd = new SqlCommand(showData, con);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        DateTime ExpirydDate;
                        DateTime.TryParse(dr["ExpiryDate"].ToString(), out ExpirydDate);  // Use StartDate from the reader


                        records.Add(new SchoolRecord
                        {
                            LicenseId = dr["LicenseId"].ToString(),
                            LicenseKey = dr["LicenseKey"].ToString(),
                            SchoolName = dr["SchoolName"].ToString(),
                            LicenseStatus = dr["LicenseStatus"].ToString(),  // Corrected column name
                            LicensedDays = dr["LicensedDays"].ToString(),
                            UsedDays = dr["UsedDays"].ToString(),
                            RemainingDays = dr["RemainingDays"].ToString(),
                            ExpiryDate = ExpirydDate,
                        });
                    }
                }
            }
            return records;
        }

        public class SchoolRecord
        {
            public string LicenseId { get; set; }
            public string SchoolName { get; set; }
            public string LicenseStatus{ get; set; }
            public string LicenseKey { get; set; }
            public string LicensedDays { get; set; }
            public string UsedDays { get; set; }
            public string RemainingDays { get; set; }
            public DateTime ExpiryDate { get; set; }  // Renamed to start with an uppercase letter
            public string ExpiryDateString => ExpiryDate.ToString("dd-MMMM yyyy");
        }

        private void BindRecordsRepeater()
        {
            List<SchoolRecord> schoolRecords = GetRecordList();
            RecordRepeater.DataSource = schoolRecords;
            RecordRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindRecordsRepeater();
        }


    }

}
