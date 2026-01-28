using SMSWEBAPP.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class SubjectAdd : Page
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

                if (Request.QueryString["SubjectID"] != null)
                {
                    int SubjectID = int.Parse(Request.QueryString["SubjectID"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteSubject(SubjectID);
                    }
                    else
                    {
                        LoadSubjectData(SubjectID);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            btnSubmit.Text = Request.QueryString["SubjectID"] != null ? "Update" : "Add";
        }

        private void DeleteSubject(int SubjectID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Subject WHERE SubjectID = @SubjectID", Con);
                cmd.Parameters.AddWithValue("@SubjectID", SubjectID);
                cmd.ExecuteNonQuery();
                Response.Redirect("Subject.aspx?deleteSuccess=true");
            }
        }

        private void LoadSubjectData(int SubjectID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Subject WHERE SubjectID = @SubjectID", Con);
                cmd.Parameters.AddWithValue("@SubjectID", SubjectID);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    txtSubjectCode.Text = dr["SubjectCode"].ToString();
                    txtSubjectName.Text = dr["SubjectName"].ToString();
                    txtDescription.Text = dr["Description"].ToString();
                }
                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["SubjectID"] != null)
            {
                int SubjectID = int.Parse(Request.QueryString["SubjectID"]);
                UpdateSubject(SubjectID);
            }
            else
            {
                AddNewSubject();
            }
            ClearControls();
        }

        private void ClearControls()
        {
            txtSubjectCode.Text = string.Empty;
            txtSubjectName.Text = string.Empty;
            txtDescription.Text = string.Empty;
        }

        private void AddNewSubject()
        {

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                try
                {
                    string query = @"INSERT INTO Subject
                                     (SubjectCode, SubjectName, Description, CreatedBy, SchoolId) 
                                     VALUES (@SubjectCode, @SubjectName, @Description, @CreatedBy, @SchoolId)";

                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@SubjectCode", txtSubjectCode.Text.ToString());
                    cmd.Parameters.AddWithValue("@SubjectName", txtSubjectName.Text.ToString());
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.ToString());
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                    Con.Open();
                    cmd.ExecuteNonQuery();

                    lblMessage.Text = "Subject saved successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);

                    // Clear fields after submission
                    ClearControls();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627) // Unique constraint error
                    {
                        lblErrorMessage.Text = "Duplicate entry detected. Please check the information and try again.";
                    }
                    else
                    {
                        lblErrorMessage.Text = "An error occurred while saving the subject. Please try again later.";
                    }
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }
        }

        private void UpdateSubject(int SubjectID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"UPDATE Subject SET
                                 SubjectCode = @SubjectCode,
                                 SubjectName = @SubjectName,
                                 Description = @Description
                                 WHERE SubjectID = @SubjectID";

                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@SubjectCode", txtSubjectCode.Text.ToString());
                cmd.Parameters.AddWithValue("@SubjectName", txtSubjectName.Text.ToString());
                cmd.Parameters.AddWithValue("@Description", txtDescription.Text.ToString());
                cmd.Parameters.AddWithValue("@SubjectID", SubjectID);

                Con.Open();
                cmd.ExecuteNonQuery();

                ClearControls();
                SetButtonText();
                lblMessage.Text = "Subject updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);

            }
        }
    }
}
