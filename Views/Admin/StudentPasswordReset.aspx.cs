using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;

namespace SMSWEBAPP.Views.Admin
{
    public partial class StudentPasswordReset : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            string systemId = Session["SystemId"] as string;

            if (!string.IsNullOrEmpty(systemId))
            {
                switch (systemId)
                {
                    case "1":
                        this.MasterPageFile = "~/Views/Admin/AdminMaster.Master";
                        break;
                    case "2":
                        this.MasterPageFile = "~/POS/AdminMaster.Master";
                        break;
                    case "3":
                        this.MasterPageFile = "~/POS/AdminMaster.Master";
                        break;
                    default:
                        this.MasterPageFile = "~/POS/AdminMaster.Master";
                        break;
                }
            }
            else
            {
                // Optionally redirect to login page if session is missing
                this.MasterPageFile = "~/POS/AdminMaster.Master";
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {// Check if the user is logged in
            if (Session["User"] == null)
            {
                // Redirect to login page
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["StudentID"] != null)
                {
                    int studentID = int.Parse(Request.QueryString["StudentID"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteStudentData(studentID);
                    }
                    else
                    {
                        BindStudentsRepeater();
                        // Load the student data if needed
                    }
                }


            }
        }

        private void DeleteStudentData(int studentID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Student WHERE StudentID = @StudentID", Con);
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                cmd.ExecuteNonQuery();
            }

            // Redirect back to the students page after deletion
            Response.Redirect("Students.aspx");
        }


        private List<Student> SearchStudents(string searchQuery)
        {
            List<Student> students = new List<Student>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand(
                    @"SELECT Top 10
                S.StudentId, 
                S.StudentNo, 
                S.FirstName, 
                S.LastName, 
                G.Name as Gender, 
                ST.Status, 
                S.Guardian, 
                S.Phone, 
                S.Address
            FROM Student S
            INNER JOIN Status ST on S.Status = ST.StatusId
            INNER JOIN Gender G on S.Gender = G.GenderId
            INNER JOIN School SL on S.School = SL.SchoolId
            WHERE S.School = @SchoolId
            AND (
                    S.FirstName LIKE @SearchQuery OR 
                    S.LastName LIKE @SearchQuery OR 
                    S.Guardian LIKE @SearchQuery OR 
                    G.Name LIKE @SearchQuery OR 
                    St.Status LIKE @SearchQuery OR 
                    S.StudentNo LIKE @SearchQuery)
            ORDER BY S.StudentId DESC", Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                cmd.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    students.Add(new Student
                    {
                        StudentID = dr["StudentID"].ToString(),
                        StudentNo = dr["StudentNo"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        Gender = dr["Gender"].ToString(),
                        Phone = dr["Phone"].ToString(),
                        Status = dr["Status"].ToString(),
                        Address = dr["Address"].ToString(),
                        Guardian = dr["Guardian"].ToString()
                    });
                }
                dr.Close();
            }
            return students;
        }

        private List<Student> GetStudentsList()
        {
            List<Student> students = new List<Student>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand(
                    @"SELECT 
       S.StudentId, 
       S.StudentNo, 
       S.FirstName, 
       S.LastName, 
       S.Username, 
       G.Name as Gender, 
       ST.Status, 
       S.Guardian, 
       S.Phone, 
       S.Address
FROM Student S
INNER JOIN Status ST on S.Status = ST.StatusId
INNER JOIN Gender G on S.Gender = G.GenderId
INNER JOIN School SL on S.School = SL.SchoolId
WHERE S.School = @SchoolId
ORDER BY S.StudentId DESC ", Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    students.Add(new Student
                    {
                        StudentID = dr["StudentID"].ToString(),
                        StudentNo = dr["StudentNo"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        Gender = dr["Gender"].ToString(),
                        Phone = dr["Phone"].ToString(),
                        Status = dr["Status"].ToString(),
                        Address = dr["Address"].ToString(),
                        Guardian = dr["Guardian"].ToString()
                    });
                }
                dr.Close();
            }
            return students;
        }

        private void BindStudentsRepeater()
        {
            List<Student> students = GetStudentsList();
            StudentsRepeater.DataSource = students;
            StudentsRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindStudentsRepeater();
        }

        //protected void searchButton_Click(object sender, EventArgs e)
        //{
        //    string searchQuery = searchInput.Text.Trim();
        //    List<Student> students = SearchStudents(searchQuery);
        //    StudentsRepeater.DataSource = students;
        //    StudentsRepeater.DataBind();

        //}
    }

    public class Studentss
    {
        public string StudentID { get; set; }
        public string StudentNo { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string Status { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Guardian { get; set; }
    }
}
