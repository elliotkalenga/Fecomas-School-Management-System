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
    public partial class Expenses : System.Web.UI.Page
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
                if (Request.QueryString["deleteSuccess"] != null && Request.QueryString["deleteSuccess"].ToString() == "true")
                {

                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessMessage", "$('#successMessage').show();", true);
                }
            }
            }

            private List<ExpenseModel> GetRecordsList()
            {
                List<ExpenseModel> expenseList = new List<ExpenseModel>();

                try
                {
                    using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        string query = @"
                    
                                                                               SELECT 
   E.ExpenseID,E.Description,E.CreatedDate,	
    R.Purpose as Requisition,
BI.ItemName as Budget,
    TN.TermNumber + ' (' + FY.FinancialYear + ')' AS Term,
    R.CreatedBy, 
    'MK ' + FORMAT(COALESCE(SUM(EI.Amount), 0), 'N0') AS Amount, 
    E.ExpenseStatus
FROM Expense E
INNER JOIN Requisition R ON E.RequisitionId = R.RequisitionId
INNER JOIN BudgetItems BI ON R.BudgetItemId = BI.BudgetItemId
INNER JOIN Term T ON E.TermID = T.TermId
INNER JOIN TermNumber TN ON T.Term = TN.TermId
INNER JOIN FinancialYear FY ON T.YearId = FY.FinancialYearId
LEFT OUTER JOIN ExpenseItem EI ON E.ExpenseID =EI.ExpenseId
WHERE R.SchoolId = @SchoolId
GROUP BY  
    E.ExpenseID,E.Description ,
    E.CreatedDate,
    R.Purpose,
BI.ItemName,
    TN.TermNumber,  
    FY.FinancialYear,
    R.CreatedBy,
    E.ExpenseStatus
ORDER BY E.ExpenseStatus DESC
 ";
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);

                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    expenseList.Add(new ExpenseModel
                                    {
                                        ExpenseId = dr["ExpenseId"].ToString(),
                                        Description = dr["Description"].ToString(),
                                        ExpenseStatus = dr["ExpenseStatus"].ToString(),
                                        Amount = dr["Amount"].ToString(),
                                        Budget = dr["Budget"].ToString(),
                                        Requisition = dr["Requisition"].ToString(),
                                        Term = dr["Term"].ToString(),
                                        CreatedBy = dr["CreatedBy"].ToString(),
                                        CreatedDate = dr.GetDateTime(dr.GetOrdinal("CreatedDate"))
                                    });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error instead of Response.Write
                    System.Diagnostics.Debug.WriteLine($"Error fetching requisition records: {ex.Message}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert",
             $"alert('An error occurred: {ex.Message}. Check logs for details.');", true);
                }

                return expenseList;
            }

            private void BindRecordsRepeater()
            {
                List<ExpenseModel> expenseList = GetRecordsList();
                RecordsRepeater.DataSource = expenseList;
                RecordsRepeater.DataBind();
            }
        }
    }

    // Model class (renamed to avoid conflict)
    public class ExpenseModel
    {
    public string ExpenseId { get; set; }
    public string ExpenseItemName { get; set; }
    public string ExpenseItemId { get; set; }
        public string Notes { get; set; }
        public string Budget { get; set; }
        public string Requisition { get; set; }
        public string ExpenseStatus { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string Term { get; set; }
        public string Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateString => CreatedDate.ToString("yyyy-MM-dd");
    }
