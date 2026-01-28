using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using SMSWEBAPP.DAL;

namespace SMSWEBAPP.Views.Admin
{
    public partial class EnrollmentReportsAdd : System.Web.UI.Page
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
                // Check if both query string parameters are available
                if (Request.QueryString["Term"] != null)
                {
                    string Term = Request.QueryString["Term"]; // Get Term from query string

                    if (Term == "delete")
                    {
                        // Handle the delete logic if the exam mode is "delete"
                        // Example: DeleteStudentRecord(StudentNo); 
                    }
                    else
                    {
                        // Load the student data using the StudentNo
                        LoadStudentData(Term); // Pass initialized values

                        // Handle the Exam (mode) - you can load different data based on exam
                        // Add any other "exam" modes you need to handle
                    }
                }
            }


        }

        private void LoadStudentData(string Term)
        {
            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                con.Open();
                string query = @"select * from Vw_EnrollmentReport WHere Term = @Term AND SchoolCode = @SchoolCode";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Term", Term);
                    cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows && dr.Read())
                        {
                            txtTerm.Text = Term;
                        }
                    }
                }
            }
        }


        protected void btnEnrollmentDetailed_Click(object sender, EventArgs e)
        {
            string query = @"SELECT StudentNo, Student, ClassName, Term, SchoolCode, SchoolName, Gender, Address, Logo, SchoolID, StudentCount, Guardian, Phone, UserName
                FROM   Vw_EnrollmentReport
                WHERE (Term = @Term) AND (SchoolCode = @SchoolCode)
                ORDER BY Student,Gender";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@Term", txtTerm.Text.ToString());
                    command.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
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
                lblMessage.Text = "No Report found for selected Term!";
                lblMessage3.Text = "PLEASE CONTACT " + LoggedInUser.SchoolName + " FOR MORE ASSISTANCE";
                lblMessage.CssClass = "alert alert-warning";
                lblMessage3.CssClass = "alert alert-warning";
                lblMessage.Visible = true;
                lblMessage3.Visible = true;
                ReportViewer1.Visible = false;
            }
            else
            {
                lblMessage.CssClass = "d-none";
                lblMessage3.CssClass = "d-none";
                lblMessage.Visible = false;
                lblMessage3.Visible = false;
                ReportViewer1.Visible = true;

                // Set the ReportViewer properties
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/EnrollmentReport.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("EnrollmentReport", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                // Set the external images path
                // string imagePath = "file:///C:/Logo/";
                //string imagePath = "file:///C:/SMSWEBAPP/SMSWEBAPP/StudentImages/";
                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                string UserName = Session["Username"] != null ? Session["Username"].ToString() : string.Empty;
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                ReportParameter Username = new ReportParameter("UserName", UserName);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { Username });



                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
            }

        }

        protected void btnREnrollmentSummary_Click(object sender, EventArgs e)
        {
            string query = @"SELECT ClassName, StudentsCount, Term, SchoolCode, SchoolName, Address, Logo, SchoolID
FROM   Vw_EnrollmentSummaryReport
WHERE (Term = @Term) AND (SchoolCode = @SchoolCode)";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@Term", txtTerm.Text.ToString());
                    command.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
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
                lblMessage.Text = "No Report found for selected Term!";
                lblMessage3.Text = "PLEASE CONTACT " + LoggedInUser.SchoolName + " FOR MORE ASSISTANCE";
                lblMessage.CssClass = "alert alert-warning";
                lblMessage3.CssClass = "alert alert-warning";
                lblMessage.Visible = true;
                lblMessage3.Visible = true;
                ReportViewer1.Visible = false;
            }
            else
            {
                lblMessage.CssClass = "d-none";
                lblMessage3.CssClass = "d-none";
                lblMessage.Visible = false;
                lblMessage3.Visible = false;
                ReportViewer1.Visible = true;

                // Set the ReportViewer properties
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/EnrollmentSummaryReport.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("EnrollmentSummaryReport", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                // Set the external images path
                // string imagePath = "file:///C:/Logo/";
                //string imagePath = "file:///C:/SMSWEBAPP/SMSWEBAPP/StudentImages/";
                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                string UserName = Session["Username"] != null ? Session["Username"].ToString() : string.Empty;
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                ReportParameter Username = new ReportParameter("UserName", UserName);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { Username });



                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
            }

        }

        protected void btnAllocationDetailed_Click(object sender, EventArgs e)
        {
            string query = @"SELECT SubjectName, SubjectCode, Teacher, ClassName, Term, SchoolCode, SchoolId, SchoolName, Logo, Address
FROM   Vw_SubjectAllocationReport
WHERE (SchoolCode = @SchoolCode) AND (Term = @Term)
ORDER BY SubjectName";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@Term", txtTerm.Text.ToString());
                    command.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
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
                lblMessage.Text = "No Report found for selected Term!";
                lblMessage3.Text = "PLEASE CONTACT " + LoggedInUser.SchoolName + " FOR MORE ASSISTANCE";
                lblMessage.CssClass = "alert alert-warning";
                lblMessage3.CssClass = "alert alert-warning";
                lblMessage.Visible = true;
                lblMessage3.Visible = true;
                ReportViewer1.Visible = false;
            }
            else
            {
                lblMessage.CssClass = "d-none";
                lblMessage3.CssClass = "d-none";
                lblMessage.Visible = false;
                lblMessage3.Visible = false;
                ReportViewer1.Visible = true;

                // Set the ReportViewer properties
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/SubjectAllocationReport.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("SubjectAllocationReport", dataTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                // Set the external images path
                // string imagePath = "file:///C:/Logo/";
                //string imagePath = "file:///C:/SMSWEBAPP/SMSWEBAPP/StudentImages/";
                string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                string UserName = Session["Username"] != null ? Session["Username"].ToString() : string.Empty;
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                ReportParameter Username = new ReportParameter("UserName", UserName);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { Username });



                // Refresh the report
                ReportViewer1.LocalReport.Refresh();
            }

        }
    }
}



