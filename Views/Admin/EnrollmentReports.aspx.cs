using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class EnrollmentReports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                // Redirect to login page
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                BindRecordRepeater();
                // Load the student data if needed

            }

        }


        private List<EnrolmentSummary> GetStudentsList()
        {
            List<EnrolmentSummary> enrolmentSummaries = new List<EnrolmentSummary>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"Select Count(EnrollmentId) as EnrollmentCount,TN.TermNumber + ' ('+F.FinancialYear + ')' As Term
                            
                            from Enrollment E
                            INNER JOIN Student S on E.StudentId=S.StudentID
                            INNER JOIN Gender G on S.Gender=G.GenderId
                            INNER JOIN Term T on E.Termid=T.TermId
                            INNER JOIN Class C on E.ClassId=C.ClassId
                            INNER JOIN TermNumber TN on T.Term=TN.TermId
                            INNER JOIN FinancialYear F on T.Yearid=F.FinancialYearid
                            Where E.SchoolId=@SchoolId
							Group by TN.TermNumber + ' ('+F.FinancialYear + ')'";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    //DateTime collectedDate;
                    //DateTime.TryParse(dr["CreatedDate"].ToString(), out collectedDate);

                    enrolmentSummaries.Add(new EnrolmentSummary
                    {
                        EnrollmentCount = Convert.ToInt32(dr["EnrollmentCount"]),
                        Term = dr["Term"].ToString(),
                        //CollectedDate = collectedDate
                    });
                }
                dr.Close();
            }
            return enrolmentSummaries;
        }

        public class EnrolmentSummary
        {
            public int EnrollmentCount  { get; set; }
            public string Term { get; set; }
        }

        private void BindRecordRepeater()
        {
            List<EnrolmentSummary> enrolmentSummaries = GetStudentsList();
            RecordRepeater.DataSource = enrolmentSummaries;
            RecordRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindRecordRepeater();
        }

    }
}

