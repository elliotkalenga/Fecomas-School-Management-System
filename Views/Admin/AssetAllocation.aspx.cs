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
    public partial class AssetAllocation : System.Web.UI.Page
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
                if (Request.QueryString["AllocationId"] != null)
                {
                    int AllocationId = int.Parse(Request.QueryString["AllocationId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                    }
                    else
                    {
                        BindStudentsRepeater();
                        // Load the student data if needed
                    }
                }


            }
        }




        private List<Asset> GetRecordsList()
        {
            List<Asset> Asset = new List<Asset>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"SELECT 
    AllocationId
    ,Barcode,aa.AllocatedStatus
    ,AssetName
    ,AssetDescription
    ,ac.AssetCategory,aa.ReturnedDate
    ,a.SchoolId
    ,a.CreatedBy
    ,aa.CreatedDate
    ,LifeSpan
    ,PurchasedDate
    ,FORMAT(PurchaseCost, 'N2') AS PurchaseCost
    ,LifespanInDays
    ,UsedSpan
    ,RemainingSpan
    ,FORMAT(AssetValue, 'N2') AS AssetValue
    ,UsagePercent
    ,a.Status, u.FirstName+' '+u.LastName as AssetHolder
FROM AssetAllocation aa inner join asset a on aa.assetId=a.assetID
inner join users u on aa.userId=u.UserId
INNER JOIN AssetCategory ac ON a.AssetCategoryId = ac.AssetCategoryId

                                    Where aa.Schoolid=@SchoolId";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DateTime CreatedDate;
                    DateTime ReturnedDate;

                    DateTime.TryParse(dr["CreatedDate"].ToString(), out CreatedDate);
                    DateTime.TryParse(dr["ReturnedDate"].ToString(), out ReturnedDate);

                    Asset.Add(new Asset
                    {
                        AllocationId = dr["AllocationId"].ToString(),
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
                        AssetHolder = dr["AssetHolder"].ToString(),
                        RemainingSpan = dr["RemainingSpan"].ToString(),
                        AssetValue = dr["AssetValue"].ToString(),
                        CreatedDate = dr["CreatedDate"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["CreatedDate"].ToString()),
                        ReturnedDate = dr["ReturnedDate"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["ReturnedDate"].ToString()),
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
            public string AllocationId { get; set; }
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
            public string AssetHolder { get; set; }
            public string AllocatedStatus { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? ReturnedDate { get; set; }
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

