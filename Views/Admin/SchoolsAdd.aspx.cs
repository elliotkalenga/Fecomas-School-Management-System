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
    public partial class SchoolsAdd : System.Web.UI.Page
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

                    if (Request.QueryString["SchoolId"] != null)
                    {
                        int SchoolId = int.Parse(Request.QueryString["SchoolId"]);
                        string mode = Request.QueryString["mode"];
                        if (mode == "delete")
                        {
                            DeleteRecord(SchoolId);
                        }
                        else
                        {
                            LoadRecordData(SchoolId);
                        }
                    }
                }
            }

            protected void SetButtonText()
            {
                if (Request.QueryString["SchoolId"] != null)
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
                    string SchooltyeQry = @"select 0 as SchooltypeId, '--- Select Grading School Type ---' as SchoolType UNION 
                                        Select SchoolTypeId, SchoolType from SchoolType";

                    string Logoqery = @"select 0 as Id,  '--- Select Logo ---' as LogoName Union Select id,LogoName from Logo 
";

                    Con.Open();
                    PopulateDropDownList(Con, SchooltyeQry, ddlSchoolType, "SchoolType", "SchooltypeId");
                    PopulateDropDownList(Con, Logoqery, ddlLogo, "LogoName", "Id");
                }
            }

            private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField)
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                //cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                SqlDataReader dr = cmd.ExecuteReader();
                ddl.DataSource = dr;
                ddl.DataTextField = textField;
                ddl.DataValueField = valueField;
                ddl.DataBind();
                dr.Close();
            }

            private void LoadRecordData(int SchoolId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM School WHERE SchoolId = @SchoolId", Con);
                    cmd.Parameters.AddWithValue("@SchoolId", SchoolId);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows && dr.Read())
                    {
                        ddlSchoolType.Text = dr["SchoolType"].ToString();
                        ddlLogo.SelectedValue = dr["LogoId"].ToString();
                    txtSchoolCode.Text = dr["SchoolCode"].ToString();
                    txtSchoolName.Text = dr["SchoolName"].ToString();
                    txtAddress.Text = dr["Address"].ToString();

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
                if (Request.QueryString["SchoolId"] != null)
                {
                    int SchoolId = int.Parse(Request.QueryString["SchoolId"]);
                    UpdateRecord(SchoolId);
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

                    if (ddlLogo.SelectedValue == "0")
                    {
                        lblErrorMessage.Text = "Please select School Logo.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                        return;
                    }

                    if (ddlSchoolType.SelectedValue == "0")
                    {
                        lblErrorMessage.Text = "Please select a School Type.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                        return;
                    }

                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        string query = @"INSERT INTO School
                                (SchoolCode, SchoolName,Address, SchoolType, Logoid)
                             VALUES
                                (@SchoolCode, @SchoolName,@Address, @SchoolType, @Logoid)";

                        using (SqlCommand cmd = new SqlCommand(query, Con))
                        {
                        cmd.Parameters.AddWithValue("@SchoolCode", txtSchoolCode.Text.Trim());
                        cmd.Parameters.AddWithValue("@SchoolName", txtSchoolName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                        cmd.Parameters.AddWithValue("@LogoId", ddlLogo.SelectedValue);
                            cmd.Parameters.AddWithValue("@SchoolType",ddlSchoolType.SelectedValue);
                            cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    lblMessage.Text = "School added successfully!";
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

            private void UpdateRecord(int SchoolId)
            {
                try
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        string query = @"Update School SET 
                                    SchoolName=@SchoolName,
                                    SchoolType=@SchoolType,
                                    LogoId=@LogoId,
                                    SchoolCode=@SchoolCode
                                    WHERE SchoolId=@SchoolId";
                        SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@SchoolCode", txtSchoolCode.Text.Trim());
                    cmd.Parameters.AddWithValue("@SchoolName", txtSchoolName.Text.Trim());
                    cmd.Parameters.AddWithValue("@LogoId", ddlLogo.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolType", ddlSchoolType.SelectedValue);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                    cmd.Parameters.AddWithValue("@SchoolId", SchoolId);
                        cmd.ExecuteNonQuery();
                        ClearControls();
                        SetButtonText();
                    }
                    lblMessage.Text = "School updated successfully!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                }
                catch (SqlException ex)
                {
                    lblErrorMessage.Text = "Error updating Record. Please try again. " + ex.Message;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }

            private void DeleteRecord(int SchoolId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM School WHERE SchoolId = @SchoolId", Con);
                    cmd.Parameters.AddWithValue("@SchoolId", SchoolId);
                    cmd.ExecuteNonQuery();
                    Response.Redirect("Schools.aspx?deleteSuccess=true");
                }
            }

            private void ClearControls()
            {
            ddlLogo.SelectedIndex = 0;
            ddlSchoolType.SelectedIndex = 0;
            txtSchoolCode.Text = "";
            txtSchoolName.Text = "";
        }
    }
    }
