using Microsoft.Reporting.WebForms;
using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class Visitors : System.Web.UI.Page
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


                string query = @" SELECT [Id]
      ,[IPAddress]
      ,[Country]
      ,[City]
      ,[Region]
      ,[DeviceType]
      ,VisitTime
FROM [dbo].[VisitorLogs]
ORDER BY id DESC";

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
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/WebsiteVisitors.rdlc");

                    // Enable external images
                    ReportViewer1.LocalReport.EnableExternalImages = true;
                    // Add the data source to the report
                    ReportDataSource reportDataSource = new ReportDataSource("WebsiteVisitors", dataTable);
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


            

            private void LoadStudentData(string Term)
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    con.Open();
                    string query = @"select * from Vw_FeesCollectionSummary WHere Term = @Term AND SchoolCode = @SchoolCode";
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

            protected void btnSubmit_Click(object sender, EventArgs e)
            {
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.Refresh();

                // Retrieve the selected exam and student number
                string Term = txtTerm.Text;

                if (string.IsNullOrEmpty(Term))
                {
                    // If either the exam or student number is not selected or empty, show a message
                    lblMessage.Text = "Please select Term to generate Report";
                    lblMessage.CssClass = "alert alert-warning";
                    lblMessage.Visible = true;
                    lblMessage2.Visible = false;
                    lblMessage3.Visible = false;
                    ReportViewer1.Visible = false;
                }
                else
                {
                    // Call the LoadReport method with both the student number and selected exam
                    LoadReport(Term);
                }
            }
            protected void btnGenerateReport_Click(object sender, EventArgs e)
            {
                LoadCollectionPerFeesName();
            }

            private void LoadReport(string Term)
            {
                Term = txtTerm.Text.ToString();
                // Define the query
                string query = @"SELECT ClassName, SchoolName, ExpectedToCollect, Collected, Balances, SchoolCode, Address, ParentSchoolID, Logo, Term, CollectionPercentage
                            FROM   VW_FeesCollectionSummary
                            WHERE (SchoolCode = @SchoolCode) AND (Term = @Term)";

                // Create a DataTable to hold the data
                DataTable dataTable = new DataTable();

                // Fetch the data
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    using (SqlCommand command = new SqlCommand(query, Con))
                    {
                        command.Parameters.AddWithValue("@Term", Term);
                        command.Parameters.AddWithValue("@SchoolCOde", Session["SchoolCode"]);
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
                    lblMessage.Text = "No Report found for selected Term";
                    lblMessage3.Text = "PLEASE CONTACT Systems Administrator";
                    lblMessage.CssClass = "alert alert-warning";
                    lblMessage3.CssClass = "alert alert-warning";
                    lblMessage.Visible = true;
                    lblMessage2.Visible = true;
                    lblMessage3.Visible = true;
                    ReportViewer1.Visible = false;
                }
                else
                {
                    lblMessage.CssClass = "d-none";
                    lblMessage3.CssClass = "d-none";
                    lblMessage.Visible = false;
                    lblMessage2.Visible = false;
                    lblMessage3.Visible = false;
                    ReportViewer1.Visible = true;

                    // Set the ReportViewer properties
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/FeesCollectionSummary.rdlc");

                    // Enable external images
                    ReportViewer1.LocalReport.EnableExternalImages = true;

                    // Add the data source to the report
                    ReportDataSource reportDataSource = new ReportDataSource("FeesCollectionSummary", dataTable);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(reportDataSource);

                    //  string imagePath = "file:///C:/Logo/";
                    //string imagePath2 = "file:///C:/SMSWEBAPP/SMSWEBAPP/StudentImages/";
                    string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                    ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                    ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });

                    // Refresh the report
                    ReportViewer1.LocalReport.Refresh();
                }
            }


            private void LoadCollectionPerFeesName()
            {
                // Define the query
                string query = @"SELECT Feesname, ClassName, SchoolName, ExpectedToCollect, Collected, Balances, SchoolCode, Address, ParentSchoolID, Term, Logo, CollectionPercentage
FROM   VW_FeesCollectionSummaryPerFeesName
WHERE (Term = @Term) AND (SchoolCode = @SchoolCode)";

                // Create a DataTable to hold the data
                DataTable dataTable = new DataTable();

                // Fetch the data
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    using (SqlCommand command = new SqlCommand(query, Con))
                    {
                        command.Parameters.AddWithValue("@Term", txtTerm.Text.ToString());
                        command.Parameters.AddWithValue("@SchoolCOde", Session["SchoolCode"]);
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
                    lblMessage.Text = "No Report found for selected Term!";
                    lblMessage2.Text = "";
                    lblMessage3.Text = "PLEASE CONTACT " + LoggedInUser.SchoolName + " FOR Assistance";
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
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/FeesCollectionSummaryPerFeesName.rdlc");

                    // Enable external images
                    ReportViewer1.LocalReport.EnableExternalImages = true;
                    // Add the data source to the report
                    ReportDataSource reportDataSource = new ReportDataSource("FeesCollectionSummaryPerFeesName", dataTable);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    // Set the external images path
                    // string imagePath = "file:///C:/Logo/";
                    string imagePath = "file:///C:/inetpub/wwwroot/SMSWEBAPP/StudentImages/";
                string UserName = Session["Username"] != null ? Session["Username"].ToString() : string.Empty;
                ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                    ReportParameter Username = new ReportParameter("UserName", UserName);
                    ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { parameter });
                    ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { Username });

                    // Refresh the report
                    ReportViewer1.LocalReport.Refresh();
                }
            }

            protected void btnResultsheet_Click(object sender, EventArgs e)
            {

                // Define the query
                string query = @"SELECT Id, IPAddress, Country, City, Region, DeviceType, VisitTime
FROM   VisitorLogs
ORDER BY Id DESC";

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
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/WebsiteVisitors.rdlc");

                    // Enable external images
                    ReportViewer1.LocalReport.EnableExternalImages = true;
                    // Add the data source to the report
                    ReportDataSource reportDataSource = new ReportDataSource("WebsiteVisitors", dataTable);
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



