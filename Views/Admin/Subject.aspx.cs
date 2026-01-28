using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class Subject : System.Web.UI.Page
    {
        private const string USER_SESSION_KEY = "User";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[USER_SESSION_KEY] == null)
            {
                // Redirect to login page
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                BindScoresRepeater();
                if (Request.QueryString["SubjectID"] != null)
                {
                    int SubjectId = int.Parse(Request.QueryString["SubjectID"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteScore(SubjectId);
                    }
                    else
                    {

                        // Load the student data if needed
                    }
                }
            }
        }

        private void DeleteScore(int SubjectID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Subject WHERE SubjectID = @SubjectID", con);
                    cmd.Parameters.AddWithValue("@SubjectID", SubjectID);
                    cmd.ExecuteNonQuery();
                }

                Response.Redirect("Subject.aspx");
            }
            catch (Exception ex)
            {
                // Log the exception and handle it gracefully
                // Example: log.Error(ex);
                Response.Write("An error occurred: " + ex.Message);
            }
        }

        private List<Subjects> GetScoresList()
        {
            List<Subjects> subjects = new List<Subjects>();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"SELECT SubjectId, SubjectCode,SubjectName,Description,CreatedDate
                                    FROM Subject 
                                   WHERE SchoolId = @SchoolId  
                                    ORDER BY SubjectName";

                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        subjects.Add(new Subjects
                        {
                            SubjectId = dr["SubjectId"].ToString(),
                            SubjectCode = dr["SubjectCode"].ToString(),
                            SUbjectName = dr["SubjectName"].ToString(),
                            Description = dr["Description"].ToString(),
                            CreatedDate = dr["CreatedDate"].ToString()
                        });
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                // Log the exception and handle it gracefully
                // Example: log.Error(ex);
                Response.Write("An error occurred: " + ex.Message);
            }
            return subjects;
        }

        public class Subjects
        {
            public string SubjectId { get; set; }
            public string SubjectCode { get; set; }
            public string SUbjectName { get; set; }
            public string CreatedDate { get; set; }
            public string Description{ get; set; }
        }

        private void BindScoresRepeater()
        {
            List<Subjects> subjects = GetScoresList();
            ScoresRepeater.DataSource = subjects;
            ScoresRepeater.DataBind();
        }
    }
}
