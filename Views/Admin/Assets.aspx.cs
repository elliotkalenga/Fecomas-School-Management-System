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
    public partial class Assets : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {// Check if the user is logged in
            if (Session["User"] == null)
            {
                // Redirect to login page
                Response.Redirect("~/Views/Admin/UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["AssetId"] != null)
                {
                    int AssetId = int.Parse(Request.QueryString["AssetId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteStudentData(AssetId);
                    }
                    else
                    {
                        BindStudentsRepeater();
                        // Load the student data if needed
                    }
                }


            }
        }

        private void DeleteStudentData(int AssetId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Exam WHERE AssetId = @AssetId", Con);
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                cmd.ExecuteNonQuery();
            }

            // Redirect back to the students page after deletion
            Response.Redirect("Asset.aspx");
        }



        private List<Asset> GetRecordsList()
        {
            List<Asset> Asset = new List<Asset>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"SELECT 
    AssetId
    ,Barcode,AllocatedStatus
    ,AssetName
    ,AssetDescription
    ,ac.AssetCategory
    ,a.SchoolId
    ,a.CreatedBy
    ,CreatedDate
    ,LifeSpan
    ,PurchasedDate
    ,FORMAT(PurchaseCost, 'N2') AS PurchaseCost
    ,LifespanInDays
    ,UsedSpan
    ,RemainingSpan
    ,FORMAT(AssetValue, 'N2') AS AssetValue
    ,UsagePercent
    ,Status
FROM [dbo].[Asset] a 
INNER JOIN AssetCategory ac ON a.AssetCategoryId = ac.AssetCategoryId

                                    Where a.Schoolid=@SchoolId";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DateTime CreatedDate;
                    DateTime.TryParse(dr["PurchasedDate"].ToString(), out CreatedDate);

                    Asset.Add(new Asset
                    {
                        AssetId = dr["AssetId"].ToString(),
                        BarCode = dr["Barcode"].ToString(),
                        AssetName = dr["AssetName"].ToString(),
                        AssetDescription = dr["AssetDescription"].ToString(),
                        AssetStatus = dr["Status"].ToString(),
                        LifeSpan = dr["LifeSpan"].ToString(),
                        LifespanInDays = dr["LifespanInDays"].ToString(),
                        UsagePercent = dr["UsagePercent"].ToString(),
                        PurchaseCost = dr["PurchaseCost"].ToString(),
                        UsedSpan = dr["UsedSpan"].ToString(),
                        AssetCategory = dr["AssetCategory"].ToString(),
                        CreatedBy = dr["CreatedBy"].ToString(),
                        AllocatedStatus = dr["AllocatedStatus"].ToString(),
                        RemainingSpan = dr["RemainingSpan"].ToString(),
                        AssetValue = dr["AssetValue"].ToString(),
                        CreatedDate = CreatedDate
                    });
                }
                dr.Close();
            }
            return Asset;
        }


        protected string GetStatusClass(string status)
        {
            switch (status.ToLower())
            {
                case "new":
                    return "badge badge-success";
                case "good":
                    return "badge badge-info";
                case "fair":
                    return "badge badge-warning";
                case "old":
                    return "badge badge-secondary";
                case "fully depreciated":
                    return "badge badge-danger";
                default:
                    return "badge badge-light";
            }
        }

        protected string GetStatusIcon(string status)
        {
            switch (status.ToLower())
            {
                case "new":
                    return "fas fa-star";
                case "good":
                    return "fas fa-thumbs-up";
                case "fair":
                    return "fas fa-adjust";
                case "old":
                    return "fas fa-hourglass-end";
                case "fully depreciated":
                    return "fas fa-times-circle";
                default:
                    return "";
            }
        }
        public class Asset
        {
            public string AssetId { get; set; }
            public string BarCode { get; set; }
            public string LifeSpan { get; set; }
            public string PurchaseCost { get; set; }
            public string LifespanInDays { get; set; }
            public string RemainingSpan { get; set; }
            public string AssetDescription { get; set; }
            public string AssetName { get; set; }
            public string AssetCategory { get; set; }
            public string AssetValue { get; set; }
            public string AssetStatus { get; set; }
            public string UsagePercent { get; set; }
            public string UsedSpan { get; set; }
            public string AllocatedStatus { get; set; }
            public string AssetHolder { get; set; }

            public string CreatedBy { get; set; }
            public DateTime CreatedDate { get; set; }
            public string PurchasedDate => CreatedDate.ToString("yyyy-MM-dd");
        }

        private void BindStudentsRepeater()
        {
            List<Asset> Asset = GetRecordsList();
            StudentsRepeater.DataSource = Asset;
            StudentsRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindStudentsRepeater();
        }


    }

}

