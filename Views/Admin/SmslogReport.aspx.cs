using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class SmslogReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                BindRecordRepeater();
            }
        }

        private List<SmsLogSummary> GetSmsLogData()
        {
            List<SmsLogSummary> smsLogSummaries = new List<SmsLogSummary>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"SELECT 
                    sentmonth,
                    TotalRecordsPerMonth as SmsSentPerMonth,
                    'MK' + FORMAT(TotalTariffPerMonth, 'N0') as TotalCostPerMonth,
                    schoolName
                FROM 
                    Vw_smslog 
                WHERE 
                    SchoolId = @SchoolId
                GROUP BY 
                    sentmonth,
                    TotalRecordsPerMonth,
                    TotalTariffPerMonth,
                    schoolName
                ORDER BY 
                    CAST(RIGHT(sentmonth, 4) AS INT) DESC, 
                    CASE 
                        WHEN sentmonth LIKE 'January%' THEN 1
                        WHEN sentmonth LIKE 'February%' THEN 2
                        WHEN sentmonth LIKE 'March%' THEN 3
                        WHEN sentmonth LIKE 'April%' THEN 4
                        WHEN sentmonth LIKE 'May%' THEN 5
                        WHEN sentmonth LIKE 'June%' THEN 6
                        WHEN sentmonth LIKE 'July%' THEN 7
                        WHEN sentmonth LIKE 'August%' THEN 8
                        WHEN sentmonth LIKE 'September%' THEN 9
                        WHEN sentmonth LIKE 'October%' THEN 10
                        WHEN sentmonth LIKE 'November%' THEN 11
                        WHEN sentmonth LIKE 'December%' THEN 12
                    END DESC";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    smsLogSummaries.Add(new SmsLogSummary
                    {
                        SentMonth = dr["sentmonth"].ToString(),
                        SmsSentPerMonth = Convert.ToInt32(dr["SmsSentPerMonth"]),
                        TotalCostPerMonth = dr["TotalCostPerMonth"].ToString(),
                        SchoolName = dr["schoolName"].ToString()
                    });
                }
                dr.Close();
            }
            return smsLogSummaries;
        }

        public class SmsLogSummary
        {
            public string SentMonth { get; set; }
            public int SmsSentPerMonth { get; set; }
            public string TotalCostPerMonth { get; set; }
            public string SchoolName { get; set; }
        }

        private void BindRecordRepeater()
        {
            List<SmsLogSummary> smsLogSummaries = GetSmsLogData();
            RecordRepeater.DataSource = smsLogSummaries;
            RecordRepeater.DataBind();
        }
    }
}