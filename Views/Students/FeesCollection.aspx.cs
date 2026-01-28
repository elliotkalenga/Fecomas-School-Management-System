using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Students
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
                    string ShowData = @"SELECT  FC.FeesCollectionId,FC.ReferenceNo,
                         S.StudentNo,
                         S.FirstName + ' ' + S.LastName AS Student,
                         F.FeesName,
                         FORMAT(F.Amount, 'N0') AS TotalFees,
                         FORMAT(FC.Amount, 'N0') AS AmountCollected,
                         c.ClassName,
                         I.InvoiceNo,
                         TN.TermNumber + ' ' + '(' + FY.FinancialYear + ')' AS Term,
                         P.PaymentMethod,
                         FC.CollectedFrom,
                         FC.TransactionID,
                         FC.CreatedBy,
                         FC.CreatedDate
FROM     FeesCollection AS FC
         INNER JOIN
         PaymentMethod AS P
         ON FC.PaymentMethod = P.PaymentMethodID
         INNER JOIN
         StudentInvoice AS I
         ON FC.InvoiceId = I.InvoiceId
         INNER JOIN Enrollment E on I.EnrollmentId=E.EnrollmentId Inner Join
         Student AS S
         ON E.StudentId = S.StudentID
         LEFT OUTER JOIN
         FeesConfiguration AS F
         ON I.FeesId = F.FeesId
         LEFT OUTER JOIN
         Term AS T
         ON E.Termid = T.TermId
         INNER JOIN
         TermNumber AS TN
         ON T.Term = TN.TermId
         INNER JOIN
         Class AS C
         ON E.ClassId = C.ClassId
         INNER JOIN
         FinancialYear AS FY
         ON T.YearId = FY.FinancialYearid
         INNER JOIN
         School AS SC
         ON E.SchoolId = SC.SchoolId
        where Sc.Schoolid=@SchoolId and T.Status=2 and s.StudentNo=@StudentNo
        ORDER BY FeesCollectionId DESC";

                    Con.Open();
                    SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                cmd.Parameters.AddWithValue("@StudentNo", Session["StudentNo"]);

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
                            TotalFees = dr["TotalFees"].ToString(),
                            AmountCollected = dr["AmountCollected"].ToString(),
                            Class = dr["ClassName"].ToString(),
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
                public string Class { get; set; }
                public string StudentNo { get; set; }
                public string Student { get; set; }
                public string AmountCollected { get; set; }
                public string TotalFees { get; set; }
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

