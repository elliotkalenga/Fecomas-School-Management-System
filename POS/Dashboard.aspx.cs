using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.POS
{
    public partial class Dashboard : System.Web.UI.Page
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
                LoadExamsinacard();
                LoadActiveTeachersCard();
                LoadActiveEnrolledStudentsCard();
                LoadActiveTerm();
                LoadChartData();
                lblLicenseStatus.Text = Session["LicenseStatus"]?.ToString();
                lblLicensedDays.Text = Session["LicensedDay"]?.ToString();
                lblUsedDays.Text = Session["UsedDays"]?.ToString();
                lblRemainingDays.Text = Session["RemainingDays"]?.ToString();
                lblEndDate.Text = Session["Enddate"] != null ?
                    Convert.ToDateTime(Session["Enddate"]).ToString("d-MMMM-yyyy") : "N/A";

                ChangeLicenseValueColor();
            }
        }

        private void ChangeLicenseValueColor()
        {
            if (Session["LicenseStatus"] != null && Session["LicenseStatus"].ToString().ToUpper() == "EXPIRED")
            {
                lblLicenseStatus.ForeColor = System.Drawing.Color.Red;
                lblLicensedDays.ForeColor = System.Drawing.Color.Red;
                lblUsedDays.ForeColor = System.Drawing.Color.Red;
                lblRemainingDays.ForeColor = System.Drawing.Color.Red;
                lblEndDate.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lblLicenseStatus.ForeColor = System.Drawing.Color.DarkSeaGreen;
                lblLicensedDays.ForeColor = System.Drawing.Color.DarkSeaGreen;
                lblUsedDays.ForeColor = System.Drawing.Color.DarkSeaGreen;
                lblRemainingDays.ForeColor = System.Drawing.Color.DarkSeaGreen;
                lblEndDate.ForeColor = System.Drawing.Color.DarkSeaGreen;
            }
        }

        private void LoadExamsinacard()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(DISTINCT ExamId) AS ExamCount FROM Score WHERE SchoolId=@SchoolId", Con))
                {
                    Con.Open();
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int cardValue = Convert.ToInt32(reader["ExamCount"]);
                            LblTermExams.Text = cardValue.ToString();
                        }
                    }
                }
            }
        }

        private void LoadActiveTeachersCard()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(@"select Count(Distinct TeacherId) as TeacherCount from subjectAllocation sa
Inner join term T on sa.termid=T.TermId where T.Status=2 and sa.schoolid=@SchoolId
								", Con))
                {
                    Con.Open();
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int cardValue = Convert.ToInt32(reader["TeacherCount"]);
                            lblActiveTeachers.Text = cardValue.ToString();
                        }
                    }
                }
            }
        }


        private void LoadActiveEnrolledStudentsCard()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(@"Select Count(StudentId) As StudentCount from Enrollment E inner join Term T on E.TermId=T.TermId
                            where T.Status=2 and E.SchoolId=@SchoolId
								", Con))
                {
                    Con.Open();
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int cardValue = Convert.ToInt32(reader["StudentCount"]);
                            lblEnrolledStudents.Text = cardValue.ToString();
                        }
                    }
                }
            }
        }



        private void LoadActiveTerm()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(@"Select TN.TermNumber+ ' ('+F.FinancialYear+')' As Term from Term T Inner join 
                                                        TermNumber TN on T.Term=TN.TermId Inner Join 
                                                        FinancialYear F on T.YearId=F.FinancialYearId where T.status=2 and T.SchoolId=@SchoolId", Con))
                {
                    Con.Open();
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string cardValue = (reader["Term"]).ToString();
                            LblTermEnrolled.Text = cardValue.ToString();
                            lblExamResults.Text = cardValue.ToString();
                            LblTermTeachers.Text = cardValue.ToString();


                        }
                    }
                }
            }
        }


        private void LoadChartData()
        {

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();

                // Example query for bar chart data
                string barChartQuery = @"Select  Count(*) AS StudentCount,C.ClassName 
from enrollment E inner join Class  C on E.ClassId=C.ClassId 
Inner Join Term T on E.Termid=T.TermId
Where E.SchoolId=@SchoolId and T.Status=2
Group By ClassName";
                SqlCommand command = new SqlCommand(barChartQuery, Con);
                command.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader reader = command.ExecuteReader();
                string labels = "";
                string data = "";

                while (reader.Read())
                {
                    labels += $"'{reader["ClassName"].ToString()}',";
                    data += $"{reader["StudentCount"].ToString()},";
                }

                reader.Close();

                BarChartLabels = labels.TrimEnd(',');
                BarChartData = data.TrimEnd(',');

                // Example query for pie chart data
                string pieChartQuery = @"Select  Count(*) AS StudentCount,C.ClassName 
from enrollment E inner join Class  C on E.ClassId=C.ClassId 
Inner Join Term T on E.Termid=T.TermId
Where E.SchoolId=@SchoolId and T.Status=2
Group By ClassName;
                            ";
                command = new SqlCommand(pieChartQuery, Con);
                command.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                reader = command.ExecuteReader();
                labels = "";
                data = "";

                while (reader.Read())
                {
                    labels += $"'{reader["ClassName"].ToString()}',";
                    data += $"{reader["StudentCount"].ToString()},";
                }

                reader.Close();

                PieChartLabels = labels.TrimEnd(',');
                PieChartData = data.TrimEnd(',');
            }
        }

        public string BarChartLabels { get; set; }
        public string BarChartData { get; set; }
        public string PieChartLabels { get; set; }
        public string PieChartData { get; set; }
        protected void ExamResultsButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("Enrollment.aspx");

        }

        protected void InvoicesButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("Enrollment.aspx");

        }

        protected void PaymentTransactionsButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("SchoolReport.aspx");

        }

        protected void LibraryRecordsButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("SchoolReport.aspx");

        }

        protected void AnnouncementsButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("SchoolReport.aspx");

        }
    }

}