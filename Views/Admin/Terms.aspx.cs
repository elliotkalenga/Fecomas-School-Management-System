using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using static SMSWEBAPP.Views.Admin.Exams;

namespace SMSWEBAPP.Views.Admin
{
    public partial class Terms : System.Web.UI.Page
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

                        BindStudentsRepeater();
                        // Load the student data if needed
                    }
                }








        private List<terms> GetRecordList()
        {
            List<terms> terms = new List<terms>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"Select T.Termid, TT.TermNumber +' ('+ F.FinancialYear +')' as Term, 
                                    T.StartDate, T.EndDate, S.Status, T.CreatedBy, T.CreatedDate 
                            from Term T 
                            Inner Join Status S on T.status = S.StatusId 
                            Inner Join FinancialYear F on T.YearId = F.FinancialYearId 
                            Inner Join TermNumber TT on T.Term = TT.TermId
                            Where T.SchoolId = @SchoolId";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DateTime StartDate;
                    DateTime.TryParse(dr["StartDate"].ToString(), out StartDate);  // Use StartDate from the reader
                    DateTime EndDate;
                    DateTime.TryParse(dr["EndDate"].ToString(), out EndDate);      // Use EndDate from the reader

                    terms.Add(new terms
                    {
                        TermId = dr["TermId"].ToString(),
                        Term = dr["Term"].ToString(),
                        CreatedBy = dr["CreatedBy"].ToString(),
                        Status = dr["Status"].ToString(),
                        StartDate = StartDate,
                        EndDate = EndDate,
                    });
                }
                dr.Close();
            }
            return terms;  // Return the list of exams
        }

        public class terms
        {
            public string TermId { get; set; }
            public string Term { get; set; }
            public string CreatedBy { get; set; }
            public string Status { get; set; }
            public DateTime StartDate { get; set; }  // Renamed to start with an uppercase letter
            public string StartDateString => StartDate.ToString("dd-MMMM yyyy");  // Use a string property for formatted date

            public DateTime EndDate { get; set; }  // Renamed to start with an uppercase letter
            public string EndDateString => EndDate.ToString("yyyy-MM-dd");  // Use a string property for formatted date
        }

        private void BindStudentsRepeater()
        {
            List<terms> terms = GetRecordList();
            RecordRepeater.DataSource = terms;
            RecordRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindStudentsRepeater();
        }


    }

}
