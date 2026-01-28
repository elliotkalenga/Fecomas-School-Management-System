using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class SchoolReports : System.Web.UI.Page
    {
        protected List<string> subjectsWithData = new List<string>(); // List to store unique subject codes from DB

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                BindStudentsRepeater();
            }
        }



        private List<exams> GetStudentsList()
        {
            List<exams> exams = new List<exams>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"SELECT DISTINCT
    St.FirstName + ' ' + St.LastName AS Student,
    TN.TermNumber + ' (' + F.FinancialYear + ')' AS Term,
    Sch.SchoolCode,
    ET.ExamType,
    St.StudentNo,
    C.ClassSection,
    C.ClassName,
    C.ClassId,
    C.ScaleDescription,
    En.TermId,
    E.ExamCode,
    E.ExamId,
    E.ExamCode + ' (' + E.ExamTitle + ')' AS Exam,
    E.ReleasedStatus AS ReleaseStatus
FROM 
    Score AS Sc WITH (NOLOCK)
    INNER JOIN Exam AS E WITH (NOLOCK) ON Sc.ExamId = E.ExamId 
    INNER JOIN Enrollment En WITH (NOLOCK) ON En.StudentId = Sc.StudentId AND En.TermId = E.TermId
    INNER JOIN SubjectAllocation AS Sa WITH (NOLOCK) ON Sc.SubjectId = Sa.AllocationId
    INNER JOIN Subject AS S WITH (NOLOCK) ON Sa.SubjectId = S.SubjectId
    INNER JOIN Users AS U WITH (NOLOCK) ON Sa.TeacherId = U.UserId
    INNER JOIN Class AS C WITH (NOLOCK) ON Sa.ClassId = C.ClassId
    INNER JOIN Term AS T WITH (NOLOCK) ON E.TermId = T.TermId 
    INNER JOIN TermNumber AS TN WITH (NOLOCK) ON T.Term = TN.TermId
    INNER JOIN FinancialYear AS F WITH (NOLOCK) ON T.YearId = F.FinancialYearId
    INNER JOIN Student AS St WITH (NOLOCK) ON St.StudentId = En.StudentId 
    INNER JOIN School AS Sch WITH (NOLOCK) ON Sc.SchoolId = Sch.SchoolId
    INNER JOIN ExamType AS ET WITH (NOLOCK) ON E.ExamTypeId = ET.ExamTypeId
    INNER JOIN ParentSchool AS PS WITH (NOLOCK) ON Sch.ParentSchoolId = PS.ParentSchoolId
    INNER JOIN Logo AS Lg WITH (NOLOCK) ON Sch.LogoId = Lg.Id
    INNER JOIN GradingSystem AS gs WITH (NOLOCK) 
        ON Sc.Score >= gs.LowerScale 
        AND Sc.Score <= gs.UpperScale 
        AND gs.ScaleTypeId = C.ScaleTypeId 
        AND Sc.SchoolId = gs.SchoolId
WHERE Sch.SchoolCode = @SchoolCode
ORDER BY ExamId DESC;


";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DateTime CreatedDate;

                    exams.Add(new exams
                    {
                        ExamId = Convert.ToInt32(dr["Examid"]),
                        TermId = Convert.ToInt32(dr["Termid"]),
                        ClassId = Convert.ToInt32(dr["Classid"]),
                        StudentNo = dr["StudentNo"].ToString(),
                        Student = dr["Student"].ToString(),
                        ClassName = dr["ClassName"].ToString(),
                        Term = dr["Term"].ToString(),
                        Exam = dr["Exam"].ToString(),
                        ExamCode = dr["ExamCode"].ToString(),
                    });
                }
                dr.Close();
            }
            return exams;
        }



        public class exams
        {
            public int ExamId { get; set; }
            public int ClassId { get; set; }
            public int TermId { get; set; }
            public string ExamCode { get; set; }
            public string Exam { get; set; }
            public string ClassName { get; set; }
            public string StudentNo { get; set; }
            public string Student { get; set; }
            public string Result { get; set; }
            public string ResultStatus { get; set; }
            public string Term { get; set; }
            public string Position { get; set; }
            public int SchoolId { get; set; }
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
