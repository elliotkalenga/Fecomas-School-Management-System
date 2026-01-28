using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class Income : System.Web.UI.Page
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

        private List<Incomes> GetRecordsList()
        {
            List<Incomes> incomes = new List<Incomes>();

            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"
                        SELECT 
                            I.IncomeId, 
                            I.Description,
                            I.Purpose,
                            I.Amount,
                            S.Source,
                            TN.TermNumber + ' (' + FY.FinancialYear + ')' AS Term,
                            I.CreatedBy,
                            I.CreatedDate 
                        FROM Income I 
                        INNER JOIN Source S ON I.Source = S.SourceId 
                        INNER JOIN Term T ON I.TermID = T.TermId
                        INNER JOIN TermNumber TN ON T.Term = TN.TermId
                        INNER JOIN FinancialYear FY ON T.YearId = FY.FinancialYearId
                        WHERE I.SchoolId = @SchoolId";  // Corrected alias issue

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                incomes.Add(new Incomes
                                {
                                    IncomeId = dr["IncomeId"].ToString(),
                                    Description = dr["Description"].ToString(),
                                    Purpose = dr["Purpose"].ToString(),
                                    Amount = dr.GetDecimal(dr.GetOrdinal("Amount")), // Ensure correct data type
                                    Source = dr["Source"].ToString(),
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
                // Log error properly instead of using Response.Write
                System.Diagnostics.Debug.WriteLine($"Error fetching income records: {ex.Message}");
                Response.Write("<script>alert('An error occurred while fetching data. Please try again later.');</script>");
            }

            return incomes;
        }

        private void BindRecordsRepeater()
        {
            List<Incomes> incomes = GetRecordsList();
            RecordsRepeater.DataSource = incomes;
            RecordsRepeater.DataBind();
        }
    }
}

// Keeping the class outside the namespace for better structure
public class Incomes
{
    public string IncomeId { get; set; }
    public string Source { get; set; }
    public string Description { get; set; }
    public string Purpose { get; set; }
    public string CreatedBy { get; set; }
    public decimal Amount { get; set; } // Changed to decimal for proper calculations
    public string Term { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedDateString => CreatedDate.ToString("yyyy-MM-dd");
}
