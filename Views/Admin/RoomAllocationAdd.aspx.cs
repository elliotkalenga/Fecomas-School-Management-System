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
    public partial class RoomAllocationAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                SetButtonText();
                PopulateDropDownLists();

                if (Request.QueryString["AllocationId"] != null)
                {
                    int AllocationId;
                    if (int.TryParse(Request.QueryString["AllocationId"], out AllocationId))
                    {
                        string mode = Request.QueryString["mode"];
                        if (mode == "delete")
                        {
                            DeleteBook(AllocationId);
                        }
                        else
                        {
                            LoadRecordData(AllocationId);
                        }
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            btnSubmit.Text = Request.QueryString["AllocationId"] != null ? "Update" : "Add";
        }

        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string StudentQry = @"SELECT 0 as StudentId,'---- Select Student Details -----' AS  Student, 0 as EnrollmentID UNION Select E.StudentID,S.FirstName + ' ' + S.LastName + ' '+S.StudentNo + ' ('+C.ClassName + ' )' as Student,EnrollmentID from Enrollment E
                                    INNER JOIN Student S on E.StudentId=S.StudentID
                                    INNER JOIN Term T on E.Termid=T.TermId
                                    INNER JOIN Class C on E.ClassId=C.ClassId
                                    INNER JOIN TermNumber TN on T.Term=TN.TermId
                                    INNER JOIN FinancialYear F on T.Yearid=F.FinancialYearid
                                    
                                    Where T.Status=2 and E.SchoolId=@SchoolId order by Student";


                string RoomQry = @"SELECT 0 as RoomId,'---- Select  Class-----' AS  Room
                            UNION Select r.RoomId ,RoomNumber +' (' +h.HostelName+')' as Room
                            from Rooms r inner join Hostels h on r.hostelid=h.HostelId Where r.schoolId=@SchoolId";


                string TermQry = @"Select T.TermId, TN.TermNumber + ' ('+F.FinancialYear + ')' As Term  from Term T
        INNER JOIN TermNumber TN on T.Term=TN.TermId
        INNER JOIN FinancialYear F on T.Yearid=F.FinancialYearid where T.Status=2 and T.SchoolId=@SchoolId";


                Con.Open();

                PopulateDropDownList(Con, StudentQry, ddlStudent, "Student", "StudentId");
                PopulateDropDownList(Con, RoomQry, ddlRoom, "Room", "RoomId");
                PopulateDropDownList(Con, TermQry, ddlTerm, "Term", "TermId");
            }
        }

        private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField)
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    ddl.DataSource = dr;
                    ddl.DataTextField = textField;
                    ddl.DataValueField = valueField;
                    ddl.DataBind();
                }
            }
        }
        private void LoadRecordData(int AllocationId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM RoomAllocations WHERE AllocationId = @AllocationId", Con))
                {
                    cmd.Parameters.AddWithValue("@AllocationId", AllocationId);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows && dr.Read())
                        {
                            txtCondition.Text = dr["Condition"].ToString();
                            ddlRoom.SelectedValue = dr["RoomId"].ToString();
                            ddlStudent.SelectedValue = dr["StudentId"].ToString();
                            ddlTerm.SelectedValue = dr["TermId"].ToString();
                        }
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["AllocationId"] != null)
            {
                int AllocationId;
                if (int.TryParse(Request.QueryString["AllocationId"], out AllocationId))
                {
                    UpdateRecord(AllocationId);
                }
            }
            else
            {
                AddNewRecord();
            }

            ClearControls();
        }

        private void AddNewRecord()
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"INSERT INTO RoomAllocations (RoomId,StudentId,TermId,Condition, CreatedBy, SchoolId) 
                                     VALUES (@RoomId,@StudentId,@TermId,@Condition, @CreatedBy, @SchoolId)";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@Condition", txtCondition.Text.Trim());
                        cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                        cmd.Parameters.AddWithValue("@StudentId", ddlStudent.SelectedValue);
                        cmd.Parameters.AddWithValue("@RoomId", ddlRoom.SelectedValue);
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"] ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Hostel Room Allocated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error adding Allocation. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void UpdateRecord(int AllocationId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"UPDATE RoomAllocations 
                                     SET RoomId = @RoomId, 
                                         Condition = @Condition, 
                                         StudentId = @StudentId 
                                     WHERE AllocationId = @AllocationId";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@Condition", txtCondition.Text.Trim());
                        cmd.Parameters.AddWithValue("@TermId", ddlTerm.SelectedValue);
                        cmd.Parameters.AddWithValue("@StudentId", ddlStudent.SelectedValue);
                        cmd.Parameters.AddWithValue("@RoomId", ddlRoom.SelectedValue);
                        cmd.Parameters.AddWithValue("@AllocationId", AllocationId);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Record updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating Room Allocation. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteBook(int AllocationId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM roomAllocations WHERE AllocationId = @AllocationId", Con))
                {
                    cmd.Parameters.AddWithValue("@AllocationId", AllocationId);
                    cmd.ExecuteNonQuery();
                }
            }
            Response.Redirect("RoomAllocation.aspx?deleteSuccess=true");
        }

        private void ClearControls()
        {
            txtCondition.Text = string.Empty;
            ddlStudent.SelectedIndex = 0;
            ddlTerm.SelectedIndex = 0;
            ddlRoom.SelectedIndex = 0;
        }
    }
}
