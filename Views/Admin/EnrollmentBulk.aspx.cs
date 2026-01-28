using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using static SMSWEBAPP.Views.Admin.EnrollmentBulk;

namespace SMSWEBAPP.Views.Admin
{
    public partial class EnrollmentBulk : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {// Check if the user is logged in
            if (Session["User"] == null)
            {
                // Redirect to login page
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["EnrollmentID"] != null)
                {
                    int EnrollmentID = int.Parse(Request.QueryString["EnrollmentID"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteStudentData(EnrollmentID);
                    }
                    else
                    {
                        BindStudentsRepeater();
                        // Load the student data if needed
                    }
                }


            }
        }

        private void DeleteStudentData(int EnrollmentID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Enrollment WHERE EnrollmentID = @EnrollmentID", Con);
                cmd.Parameters.AddWithValue("@EnrollmentID", EnrollmentID);
                cmd.ExecuteNonQuery();
            }

            // Redirect back to the students page after deletion
            Response.Redirect("Enrollment.aspx");
        }



        private List<Enrollments> GetStudentsList()
        {
            List<Enrollments> enrollments = new List<Enrollments>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"Select EnrollmentId,S.StudentNo, S.FirstName, S.LastName, 
                            C.ClassName,G.Name as Gender,TN.TermNumber + ' ('+F.FinancialYear + ')' As Term,
                            S.Guardian, S.Phone,E.CreatedDate as DateEnrolled
                            from Enrollment E
                            INNER JOIN Student S on E.StudentId=S.StudentID
                            INNER JOIN Gender G on S.Gender=G.GenderId
                            INNER JOIN Term T on E.Termid=T.TermId
                            INNER JOIN Class C on E.ClassId=C.ClassId
                            INNER JOIN TermNumber TN on T.Term=TN.TermId
                            INNER JOIN FinancialYear F on T.Yearid=F.FinancialYearid 
                            WHERE T.Status=2 AND E.SchoolId=@SchoolId";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DateTime enrolledDate;
                    DateTime.TryParse(dr["DateEnrolled"].ToString(), out enrolledDate);

                    enrollments.Add(new Enrollments
                    {
                        EnrollmentID = dr["EnrollmentID"].ToString(),
                        StudentNo = dr["StudentNo"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        Gender = dr["Gender"].ToString(),
                        Phone = dr["Phone"].ToString(),
                        ClassName = dr["ClassName"].ToString(),
                        Term = dr["Term"].ToString(),
                        Guardian = dr["Guardian"].ToString(),
                        EnrolledDate = enrolledDate
                    });
                }
                dr.Close();
            }
            return enrollments;
        }

        public class Enrollments
        {
            public string EnrollmentID { get; set; }
            public string StudentNo { get; set; }
            public string FirstName { get; set; }
            public string Gender { get; set; }
            public string ClassName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public string Term { get; set; }
            public string Guardian { get; set; }
            public DateTime EnrolledDate { get; set; }
            public string DateEnrolledString => EnrolledDate.ToString("yyyy-MM-dd");
        }

        private void BindStudentsRepeater()
        {
            List<Enrollments> enrollments = GetStudentsList();
            StudentsRepeater.DataSource = enrollments;
            StudentsRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindStudentsRepeater();
        }


    }

    public class enrollments
    {
        public string StudentID { get; set; }
        public string StudentNo { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string ClassName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Term { get; set; }
        public string Guardian { get; set; }
        public DateTime EnrolledDate { get; set; }
    }
}
