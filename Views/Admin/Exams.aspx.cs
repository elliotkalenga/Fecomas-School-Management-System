using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using static SMSWEBAPP.Views.Admin.Exams;

namespace SMSWEBAPP.Views.Admin
{
    public partial class Exams : System.Web.UI.Page
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
                if (Request.QueryString["ExamId"] != null)
                {
                    int ExamId = int.Parse(Request.QueryString["ExamId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteStudentData(ExamId);
                    }
                    else
                    {
                        BindStudentsRepeater();
                        // Load the student data if needed
                    }
                }


            }
        }

        private void DeleteStudentData(int ExamId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Exam WHERE ExamId = @ExamId", Con);
                cmd.Parameters.AddWithValue("@ExamId", ExamId);
                cmd.ExecuteNonQuery();
            }

            // Redirect back to the students page after deletion
            Response.Redirect("Exams.aspx");
        }



        private List<exams> GetStudentsList()
        {
            List<exams> exams = new List<exams>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"select E.ExamId,E.ExamWeight ,ExamCode,ExamTitle,ET.ExamType,TN.TermNumber + ' ('+F.FinancialYear +')' As Term,E.Description,E.CreatedBy,E.CreatedDate,RE.ReleaseStatus as ExamStatus from Exam E 
                                    inner join Term T on E.termId=T.TermId
                                    inner Join TermNumber TN on T.Term=TN.TermId
                                    Inner Join ExamType ET on E.ExamTypeId=ET.ExamTypeId
                                    inner join FinancialYear F on T.Yearid=F.FinancialYearid 
									Inner Join ReleaseExam RE on RE.ReleaseExamId=E.ReleasedStatus
                                    Where E.Schoolid=@SchoolId";

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
                        ExamId = dr["ExamId"].ToString(),
                        ExamCode = dr["ExamCode"].ToString(),
                        ExamWeight = dr["ExamWeight"].ToString(),
                        ExamTitle = dr["ExamTitle"].ToString(),
                        ExamType = dr["ExamType"].ToString(),
                        ExamStatus = dr["ExamStatus"].ToString(),
                        Term = dr["Term"].ToString(),
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
            public string ExamId { get; set; }
            public string ExamCode { get; set; }
            public string ExamWeight { get; set; }
            public string ExamTitle { get; set; }
            public string ExamStatus { get; set; }
            public string ExamType { get; set; }
            public string Term { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedDate { get; set; }
            public string CreatedDateString => CreatedDate.ToString("yyyy-MM-dd");
        }

        private void BindStudentsRepeater()
        {
            List<exams> exams = GetStudentsList();
            StudentsRepeater.DataSource = exams;
            StudentsRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindStudentsRepeater();
        }


    }

}
