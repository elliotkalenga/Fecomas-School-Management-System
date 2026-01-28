using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using static SMSWEBAPP.Views.Admin.Classes;

namespace SMSWEBAPP.Views.Admin
{
    public partial class Classes : System.Web.UI.Page
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

                BindRecordsRepeater();
                // Load the student data if needed
            }
        }

                private List<classes> GetRecordList()
        {
            List<classes> classes = new List<classes>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"select ClassId, ClassName,ST.ScaleType,C.ScaleDescription,C.CreatedBy,C.CreatedDate,CS.Section from Class
                                    C inner Join GradingSystemScaleType ST on C.ScaleTypeId=ST.ScaleTypeId   
Inner Join ClassSection CS on C.ClassSection=CS.SectionId
where C.SchoolId=@SchoolId

                                    
";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DateTime CreatedDate;
                    DateTime.TryParse(dr["CreatedDate"].ToString(), out CreatedDate);  // Use StartDate from the reader

                    classes.Add(new classes
                    {
                        ClassId = dr["ClassId"].ToString(),
                        Section = dr["Section"].ToString(),
                        ClassName = dr["ClassName"].ToString(),
                        ScaleDescription = dr["ScaleDescription"].ToString(),
                        ScaleType = dr["ScaleType"].ToString(),
                        CreatedBy = dr["CreatedBy"].ToString(),
                        CreatedDate = CreatedDate,
                    });
                }
                dr.Close();
            }
            return classes;  // Return the list of exams
        }

        public class classes
        {
            public string ClassId { get; set; }
            public string Section{ get; set; }
            public string ClassName { get; set; }
            public string CreatedBy { get; set; }
            public string ScaleDescription { get; set; }
            public string ScaleType { get; set; }
            public DateTime CreatedDate { get; set; }  // Renamed to start with an uppercase letter
            public string CreatedDateString => CreatedDate.ToString("dd-MMMM yyyy");  // Use a string property for formatted date

        }

        private void BindRecordsRepeater()
        {
            List<classes> classes = GetRecordList();
            RecordRepeater.DataSource = classes;
            RecordRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindRecordsRepeater();
        }


    }

}
