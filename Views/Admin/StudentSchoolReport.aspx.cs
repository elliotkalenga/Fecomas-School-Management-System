using Microsoft.Reporting.WebForms;
using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class StudentSchoolReport : System.Web.UI.Page
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

                SetUserPermissions();
                // Check if both query string parameters are available
                if (Request.QueryString["StudentNo"] != null && Request.QueryString["Exam"] != null)
                {
                    string StudentNo = Request.QueryString["StudentNo"]; // Get StudentNo from query string
                    string exam = Request.QueryString["Exam"]; // Get Exam (mode) from query string

                    // Ensure TermId, ClassId, and ExamId are initialized before use
                    int ExamId = Request.QueryString["ExamId"] != null ? Convert.ToInt32(Request.QueryString["ExamId"]) : 0;
                    int TermId = Request.QueryString["TermId"] != null ? Convert.ToInt32(Request.QueryString["TermId"]) : 0;
                    int ClassId = Request.QueryString["ClassId"] != null ? Convert.ToInt32(Request.QueryString["ClassId"]) : 0;

                    if (exam == "delete")
                    {
                        // Handle the delete logic if the exam mode is "delete"
                        // Example: DeleteStudentRecord(StudentNo); 
                    }
                    else
                    {
                        // Load the student data using the StudentNo
                        LoadStudentData(StudentNo, exam, ExamId, TermId, ClassId); // Pass initialized values


                        // Handle the Exam (mode) - you can load different data based on exam
                        // Add any other "exam" modes you need to handle
                    }
                }
            }


        }

        private void LoadStudentData(string studentNo, string exam, int examId, int termId, int classId)
        {
            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                con.Open();
                string query = @"SELECT DISTINCT
    St.FirstName + ' ' + St.LastName AS Student,
    TN.TermNumber + ' (' + F.FinancialYear + ')' AS Term,
    Sch.SchoolCode,Sch.SchoolName,st.Phone,
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
WHERE St.StudentNo = @StudentNo AND E.ExamCode + ' (' + E.ExamTitle + ')' = @Exam AND E.ExamId = @ExamId AND C.ClassId = @ClassId AND en.TermId = @TermId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentNo", studentNo);
                    cmd.Parameters.AddWithValue("@Exam", exam);
                    cmd.Parameters.AddWithValue("@ExamId", examId);
                    cmd.Parameters.AddWithValue("@ClassId", classId);
                    cmd.Parameters.AddWithValue("@TermId", termId);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows && dr.Read())
                        {
                            txtStudentNo.Text = studentNo;
                            txtExam.Text = exam;
                            TxtExamId.Text = examId.ToString();
                            txtClassId.Text = classId.ToString();
                            txtTermId.Text = termId.ToString();
                            txtSchoolName.Text = dr["SchoolName"].ToString();
                            txtStudentName.Text = dr["Student"].ToString().Replace(" ", string.Empty);
                            txtTerm.Text = dr["Term"].ToString();
                            txtPhone.Text = dr["Phone"].ToString();
                            txtExamName.Text = dr["Exam"].ToString();
                            txtClass.Text = dr["ClassName"].ToString();
                       }
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
           
        }
        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadTranscript();
            SaveReportAsPDF();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);



        }

        protected void BtnContAssessment_Click(object sender, EventArgs e)
        {
            LoadContTranscript();
            SaveReportAsPDF();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);


        }


        private void LoadReport(string exam, string StudentNo)
        {
            exam = txtExam.Text.ToString();
            StudentNo = txtStudentNo.Text.ToString();
            // Define the query
            string query = @"SELECT  Position, ResultStatus, SubjectName, SubjectCode, Score, Grade, StudentNo, Comment, Student, Result, SchoolName, 
                     ParentSchoolID, ParentSchoolName, ParentSchoolCode, SchoolCode, CreatedBy, ExamType, Exam, ClassName, Address, logo, 
                     Term, TermId, Remark, ClassCount, ImagePath, ScaleDescription, ScoreRank, StudentCount
                     FROM Vw_TransCript 
                     WHERE (StudentNo = @StudentNo) AND (Exam = @Exam) order by Score Desc";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@StudentNo", StudentNo);
                    command.Parameters.AddWithValue("@Exam", exam);
                    // Increase the command timeout (e.g., 120 seconds)
                    command.CommandTimeout = 130;
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }

            }

            // Check if the DataTable is empty
            if (dataTable.Rows.Count == 0)
            {
                lblMessage.Text = "No Results found for selected Exam!";
                lblMessage2.Text = "EITHER Results for this Exam has not been uploaded OR " + LoggedInUser.FirstName + " Did not write this Exam";
                lblMessage3.Text = "PLEASE CONTACT " + LoggedInUser.SchoolName + " FOR MORE INFORMATION";
                lblMessage.CssClass = "alert alert-warning";
                lblMessage2.CssClass = "alert alert-warning";
                lblMessage3.CssClass = "alert alert-warning";
                lblMessage.Visible = true;
                lblMessage2.Visible = true;
                lblMessage3.Visible = true;
                ReportViewer1.Visible = false;
            }
            else
            {
                lblMessage.CssClass = "d-none";
                lblMessage2.CssClass = "d-none";
                lblMessage3.CssClass = "d-none";
                lblMessage.Visible = false;
                lblMessage2.Visible = false;
                lblMessage3.Visible = false;
                ReportViewer1.Visible = true;

                // Set the ReportViewer properties
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/SchoolReport.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;

                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("SchoolReportDataset", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);

                // Generate comments based on student performance
                DataRow firstRow = dataTable.Rows[0];
                string studentName = firstRow["Student"].ToString();
                string overallComment = GenerateOverallComment(dataTable, studentName);

                // Pass the overallComment as a parameter
                ReportParameter rpOverallComment = new ReportParameter("OverallComment", overallComment);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpOverallComment });

                // Set the external images path
                string imagePath = "file:///C:/Logo/";
                //string imagePath2 = "file:///C:/SMSWEBAPP/SMSWEBAPP/StudentImages/";
                string imagePath2 = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportParameter parameter2 = new ReportParameter("ImagePath2", imagePath2);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter2 });

                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
                SaveReportAsPDF();
            }
        }


        private void LoadReportFinal(string termId, string StudentNo)
        {
            termId = txtTermId.Text.ToString();
            StudentNo = txtStudentNo.Text.ToString();
            // Define the query
            string query = @"SELECT Position, ResultStatus, SubjectName, SubjectCode, Score, Grade, Comment, StudentNo, Student, Imagepath, Result, SchoolName, SchoolCode, CreatedBy, ExamType, Exam, ExamCode, ClassName, ScaleDescription, Address, logo, Term, TermId, Remark, ClassCount, ReleaseStatus, 
                              ContScore, FinalExamScore, StudentCount, ScoreId,ScoreRank
                            FROM   Vw_TransCript
                            WHERE (StudentNo = @StudentNo) AND (TermId = @TermId) order by Score Desc";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@StudentNo", StudentNo);
                    command.Parameters.AddWithValue("@TermId",termId);
                    // Increase the command timeout (e.g., 120 seconds)
                    command.CommandTimeout = 130;
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }

            // Check if the DataTable is empty
            if (dataTable.Rows.Count == 0)
            {
                lblMessage.Text = "No Results found for selected Exam!";
                lblMessage2.Text = "EITHER Results for this Exam has not been uploaded OR " + LoggedInUser.FirstName + " Did not write this Exam";
                lblMessage3.Text = "PLEASE CONTACT " + LoggedInUser.SchoolName + " FOR MORE INFORMATION";
                lblMessage.CssClass = "alert alert-warning";
                lblMessage2.CssClass = "alert alert-warning";
                lblMessage3.CssClass = "alert alert-warning";
                lblMessage.Visible = true;
                lblMessage2.Visible = true;
                lblMessage3.Visible = true;
                ReportViewer1.Visible = false;
            }
            else
            {
                lblMessage.CssClass = "d-none";
                lblMessage2.CssClass = "d-none";
                lblMessage3.CssClass = "d-none";
                lblMessage.Visible = false;
                lblMessage2.Visible = false;
                lblMessage3.Visible = false;
                ReportViewer1.Visible = true;

                // Set the ReportViewer properties
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/SchoolReportFinal.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;

                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("SchoolReportDatasetFinal", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);

                // Generate comments based on student performance
                DataRow firstRow = dataTable.Rows[0];
                string studentName = firstRow["Student"].ToString();
                string overallComment = GenerateOverallComment(dataTable, studentName);

                // Pass the overallComment as a parameter
                ReportParameter rpOverallComment = new ReportParameter("OverallComment", overallComment);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpOverallComment });

                // Set the external images path
                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                //string imagePath2 = "file:///C:/SMSWEBAPP/SMSWEBAPP/StudentImages/";
                string imagePath2 = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportParameter parameter2 = new ReportParameter("ImagePath2", imagePath2);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter2 });

                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
                SaveReportAsPDF();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);

            }
        }


        private string GenerateOverallComment(DataTable performanceTable, string studentName)
        {
            StringBuilder commentBuilder = new StringBuilder();

            // Subjects where the student needs improvement
            List<string> subjectsToImprove = new List<string>();

            bool isPassed = true;

            foreach (DataRow row in performanceTable.Rows)
            {
                string subject = row["SubjectName"].ToString();
                decimal score = Convert.ToDecimal(row["Score"]);
                string resultStatus = row["ResultStatus"].ToString();

                if (resultStatus == "FAIL")
                {
                    isPassed = false;
                }

                // Assuming a score below 60 is considered needing improvement
                if (score < 60)
                {
                    subjectsToImprove.Add(subject);
                }
            }

            if (!isPassed)
            {
                commentBuilder.Append($"Unfortunately, {studentName}  has failed this exam. Improvement is needed in the following subjects: ");
            }
            else if (subjectsToImprove.Count > 0)
            {
                commentBuilder.Append($"{studentName} has passed this exam. However, extra effort is needed in the following subjects: ");
            }

            if (subjectsToImprove.Count > 0)
            {
                commentBuilder.Append($"{string.Join(", ", subjectsToImprove)}. ");
                commentBuilder.Append("With dedication and the right support, improvement is definitely achievable.");
            }
            else if (isPassed)
            {
                commentBuilder.Append($"{studentName} has passed this exam. ");

                commentBuilder.Append("Keep up the good work and strive for excellence across all subjects.");
            }

            return commentBuilder.ToString();
        }

        private void LoadTranscript()
        {
            // Define the query
            string query = @"SELECT Position, ResultStatus, SubjectName, SubjectCode, Score, Grade, Comment, StudentNo, Student, Imagepath, Result, SchoolName, ParentSchoolID, ParentSchoolName, ParentSchoolCode, SchoolCode, CreatedBy, ExamType, Exam, ExamCode, ClassName, ScaleDescription, Address, 
             logo, Term, TermId, Remark, ClassCount, ReleaseStatus, ScoreRank, StudentCount
FROM   Vw_TransCript
WHERE (StudentNo = @StudentNo) AND (ExamType = 'END OF TERM EXAM') order by Score Desc";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@StudentNo", txtStudentNo.Text.ToString());
                    // Increase the command timeout (e.g., 120 seconds)
                    command.CommandTimeout = 130;
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }

            // Check if the DataTable is empty
            if (dataTable.Rows.Count == 0)
            {
                lblMessage.Text = "No Results found for selected Exam!";
                lblMessage2.Text = "EITHER Results for this Exam has not been uploaded OR " + LoggedInUser.FirstName + " Did not write this Exam";
                lblMessage3.Text = "PLEASE CONTACT " + LoggedInUser.SchoolName + " FOR MORE INFORMATION";
                lblMessage.CssClass = "alert alert-warning";
                lblMessage2.CssClass = "alert alert-warning";
                lblMessage3.CssClass = "alert alert-warning";
                lblMessage.Visible = true;
                lblMessage2.Visible = true;
                lblMessage3.Visible = true;
                ReportViewer1.Visible = false;
            }
            else
            {
                lblMessage.CssClass = "d-none";
                lblMessage2.CssClass = "d-none";
                lblMessage3.CssClass = "d-none";
                lblMessage.Visible = false;
                lblMessage2.Visible = false;
                lblMessage3.Visible = false;
                ReportViewer1.Visible = true;

                // Set the ReportViewer properties
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/Transcript.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("Transcript", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                // Set the external images path
                // string imagePath = "file:///C:/Logo/";
                //string imagePath = "file:///C:/SMSWEBAPP/SMSWEBAPP/StudentImages/";
                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });

                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
                SaveReportAsPDF();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);

            }
        }


        private void LoadContTranscript()
        {
            // Define the query
            string query = @"SELECT Position, ResultStatus, SubjectName, SubjectCode, Score, Grade, Comment, StudentNo, Student, Imagepath, Result, SchoolName, ParentSchoolID, ParentSchoolName, ParentSchoolCode, SchoolCode, CreatedBy, ExamType, Exam, ExamCode, ClassName, ScaleDescription, Address, 
             logo, Term, TermId, Remark, ClassCount, ReleaseStatus, ScoreRank, StudentCount
FROM   Vw_TransCript
WHERE (StudentNo = @StudentNo) order by Score Desc";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@StudentNo", txtStudentNo.Text.ToString());
                    // Increase the command timeout (e.g., 120 seconds)
                    command.CommandTimeout = 130;
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }

            // Check if the DataTable is empty
            if (dataTable.Rows.Count == 0)
            {
                lblMessage.Text = "No Results found for selected Exam!";
                lblMessage2.Text = "EITHER Results for this Exam has not been uploaded OR " + LoggedInUser.FirstName + " Did not write this Exam";
                lblMessage3.Text = "PLEASE CONTACT " + LoggedInUser.SchoolName + " FOR MORE INFORMATION";
                lblMessage.CssClass = "alert alert-warning";
                lblMessage2.CssClass = "alert alert-warning";
                lblMessage3.CssClass = "alert alert-warning";
                lblMessage.Visible = true;
                lblMessage2.Visible = true;
                lblMessage3.Visible = true;
                ReportViewer1.Visible = false;
            }
            else
            {
                lblMessage.CssClass = "d-none";
                lblMessage2.CssClass = "d-none";
                lblMessage3.CssClass = "d-none";
                lblMessage.Visible = false;
                lblMessage2.Visible = false;
                lblMessage3.Visible = false;
                ReportViewer1.Visible = true;

                // Set the ReportViewer properties
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/ContAssessment.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("Transcript", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                // Set the external images path
                // string imagePath = "file:///C:/Logo/";
                //string imagePath = "file:///C:/SMSWEBAPP/SMSWEBAPP/StudentImages/";
                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });

                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
                SaveReportAsPDF();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);

            }
        }

        protected void btnResultsheet_Click(object sender, EventArgs e)
        {
            // Define the query
            string query = @"WITH RankedData AS (
    SELECT *,
           ROW_NUMBER() OVER (PARTITION BY [classId], [ExamId] ORDER BY 
               CASE 
                   WHEN ISNUMERIC([Position]) = 1 THEN 1 -- Numeric values first
                   WHEN [Position] = 'STATEMENT' THEN 2 -- Then 'STATEMENT'
                   ELSE 3 -- Finally other strings
               END,
               CASE 
                   WHEN ISNUMERIC([Position]) = 1 THEN CAST([Position] AS INT) -- Sort numeric values in ascending order
                   ELSE NULL
               END,
               [Position]) AS PositionRank
    FROM ExamAnalysis
)
SELECT * 
FROM RankedData 
WHERE (ExamId=@ExamId) AND (ClassId=@ClassId) AND (TermId=@TermId)
ORDER BY [classId], [ExamId], PositionRank";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@ClassId", txtClassId.Text);
                    command.Parameters.AddWithValue("@ExamId", TxtExamId.Text);
                    command.Parameters.AddWithValue("@TermId", txtTermId.Text);
                    command.CommandTimeout = 130; // Increase timeout

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }

            // Check if the DataTable is empty
            if (dataTable.Rows.Count == 0)
            {
                lblMessage.Text = "No Results found for selected Exam!";
                lblMessage2.Text = "EITHER Results for this Exam has not been uploaded OR " + LoggedInUser.FirstName + " Did not write this Exam";
                lblMessage3.Text = "PLEASE CONTACT " + LoggedInUser.SchoolName + " FOR MORE INFORMATION";
                lblMessage.CssClass = "alert alert-warning";
                lblMessage2.CssClass = "alert alert-warning";
                lblMessage3.CssClass = "alert alert-warning";
                lblMessage.Visible = true;
                lblMessage2.Visible = true;
                lblMessage3.Visible = true;
                ReportViewer1.Visible = false;
            }
            else
            {
                lblMessage.CssClass = "d-none";
                lblMessage2.CssClass = "d-none";
                lblMessage3.CssClass = "d-none";
                lblMessage.Visible = false;
                lblMessage2.Visible = false;
                lblMessage3.Visible = false;
                ReportViewer1.Visible = true;

                // **Remove empty columns**

                List<string> hiddenColumns = new List<string>();

                foreach (DataColumn col in dataTable.Columns)
                {
                    bool isEmpty = dataTable.AsEnumerable().All(row => row.IsNull(col) || string.IsNullOrWhiteSpace(row[col].ToString()));
                    if (isEmpty)
                    {
                        hiddenColumns.Add(col.ColumnName);
                    }
                }

                // Convert list to comma-separated string

                // Set up the ReportViewer
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/ExamResultsheet.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;

                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("ResultSheet", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);

                // Set the external images path
                string hiddenColumnsParam = string.Join(",", hiddenColumns);

                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                ReportParameter hiddenColumnsReportParam = new ReportParameter("HiddenColumns", hiddenColumnsParam);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { hiddenColumnsReportParam });

                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
                SaveReportAsPDF();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideLoadingOverlay", "hideLoadingOverlay();", true);

            }
        }

        protected void btnsummative_Click(object sender, EventArgs e)
        {
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.Refresh();

            // Retrieve the selected exam and student number
            string Exam = txtExam.Text;
            string studentNo = txtStudentNo.Text; // Assuming txtStudentNo is the TextBox holding the student number

            if (string.IsNullOrEmpty(Exam) || string.IsNullOrEmpty(studentNo))
            {
                // If either the exam or student number is not selected or empty, show a message
                lblMessage.Text = "Please select both an exam and a student number.";
                lblMessage.CssClass = "alert alert-warning";
                lblMessage.Visible = true;
                lblMessage2.Visible = false;
                lblMessage3.Visible = false;
                ReportViewer1.Visible = false;
            }
            else
            {
                // Call the LoadReport method with both the student number and selected exam
                LoadReportFinal(studentNo, Exam);
            }
        }

        protected void btnContinuos_Click(object sender, EventArgs e)
        {
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.Refresh();

            // Retrieve the selected exam and student number
            string Exam = txtExam.Text;
            string studentNo = txtStudentNo.Text; // Assuming txtStudentNo is the TextBox holding the student number

            if (string.IsNullOrEmpty(Exam) || string.IsNullOrEmpty(studentNo))
            {
                // If either the exam or student number is not selected or empty, show a message
                lblMessage.Text = "Please select both an exam and a student number.";
                lblMessage.CssClass = "alert alert-warning";
                lblMessage.Visible = true;
                lblMessage2.Visible = false;
                lblMessage3.Visible = false;
                ReportViewer1.Visible = false;
            }
            else
            {
                // Call the LoadReport method with both the student number and selected exam
                LoadReport(studentNo, Exam);
            }
        }


        private void SaveReportAsPDF()
        {
            string fileName = "SchoolReport" + txtStudentName.Text + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
            string folderPath = Server.MapPath("~/Reports/SchoolReports/"); // Local path
            string filePath = Path.Combine(folderPath, fileName);

            byte[] bytes;
            string mimeType, encoding, fileNameExtension;
            string[] streams;
            Microsoft.Reporting.WebForms.Warning[] warnings;

            bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            // Ensure directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.WriteAllBytes(filePath, bytes);

            // Ensure file exists before storing session variable
            if (File.Exists(filePath))
            {
                // Replace "localhost" with your actual domain or VPS IP
                string baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string publicUrl = baseUrl + "/Reports/SchoolReports/" + fileName;

                Session["ReportPDF"] = publicUrl; // Store the publicly accessible URL
            }
            else
            {
                Session["ReportPDF"] = null;
            }
        }



        private void SetUserPermissions()
        {
            if (Session["Permissions"] != null && Session["RoleId"] != null)
            {
                List<string> userPermissions = (List<string>)Session["Permissions"];
                int roleId = Convert.ToInt32(Session["RoleId"]);


                if (userPermissions.Contains("SendWhatsUp_Message") ||
                    userPermissions.Contains("SendWhatsU_Message"))
                {
                    pnlWhatsAppButtons.Visible = true;
                }


            }
        }

    }
}



