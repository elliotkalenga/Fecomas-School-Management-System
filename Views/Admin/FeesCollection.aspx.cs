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
    public partial class FeesCollection : System.Web.UI.Page
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
                string ShowData = @"SELECT TOP 100 PERCENT
    E.EnrollmentID,

    /* Transaction fields */
    FC.FeesCollectionId,
    FC.ReferenceNo,
    FC.Amount AS TransactionAmount,
    CONVERT(VARCHAR(10), FC.CreatedDate, 105) AS TransactionDate,

    /* Student / Invoice */
    S.Phone,
    S.Email,
    I.SchoolId,
    I.PaidStatus,
    S.Guardian,
    I.InvoiceId,
    I.InvoiceNo,

    TN.TermNumber + ' (' + FY.FinancialYear + ')' AS Term,
    S.FirstName + ' ' + S.LastName AS Student,
    C.ClassName,
    CST.StreamName,
    S.StudentNO,

    /* Fees */
F.FeesName 
+ ' (' 
+ FORMAT(
      ISNULL(F.Amount, 0) + ISNULL(PB.PreviousTermBalance, 0),
      'N0'
  )
+ ')' AS FeesName,


    /* 🔹 Current fees */
    ISNULL(F.Amount, 0) AS CurrentTermFees,

    /* 🔹 Previous term balance (same logic as your view) */
    ISNULL(PB.PreviousTermBalance, 0) AS PreviousTermBalance,

    /* ✅ Total fees INCLUDING previous balance */
    ISNULL(F.Amount, 0) + ISNULL(PB.PreviousTermBalance, 0) AS TotalFees,

    /* Paid */
    FORMAT(ISNULL(FT.TotalPaid, 0), 'N0') AS AmountCollected,

    /* ✅ Balance INCLUDING previous balance */
    FORMAT(
        (ISNULL(F.Amount, 0) + ISNULL(PB.PreviousTermBalance, 0)) 
        - ISNULL(FT.TotalPaid, 0),
        'N0'
    ) AS Balance,

    TN.TermNumber,
    T.Status,
    I.CreatedBy,
    FY.FinancialYear,
    CONVERT(VARCHAR(10), I.CreatedDate, 105) AS CreatedDate,

    SC.SchoolName,
    SC.SchoolCode,
    L.LogoName AS Logo,
    SC.LogoId,
    SC.Address,

    PS.ParentSchoolCode,
    PS.ParentSchoolName,
    PS.ParentSchoolId,

    T.TermId,
    SmsStatus,

    /* ✅ Invoice status based on FULL amount */
    CASE 
        WHEN ISNULL(FT.TotalPaid, 0) = 0 THEN 'NOT PAID'
        WHEN ISNULL(FT.TotalPaid, 0) 
             < (ISNULL(F.Amount, 0) + ISNULL(PB.PreviousTermBalance, 0))
             THEN 'PARTLY PAID'
        WHEN ISNULL(FT.TotalPaid, 0) 
             = (ISNULL(F.Amount, 0) + ISNULL(PB.PreviousTermBalance, 0))
             THEN 'PAID IN FULL'
        ELSE 'OVER PAID'
    END AS InvoiceStatus

FROM StudentInvoice I

INNER JOIN Term T 
    ON I.TermId = T.TermId

INNER JOIN Status ST 
    ON T.Status = ST.StatusID

INNER JOIN TermNumber TN 
    ON T.Term = TN.TermId

INNER JOIN FinancialYear FY 
    ON T.YearId = FY.FinancialYearId

INNER JOIN Enrollment E 
    ON I.StudentId = E.StudentId 
   AND E.TermId = I.TermId

INNER JOIN Student S 
    ON E.StudentId = S.StudentID

INNER JOIN FeesConfiguration F 
    ON I.FeesId = F.FeesId

INNER JOIN Class C 
    ON E.ClassId = C.ClassId

LEFT JOIN ClassStream CST 
    ON E.StreamId = CST.StreamId

/* 🔹 Each transaction */
LEFT JOIN FeesCollection FC
    ON I.InvoiceId = FC.InvoiceId
   AND FC.ReferenceNo IS NOT NULL

/* 🔹 Invoice-level total paid */
LEFT JOIN (
    SELECT 
        InvoiceId,
        SUM(Amount) AS TotalPaid
    FROM FeesCollection
    WHERE ReferenceNo IS NOT NULL
    GROUP BY InvoiceId
) FT 
    ON I.InvoiceId = FT.InvoiceId

/* 🔹 Previous term balance (same logic as FeesCollectionSummary) */
LEFT JOIN (
    SELECT 
        IP.StudentId,
        IP.TermId,
        SUM(ISNULL(FP.Amount, 0) - ISNULL(PC.Collected, 0)) AS PreviousTermBalance
    FROM StudentInvoice IP
    INNER JOIN FeesConfiguration FP 
        ON IP.FeesId = FP.FeesId
    LEFT JOIN (
        SELECT InvoiceId, SUM(Amount) AS Collected
        FROM FeesCollection
        GROUP BY InvoiceId
    ) PC 
        ON IP.InvoiceId = PC.InvoiceId
    GROUP BY IP.StudentId, IP.TermId
) PB 
ON PB.StudentId = I.StudentId
AND PB.TermId = T.PreviousTerm

INNER JOIN School SC 
    ON E.SchoolId = SC.SchoolID

INNER JOIN Logo L 
    ON SC.LogoId = L.Id

INNER JOIN ParentSchool PS 
    ON SC.ParentSchoolId = PS.ParentSchoolId

WHERE 
    S.School = @SchoolId
    AND T.Status = 2 and FC.FeesCollectionId>0

ORDER BY FC.FeesCollectionId DESC;

";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DateTime collectedDate;
                    DateTime.TryParse(dr["CreatedDate"].ToString(), out collectedDate);

                    feesCollections.Add(new FeesCollections
                    {
                        FeesCollectionId = dr["FeesCollectionId"].ToString(),
                        ReferenceNo = dr["ReferenceNo"].ToString(),
                        StudentNo = dr["StudentNo"].ToString(),
                        Student = dr["Student"].ToString(),
                        FeesName = dr["FeesName"].ToString(),
                        Balance = dr["Balance"].ToString(),
                        TotalFees = dr["TotalFees"].ToString(),
                        AmountCollected = dr["AmountCollected"].ToString(),
                        Class = dr["ClassName"].ToString(),
                        StreamName = dr["StreamName"].ToString(),
                        Term = dr["Term"].ToString(),
                        InvoiceNo = dr["InvoiceNo"].ToString(),
                        CreatedBy = dr["CreatedBy"].ToString(),
                        CollectedDate = collectedDate
                    });
                }
                dr.Close();
            }
            return feesCollections;
        }

        public class FeesCollections
        {
            public string FeesCollectionId { get; set; }
            public string ReferenceNo { get; set; }
            public string FeesName { get; set; }
            public string Balance { get; set; }
            public string Class { get; set; }
            public string StreamName { get; set; }
            public string StudentNo { get; set; }
            public string Student { get; set; }
            public string AmountCollected { get; set; }
            public string TotalFees{ get; set; }
            public string Term { get; set; }
            public string InvoiceNo { get; set; }
            public string CreatedBy { get; set; }
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

