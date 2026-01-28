using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;

namespace SMSWEBAPP.Views.Admin
{
    public partial class BookLocations : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {// Check if the user is logged in
            if (Session["User"] == null)
            {
                // Redirect to login page
                Response.Redirect("~/Views/Admin/UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["ExamId"] != null)
                {
                    int ExamId = int.Parse(Request.QueryString["ExamId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                    }
                    else
                    {
                        BindStudentsRepeater();
                        // Load the student data if needed
                    }
                }


            }
        }




        private List<Locations> GetRecordsList()
        {
            List<Locations> locations = new List<Locations>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                    string ShowData = @"Select * from Location
                                    Where Schoolid=@SchoolId";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DateTime CreatedDate;
                    DateTime.TryParse(dr["CreatedDate"].ToString(), out CreatedDate);

                    locations.Add(new Locations
                    {
                        LocationId = dr["LocationId"].ToString(),
                        Location = dr["Location"].ToString(),
                        Description= dr["Description"].ToString(),
                        CreatedDate = CreatedDate
                    });
                }
                dr.Close();
            }
            return locations;
        }

        public class Locations
        {
            public string LocationId { get; set; }
            public string Location { get; set; }
            public string Description { get; set; }
            public DateTime CreatedDate { get; set; }
            public string CreatedDateString => CreatedDate.ToString("yyyy-MM-dd");
        }

        private void BindStudentsRepeater()
        {
            List<Locations> locations = GetRecordsList();
            StudentsRepeater.DataSource = locations;
            StudentsRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindStudentsRepeater();
        }


    }

}
