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
    public partial class Schools : System.Web.UI.Page
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
            SELECT SchoolId, SchoolCode, SchoolName, Address, L.LogoName, ST.SchoolType 
            FROM school S 
            INNER JOIN Logo L ON S.Logoid = L.Id
            INNER JOIN SchoolType ST ON S.SchoolType = ST.SchoolTypeId";  // Added WHERE clause for parameter usage

                con.Open();
                SqlCommand cmd = new SqlCommand(showData, con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        records.Add(new SchoolRecord
                        {
                            SchoolId = dr["SchoolId"].ToString(),
                            SchoolName = dr["SchoolName"].ToString(),
                            SchoolCode = dr["SchoolCode"].ToString(),
                            SchoolType = dr["SchoolType"].ToString(),  // Corrected column name
                            LogoName = dr["LogoName"].ToString(),
                            Address = dr["Address"].ToString(),
                        });
                    }
                }
            }
            return records;
        }

        public class SchoolRecord
        {
            public string SchoolId { get; set; }
            public string SchoolName { get; set; }
            public string Address { get; set; }
            public string SchoolType { get; set; }
            public string LogoName { get; set; }
            public string SchoolCode { get; set; }
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
