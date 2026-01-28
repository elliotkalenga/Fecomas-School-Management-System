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
    public partial class InvoicesPrint : System.Web.UI.Page
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
    ClassName,
    StreamName,
    SchoolId,
    Term,

    /* ===============================
       1. THIS TERM TOTAL (FIXED)
       =============================== */
    FORMAT(
        SUM(
            CAST(TotalFees AS DECIMAL(18,2))
            - CAST(PreviousTermBalance AS DECIMAL(18,2))
        ),
        'N0'
    ) AS ThisTermTotal,

    /* ===============================
       2. PREVIOUS TERM BALANCE
       =============================== */
    FORMAT(
        MAX(CAST(PreviousTermBalance AS DECIMAL(18,2))),
        'N0'
    ) AS PreviousTermBalance,

    /* ===============================
       3. GRAND TOTAL PAYABLE
       =============================== */
    FORMAT(
        SUM(
            CAST(TotalFees AS DECIMAL(18,2))
            - CAST(PreviousTermBalance AS DECIMAL(18,2))
        )
        + MAX(CAST(PreviousTermBalance AS DECIMAL(18,2))),
        'N0'
    ) AS TotalFees,

    /* ===============================
       TOTAL COLLECTED
       =============================== */
    FORMAT(
        SUM(CAST(TotalCollected AS DECIMAL(18,2))),
        'N0'
    ) AS TotalCollected,

    /* ===============================
       FINAL BALANCE
       =============================== */
    FORMAT(
        (
            SUM(
                CAST(TotalFees AS DECIMAL(18,2))
                - CAST(PreviousTermBalance AS DECIMAL(18,2))
            )
            + MAX(CAST(PreviousTermBalance AS DECIMAL(18,2)))
        )
        - SUM(CAST(TotalCollected AS DECIMAL(18,2))),
        'N0'
    ) AS Balance,

    /* ===============================
       PAYMENT STATUS
       =============================== */
    CASE 
        WHEN 
            (
                (
                    SUM(
                        CAST(TotalFees AS DECIMAL(18,2))
                        - CAST(PreviousTermBalance AS DECIMAL(18,2))
                    )
                    + MAX(CAST(PreviousTermBalance AS DECIMAL(18,2)))
                )
                - SUM(CAST(TotalCollected AS DECIMAL(18,2)))
            ) = 0 THEN 'Fully Paid'
        WHEN SUM(CAST(TotalCollected AS DECIMAL(18,2))) = 0 THEN 'Not Paid'
        ELSE 'Partly Paid'
    END AS PaidStatus

FROM PrintInvoice
WHERE SchoolId = @SchoolId
  AND Status = 2

GROUP BY 
    StudentNo,
    Student,
    ClassName,
    StreamName,
    Term,
    SchoolId

ORDER BY
    (
        (
            SUM(
                CAST(TotalFees AS DECIMAL(18,2))
                - CAST(PreviousTermBalance AS DECIMAL(18,2))
            )
            + MAX(CAST(PreviousTermBalance AS DECIMAL(18,2)))
        )
        - SUM(CAST(TotalCollected AS DECIMAL(18,2)))
    ) DESC;
";

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
                        Schoolid = Convert.ToInt32(dr["Schoolid"]),
                        StudentNo = dr["StudentNo"].ToString(),
                        Student = dr["Student"].ToString(),
                        ClassName = dr["ClassName"].ToString(),
                        StreamName = dr["StreamName"].ToString(),
                        Balance = dr["Balance"].ToString(),
                        ThisTermTotal = dr["ThisTermTotal"].ToString(),
                        PreviousTermBalance = dr["PreviousTermBalance"].ToString(),
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
            public string StreamName { get; set; }
            public string StudentNo { get; set; }
            public string Student { get; set; }
            public string TotalCollected { get; set; }
            public string TotalFees { get; set; }
            public string ThisTermTotal { get; set; }
            public string PreviousTermBalance { get; set; }
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

