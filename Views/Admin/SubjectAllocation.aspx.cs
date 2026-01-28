using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class SubjectAllocation : System.Web.UI.Page
    {
        private const string USER_SESSION_KEY = "User";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[USER_SESSION_KEY] == null)
            {
                Response.Redirect("UserLogin.aspx", true); // Immediate redirect
            }

            if (!IsPostBack)
            {
                BindScoresRepeater();
                if (Request.QueryString["AllocationId"] != null && int.TryParse(Request.QueryString["AllocationId"], out int allocationId))
                {
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteAllocation(allocationId);
                    }
                }
            }
        }

        private void DeleteAllocation(int allocationId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM SubjectAllocation WHERE AllocationId = @AllocationId", con))
                    {
                        cmd.Parameters.AddWithValue("@AllocationId", allocationId);
                        cmd.ExecuteNonQuery();
                    }
                }
                Response.Redirect("SubjectAllocation.aspx", true);
            }
            catch (Exception ex)
            {
                Response.Write("An error occurred: " + ex.Message);
            }
        }

        private List<SubjectAllocationModel> GetSubjectAllocationList()
        {
            List<SubjectAllocationModel> subjectAllocations = new List<SubjectAllocationModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string query = @"
                        SELECT 
                            sa.AllocationId, 
                            s.SubjectName,
                            T.FirstName + ' ' + T.LastName + ' (' + 'Teacher' + ')' AS TeacherName,
                            Tn.TermNumber + ' (' + y.FinancialYear + ')' AS Term,
                            C.ClassName, 
                            St.Status, 
                            sa.CreatedBy, 
                            sa.CreatedDate
                        FROM SubjectAllocation sa
                        INNER JOIN Subject s ON sa.SubjectId = s.SubjectId
                        INNER JOIN Users T ON sa.TeacherId = T.UserId
                        INNER JOIN Term tm ON sa.TermId = tm.TermId
                        INNER JOIN TermNumber TN ON tm.Term = TN.TermId
                        INNER JOIN Status St ON tm.Status = St.StatusId
                        INNER JOIN FinancialYear Y ON tm.YearId = y.FinancialYearId
                        INNER JOIN Class C ON sa.ClassId = C.ClassId
                        WHERE tm.Status = 2 AND sa.SchoolId = @SchoolId
                        ORDER BY s.SubjectName";

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                subjectAllocations.Add(new SubjectAllocationModel
                                {
                                    AllocationId = Convert.ToInt32(dr["AllocationId"]),
                                    SubjectName = dr["SubjectName"].ToString(),
                                    TeacherName = dr["TeacherName"].ToString(),
                                    Term = dr["Term"].ToString(),
                                    ClassName = dr["ClassName"].ToString(),
                                    Status = dr["Status"].ToString(),
                                    CreatedBy = dr["CreatedBy"].ToString(),
                                    CreatedDate = Convert.ToDateTime(dr["CreatedDate"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("An error occurred: " + ex.Message);
            }
            return subjectAllocations;
        }

        public class SubjectAllocationModel
        {
            public int AllocationId { get; set; }
            public string SubjectName { get; set; }
            public string TeacherName { get; set; }
            public string Term { get; set; }
            public string ClassName { get; set; }
            public string Status { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        private void BindScoresRepeater()
        {
            try
            {
                List<SubjectAllocationModel> subjectAllocations = GetSubjectAllocationList();
                ScoresRepeater.DataSource = subjectAllocations;
                ScoresRepeater.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("Data binding error: " + ex.Message);
            }

            if (Request.QueryString["mode"] == "report")
            {
                List<SubjectAllocationModel> subjectAllocations = GetSubjectAllocationList();
                if (subjectAllocations.Count > 0)
                {
                    string term = subjectAllocations[0].Term; // Get the first term
                    Response.Redirect($"SubjectAllocationReport.aspx?Term={term}");
                }
            }

        }
    }
}
