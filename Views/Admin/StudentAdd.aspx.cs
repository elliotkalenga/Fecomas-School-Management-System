using SMSWEBAPP.DAL;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class StudentAdd : System.Web.UI.Page
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
                PopulateDropDownLists();
                if (Request.QueryString["StudentID"] != null)
                {
                    int studentID = int.Parse(Request.QueryString["StudentID"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteStudentData(studentID);
                    }
                    else
                    {
                        LoadStudentData(studentID);
                        // Load the student data if needed
                    }
                }


            }

        }


        private void DeleteStudentData(int studentID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Student WHERE StudentID = @StudentID", Con);
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                cmd.ExecuteNonQuery();
                // Set a query parameter to indicate successful deletion
                Response.Redirect("Students.aspx?deleteSuccess=true");
            }

            // Redirect back to the students page after deletion
            Response.Redirect("Students.aspx");
        }

        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                PopulateDropDownList(Con, "SELECT 0 as GenderId, '---Select Gender---' as Name Union SELECT GenderId, Name FROM Gender", ddlGender, "Name", "GenderId");
                PopulateDropDownList(Con, "SELECT 0 as StatusId, '---Select Status---' as Status Union SELECT StatusId, Status FROM Status", ddlStatus, "Status", "StatusId");
            }
        }

        private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            ddl.DataSource = dr;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            dr.Close();
        }

        private void LoadStudentData(int studentID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Student WHERE StudentID = @StudentID", Con);
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtFirstName.Text = dr["FirstName"].ToString();
                    txtLastName.Text = dr["LastName"].ToString();
                    ddlGender.SelectedValue = dr["Gender"].ToString();
                    ddlStatus.SelectedValue = dr["Status"].ToString();
                    txtGuardian.Text = dr["Guardian"].ToString();
                    txtPhone.Text = dr["Phone"].ToString();
                    txtAddress.Text = dr["Address"].ToString();
                    txtEmail.Text = dr["Email"].ToString();
                    txtRegCode.Text = dr["RegCode"].ToString();

                    if (dr["DOB"] != DBNull.Value)
                    {
                        txtDob.Text = Convert.ToDateTime(dr["DOB"]).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        txtDob.Text = ""; // Or handle it as you see fit
                    }
                    // Handle other fields if any
                }
                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string username = txtLastName.Text+txtLastName.Text.Trim() + DateTime.UtcNow;
            string fileName = "";
            string filePath = "";


            if (Request.QueryString["StudentID"] != null)
            {
                int studentID = int.Parse(Request.QueryString["StudentID"]);
                UpdateStudentData(studentID);
                ClearFormFields();
            }
            else
            {
                if (DoesUsernameExist(username))
                {
                    ErrorMessage.Text = "Username Already Exists! Please Specify a Different Username.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#ErrorModal').modal('show');", true);
                }
                else
                {
                    AddNewStudent(username, fileName, filePath);
                }
            }
        }

        private void AddNewStudent(string username, string fileName, string filePath)
        {
            // Check if the user has selected a value from the Gender and Status dropdowns
            if (ddlGender.SelectedValue == "0")
            {
                ErrorMessage.Text = "Please select a gender.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#ErrorModal').modal('show');", true);
                return;
            }

            if (ddlStatus.SelectedValue == "0")
            {
                ErrorMessage.Text = "Please select a status.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#ErrorModal').modal('show');", true);

                return;
            }

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                string query = "INSERT INTO Student (Username,RegCode,FirstName, LastName, Gender, Status, School, Guardian, Phone, Address, Email, Password, SchoolCode, CreatedBy, DOB) " +
                               "VALUES (@Username,@RegCode,@FirstName, @LastName, @Gender, @Status, @School, @Guardian, @Phone, @Address, @Email,  @Password, @SchoolCode, @CreatedBy, @DOB)";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                cmd.Parameters.AddWithValue("@RegCode", txtRegCode.Text);
                cmd.Parameters.AddWithValue("@Gender", ddlGender.SelectedValue);
                cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@School", Session["SchoolId"]);
                cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                cmd.Parameters.AddWithValue("@Guardian", txtGuardian.Text);
                cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                string password = new Random().Next(100000, 999999).ToString();
                cmd.Parameters.AddWithValue("@Password", password);
                string UserName = new Random().Next(1000, 9999).ToString();
                cmd.Parameters.AddWithValue("@Username", Session["SchoolCode"]+UserName);
                DateTime dob = string.IsNullOrEmpty(txtDob.Text) ? DateTime.Now : DateTime.Parse(txtDob.Text);
                cmd.Parameters.AddWithValue("@DOB", dob); cmd.ExecuteNonQuery();
            }

            if (fuImage.HasFile)
            {
                string extension = Path.GetExtension(fuImage.PostedFile.FileName);
                fileName = username + extension;
                filePath = Server.MapPath("~/StudentImages/") + fileName;
                fuImage.PostedFile.SaveAs(filePath);
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string updateQuery = "UPDATE Student SET ImagePath = @ImagePath WHERE UserName = @UserName";
                    SqlCommand cmdUpdate = new SqlCommand(updateQuery, Con);
                    cmdUpdate.Parameters.AddWithValue("@ImagePath", fileName);
                    cmdUpdate.Parameters.AddWithValue("@UserName", username);
                    cmdUpdate.ExecuteNonQuery();
                }
            }

            ClearFormFields();
            lblMessage.Text = "Student registered successfully!";
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#successModal').modal('show');", true);
        }

        private void UpdateStudentData(int studentID)
        {
            DateTime dob = string.IsNullOrEmpty(txtDob.Text) ? DateTime.Now : DateTime.Parse(txtDob.Text);

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                string query = @"UPDATE Student 
                         SET RegCode=@RegCode, FirstName=@FirstName, LastName=@LastName, 
                             Gender=@Gender, Status=@Status, Guardian=@Guardian, 
                             Phone=@Phone, Address=@Address, Email=@Email, DOB=@DOB 
                         WHERE StudentID=@StudentID";

                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                cmd.Parameters.AddWithValue("@RegCode", txtRegCode.Text);
                cmd.Parameters.AddWithValue("@Gender", ddlGender.SelectedValue);
                cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@Guardian", txtGuardian.Text);
                cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@DOB", dob);
                cmd.Parameters.AddWithValue("@StudentID", studentID); // <-- add BEFORE ExecuteNonQuery()

                cmd.ExecuteNonQuery(); // <-- execute once
            }

            if (fuImage.HasFile)
            {
                string extension = Path.GetExtension(fuImage.PostedFile.FileName);
                string fileName = txtFirstName.Text + txtLastName.Text + DateTime.UtcNow.Ticks + extension;
                string filePath = Server.MapPath("~/StudentImages/") + fileName;
                fuImage.PostedFile.SaveAs(filePath);

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string updateQuery = "UPDATE Student SET ImagePath = @ImagePath WHERE StudentID = @StudentID";
                    SqlCommand cmdUpdate = new SqlCommand(updateQuery, Con);
                    cmdUpdate.Parameters.AddWithValue("@ImagePath", fileName);
                    cmdUpdate.Parameters.AddWithValue("@StudentID", studentID);
                    cmdUpdate.ExecuteNonQuery();
                }
            }

            ClearFormFields();
            lblMessage.Text = "Student updated successfully!";
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#successModal').modal('show');", true);
        }




        private bool DoesUsernameExist(string username)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                string query = "SELECT COUNT(*) FROM Student WHERE UserName = @UserName";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@UserName", username);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private void ClearFormFields()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            ddlGender.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
            txtGuardian.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
            txtEmail.Text = "";
            fuImage.Attributes.Clear();
        }
    }
}
