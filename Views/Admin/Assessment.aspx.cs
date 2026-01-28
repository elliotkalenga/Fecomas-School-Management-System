using Microsoft.Data.SqlClient;
using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class Assessment : System.Web.UI.Page
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
            Response.Redirect("Assessments.aspx");
        }



        private List<Assessments> GetStudentsList()
        {
            List<Assessments> Assessments = new List<Assessments>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"select AssessmentId,RE.ReleaseStatus, AssessmentTitle+'-'+ C.ClassName as AssessmentTitle,ET.ExamType,
S.SubjectName+'-'+C.ClassName as SubjectName,TN.TermNumber + ' ('+F.FinancialYear +')' As Term,Sta.Status,
Contribution,A.CreatedBy,A.CreatedDate from Assessment A
Inner Join ExamType ET on A.AssessmentTypeId=ET.ExamTypeId
inner join Term T on A.termId=T.TermId
inner Join TermNumber TN on T.Term=TN.TermId
inner join FinancialYear F on T.Yearid=F.FinancialYearid 
Inner Join ReleaseExam RE on RE.ReleaseExamId=A.AssessmentStatus
Inner Join SubjectAllocation sa on A.SubjectId=sa.AllocationId
Inner Join Subject s on sa.SubjectId=S.SubjectId
Inner Join Class C on sa.Classid=C.ClassId
Inner Join Users u on sa.TeacherId=u.UserId
Inner Join Status sta on a.status =sta.StatusId
Where A.Schoolid=@SchoolId and u.Username=@Username and T.Status=2 Order by AssessmentId Desc ";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                cmd.Parameters.AddWithValue("@UserName", Session["UserName"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DateTime CreatedDate;
                    DateTime.TryParse(dr["CreatedDate"].ToString(), out CreatedDate);

                    Assessments.Add(new Assessments
                    {
                        AssessmentId = dr["AssessmentId"].ToString(),
                        AssessmentTitle = dr["AssessmentTitle"].ToString(),
                        Contribution = dr["Contribution"].ToString(),
                        SubjectName = dr["SubjectName"].ToString(),
                        ExamType = dr["ExamType"].ToString(),
                        AssessmentStatus = dr["ReleaseStatus"].ToString(),
                        Status = dr["Status"].ToString(),
                        Term = dr["Term"].ToString(),
                        CreatedBy = dr["CreatedBy"].ToString(),
                        CreatedDate = CreatedDate
                    });
                }
                dr.Close();
            }
            return Assessments;
        }

        public class Assessments
        {
            public string AssessmentId { get; set; }
            public string AssessmentTitle{ get; set; }
            public string Contribution{ get; set; }
            public string SubjectName { get; set; }
            public string AssessmentStatus { get; set; }
            public string Status { get; set; }
            public string ExamType { get; set; }
            public string Term { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedDate { get; set; }
            public string CreatedDateString => CreatedDate.ToString("yyyy-MM-dd");
        }

        private void BindStudentsRepeater()
        {
            List<Assessments> Assessments = GetStudentsList();
            StudentsRepeater.DataSource = Assessments;
            StudentsRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindStudentsRepeater();
        }


    }

}
