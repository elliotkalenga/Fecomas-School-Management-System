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
	public partial class StudentInvoices : System.Web.UI.Page
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
    StudentNo,
    Student,
    ClassName,schoolid,
    Term,
    'MK' + FORMAT(SUM(CAST(TotalFees AS DECIMAL(18, 2))), 'N0') AS TotalFees,
    'MK' + FORMAT(SUM(CAST(TotalCollected AS DECIMAL(18, 2))), 'N0') AS TotalCollected,
    'MK' + FORMAT(SUM(CAST(TotalFees AS DECIMAL(18, 2))) - SUM(CAST(TotalCollected AS DECIMAL(18, 2))), 'N0') AS Balance,
    CASE 
        WHEN (SUM(CAST(TotalFees AS DECIMAL(18, 2))) - SUM(CAST(TotalCollected AS DECIMAL(18, 2)))) = 0 THEN 'Fully Paid'
        WHEN SUM(CAST(TotalCollected AS DECIMAL(18, 2))) = 0 THEN 'Not Paid'
        ELSE 'Partly Paid'
    END AS PaidStatus
FROM 
    PrintInvoice where Schoolid=@SchoolId and StudentNo=@studentNo
GROUP BY 
    StudentNo, Student, ClassName, Term,schoolid

          ORDER BY Student";

                    Con.Open();
                    SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                cmd.Parameters.AddWithValue("@StudentNo", Session["StudentNo"]);

                SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        //DateTime collectedDate;
                        //DateTime.TryParse(dr["CreatedDate"].ToString(), out collectedDate);

                        feesCollections.Add(new FeesCollections
                        {
                            Schoolid = Convert.ToInt32(dr["Schoolid"]),
                            StudentNo = dr["StudentNo"].ToString(),
                            Student = dr["Student"].ToString(),
                            ClassName = dr["ClassName"].ToString(),
                            Balance = dr["Balance"].ToString(),
                            TotalFees = dr["TotalFees"].ToString(),
                            TotalCollected = dr["TotalCollected"].ToString(),
                            Term = dr["Term"].ToString(),
                            PaidStatus = dr["PaidStatus"].ToString(),
                            //CollectedDate = collectedDate
                        });
                    }
                    dr.Close();
                }
                return feesCollections;
            }

            public class FeesCollections
            {
                public int Schoolid { get; set; }
                public string FeesName { get; set; }
                public string ClassName { get; set; }
                public string StudentNo { get; set; }
                public string Student { get; set; }
                public string TotalCollected { get; set; }
                public string TotalFees { get; set; }
                public string Term { get; set; }
                public string InvoiceNo { get; set; }
                public string PaidStatus { get; set; }
                public string Balance { get; set; }
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

