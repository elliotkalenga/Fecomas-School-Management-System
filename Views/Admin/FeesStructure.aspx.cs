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
    public partial class FeesStructure : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {// Check if the user is logged in
            if (Session["User"] == null)
            {
                // Redirect to login page
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["FeesId"] != null)
                {
                    int FeesId = int.Parse(Request.QueryString["FeesId"]);
                        BindRecordsRepeater();
                        // Load the student data if needed
                }


            }
        }

        private List<exams> GetRecordsList()
        {
            List<exams> exams = new List<exams>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"select FeesId,FeesName,Description,Amount,S.Status,CreatedBy,CreatedDate
                                    from feesconfiguration F Inner Join Status S on F.Status=S.StatusId
                                    Where Schoolid=@SchoolId";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DateTime CreatedDate;
                    DateTime.TryParse(dr["CreatedDate"].ToString(), out CreatedDate);

                    exams.Add(new exams
                    {
                        FeesId = dr["FeesId"].ToString(),
                        FeesName = dr["FeesName"].ToString(),
                        Description = dr["Description"].ToString(),
                        Status = dr["Status"].ToString(),
                        Amount = dr["Amount"].ToString(),
                        CreatedBy = dr["CreatedBy"].ToString(),
                        CreatedDate = CreatedDate
                    });
                }
                dr.Close();
            }
            return exams;
        }

        public class exams
        {
            public string FeesId { get; set; }
            public string FeesName { get; set; }
            public string Description { get; set; }
            public string Amount { get; set; }
            public string Status { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedDate { get; set; }
            public string CreatedDateString => CreatedDate.ToString("yyyy-MM-dd");
        }

        private void BindRecordsRepeater()
        {
            List<exams> exams = GetRecordsList();
            RecordsRepeater.DataSource = exams;
            RecordsRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindRecordsRepeater();
        }


    }
}
