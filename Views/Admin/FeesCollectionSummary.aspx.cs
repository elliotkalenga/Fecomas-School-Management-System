using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class FeesCollectionSummary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                // Redirect to login page
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                BindCollectionsRepeater();
                // Load the student data if needed

            }

        }


        private List<FeesCollections> GetStudentsList()
        {
            List<FeesCollections> feesCollections = new List<FeesCollections>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"SELECT 
   schoolid,
   SchoolName,
   Logo,
   Address,
   SchoolCode,
   Term,
   'MK' + FORMAT(SUM(CAST(TotalFees AS DECIMAL(18, 2))), 'N0') AS TotalFees,
   'MK' + FORMAT(SUM(CAST(TotalCollected AS DECIMAL(18, 2))), 'N0') AS TotalCollected,
   'MK' + FORMAT(SUM(CAST(TotalFees AS DECIMAL(18, 2))) - SUM(CAST(TotalCollected AS DECIMAL(18, 2))), 'N0') AS Balance,
   CASE 
        WHEN (SUM(CAST(TotalFees AS DECIMAL(18, 2))) - SUM(CAST(TotalCollected AS DECIMAL(18, 2)))) = 0 THEN 'Fully Paid'
        WHEN SUM(CAST(TotalCollected AS DECIMAL(18, 2))) = 0 THEN 'Not Paid'
        ELSE 'Partly Paid'
   END AS PaidStatus,
   CASE 
        WHEN SUM(CAST(TotalFees AS DECIMAL(18, 2))) = 0 THEN '0%'
        ELSE FORMAT((SUM(CAST(TotalCollected AS DECIMAL(18, 2))) / SUM(CAST(TotalFees AS DECIMAL(18, 2)))) * 100, 'N2') + '%'
   END AS CollectionPercentage
FROM 
   PrintInvoice Where SchoolId=@SchoolId
GROUP BY 
   Term, schoolid, SchoolName, Logo, Address, SchoolCode";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    //DateTime collectedDate;
                    //DateTime.TryParse(dr["CreatedDate"].ToString(), out collectedDate);

                    feesCollections.Add(new FeesCollections
                    {
                        SchoolId = Convert.ToInt32(dr["Schoolid"]),
                        Term = dr["Term"].ToString(),
                        SchoolCode = dr["SchoolCode"].ToString(),
                        TotalFees = dr["TotalFees"].ToString(),
                        TotalCollected = dr["TotalCollected"].ToString(),
                        Balance = dr["Balance"].ToString(),
                        PaidStatus = dr["PaidStatus"].ToString(),
                        CollectionPercentage = dr["CollectionPercentage"].ToString(),
                        //CollectedDate = collectedDate
                    });
                }
                dr.Close();
            }
            return feesCollections;
        }

        public class FeesCollections
        {
            public string SchoolCode { get; set; }
            public string TotalFees { get; set; }
            public string TotalCollected { get; set; }
            public string Balance{ get; set; }
            public string PaidStatus { get; set; }
            public string Term { get; set; }
            public string CollectionPercentage { get; set; }
            public int SchoolId { get; set; }
            public DateTime CollectedDate { get; set; }
            public string DateCollectedString => CollectedDate.ToString("yyyy-MM-dd");
        }

        private void BindCollectionsRepeater()
        {
            List<FeesCollections> feesCollections = GetStudentsList();
            CollectionsRepeater.DataSource = feesCollections;
            CollectionsRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindCollectionsRepeater();
        }

    }
}

