using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class Requisition : System.Web.UI.Page
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
            }
        }


        private List<RequisitionModel> GetRecordsList()
        {
            List<RequisitionModel> requisitionList = new List<RequisitionModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"
                    
                                        SELECT 
    R.RequisitionId, 
    R.CreatedDate,
    R.Purpose,
BI.ItemName,
    TN.TermNumber + ' (' + FY.FinancialYear + ')' AS Term,
    R.CreatedBy,
    'MK ' + FORMAT(COALESCE(SUM(RI.Amount), 0), 'N0') AS Amount, 
    R.RequisitionStatus
FROM Requisition R
INNER JOIN BudgetItems BI ON R.BudgetItemId = BI.BudgetItemId
INNER JOIN Term T ON R.TermID = T.TermId
INNER JOIN TermNumber TN ON T.Term = TN.TermId
INNER JOIN FinancialYear FY ON T.YearId = FY.FinancialYearId
LEFT OUTER JOIN RequisitionItem RI ON R.RequisitionID = RI.RequisitionId
WHERE R.SchoolId = @SchoolId
GROUP BY  
    R.RequisitionId, 
    R.CreatedDate,
    R.Purpose,
BI.ItemName,
    TN.TermNumber,  
    FY.FinancialYear,
    R.CreatedBy,
    R.RequisitionStatus
ORDER BY R.RequisitionStatus DESC
  ";

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                requisitionList.Add(new RequisitionModel
                                {
                                    RequisitionId = dr["RequisitionId"].ToString(),
                                    RequisitionStatus = dr["RequisitionStatus"].ToString(),
                                    Amount = dr["Amount"].ToString(),
                                    Purpose = dr["Purpose"].ToString(),
                                    ItemName = dr["ItemName"].ToString(),
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

            return requisitionList;
        }

        private void BindRecordsRepeater()
        {
            List<RequisitionModel> requisitions = GetRecordsList();

            if (requisitions.Count > 0)
            {
                RecordsRepeater.DataSource = requisitions;
                RecordsRepeater.DataBind();
                noDataRow.Visible = false; // Hide message when data exists
            }
            else
            {
                RecordsRepeater.DataSource = null;
                RecordsRepeater.DataBind();
                noDataRow.Visible = true; // Show message when no data exists
            }
        }
    }
}

// Model class (renamed to avoid conflict)
public class RequisitionModel
{
    public string RequisitionId { get; set; }
    public string RequisitionItemId { get; set; }
    public string Notes{ get; set; }
    public string ItemName { get; set; }
    public string RequisitionItemName { get; set; }
    public string RequisitionStatus{ get; set; }
    public string Purpose { get; set; }
    public string CreatedBy { get; set; }
    public string Term { get; set; }
    public string Amount { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedDateString => CreatedDate.ToString("yyyy-MM-dd");
}
