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
    public partial class InvoiceDetails : System.Web.UI.Page
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

        private List<InvoiceDetail> GetRecordsList()
        {
            List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>();

            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"SELECT 
    TN.TermNumber + ' (' + FY.FinancialYear + ')' AS Term,
    i.InvoiceId,
    i.InvoiceNumber,
    ISNULL(SUM(it.Subtotal), 0) AS Amount,  -- Ensure 0 if no items exist
    i.InvoiceDescription, 
    i.status, 
    S.SchoolName 
FROM Invoice i
LEFT JOIN InvoiceItems it ON it.InvoiceID = i.InvoiceId  -- Change INNER JOIN to LEFT JOIN
INNER JOIN School S ON i.CustomerId = S.SchoolID
INNER JOIN Term T ON i.TermID = T.TermId
INNER JOIN TermNumber TN ON T.Term = TN.TermId
INNER JOIN FinancialYear FY ON T.YearId = FY.FinancialYearId
GROUP BY 
    i.InvoiceId, 
    i.InvoiceNumber, 
    i.InvoiceDescription, 
    i.status, 
    S.SchoolName, 
    TN.TermNumber, 
    FY.FinancialYear

 ";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                invoiceDetails.Add(new InvoiceDetail
                                {
                                    InvoiceId = dr["InvoiceId"].ToString(),
                                    InvoiceNumber = dr["InvoiceNumber"].ToString(),
                                    InvoiceDescription = dr["InvoiceDescription"].ToString(),
                                    Status = dr["Status"].ToString(),
                                    Amount = dr["Amount"].ToString(),
                                    SchoolName = dr["Schoolname"].ToString(),
                                    Term = dr["Term"].ToString(),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error instead of Response.Write
                System.Diagnostics.Debug.WriteLine($"Error fetching Invoice records: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert",
         $"alert('An error occurred: {ex.Message}. Check logs for details.');", true);
            }

            return invoiceDetails;
        }

        private void BindRecordsRepeater()
        {
            List<InvoiceDetail> invoiceDetails = GetRecordsList();
            RecordsRepeater.DataSource = invoiceDetails;
            RecordsRepeater.DataBind();
        }
    }
}

// Model class (renamed to avoid conflict)
public class InvoiceDetail
{
    public string InvoiceId { get; set; }
    public string InvoiceDescription { get; set; }
    public string SchoolName { get; set; }
    public string Item { get; set; }
    public string UnitPrice { get; set; }
    public string Quantity { get; set; }
    public string SubTotal{ get; set; }
    public string Amount{ get; set; }
    public string InvoiceNumber{ get; set; }
    public string Status { get; set; }
    public string CreatedBy { get; set; }
    public string Term { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedDateString => CreatedDate.ToString("yyyy-MM-dd");
}
