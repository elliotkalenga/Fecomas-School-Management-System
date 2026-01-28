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
    public partial class PeriodRegister : System.Web.UI.Page
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
                if (Request.QueryString["SchemeId"] != null)
                {
                    int SchemeId = int.Parse(Request.QueryString["SchemeId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteStudentData(SchemeId);
                    }
                    else
                    {
                        BindRecordsRepeater();
                        // Load the student data if needed
                    }
                }


            }
        }

        private void DeleteStudentData(int SchemeId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM PeriodRegister WHERE RegisterId = @RegisterId", Con);
                cmd.Parameters.AddWithValue("@SchemeId", SchemeId);
                cmd.ExecuteNonQuery();
            }

            // Redirect back to the students page after deletion
            Response.Redirect("LessonPlan.aspx");
        }


        private List<Register> GetRecordsList()
        {
            List<Register> Registers = new List<Register>();
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ShowData = @"
SELECT 
    p.RegisterId,
    LP.WeekNo + ' '+Tn.TermNumber + ' (' + F.FinancialYear + ') ' +S.SubjectName + ' ' +sc.Topic as Lesson, 
    cs.StreamName+' - '+C.ClassName as Stream ,
    p.PeriodNumber,
    P.DeliveryFrom,
    P.DeliveryTo,
    CONVERT(VARCHAR(5), DATEDIFF(MINUTE, P.DeliveryFrom, P.DeliveryTo) / 60) + ':' + 
    RIGHT('0' + CONVERT(VARCHAR(2), DATEDIFF(MINUTE, P.DeliveryFrom, P.DeliveryTo) % 60), 2) AS Duration,
    u.FirstName +' '+u.Lastname as Teacher,
    P.Attendance,
    P.CreatedBy
FROM 
    PeriodRegister P WITH (NOLOCK)  
INNER JOIN 
    LessonPlan lp  WITH (NOLOCK) On P.LessonPlanId=LP.LessonId
INNER JOIN 
    Schemesofwork sc WITH (NOLOCK) on lp.schemeid=sc.schemeid 
INNER JOIN 
    Subjectallocation sa WITH (NOLOCK) on sc.subjectId = sa.allocationId 
INNER JOIN 
    users u on sa.teacherid=u.userid
INNER JOIN 
    Class c on sa.classId = c.ClassId  
INNER JOIN 
    ClassStream cs on p.StreamId=cS.StreamId
INNER JOIN 
    Subject s WITH (NOLOCK) on sa.subjectid = s.subjectId 
INNER JOIN 
    Term AS T WITH (NOLOCK) ON Sc.TermId = T.TermId
INNER JOIN 
    TermNumber AS TN WITH (NOLOCK) ON T.Term = TN.TermId
INNER JOIN 
    FinancialYear AS F WITH (NOLOCK) ON T.YearId = F.FinancialYearId
WHERE 
    sc.Schoolid =@SchoolId  and T.status = 2";

                Con.Open();
                using (SqlCommand cmd = new SqlCommand(ShowData, Con))
                {
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            DateTime? fromtime = null;
                            DateTime? totime = null;
                            if (dr["DeliveryFrom"] != DBNull.Value)
                            {
                                fromtime = Convert.ToDateTime(dr["DeliveryFrom"]);
                            }
                            if (dr["DeliveryTo"] != DBNull.Value)
                            {
                                totime = Convert.ToDateTime(dr["DeliveryTo"]);
                            }

                            Registers.Add(new Register
                            {
                                RegisterId = dr["RegisterId"].ToString(),
                                LessonPlan = dr["Lesson"].ToString(),
                                duration = dr["Duration"].ToString(),
                                ClassStream = dr["Stream"].ToString(),
                                PeriodNumber = dr["PeriodNumber"].ToString(),
                                Attendance = dr["Attendance"].ToString(),
                                Teacher = dr["Teacher"].ToString(),
                                CreatedBy = dr["CreatedBy"].ToString(),
                                Fromtime = fromtime,
                                Totime = totime
                            });
                        }
                    }
                }
            }
            return Registers;
        }

        protected string GetStatusClass(string status)
        {
            switch (status.ToLower())
            {
                case "new":
                    return "badge badge-success";
                case "good":
                    return "badge badge-info";
                case "fair":
                    return "badge badge-warning";
                case "old":
                    return "badge badge-secondary";
                case "fully depreciated":
                    return "badge badge-danger";
                default:
                    return "badge badge-light";
            }
        }

        protected string GetStatusIcon(string status)
        {
            switch (status.ToLower())
            {
                case "new":
                    return "fas fa-star";
                case "good":
                    return "fas fa-thumbs-up";
                case "fair":
                    return "fas fa-adjust";
                case "old":
                    return "fas fa-hourglass-end";
                case "fully depreciated":
                    return "fas fa-times-circle";
                default:
                    return "";
            }
        }
        public class Register
        {
            public string RegisterId { get; set; }
            public string LessonPlan { get; set; }
            public string ClassStream { get; set; }
            public string Attendance { get; set; }
            public string Teacher { get; set; }
            public string PeriodNumber { get; set; }
            public string CreatedBy { get; set; }
            public string duration { get; set; }
            public DateTime? Fromtime { get; set; }
            public DateTime? Totime { get; set; }
        }

        private void BindRecordsRepeater()
        {
            List<Register> Register = GetRecordsList();
            RecordsRepeater.DataSource = Register;
            RecordsRepeater.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindRecordsRepeater();
        }


    }

}

