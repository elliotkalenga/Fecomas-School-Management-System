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
    public partial class AssessmentScoreAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                ddlExam.Enabled = false;

                BindExamDropdown();
                BindStudentDropdown();

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

                SetButtonText();  // moved here ✅
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
                SqlCommand cmd = new SqlCommand("DELETE FROM AssessmentScores WHERE ScoreID = @ScoreID", Con);
                cmd.Parameters.AddWithValue("@ScoreID", ScoreID);
                cmd.ExecuteNonQuery();
                Response.Redirect("AssessmentScore.aspx?deleteSuccess=true");
            }
        }

        private void LoadScoreData(int ScoreID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM AssessmentScores WHERE ScoreID = @ScoreID", Con);
                cmd.Parameters.AddWithValue("@ScoreID", ScoreID);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    txtScore.Text = dr["Score"].ToString();
                    ddlExam.SelectedValue = dr["Assessmentid"].ToString();
                    ddlStudent.SelectedValue = dr["Studentid"].ToString();
                }
                dr.Close();
            }
        }

        private void BindExamDropdown()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"SELECT  A.AssessmentId,
    A.AssessmentTitle+'-'+C.ClassName+' '+ S.SubjectName+'-'+TN.TermNumber + ' (' + F.FinancialYear + ')'  Assessment
FROM 
    Assessment A
                                    INNER JOIN SubjectAllocation Sa ON A.SubjectId = Sa.AllocationId
									inner join Users u on sa.TeacherId=u.UserId
                                    INNER JOIN Subject S ON Sa.SubjectId = S.SubjectId
                                    INNER JOIN Term T ON Sa.TermId = T.TermId 
                                    INNER JOIN TermNumber TN ON T.Term = TN.TermId
								    INNER JOIN FinancialYear F ON T.YearId = F.FinancialYearId
									Inner Join Class C on sa.ClassId=C.ClassId

WHERE 
    A.Status= 2 
    AND T.Status = 2 
    And U.username=@UserName
    AND T.SchoolId=@SchoolId";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                cmd.Parameters.AddWithValue("@UserName", Session["UserName"]);

                Con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                ddlExam.DataSource = dr;
                ddlExam.DataTextField = "Assessment";
                ddlExam.DataValueField = "AssessmentId";
                ddlExam.DataBind();
                dr.Close();
            }

        }


        private void BindStudentDropdown()
        {

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"
Select Distinct 
    E.EnrollmentId as StudentId, 
    S.FirstName + ' ' + S.LastName + ' ' + C.ClassName as Student
from 
    Enrollment E
inner Join 
    Student S on E.Studentid = S.StudentId 
inner Join 
    Term T on E.TermId = T.TermId
inner Join 
    SubjectAllocation sa on E.ClassId = sa.ClassId
	Inner join users u on sa.Teacherid=u.Userid
	Inner join Assessment a on sa.AllocationId=a.SubjectId
inner join 
    Class C on E.ClassId = C.ClassId
Where 
    T.Status = 2 and u.username=@Username
  
    and a.assessmentStatus=2 and a.Status=2
    ";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@UserName", Session["UserName"]);

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

                    if (ddlStudent.SelectedValue == "0")
                    {
                        lblErrorMessage.Text = "Please select student";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                        return;
                    }
                    string query = @"INSERT INTO AssessmentScores
                    (AssessmentId, StudentId, Score, CreatedBy, SchoolId) 
                    VALUES(@ExamId, @StudentId, @Score, @CreatedBy, @SchoolId)";

                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@ExamId", examId);
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
                        lblErrorMessage.Text = "Duplicate entry detected. Assessment Score for the student already assigned.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                    else
                    {
                        lblErrorMessage.Text = "An error occurred while saving the score. Error details: " + ex.Message;
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                }
            }
        }
  
        private void UpdateScore(int ScoreID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"UPDATE AssessmentScores SET
                                    Score = @Score,
                                    UpdatedTime=@UpdatedTime,
                                    ModifiedBy=@ModifiedBy
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
            BindStudentDropdown();

        }

        protected void ddlSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
