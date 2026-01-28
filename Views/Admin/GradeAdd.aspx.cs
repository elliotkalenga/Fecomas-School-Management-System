using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SMSWEBAPP.Views.Admin.Exams;

namespace SMSWEBAPP.Views.Admin
{
    public partial class GradeAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                BindExamDropdown();
                BindSubjectDropdown();
                SetButtonText();

                if (Request.QueryString["ScoreID"] != null)
                {
                    int ScoreID = int.Parse(Request.QueryString["ScoreID"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteScore(ScoreID);
                    }
                    else
                    {
                        LoadScoreData(ScoreID);
                    }
                }
            }
        }
        protected void SetButtonText()
        {
            if (Request.QueryString["ScoreID"] != null)
            {
                btnSubmit.Text = "Update";
            }
            else
            {
                btnSubmit.Text = "Add";
            }
        }
        private void DeleteScore(int ScoreID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Score WHERE ScoreID = @ScoreID", Con);
                cmd.Parameters.AddWithValue("@ScoreID", ScoreID);
                cmd.ExecuteNonQuery();
                Response.Redirect("Scores.aspx?deleteSuccess=true");
            }
        }

        private void LoadScoreData(int ScoreID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Score WHERE ScoreID = @ScoreID", Con);
                cmd.Parameters.AddWithValue("@ScoreID", ScoreID);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    txtScore.Text = dr["Score"].ToString();
                    ddlSubject.SelectedValue = dr["Subjectid"].ToString();
                    ddlExam.SelectedValue = dr["Examid"].ToString();
                    ddlStudent.SelectedValue = dr["Studentid"].ToString();
                }
                dr.Close();
            }
        }

        private void BindExamDropdown()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"SELECT 
    E.ExamId,
    E.ExamTitle + ' (' + E.ExamCode + ')' AS Exam
FROM 
    Exam E
INNER JOIN 
    Term T ON E.TermId = T.TermId
WHERE 
    E.ReleasedStatus = 2 
    AND T.Status = 2 
    AND T.SchoolId=@SchoolId";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                Con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                ddlExam.DataSource = dr;
                ddlExam.DataTextField = "Exam";
                ddlExam.DataValueField = "ExamId";
                ddlExam.DataBind();
                dr.Close();
            }
        }

        private void BindSubjectDropdown()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"
                SELECT sa.AllocationId AS Allocationid, 
                       s.SubjectName + ' ' + ' ' + C.ClassName AS Subject 
                FROM subjectAllocation sa
                INNER JOIN subject s ON sa.subjectid = s.subjectid
                INNER JOIN users T ON sa.Teacherid = T.userid
                INNER JOIN Class C ON sa.ClassId = C.ClassId 
                INNER JOIN Term TM ON sa.TermId = TM.TermId
                WHERE TM.status = 2 
                  AND sa.Schoolid = @schoolId 
                  AND T.username = @Username";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                        cmd.Parameters.AddWithValue("@username", Session["username"]);


                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            ddlSubject.DataSource = dr;
                            ddlSubject.DataTextField = "Subject";
                            ddlSubject.DataValueField = "AllocationId";
                            ddlSubject.DataBind();
                        }
                    }
                }

                ddlSubject.Items.Insert(0, new ListItem("-- Select Subject --", "0"));
            }
            catch (Exception ex)
            {
                // Handle exception (log it, show message to user, etc.)
                // Optionally, you can log the exception details here.
                Console.WriteLine(ex.Message);
            }
        }

        private void BindStudentDropdown()
        {

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"Select Distinct 
    E.StudentId, 
    S.FirstName + ' ' + S.LastName + ' ' + C.ClassName as Student
from 
    Enrollment E
inner Join 
    Student S on E.Studentid = S.StudentId 
inner Join 
    Term T on E.TermId = T.TermId
inner Join 
    SubjectAllocation sa on E.ClassId = sa.ClassId
inner join 
    Class C on E.ClassId = C.ClassId
Where 
    T.Status = 2 
    and E.SchoolId = @SchoolId 
    and sa.TeacherId = @TeacherId 
    and sa.AllocationId=@AllocationId
    ";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                cmd.Parameters.AddWithValue("@TeacherId", Session["UserId"]);
                cmd.Parameters.AddWithValue("@AllocationId", ddlSubject.SelectedValue);

                Con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                ddlStudent.DataSource = dr;
                ddlStudent.DataTextField = "Student";
                ddlStudent.DataValueField = "StudentId";
                ddlStudent.DataBind();
                dr.Close();
            }
            ddlStudent.Items.Insert(0, new ListItem("-- Select Student --", "0"));
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["ScoreID"] != null)
            {
                int ScoreID = int.Parse(Request.QueryString["ScoreID"]);
                UpdateScore(ScoreID);
            }
            else
            {
                AddNewScore();
            }
            ClearControls();


        }

        private void ClearControls()
        {
            ddlExam.SelectedIndex = 0;
            ddlStudent.SelectedIndex = 0;
            txtScore.Text = "";
        }

        private void AddNewScore()
        {
            int examId = int.Parse(ddlExam.SelectedValue);
            int subjectId = int.Parse(ddlSubject.SelectedValue);
            int studentId = int.Parse(ddlStudent.SelectedValue);
            int score;

            if (!Int32.TryParse(txtScore.Text.Trim(), out score))
            {
                lblErrorMessage.Text = "Invalid Score. Please enter a valid number.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return;
            }

            // Validate score range
            if (score < 0 || score > 100)
            {
                lblErrorMessage.Text = "Invalid Score. The score must be between 0 and 100.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                return;
            }


            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                try
                {
                    // Check if the user has selected a value from each dropdown
                    if (ddlSubject.SelectedValue == "0")
                    {
                        lblErrorMessage.Text = "Please select Subject";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                        return;
                    }

                    if (ddlStudent.SelectedValue == "0")
                    {
                        lblErrorMessage.Text = "Please select student";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                        return;
                    }
                    string query = @"INSERT INTO Score
                    (ExamId, StudentId, SubjectId, Score, CreatedBy, SchoolId) 
                    VALUES(@ExamId, @StudentId, @SubjectId, @Score, @CreatedBy, @SchoolId)";

                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@ExamId", examId);
                    cmd.Parameters.AddWithValue("@SubjectId", subjectId);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@Score", score);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                    Con.Open();
                    cmd.ExecuteNonQuery();

                    lblMessage.Text = "Score saved successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);

                    // Clear fields after submission
                    ClearControls();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627) // Unique constraint error
                    {
                        lblErrorMessage.Text = "Duplicate entry detected. Subject Score for the student already assigned.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                    else
                    {
                        lblErrorMessage.Text = "An error occurred while saving the score. Please try again later.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                }
            }
        }

        private void UpdateScore(int ScoreID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"UPDATE Score SET
                                    Score = @Score,
                                    UpdatedTime=@UpdatedTime
                                 WHERE ScoreID = @ScoreID";

                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@Score", txtScore.Text);
                cmd.Parameters.AddWithValue("@UpdatedTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@ModifiedBy", Session["Username"]);
                cmd.Parameters.AddWithValue("@ScoreID", ScoreID);

                Con.Open();
                cmd.ExecuteNonQuery();
                ClearControls();
                SetButtonText();
                lblMessage.Text = "Score Updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);

            }
        }

        protected void ddlExam_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindStudentDropdown();
        }
    }
}
