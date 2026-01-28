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
    public partial class AdminAssessmentScores : System.Web.UI.Page
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
                BindScoresRepeater();
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
                    SqlCommand cmd = new SqlCommand("DELETE FROM Scores WHERE ScoreId = @ScoreId", con);
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

        private List<Score> GetScoresList()
        {
            List<Score> scores = new List<Score>();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"SELECT
    DENSE_RANK() OVER (PARTITION BY Sc.AssessmentId ORDER BY Sc.Score DESC) AS ScoreRank,
    ScoreId,
    a.AssessmentTitle,
    St.FirstName + ' ' + St.LastName + ' ' + C.ClassName AS Student,
    S.SubjectName+'-'+C.ClassName as SubjectName,
    Sc.Score,
    CONVERT(DECIMAL(10, 2), Sc.Score * a.contribution / 100.0) AS Contribution,
    CASE
        WHEN gs.ScaleTypeId = 1 THEN CONVERT(NVARCHAR(10), Grade1)
        WHEN gs.ScaleTypeId = 2 THEN CONVERT(NVARCHAR(10), Grade2)
        ELSE 'Grade Invalid'
    END AS Grade,
    gs.Description AS GradeDescription,
    gs.Remark AS Remark,
    Sc.CreatedBy AS SubjectTeacher,
    TN.TermNumber + ' (' + F.FinancialYear + ')' AS Term,
    ac.ScoreCount
FROM
    AssessmentScores Sc
INNER JOIN
    Assessment A ON Sc.AssessmentId = A.AssessmentId
INNER JOIN
    Enrollment E ON sc.StudentId = E.EnrollmentId
INNER JOIN
    Student St ON E.StudentId = St.StudentId
INNER JOIN
    SubjectAllocation Sa ON A.SubjectId = Sa.AllocationId
INNER JOIN
    Users u ON sa.TeacherId = u.UserId
INNER JOIN
    Subject S ON Sa.SubjectId = S.SubjectId
INNER JOIN
    Term T ON Sa.TermId = T.TermId
INNER JOIN
    TermNumber TN ON T.Term = TN.TermId
INNER JOIN
    FinancialYear F ON T.YearId = F.FinancialYearId
INNER JOIN
    Class C ON Sa.ClassId = C.ClassId
INNER JOIN
    GradingSystem Gs ON Sc.Score >= Gs.LowerScale AND Sc.Score <= Gs.UpperScale
    AND (Gs.ScaleTypeId = C.ScaleTypeId AND Sc.SchoolId = Gs.SchoolId)
INNER JOIN
    (SELECT AssessmentId, COUNT(*) as ScoreCount
     FROM AssessmentScores
     GROUP BY AssessmentId) ac
ON Sc.AssessmentId = ac.AssessmentId
WHERE
     Sc.SchoolId = @SchoolId 
    AND T.Status = 2 
ORDER BY
    Sc.AssessmentId, Sc.Score DESC";

                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    cmd.Parameters.AddWithValue("@UserName", Session["Username"]);
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        scores.Add(new Score
                        {
                            ScoreId = dr["ScoreId"].ToString(),
                            AssessmentTitle = dr["AssessmentTitle"].ToString(),
                            Student = dr["Student"].ToString(),
                            SubjectName = dr["SubjectName"].ToString(),
                            ExamScore = dr["Score"].ToString(),
                            Contribution = dr["Contribution"].ToString(),
                            Grade = dr["Grade"].ToString(),
                            ScoreRank = dr["ScoreRank"].ToString(),
                            ScoreCount = dr["ScoreCount"].ToString(),
                            GradeDescription = dr["GradeDescription"].ToString(),
                            Remark = dr["Remark"].ToString(),
                            SubjectTeacher = dr["SubjectTeacher"].ToString(),
                            Term = dr["Term"].ToString()
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
            return scores;
        }

        public class Score
        {
            public string ScoreId { get; set; }
            public string ScoreRank { get; set; }
            public string ScoreCount { get; set; }
            public string AssessmentTitle { get; set; }
            public string Contribution { get; set; }
            public string Student { get; set; }
            public string SubjectName { get; set; }
            public string ExamScore { get; set; }
            public string Grade { get; set; }
            public string GradeDescription { get; set; }
            public string Remark { get; set; }
            public string SubjectTeacher { get; set; }
            public string Term { get; set; }
        }

        private void BindScoresRepeater()
        {
            List<Score> scores = GetScoresList();
            ScoresRepeater.DataSource = scores;
            ScoresRepeater.DataBind();
        }
    }
}
