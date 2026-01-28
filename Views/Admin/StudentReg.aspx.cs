using SMSWEBAPP.DAL;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class StudentReg : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            { 
                PopulateDropDownLists();

            }
        }

        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                PopulateDropDownList(Con, "SELECT GenderId, Name FROM Gender", ddlGender, "Name", "GenderId");
                PopulateDropDownList(Con, "SELECT StatusId, Status FROM Status", ddlStatus, "Status", "StatusId");
            }
        }

        private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField)
        {
            ddlGender.SelectedIndex = -1;
            ddlStatus.SelectedIndex = -1;

            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            ddl.DataSource = dr;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            dr.Close();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string username = txtUserName.Text.Trim();
            string fileName = "";
            string filePath = "";

            // Save the data to the database first
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                string query = "INSERT INTO Student (FirstName, LastName, Gender, Status, School, Guardian, Phone, Address, Email, UserName, Password, SchoolCode) " +
                               "VALUES (@FirstName, @LastName, @Gender, @Status, @School, @Guardian, @Phone, @Address, @Email, @UserName, @Password, @SchoolCode)";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                cmd.Parameters.AddWithValue("@Gender", ddlGender.SelectedValue);
                cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@School", Session["SchoolId"]); // Assuming a session or static property
                cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);
                cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                cmd.Parameters.AddWithValue("@Guardian", txtGuardian.Text);
                cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@UserName", username);
                cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                cmd.ExecuteNonQuery();
            }

            // Handle uploaded image
            if (fuImage.HasFile)
            {
                string extension = Path.GetExtension(fuImage.PostedFile.FileName);
                fileName = username + extension;
                filePath = Server.MapPath("~/StudentImages/") + fileName;
                fuImage.PostedFile.SaveAs(filePath);
            }

            // Handle captured image (if any)
            if (!string.IsNullOrEmpty(Request.Form["capturedImage"]))
            {
                try
                {
                    string base64String = Request.Form["capturedImage"];
                    if (base64String.Contains(","))
                    {
                        base64String = base64String.Split(',')[1];
                    }

                    byte[] imageBytes = Convert.FromBase64String(base64String);

                    // Define file name and path
                    fileName = username + ".png";
                    string folderPath = Server.MapPath("~/StudentImages/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath); // Ensure directory exists
                    }

                    filePath = Path.Combine(folderPath, fileName);

                    // Save the image file to disk
                    File.WriteAllBytes(filePath, imageBytes);

                    System.Diagnostics.Debug.WriteLine("Captured Image Saved Successfully: " + filePath);
                }
                catch (FormatException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Invalid Base64 format: " + ex.Message);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error Saving Captured Image: " + ex.Message);
                }
            }

            // Update the database with the image path after saving the image
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

            // Clear form fields after successful registration

            //lblMessage.Text = "Student registered successfully!";
            //lblMessage.Visible = true;


            //string script = "toastr.success('Student registered successfully!');";
            //ScriptManager.RegisterStartupScript(this, GetType(), "showToast", script, true);
        




            // Set the success message
            lblMessage.Text = "Student registered successfully!";

            // Show the modal
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#successModal').modal('show');", true);
        
    ClearFormFields();


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
            txtUserName.Text = "";
            txtPassword.Text = "";
            imgPreview.ImageUrl = "";
            fuImage.Attributes.Clear();
        }

    }
}
