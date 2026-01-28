using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using static SMSWEBAPP.Views.Admin.Classes;

namespace SMSWEBAPP.Views.Admin
{
    public partial class AcademicYear : System.Web.UI.Page
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

                BindRecordsRepeater();
                // Load the student data if needed
            }
        }

        private List<classes> GetRecordList()
        {
            List<classes> classes = new List<classes>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"Select * from FinancialYear where SchoolId=@SchoolId order by FinancialYearId Desc";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DateTime CreatedDate;
                    DateTime.TryParse(dr["CreatedDate"].ToString(), out CreatedDate);  // Use StartDate from the reader

                    classes.Add(new classes
                    {
                        FinancialYearId = dr["FinancialYearId"].ToString(),
                        FinancialYear = dr["FinancialYear"].ToString(),
                        CreatedBy = dr["CreatedBy"].ToString(),
                        CreatedDate = CreatedDate,
                    });
                }
                dr.Close();
            }
            return classes;  // Return the list of exams
        }

        public class classes
        {
            public string FinancialYearId { get; set; }
            public string FinancialYear { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedDate { get; set; }  // Renamed to start with an uppercase letter
            public string CreatedDateString => CreatedDate.ToString("dd-MMMM yyyy");  // Use a string property for formatted date

        }

        private void BindRecordsRepeater()
        {
            List<classes> classes = GetRecordList();
            RecordRepeater.DataSource = classes;
            RecordRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindRecordsRepeater();
        }


    }

}
