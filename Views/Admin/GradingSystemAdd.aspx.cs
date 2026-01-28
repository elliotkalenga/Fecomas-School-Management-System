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
    public partial class GradingSystemAdd : System.Web.UI.Page
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

                if (Request.QueryString["ScaleId"] != null)
                {
                    int ScaleId = int.Parse(Request.QueryString["ScaleId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteRecord(ScaleId);
                    }
                    else
                    {
                        LoadRecordData(ScaleId);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["ScaleId"] != null)
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

                string GradingSstemqry = @"select '--- Select Grading System ---' as ScaleDescriptionId, '--- Select Grading System ---' as ScaleDescription UNION
                                            select 'Average Score' as ScaleDescriptionId, 'Average Score' as ScaleDescription UNION
                                            select 'Aggregate Points' as ScaleDescriptionId, 'Aggregate Points' as ScaleDescription
";

                Con.Open();
                PopulateDropDownList(Con, ScaleTypeQry, ddlScaleType, "ScaleType", "ScaleTypeId");
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

        private void LoadRecordData(int ScaleId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand(@"select gst.ScaleTypeid, ScaleId, gs.LowerScale,gs.UpperScale,
                                CASE 
                                    WHEN gs.ScaleTypeId = 1 THEN gs.Grade1
                                    WHEN gs.ScaleTypeId = 2 THEN CAST(gs.Grade2 AS NVARCHAR(10))
                                END AS Grade,
                                Description as GradeDescription,
                                Remark as Remarks,
                                gst.ScaleType as GradingSystem
                                from GradingSystem gs
                                Inner Join GradingSystemScaleType gst on gs.ScaletypeId=gst.ScaleTypeId
                                Where ScaleId = @ScaleId order by gst.ScaleType", Con);
                cmd.Parameters.AddWithValue("@ScaleId", ScaleId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
                {
                    ddlScaleType.Text = dr["ScaleTypeId"].ToString();
                    txtLowerScale.Text = dr["LowerScale"].ToString();
                    txtGradeDescription.Text = dr["GradeDescription"].ToString();
                    txtUpperScale.Text = dr["UpperScale"].ToString();
                    txtGrade.Text = dr["Grade"].ToString();
                    txtRemark.Text = dr["Remarks"].ToString();
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
            if (Request.QueryString["ScaleId"] != null)
            {
                int ScaleId = int.Parse(Request.QueryString["ScaleId"]);
                UpdateRecord(ScaleId);
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



                if (ddlScaleType.SelectedValue == "0")
                {
                    lblErrorMessage.Text = "Please select a Grading Scale.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlScaleType.SelectedIndex == 1)
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        string query = @"INSERT INTO GradingSystem 
                                    (LowerScale,UpperScale,Grade1,Description,Remark,SchoolId,ScaleTypeId,CreatedBy)
                                    VALUES(@LowerScale,@UpperScale,@Grade1, @Description,@Remark,@SchoolId,@ScaleTypeId, @CreatedBy)";

                        using (SqlCommand cmd = new SqlCommand(query, Con))
                        {
                            cmd.Parameters.AddWithValue("@LowerScale", txtLowerScale.Text.Trim());
                            cmd.Parameters.AddWithValue("@UpperScale", txtUpperScale.Text.Trim());
                            cmd.Parameters.AddWithValue("@Grade1", txtGrade.Text.Trim());
                            cmd.Parameters.AddWithValue("@Remark", txtRemark.Text.Trim());
                            cmd.Parameters.AddWithValue("@Description", txtGradeDescription.Text.Trim());
                            cmd.Parameters.AddWithValue("@ScaleTypeId", ddlScaleType.SelectedValue);
                            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                            cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                            cmd.ExecuteNonQuery();
                        }
                    }

                }
                else
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        string query = @"INSERT INTO GradingSystem 
                                    (LowerScale,UpperScale,Grade2,Description,Remark,SchoolId,ScaleTypeId,CreatedBy)
                                    VALUES(@LowerScale,@UpperScale,@Grade2, @Description,@Remark,@SchoolId,@ScaleTypeId, @CreatedBy)";

                        using (SqlCommand cmd = new SqlCommand(query, Con))
                        {
                            cmd.Parameters.AddWithValue("@LowerScale", txtLowerScale.Text.Trim());
                            cmd.Parameters.AddWithValue("@UpperScale", txtUpperScale.Text.Trim());
                            cmd.Parameters.AddWithValue("@Grade2", txtGrade.Text.Trim());
                            cmd.Parameters.AddWithValue("@Remark", txtRemark.Text.Trim());
                            cmd.Parameters.AddWithValue("@Description", txtGradeDescription.Text.Trim());
                            cmd.Parameters.AddWithValue("@ScaleTypeId", ddlScaleType.SelectedValue);
                            cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                            cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                            cmd.ExecuteNonQuery();
                        }
                    }

                }

                lblMessage.Text = "Grade Scale added successfully!";
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

        private void UpdateRecord(int ScaleId)
        {
            try
            {


                if (ddlScaleType.SelectedIndex == 1)
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        string query = @"UPDATE GradingSystem
                                    SET 
                                    LowerScale=@LowerScale,
                                    UpperScale=@UpperScale,
                                    Description=@Description,
                                    Grade1=@Grade1,
                                    Remark=@Remark,
                                    ScaleTypeId=@ScaleTypeId
                                    Where ScaleId=@ScaleId";
                        SqlCommand cmd = new SqlCommand(query, Con);
                        cmd.Parameters.AddWithValue("@LowerScale", txtLowerScale.Text.Trim());
                        cmd.Parameters.AddWithValue("@UpperScale", txtUpperScale.Text.Trim());
                        cmd.Parameters.AddWithValue("@Grade1", txtGrade.Text.Trim());
                        cmd.Parameters.AddWithValue("@Remark", txtRemark.Text.Trim());
                        cmd.Parameters.AddWithValue("@Description", txtGradeDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@ScaleTypeId", ddlScaleType.SelectedValue);
                        cmd.Parameters.AddWithValue("@ScaleId", ScaleId);
                        cmd.ExecuteNonQuery();
                        ClearControls();
                        SetButtonText();
                    }

                }

                else
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        string query = @"UPDATE GradingSystem
                                    SET 
                                    LowerScale=@LowerScale,
                                    UpperScale=@UpperScale,
                                    Description=@Description,
                                    Grade2=@Grade2,
                                    Remark=@Remark,
                                    ScaleTypeId=@ScaleTypeId
                                    Where ScaleId=@ScaleId";
                        SqlCommand cmd = new SqlCommand(query, Con);
                        cmd.Parameters.AddWithValue("@LowerScale", txtLowerScale.Text.Trim());
                        cmd.Parameters.AddWithValue("@UpperScale", txtUpperScale.Text.Trim());
                        cmd.Parameters.AddWithValue("@Grade2", txtGrade.Text.Trim());
                        cmd.Parameters.AddWithValue("@Remark", txtRemark.Text.Trim());
                        cmd.Parameters.AddWithValue("@Description", txtGradeDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@ScaleTypeId", ddlScaleType.SelectedValue);
                        cmd.Parameters.AddWithValue("@ScaleId", ScaleId);
                        cmd.ExecuteNonQuery();
                        ClearControls();
                        SetButtonText();
                    }

                }
                lblMessage.Text = "Grade Scale updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating Record. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteRecord(int ScaleId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM GradingSystem WHERE ScaleId = @ScaleId", Con);
                cmd.Parameters.AddWithValue("@ScaleId", ScaleId);
                cmd.ExecuteNonQuery();
                Response.Redirect("GradingSystem.aspx?deleteSuccess=true");
            }
        }

        private void ClearControls()
        {
            ddlScaleType.SelectedIndex = 0;
            txtGrade.Text = "";
            txtLowerScale.Text = "";
            txtUpperScale.Text = "";
            txtRemark.Text = "";
            txtGradeDescription.Text = "";
        }
    }
}
