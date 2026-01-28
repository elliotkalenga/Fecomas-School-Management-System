using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;

namespace SMSWEBAPP.Views.Admin
{
    public partial class GradingSystem : System.Web.UI.Page
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

        private List<gradingsystem> GetRecordList()
        {
            List<gradingsystem> gradingsystem = new List<gradingsystem>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"select ScaleId, gs.LowerScale,gs.UpperScale,
                                CASE 
                                    WHEN gs.ScaleTypeId = 1 THEN gs.Grade1
                                    WHEN gs.ScaleTypeId = 2 THEN CAST(gs.Grade2 AS NVARCHAR(10))
                                END AS Grade,
                                Description as GradeDescription,
                                Remark as Remarks,
                                gst.ScaleType as GradingSystem
                                from GradingSystem gs
                                Inner Join GradingSystemScaleType gst on gs.ScaletypeId=gst.ScaleTypeId
                                Where SchoolId=@SchoolId order by gst.ScaleType";

                Con.Open();
                SqlCommand cmd = new SqlCommand(ShowData, Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    //DateTime CreatedDate;
                    //DateTime.TryParse(dr["CreatedDate"].ToString(), out CreatedDate);  // Use StartDate from the reader

                    gradingsystem.Add(new gradingsystem
                    {
                        ScaleId = dr["ScaleId"].ToString(),
                        LowerScale= dr["LowerScale"].ToString(),
                        UpperScale = dr["UpperScale"].ToString(),
                        Grade = dr["Grade"].ToString(),
                        GradeDescription = dr["GradeDescription"].ToString(),
                        Remark = dr["Remarks"].ToString(),
                        GradingSystem = dr["Gradingsystem"].ToString(),
                        //CreatedDate = CreatedDate,
                    });
                }
                dr.Close();
            }
            return gradingsystem;  // Return the list of exams
        }

        public class gradingsystem
        {
            public string ScaleId { get; set; }
            public string UpperScale{ get; set; }
            public string LowerScale { get; set; }
            public string Grade{ get; set; }
            public string GradeDescription { get; set; }
            public string Remark { get; set; }
            public string GradingSystem { get; set; }
            //public DateTime CreatedDate { get; set; }  // Renamed to start with an uppercase letter
            //public string CreatedDateString => CreatedDate.ToString("dd-MMMM yyyy");  // Use a string property for formatted date

        }

        private void BindRecordsRepeater()
        {
            List<gradingsystem> gradingsystem = GetRecordList();
            RecordRepeater.DataSource = gradingsystem;
            RecordRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindRecordsRepeater();
        }


    }

}
