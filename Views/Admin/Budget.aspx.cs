using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class Budget : System.Web.UI.Page
    {
        private const string USER_SESSION_KEY = "User";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[USER_SESSION_KEY] == null)
            {
                // Redirect to login page if session is null
                Response.Redirect("UserLogin.aspx");
                return;
            }

            if (Session["SchoolId"] == null)
            {
                Response.Write("<script>alert('Invalid session. Please log in again.');</script>");
                Response.Redirect("UserLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindRecordsRepeater();
                BindRecordsRepeater2();
            }
        }

        private List<Budgets> GetRecordsList()
        {
            List<Budgets> budgets = new List<Budgets>();

            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"SELECT 
    B.BudgetId, 
    B.BudgetName, 
    TN.TermNumber +  ' ('+F.FinancialYear+')' as Term,
    COALESCE(B.Contingency, 0) as ContingencyPercent,
    FORMAT(COALESCE(B.TotalIncome, 0), 'N2') AS TotalIncome,
    FORMAT(COALESCE(B.TotalBudget, 0), 'N2') AS TotalBudget,
    FORMAT(COALESCE(B.TotalContingency, 0), 'N2') AS Contingency,
    FORMAT(COALESCE(B.TotalContingency, 0) + COALESCE(B.TotalBudget, 0), 'N2')   AS AmountPlusContingency,

    FORMAT(COALESCE(SUM(EI.Amount), 0), 'N2') AS Spent,
    FORMAT(COALESCE(B.TotalBudget, 0) - COALESCE(SUM(EI.Amount), 0), 'N2') AS Variance,
    CASE 
        WHEN  COALESCE(B.TotalBudget, 0) - COALESCE(SUM(EI.Amount), 0) > 0 THEN 'Within Budget'
        WHEN  COALESCE(B.TotalBudget, 0) - COALESCE(SUM(EI.Amount), 0) < 0 THEN 'Over Spending'
        WHEN  COALESCE(B.TotalBudget, 0) - COALESCE(SUM(EI.Amount), 0) = 0 THEN 'On Budget'
    END AS BudgetStatus, 
    St.Status
FROM BudgetItems BI WITH (NOLOCK)
INNER JOIN Budget B WITH (NOLOCK) ON BI.BudgetID = B.BudgetId
INNER JOIN BudgetCategory BC WITH (NOLOCK) ON BI.BudgetCategoryId = BC.BudgetCategoryID
INNER JOIN School S WITH (NOLOCK) ON BI.SchoolId = S.SchoolID
INNER JOIN Logo L WITH (NOLOCK) ON S.LogoId = L.id
LEFT JOIN Requisition R WITH (NOLOCK) ON BI.BudgetItemId = R.BudgetItemId
LEFT JOIN RequisitionItem RI WITH (NOLOCK) ON R.RequisitionID = RI.RequisitionId
LEFT JOIN Expense E WITH (NOLOCK) ON R.RequisitionId = E.RequisitionId
LEFT JOIN ExpenseItem EI WITH (NOLOCK) ON E.ExpenseID = EI.ExpenseId
LEFT JOIN ExpenseCategory EC WITH (NOLOCK) ON E.ExpenseCategoryId = EC.ExpenseCategoryId
INNER JOIN Term T WITH (NOLOCK) ON B.Termid = T.TermId
INNER JOIN FinancialYear F WITH (NOLOCK) ON T.Yearid = F.FinancialYearid
INNER JOIN TermNumber TN WITH (NOLOCK) ON T.Term = TN.TermId
INNER JOIN Status St WITH (NOLOCK) ON B.Status = ST.StatusId
WHERE B.SchoolId = @SchoolId
                        
GROUP BY 
    B.BudgetId, B.TotalIncome,
    B.BudgetName, 
    B.TotalBudget,
    B.Contingency,
    B.TotalContingency,
    TN.TermNumber,
    F.FinancialYear,
    St.Status
ORDER BY St.Status         ";

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                budgets.Add(new Budgets
                                {
                                    BudgetId = dr["BudgetId"].ToString(),
                                    BudgetName = dr["BudgetName"].ToString(),
                                    Term = dr["Term"].ToString(),
                                    Status = dr["Status"].ToString(),
                                    Amount = dr["TotalBudget"].ToString(),
                                    TotalIncome = dr["TotalIncome"].ToString(),
                                    Contingency = dr["Contingency"].ToString(),
                                    ContingencyPercent = dr["ContingencyPercent"].ToString(),
                                    AmountPlusContingency = dr["AmountPlusContingency"].ToString(),
                                    Spent = dr["Spent"].ToString(),
                                    Variance = dr["Variance"].ToString(),
                                    BudgetStatus = dr["BudgetStatus"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }

            return budgets;
        }

        private List<Budgets> GetRecordsList2()
        {
            List<Budgets> budgets = new List<Budgets>();

            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"SELECT 
    Bi.BudgetItemId,
    BI.ItemName,
    B.BudgetName, 
    TN.TermNumber +  ' ('+F.FinancialYear+')' as Term,
    FORMAT(COALESCE(Bi.Amount, 0), 'N2') AS Amount,
    FORMAT(COALESCE(Bi.Contingency, 0), 'N2') AS Contingency,
    FORMAT(COALESCE(Bi.AmountPlusContingency, 0), 'N2') AS AmountPlusContingency,
    FORMAT(COALESCE(SUM(EI.Amount), 0), 'N2') AS Spent,
    FORMAT(COALESCE(BI.Amount, 0) - COALESCE(SUM(EI.Amount), 0), 'N2') AS Variance,
    CASE 
        WHEN  COALESCE(BI.Amount, 0) - COALESCE(SUM(EI.Amount), 0) > 0 THEN 'Within Budget'
        WHEN  COALESCE(BI.Amount, 0) - COALESCE(SUM(EI.Amount), 0) < 0 THEN 'Over Spending'
        WHEN  COALESCE(BI.Amount, 0) - COALESCE(SUM(EI.Amount), 0) = 0 THEN 'On Budget'
    END AS BudgetStatus
FROM BudgetItems BI WITH (NOLOCK)
INNER JOIN Budget B WITH (NOLOCK) ON BI.BudgetID = B.BudgetId
INNER JOIN BudgetCategory BC WITH (NOLOCK) ON BI.BudgetCategoryId = BC.BudgetCategoryID
INNER JOIN School S WITH (NOLOCK) ON BI.SchoolId = S.SchoolID
INNER JOIN Logo L WITH (NOLOCK) ON S.LogoId = L.id
LEFT JOIN Requisition R WITH (NOLOCK) ON BI.BudgetItemId = R.BudgetItemId
LEFT JOIN RequisitionItem RI WITH (NOLOCK) ON R.RequisitionID = RI.RequisitionId
LEFT JOIN Expense E WITH (NOLOCK) ON R.RequisitionId = E.RequisitionId
LEFT JOIN ExpenseItem EI WITH (NOLOCK) ON E.ExpenseID = EI.ExpenseId
LEFT JOIN ExpenseCategory EC WITH (NOLOCK) ON E.ExpenseCategoryId = EC.ExpenseCategoryId
INNER JOIN Term T WITH (NOLOCK) ON B.Termid = T.TermId
INNER JOIN FinancialYear F WITH (NOLOCK) ON T.Yearid = F.FinancialYearid
INNER JOIN TermNumber TN WITH (NOLOCK) ON T.Term = TN.TermId
INNER JOIN Status St WITH (NOLOCK) ON B.Status = ST.StatusId
WHERE BI.SchoolId = @SchoolId AND B.Status = 2 
GROUP BY 
    Bi.BudgetItemId,
    BI.ItemName,
    B.BudgetName, 
    BI.Amount,
    Bi.Contingency,
    TN.TermNumber,
    F.FinancialYear,
    Bi.AmountPlusContingency order by bi.amount desc
";

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                budgets.Add(new Budgets
                                {
                                    BudgetItemId = dr["BudgetItemId"].ToString(),
                                    ItemName = dr["ItemName"].ToString(),
                                    BudgetName = dr["BudgetName"].ToString(),
                                    Term = dr["Term"].ToString(),
                                    Amount = dr["Amount"].ToString(),
                                    Contingency = dr["Contingency"].ToString(),
                                    AmountPlusContingency = dr["AmountPlusContingency"].ToString(),
                                    Spent = dr["Spent"].ToString(),
                                    Variance = dr["Variance"].ToString(),
                                    BudgetStatus = dr["BudgetStatus"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }

            return budgets;
        }
        private void BindRecordsRepeater()
        {
            RecordsRepeater.DataSource = GetRecordsList();
            RecordsRepeater.DataBind();
        }

        private void BindRecordsRepeater2()
        {
            RecordsRepeater2.DataSource = GetRecordsList2();
            RecordsRepeater2.DataBind();
        }
    }

    // Budget model class - consider moving this to a separate file
    public class Budgets
    {
        public string BudgetId { get; set; }
        public string BudgetItemId { get; set; }
        public string ItemName { get; set; }
        public string TotalIncome { get; set; }
        public string Amount { get; set; }
        public string CategoryName { get; set; }
        public string Budget { get; set; }
        public string Term { get; set; }
        public string Spent { get; set; }
        public string BudgetName { get; set; }
        public string BudgetStatus { get; set; }
        public string Status { get; set; }
        public string Variance { get; set; }
        public string Description { get; set; }
        public string Contingency { get; set; }
        public string ContingencyPercent { get; set; }
        public string AmountPlusContingency { get; set; }
    }
}
