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
    public partial class LessonPlans : System.Web.UI.Page
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
                        DeleteStudentData(SchemeId);
                    }
                    else
                    {
                        BindRecordsRepeater();
                        // Load the student data if needed
                    }
                }


            }
        }

        private void DeleteStudentData(int SchemeId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Exam WHERE SchemeId = @SchemeId", Con);
                cmd.Parameters.AddWithValue("@SchemeId", SchemeId);
                cmd.ExecuteNonQuery();
            }

            // Redirect back to the students page after deletion
            Response.Redirect("LessonPlan.aspx");
        }


        private List<LesonPlan> GetRecordsList()
        {
            List<LesonPlan> LesonPlans = new List<LesonPlan>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"
    SELECT 
        lp.LessonId,
        sc.WeekNo +'. TERM :'+Tn.TermNumber + ' (' + F.FinancialYear + '). SUBJECT: '+S.SubjectName + '-' + C.ClassName + '. TOPIC: ' +sc.Topic as SchemeOfWork, 
        lp.Introduction, 
        lp.WeekNo,
        lp.LessonTopic, 
        lp.LessonObjectives, 
        LP.LessonOutcome, 
        LP.TeachingMethods, 
        LP.PlannedACtivities, 
        LP.LessonEvaluation, 
        lP.Resources, 
        lP.AssessmentCriteria, 
        LP.CreatedBy as Teacher, 
        LP.CheckStatus,
        LP.CheckedBy,
        LP.CheckedDate,
        LP.DeliveryTime,
        lp.CreatedBy
    FROM 
        LessonPlan lp  WITH (NOLOCK) 
    INNER JOIN 
        Schemesofwork sc WITH (NOLOCK) on lp.schemeid=sc.schemeid 
    INNER JOIN 
        Subjectallocation sa WITH (NOLOCK) on sc.subjectId = sa.allocationId 
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
        sc.Schoolid =@SchoolId  and T.status = 2 and lp.createdby=@Createdby order by lp.lessonid desc
";

                Con.Open();
                using (SqlCommand cmd = new SqlCommand(ShowData, Con))
                {
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            DateTime? CheckDate = null;
                            DateTime? DeliveryTime = null;
                            if (dr["CheckedDate"] != DBNull.Value)
                            {
                                CheckDate = Convert.ToDateTime(dr["CheckedDate"]);
                            }
                            if (dr["DeliveryTime"] != DBNull.Value)
                            {
                                DeliveryTime = Convert.ToDateTime(dr["DeliveryTime"]);
                            }

                            LesonPlans.Add(new LesonPlan
                            {
                                LessonId = dr["LessonId"].ToString(),
                                SchemesofWork = dr["SchemeOfWork"].ToString(),
                                LessonTopic = dr["LessonTopic"].ToString(),
                                Introduction = dr["Introduction"].ToString(),
                                LessonObjectives = dr["LessonObjectives"].ToString(),
                                AssessmentCriteria = dr["AssessmentCriteria"].ToString(),
                                LessonOutcome = dr["LessonOutcome"].ToString(),
                                TeachingMethods = dr["TeachingMethods"].ToString(),
                                PlannedActivities = dr["PlannedActivities"].ToString(),
                                LessonEvaluation = dr["LessonEvaluation"].ToString(),
                                Resources = dr["Resources"].ToString(),
                                CheckStatus = dr["CheckStatus"].ToString(),
                                CreatedBy = dr["Teacher"].ToString(),
                                CheckDate = CheckDate,
                                DeliveryTime = DeliveryTime
                            });
                        }
                    }
                }
            }
            return LesonPlans;
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
        public class LesonPlan
        {
            public string LessonId { get; set; }
            public string SchemesofWork{ get; set; }
            public string LessonTopic { get; set; }
            public string Introduction { get; set; }
            public string LessonObjectives { get; set; }
            public string LessonOutcome { get; set; }
            public string TeachingMethods { get; set; }
            public string AssessmentCriteria { get; set; }
            public string Resources { get; set; }
            public string LessonEvaluation { get; set; }
            public string CheckStatus { get; set; }
            public string PlannedActivities { get; set; }

            public string CheckeddBy { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? CheckDate { get; set; }
            public DateTime? DeliveryTime { get; set; }
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

