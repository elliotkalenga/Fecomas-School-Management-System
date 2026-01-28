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
	public partial class Candidate : System.Web.UI.Page
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
                    if (Request.QueryString["CandidateID"] != null)
                    {
                        int studentID = int.Parse(Request.QueryString["StudentID"]);
                        string mode = Request.QueryString["mode"];
                        if (mode == "delete")
                        {
                            DeleteRecordData(studentID);
                        }
                        else
                        {
                            BindRecordsRepeater();
                            // Load the student data if needed
                        }
                    }


                }
            }

            private void DeleteRecordData(int studentID)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Candidate WHERE CandidateID = @CandidateID", Con);
                    cmd.Parameters.AddWithValue("@StudentID", studentID);
                    cmd.ExecuteNonQuery();
                }

                // Redirect back to the students page after deletion
                Response.Redirect("Candidate.aspx");
            }



            private List<Candidates> GetRecordsList()
            {
                List<Candidates> candidates = new List<Candidates>();
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand(
                        @"SELECT 
       S.CandidateId, 
       S.CandidateNo, 
       S.FirstName, 
       S.LastName, 
       S.Username, 
       G.Name as Gender, 
       ST.Status, 
       S.Guardian, 
       S.Phone, 
       S.Address
FROM Candidate S
INNER JOIN Status ST on S.Status = ST.StatusId
INNER JOIN Gender G on S.Gender = G.GenderId
INNER JOIN School SL on S.School = SL.SchoolId
WHERE S.School = @SchoolId
ORDER BY S.CandidateId DESC ", Con);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                    candidates.Add(new Candidates
                    {
                            CandidateID = dr["CandidateID"].ToString(),
                            CandidateNo = dr["CandidateNo"].ToString(),
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
                return candidates;
            }

            private void BindRecordsRepeater()
            {
                List<Candidates> candidatess = GetRecordsList();
                RecordsRepeater.DataSource = candidatess;
                RecordsRepeater.DataBind();
            }

            protected void Page_PreRender(object sender, EventArgs e)
            {
                BindRecordsRepeater();
            }

        }

        public class Candidates
        {
            public string CandidateID { get; set; }
            public string CandidateNo { get; set; }
            public string FirstName { get; set; }
            public string Gender { get; set; }
            public string Status { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string Guardian { get; set; }
        }
    }
