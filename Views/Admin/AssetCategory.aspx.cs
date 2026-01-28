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
    public partial class AssetCategory : System.Web.UI.Page
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

        private List<Asset> GetRecordsList()
        {
            List<Asset> assets = new List<Asset>();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"
                SELECT * FROM AssetCategory 
WHERE 
    SchoolId = @SchoolId"; 

                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            assets.Add(new Asset
                            {
                                AssetCategoryId = dr["AssetCategoryId"].ToString(),
                                AssetCategory = dr["AssetCategory"].ToString(),
                                CreatedBy = dr["CreatedBy"].ToString(),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // Logger.LogError(ex);
                // Return an empty list or null
                return new List<Asset>();
            }
            return assets;
        }
        private void BindRecordsRepeater()
        {
            List<Asset> assets = GetRecordsList();
            RecordsRepeater.DataSource = assets;
            RecordsRepeater.DataBind();
        }
    }

    // Moved the class outside to maintain better structure
    public class Asset
    {
        public string AssetCategoryId { get; set; }
        public string AssetCategory { get; set; }
        public string CreatedBy { get; set; }
    }
}
