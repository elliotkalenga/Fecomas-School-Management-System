using SMSWEBAPP.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class StudentAttendance : System.Web.UI.Page
    {
        private const string USER_SESSION_KEY = "User";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[USER_SESSION_KEY] == null)
            {
                // Redirect to login page if user is not logged in
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                txtBarcode.Focus();

                // Bind records or other initial setups
            }
        }

        protected void SubmitBarcode(object sender, EventArgs e)
        {
            string barcode = txtBarcode.Text.Trim();
            if (!string.IsNullOrEmpty(barcode))
            {
                MarkAttendance(barcode);
            }
        }

        private void MarkAttendance(string barcode)
        {
            // Ensure valid barcode input
            if (string.IsNullOrEmpty(barcode))
            {
                lblMessage.Text = "Please scan a valid barcode.";
                txtBarcode.Text = "";
                txtBarcode.Focus();
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Visible = true;
                return;
            }

            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                con.Open();

                string studentName = "";

                // Fetch student name first
                string nameQuery = @"
            SELECT TOP 1 Student 
            FROM StudentAttendance 
            WHERE StudentBarcode = @StudentBarcode 
                AND SchoolId = @SchoolId 
                AND AttendanceDate = CAST(GETDATE() AS DATE)";

                using (SqlCommand nameCmd = new SqlCommand(nameQuery, con))
                {
                    nameCmd.Parameters.AddWithValue("@StudentBarcode", barcode);
                    nameCmd.Parameters.Add("@SchoolId", SqlDbType.Int).Value = Session["SchoolId"] != null ? Convert.ToInt32(Session["SchoolId"]) : (object)DBNull.Value;

                    object result = nameCmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        studentName = result.ToString();
                    }
                    else
                    {
                        studentName = barcode; // fallback if student name is not found
                    }
                }

                // Check if attendance is already marked
                string checkQuery = @"
            SELECT COUNT(*) 
            FROM StudentAttendance 
            WHERE StudentBarcode = @StudentBarcode 
                AND SchoolId = @SchoolId
                AND AttendanceDate = CAST(GETDATE() AS DATE) 
                AND Clockin IS NOT NULL";

                using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@StudentBarcode", barcode);
                    checkCmd.Parameters.Add("@SchoolId", SqlDbType.Int).Value = Session["SchoolId"] != null ? Convert.ToInt32(Session["SchoolId"]) : (object)DBNull.Value;

                    int studentExists = (int)checkCmd.ExecuteScalar();

                    if (studentExists > 0)
                    {
                        lblMessage.Text = $"Attendance for {studentName} has already been marked for today!";
                        txtBarcode.Text = "";
                        txtBarcode.Focus();
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        lblMessage.Visible = true;
                        return;
                    }
                }

                // Mark attendance
                string updateQuery = @"
            UPDATE StudentAttendance
            SET Status = 'P', Clockin = GETDATE(), AttendanceWeek = @AttendanceWeek,
                UpdatedBy = @UpdatedBy
            WHERE StudentBarcode = @StudentBarcode 
                AND SchoolId = @SchoolId
                AND AttendanceDate = CAST(GETDATE() AS DATE)";

                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@StudentBarcode", barcode);
                    cmd.Parameters.AddWithValue("@AttendanceWeek", ddlWeek.SelectedValue);
                    cmd.Parameters.Add("@SchoolId", SqlDbType.Int).Value = Session["SchoolId"] != null ? Convert.ToInt32(Session["SchoolId"]) : (object)DBNull.Value;
                    cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = Session["Username"] != null ? Session["Username"].ToString() : (object)DBNull.Value;

                    int rowsAffected = cmd.ExecuteNonQuery();
                    ddlWeek.Enabled = false;

                    if (rowsAffected > 0)
                    {
                        lblMessage.Visible = true;
                        lblMessage.Text = $"Attendance for {studentName} marked successfully!";
                        lblMessage.ForeColor = System.Drawing.Color.Green;
                        txtBarcode.Text = "";
                        txtBarcode.Focus();
                    }
                    else
                    {
                        lblMessage.Visible = true;
                        lblMessage.Text = "Attendance record not found for today.";
                        txtBarcode.Text = "";
                        txtBarcode.Focus();
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }

                // Update AttendanceWeek for all records (if needed)
                string updateWeekQuery = @"
            UPDATE StudentAttendance
            SET AttendanceWeek = @AttendanceWeek
            WHERE SchoolId = @SchoolId
                AND AttendanceDate = CAST(GETDATE() AS DATE)";

                using (SqlCommand cmd = new SqlCommand(updateWeekQuery, con))
                {
                    cmd.Parameters.AddWithValue("@AttendanceWeek", ddlWeek.SelectedValue);
                    cmd.Parameters.Add("@SchoolId", SqlDbType.Int).Value = Session["SchoolId"] != null ? Convert.ToInt32(Session["SchoolId"]) : (object)DBNull.Value;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
