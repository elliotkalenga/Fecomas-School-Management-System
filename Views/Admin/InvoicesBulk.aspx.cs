using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class InvoicesBulk : System.Web.UI.Page
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
                if (Request.QueryString["InvoiceID"] != null)
                {
                    int InvoiceID = int.Parse(Request.QueryString["InvoiceID"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteStudentData(InvoiceID);
                    }
                    else
                    {
                        BindStudentsRepeater();
                        // Load the student data if needed
                    }
                }


            }

        }



        private void DeleteStudentData(int InvoiceID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM STudentInvoice WHERE InvoiceID = @InvoiceID", Con);
                cmd.Parameters.AddWithValue("@InvoiceID", InvoiceID);
                cmd.ExecuteNonQuery();
            }

            // Redirect back to the students page after deletion
            Response.Redirect("InvoicesBulk.aspx");
        }



        private List<invoices> GetStudentsList()
        {
            List<invoices> invoices = new List<invoices>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"select EnrollmentId,Status,InvoiceId,InvoiceNo,Student,ClassName,TermNumber+'('+FinancialYear+')' AS Term,
                                FeesName +' '+'('+'MK'+FORMAT(TotalFees,'N0')+')' AS FeesName,
                                'MK'+FORMAT(TotalCollected,'N0') AS TotalCollected,
                                'MK'+FORMAT(Balance,'N0') as OutstandingBalance, 
                                				 				 CASE 
				                 WHEN Balance> 0 And TotalFees>Balance THEN 'PARTLY PAID'
				                 WHEN Balance= 0  THEN 'PAID IN FULL'
				                 WHEN TotalFees=Balance THEN 'NOT PAID'
				                 ELSE 'OVER PAID'
				                 END 
				                 AS InvoiceStatus
				
                                from FeesCollectionSummary 
                                    WHERE SchoolId=@SchoolId and status=2";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    //DateTime enrolledDate;
                    //DateTime.TryParse(dr["DateEnrolled"].ToString(), out enrolledDate);

                    invoices.Add(new invoices
                    {
                        InvoiceID = dr["InvoiceID"].ToString(),
                        InvoiceNo = dr["InvoiceNo"].ToString(),
                        Student = dr["Student"].ToString(),
                        FeesName = dr["FeesName"].ToString(),
                        OutstandingBalance = dr["OutstandingBalance"].ToString(),
                        TotalCollected = dr["TotalCollected"].ToString(),
                        ClassName = dr["ClassName"].ToString(),
                        Term = dr["Term"].ToString(),
                        InvoiceStatus = dr["InvoiceStatus"].ToString(),
                        //EnrolledDate = enrolledDate
                    });
                }
                dr.Close();
            }
            return invoices;
        }

        public class invoices
        {
            public string InvoiceID { get; set; }
            public string InvoiceNo { get; set; }
            public string FeesName { get; set; }
            public string ClassName { get; set; }
            public string Student { get; set; }
            public string TotalCollected { get; set; }
            public string OutstandingBalance { get; set; }
            public string Term { get; set; }
            public string InvoiceStatus { get; set; }
            public DateTime EnrolledDate { get; set; }
            public string DateEnrolledString => EnrolledDate.ToString("yyyy-MM-dd");
        }

        private void BindStudentsRepeater()
        {
            List<invoices> invoices = GetStudentsList();
            StudentsRepeater.DataSource = invoices;
            StudentsRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindStudentsRepeater();
        }



    }
}

