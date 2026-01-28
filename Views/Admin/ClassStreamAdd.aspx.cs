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
    public partial class ClassStreamAdd : System.Web.UI.Page
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

                if (Request.QueryString["StreamId"] != null)
                {
                    int StreamId = int.Parse(Request.QueryString["StreamId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteRecord(StreamId);
                    }
                    else
                    {
                        LoadRecordData(StreamId);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["StreamId"] != null)
            {
                btnSubmit.Text = "Update";
            }
            else
            {
                btnSubmit.Text = "Add";
            }
        }
        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string ClassQry = @"select 0 as ClassId, '--- Select Class ---' as ClassName UNION 
                                        Select ClassId, ClassName from Class where SchoolId=@SchoolId";


                Con.Open();
                PopulateDropDownList(Con, ClassQry, ddlClassName, "ClassName", "ClassId");
            }
        }

        private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
            SqlDataReader dr = cmd.ExecuteReader();
            ddl.DataSource = dr;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            dr.Close();
        }

        private void LoadRecordData(int StreamId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM ClassStream WHERE StreamId = @StreamId", Con);
                cmd.Parameters.AddWithValue("@StreamId", StreamId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    ddlClassName.SelectedValue = dr["ClassId"].ToString();
                    txtClassStream.Text = dr["StreamName"].ToString();
                    //if (dr["StartDate"] != DBNull.Value)
                    //{
                    //    txtStartDate.Text = Convert.ToDateTime(dr["StartDate"]).ToString("yyyy-MM-dd");
                    //}
                    //else
                    //{
                    //    txtStartDate.Text = ""; // Or handle it as you see fit
                    //}




                }
                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["StreamId"] != null)
            {
                int StreamId = int.Parse(Request.QueryString["StreamId"]);
                UpdateRecord(StreamId);
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
                // Validate input fields
                if (string.IsNullOrWhiteSpace(txtClassStream.Text))
                {
                    lblErrorMessage.Text = "Please enter a Class Stream.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlClassName.SelectedValue == "0")
                {
                    lblErrorMessage.Text = "Please select Class.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }


                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"INSERT INTO ClassStream
                                (ClassId, StreamName,  SchoolId)
                             VALUES
                                (@ClassId, @StreamName,  @SchoolId)";

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@StreamName", txtClassStream.Text.Trim());
                        cmd.Parameters.AddWithValue("@ClassId", ddlClassName.SelectedValue);
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                     //   cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Class Stream added successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                return; // Ensure no further execution

            }
            catch (SqlException ex)
            {
                // Handle unique constraint violation (duplicate entry)
                if (ex.Number == 2627)
                {
                    lblErrorMessage.Text = "A class Stream record with the same name already exists.";
                }
                // Handle error caused by the CHECK constraint violation
                else if (ex.Message.Contains("CHECK constraint"))
                {
                    lblErrorMessage.Text = "Invalid grading system selected. Please choose either 'Average Score' or 'Aggregate Points'.";
                }
                else
                {
                    lblErrorMessage.Text = "Error adding class. Please try again. " + ex.Message;
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
            catch (Exception ex) // Catch non-SQL exceptions
            {
                lblErrorMessage.Text = "An unexpected error occurred: " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void UpdateRecord(int StreamId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"UPDATE ClassStream SET 
                                    StreamName=@StreamName,
                                    ClassId=@ClassId
                                    WHERE StreamId=@StreamId";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@StreamName", txtClassStream.Text.Trim());
                    cmd.Parameters.AddWithValue("@ClassId", ddlClassName.SelectedValue);
                    cmd.Parameters.AddWithValue("@StreamId", StreamId);
                    cmd.ExecuteNonQuery();
                    ClearControls();
                    SetButtonText();
                }
                lblMessage.Text = "Class Stream Record updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating Record. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteRecord(int StreamId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM ClassStream WHERE StreamId = @StreamId", Con);
                cmd.Parameters.AddWithValue("@StreamId", StreamId);
                cmd.ExecuteNonQuery();
                Response.Redirect("ClassStream.aspx?deleteSuccess=true");
            }
        }

        private void ClearControls()
        {
            ddlClassName.SelectedIndex = 0;
            txtClassStream.Text = "";
        }
    }
}
