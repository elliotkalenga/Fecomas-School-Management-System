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
    public partial class ExaminationReports : System.Web.UI.Page
    {
        private const string USER_SESSION_KEY = "User";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[USER_SESSION_KEY] == null)
            {
                // Redirect to login page
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                BindExamRepeater();
                if (Request.QueryString["ScoreId"] != null)
                {
                    int examId = int.Parse(Request.QueryString["ScoreId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteScore(examId);
                    }
                    else
                    {

                        // Load the student data if needed
                    }
                }
            }
        }

        private void DeleteScore(int scoreId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Exam WHERE ScoreId = @ScoreId", con);
                    cmd.Parameters.AddWithValue("@ScoreId", scoreId);
                    cmd.ExecuteNonQuery();
                }

                Response.Redirect("Grades.aspx");
            }
            catch (Exception ex)
            {
                // Log the exception and handle it gracefully
                // Example: log.Error(ex);
                Response.Write("An error occurred: " + ex.Message);
            }
        }

        private List<Exam> GetExamList()
        {
            List<Exam> Exam = new List<Exam>();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"WITH Exams AS (
    SELECT SchoolCode, TermId, ReleaseStatus, ReleasedTime, ReleasedBy,
           AssessmentTitle AS AssessmentTitleId, AssessmentTitle
    FROM JCEEndofterm WHERE SchoolCode = @SchoolCode
    UNION
    SELECT SchoolCode, TermId, ReleaseStatus, ReleasedTime, ReleasedBy,
           AssessmentTitle AS AssessmentTitleId, AssessmentTitle
    FROM MSCEEndofterm WHERE SchoolCode = @SchoolCode
    UNION
    SELECT SchoolCode, TermId, ReleaseStatus, ReleasedTime, ReleasedBy,
           AssessmentTitle AS AssessmentTitleId, AssessmentTitle
    FROM PRIEndofterm WHERE SchoolCode = @SchoolCode
    UNION
    SELECT SchoolCode, TermId, ReleaseStatus, ReleasedTime, ReleasedBy,
           AssessmentTitle AS AssessmentTitleId, AssessmentTitle
    FROM JCEOTHERS WHERE SchoolCode = @SchoolCode
    UNION
    SELECT SchoolCode, TermId, ReleaseStatus, ReleasedTime, ReleasedBy,
           AssessmentTitle AS AssessmentTitleId, AssessmentTitle
    FROM MSCEOTHERS WHERE SchoolCode = @SchoolCode
    UNION
    SELECT SchoolCode, TermId, ReleaseStatus, ReleasedTime, ReleasedBy,
           AssessmentTitle AS AssessmentTitleId, AssessmentTitle
    FROM PRIOTHERS WHERE SchoolCode = @SchoolCode
)

SELECT SchoolCode, TermId, ReleaseStatus, 
       MAX(ReleasedTime) AS ReleasedTime,  -- ✅ Pick latest release
       MAX(ReleasedBy) AS ReleasedBy,
       AssessmentTitleId, AssessmentTitle
FROM Exams
GROUP BY SchoolCode, TermId, AssessmentTitleId, AssessmentTitle, ReleaseStatus
ORDER BY TermId DESC;
";

                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]); // corrected parameter name
                                                                                       // removed @UserName parameter as it's not being used in the query
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Exam.Add(new Exam
                        {
                            AssessmentId = dr["AssessmentTitleId"].ToString(),
                            Assessment = dr["AssessmentTitle"].ToString(),
                            ReleaseStatus = dr["ReleaseStatus"].ToString(),
                            ReleasedBy = dr["ReleasedBy"].ToString(),
                            ReleasedTime = dr["ReleasedTime"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["ReleasedTime"]).ToString("yyyy-MM-dd HH:mm:ss")
                        });
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                // Log the exception and handle it gracefully
                // Example: log.Error(ex);
                Response.Write("An error occurred: " + ex.Message);
            }
            return Exam;
        }
        public class Exam
        {
            public string AssessmentId { get; set; }
            public string Assessment { get; set; }
            public string ReleaseStatus { get; set; }
            public string ReleasedTime { get; set; }
            public string ReleasedBy { get; set; }
        }

        private void BindExamRepeater()
        {
            List<Exam> Exam = GetExamList();
            ScoresRepeater.DataSource = Exam;
            ScoresRepeater.DataBind();
        }
    }
}
