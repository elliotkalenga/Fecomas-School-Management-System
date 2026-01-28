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
    public partial class SchemesApprove : System.Web.UI.Page
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
                if (Request.QueryString["SchemeId"] != null)
                {
                    int SchemeId = int.Parse(Request.QueryString["SchemeId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteRecordsData(SchemeId);
                    }
                    else
                    {
                        BindRecordsRepeater();
                        // Load the student data if needed
                    }
                }


            }
        }

        private void DeleteRecordsData(int SchemeId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Exam WHERE SchemeId = @SchemeId", Con);
                cmd.Parameters.AddWithValue("@SchemeId", SchemeId);
                cmd.ExecuteNonQuery();
            }

            // Redirect back to the students page after deletion
            Response.Redirect("LesonPlan.aspx");
        }



        private List<LesonPlan> GetRecordsList()
        {
            List<LesonPlan> LesonPlans = new List<LesonPlan>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"    SELECT 
        Sc.SchemeId, 
        sc.WeekNo, 
        Tn.TermNumber + ' (' + F.FinancialYear + ')' as Term, 
        S.SubjectName + '-' + C.ClassName as SubjectName,
        SC.Topic, 
        Sc.Lessons as LessonsCount,
        sc.LearningObjectives, 
        Sc.LearningOutcome, 
        Sc.SchemeEvaluation, 
        sc.Resources, 
        Sc.CreatedBy as Teacher, 
        Sc.DateCompleted,
        Sc.CompleteStatus,sc.Remarks,Schemereferences
    FROM 
        SchemesOfWork sc WITH (NOLOCK) 
    INNER JOIN 
        Subjectallocation sa WITH (NOLOCK) on sc.subjectId = sa.AllocationId
    INNER JOIN 
        Class c on sa.classId = c.ClassId 
    INNER JOIN 
        Subject s WITH (NOLOCK) on sa.subjectid = s.subjectId 
    INNER JOIN 
        Term AS T WITH (NOLOCK) ON Sc.TermId = T.TermId
    INNER JOIN 
        TermNumber AS TN WITH (NOLOCK) ON T.Term = TN.TermId
    INNER JOIN 
        FinancialYear AS F WITH (NOLOCK) ON T.YearId = F.FinancialYearId
    WHERE 
        sc.Schoolid = @SchoolId and T.status = 2 
";

                Con.Open();
                using (SqlCommand cmd = new SqlCommand(ShowData, Con))
                {
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            DateTime? completedDate = null;
                            if (dr["DateCompleted"] != DBNull.Value)
                            {
                                completedDate = Convert.ToDateTime(dr["DateCompleted"]);
                            }

                            LesonPlans.Add(new LesonPlan
                            {
                                SchemeId = dr["SchemeId"].ToString(),
                                Subject = dr["SubjectName"].ToString(),
                                Term = dr["Term"].ToString(),
                                WeekNo = dr["WeekNo"].ToString(),
                                Topic = dr["Topic"].ToString(),
                                Lessons = dr["LessonsCount"].ToString(),
                                LearningObjectives = dr["LearningObjectives"].ToString(),
                                LearningOutcome = dr["LearningOutcome"].ToString(),
                                SchemeEvaluation = dr["SchemeEvaluation"].ToString(),
                                Resources = dr["Resources"].ToString(),
                                Remarks = dr["Remarks"].ToString(),
                                CompleteStatus = dr["CompleteStatus"].ToString(),
                                References = dr["SchemeReferences"].ToString(),
                                CreatedBy = dr["Teacher"].ToString(),
                                CreatedDated = completedDate
                            });
                        }
                    }
                }
            }
            return LesonPlans;
        }

        public class LesonPlan
        {
            public string SchemeId { get; set; }
            public string Subject { get; set; }
            public string Term { get; set; }
            public string WeekNo { get; set; }
            public string Topic { get; set; }
            public string Lessons { get; set; }
            public string LearningObjectives { get; set; }
            public string LearningOutcome { get; set; }
            public string Resources { get; set; }
            public string SchemeEvaluation { get; set; }
            public string CompleteStatus { get; set; }
            public string Remarks { get; set; }
            public string References { get; set; }

            public string CreatedBy { get; set; }
            public DateTime? CreatedDated { get; set; }
        }

        private void BindRecordsRepeater()
        {
            List<LesonPlan> LesonPlan = GetRecordsList();
            RecordsRepeater.DataSource = LesonPlan;
            RecordsRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindRecordsRepeater();
        }


    }

}

