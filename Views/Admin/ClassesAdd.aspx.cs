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
    public partial class ClassesAdd : System.Web.UI.Page
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

                if (Request.QueryString["ClassId"] != null)
                {
                    int ClassId = int.Parse(Request.QueryString["ClassId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteRecord(ClassId);
                    }
                    else
                    {
                        LoadRecordData(ClassId);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["ClassId"] != null)
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
                string ScaleTypeQry = @"select 0 as ScaletypeId, '--- Select Grading Scale Type ---' as ScaleType UNION 
                                        Select ScaleTypeId, ScaleType from GradingSystemScaleType";

                string SectionQry = @"select 0 as SectionId, '--- Select Class Section ---' as Section UNION 
                                        Select SectionId, Section from ClassSection";


                string GradingSstemqry = @"select '--- Select Grading System ---' as ScaleDescriptionId, '--- Select Grading System ---' as ScaleDescription UNION
                                            select 'Average Score' as ScaleDescriptionId, 'Average Score' as ScaleDescription UNION
                                            select 'Aggregate Points' as ScaleDescriptionId, 'Aggregate Points' as ScaleDescription Union 
                                            select 'Total Marks' as ScaleDescriptionId, 'Total Marks' as ScaleDescription
";

                Con.Open();
                PopulateDropDownList(Con, SectionQry, ddlClassSection, "Section", "SectionId");
                PopulateDropDownList(Con, ScaleTypeQry, ddlScaleType, "ScaleType", "ScaleTypeId");
                PopulateDropDownList(Con, GradingSstemqry, ddlScaleDescription, "ScaleDescription", "ScaleDescriptionId");
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

        private void LoadRecordData(int ClassId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Class WHERE ClassId = @ClassId", Con);
                cmd.Parameters.AddWithValue("@ClassId", ClassId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    ddlScaleType.Text = dr["ScaleTypeId"].ToString();
                    txtClassName.Text = dr["ClassName"].ToString();
                    ddlScaleDescription.SelectedValue = dr["ScaleDescription"].ToString();
                    ddlClassSection.SelectedValue = dr["ClassSection"].ToString();
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
            if (Request.QueryString["ClassId"] != null)
            {
                int ClassId = int.Parse(Request.QueryString["ClassId"]);
                UpdateRecord(ClassId);
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
                if (string.IsNullOrWhiteSpace(txtClassName.Text))
                {
                    lblErrorMessage.Text = "Please enter a Class Name.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlScaleDescription.SelectedValue == "0")
                {
                    lblErrorMessage.Text = "Please select a Grading System.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlScaleType.SelectedValue == "0")
                {
                    lblErrorMessage.Text = "Please select a Grading Scale.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"INSERT INTO Class
                                (ClassName, ScaleTypeId, ScaleDescription, SchoolId, CreatedBy,ClassSection)
                             VALUES
                                (@ClassName, @ScaleTypeId, @ScaleDescription, @SchoolId, @CreatedBy,@ClassSection)";

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@ClassName", txtClassName.Text.Trim());
                        cmd.Parameters.AddWithValue("@ScaleDescription", ddlScaleDescription.SelectedValue);
                        cmd.Parameters.AddWithValue("@ScaleTypeId", ddlScaleType.SelectedValue);
                        cmd.Parameters.AddWithValue("@ClassSection", ddlClassSection.SelectedValue);
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Class added successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                return; // Ensure no further execution

            }
            catch (SqlException ex)
            {
                // Handle unique constraint violation (duplicate entry)
                if (ex.Number == 2627)
                {
                    lblErrorMessage.Text = "A class with the same name already exists.";
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

        private void UpdateRecord(int ClassId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"UPDATE Class SET 
                                    ClassName=@ClassName,
                                    ScaleTypeId=@ScaleType,
                                    ClassSection=@ClassSection,
                                    ScaleDescription=@ScaleDescription
                                    WHERE ClassId=@ClassId";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@ClassName", txtClassName.Text.Trim());
                    cmd.Parameters.AddWithValue("@ScaleType", ddlScaleType.SelectedValue);
                    cmd.Parameters.AddWithValue("@ScaleDescription", ddlScaleDescription.SelectedValue);
                    cmd.Parameters.AddWithValue("@ClassSection", ddlClassSection.SelectedValue);
                    cmd.Parameters.AddWithValue("@ClassId", ClassId);
                    cmd.ExecuteNonQuery();
                    ClearControls();
                    SetButtonText();
                }
                lblMessage.Text = "Class updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating Record. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteRecord(int ClassId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Class WHERE ClassId = @ClassId", Con);
                cmd.Parameters.AddWithValue("@ClassId", ClassId);
                cmd.ExecuteNonQuery();
                Response.Redirect("Classes.aspx?deleteSuccess=true");
            }
        }

        private void ClearControls()
        {
            ddlScaleDescription.SelectedIndex = 0;
            ddlScaleType.SelectedIndex = 0;
            txtClassName.Text = "";
        }
    }
}
