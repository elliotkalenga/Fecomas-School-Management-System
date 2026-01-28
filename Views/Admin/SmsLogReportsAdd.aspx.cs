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
    public partial class SmsLogReportsAdd : System.Web.UI.Page
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
                    if (Request.QueryString["SentMonth"] != null)
                    {
                        string SentMonth = Request.QueryString["SentMonth"]; // Get Term from query string

                        if (SentMonth == "delete")
                        {
                            // Handle the delete logic if the exam mode is "delete"
                            // Example: DeleteStudentRecord(StudentNo); 
                        }
                        else
                        {
                            // Load the student data using the StudentNo
                            LoadSMSData(SentMonth); // Pass initialized values

                            // Handle the Exam (mode) - you can load different data based on exam
                            // Add any other "exam" modes you need to handle
                        }
                    }
                }


            }

            private void LoadSMSData(string SentMonth)
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    con.Open();
                    string query = @"select * from Vw_smslog WHere SentMonth = @SentMonth AND SchoolCode = @SchoolCode";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SentMonth", SentMonth);
                        cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows && dr.Read())
                            {
                                txtTerm.Text = SentMonth;
                            }
                        }
                    }
                }
            }








        protected void btnSMSReportDetailed_Click(object sender, EventArgs e)
        {
            string query = @"SELECT       Message, Phone, Student, SentBy, SentDate, SentMonth, SchoolId, Tariff, TotalRecordsPerMonth, TotalTariffPerMonth, SchoolName, Logo, Address, SchoolCode
FROM            Vw_smslog
WHERE        (SchoolCode = @SchoolCode) AND (SentMonth = @SentMonth) order by SentDate Desc";

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Fetch the data
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(query, Con))
                {
                    command.Parameters.AddWithValue("@SentMonth", txtTerm.Text.ToString());
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
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/SmsSentReport.rdlc");

                // Enable external images
                ReportViewer1.LocalReport.EnableExternalImages = true;
                // Add the data source to the report
                ReportDataSource reportDataSource = new ReportDataSource("SmsReport", dataTable);
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



