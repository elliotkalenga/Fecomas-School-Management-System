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
    public partial class BulkEnrollment : System.Web.UI.Page
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
            }
        }

        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();

                string ClassSourceQry = @"SELECT 0 as ClassId,'---- Select  Class-----' AS ClassName
                                          UNION SELECT ClassId,ClassName FROM Class WHERE SchoolId=@SchoolId
                                          ORDER BY ClassName";

                string TermSourceQry = @"SELECT T.TermId, TN.TermNumber + ' (' + F.FinancialYear + ')' AS Term 
                                         FROM Term T
                                         INNER JOIN TermNumber TN ON T.Term=TN.TermId
                                         INNER JOIN FinancialYear F ON T.Yearid=F.FinancialYearid
                                         WHERE T.SchoolId=@SchoolId";

                string ClassDestinationQry = @"SELECT 0 as ClassId,'---- Select  Class-----' AS ClassName
                                               UNION SELECT ClassId,ClassName FROM Class WHERE SchoolId=@SchoolId
                                               ORDER BY ClassName";

                string TermDestinationQry = @"SELECT T.TermId, TN.TermNumber + ' (' + F.FinancialYear + ')' AS Term 
                                              FROM Term T
                                              INNER JOIN TermNumber TN ON T.Term=TN.TermId
                                              INNER JOIN FinancialYear F ON T.Yearid=F.FinancialYearid
                                              WHERE T.SchoolId=@SchoolId";

                PopulateDropDownList(Con, ClassSourceQry, ddlSourceClass, "ClassName", "ClassId");
                PopulateDropDownList(Con, TermSourceQry, ddlSourceTerm, "Term", "TermId");
                PopulateDropDownList(Con, ClassDestinationQry, ddlDestinationClass, "ClassName", "ClassId");
                PopulateDropDownList(Con, TermDestinationQry, ddlDestinationTerm, "Term", "TermId");
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            AddBulkEnrollment();
        }

        private void AddBulkEnrollment()
        {
            try
            {
                if (ddlSourceClass.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select a Source Class.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlSourceTerm.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select a Source Term.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlDestinationClass.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select a Destination Class.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlDestinationTerm.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select a Destination Term.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"INSERT INTO [dbo].[Enrollment] 
                                    ([StudentId], [TermId], [ClassId], [CreatedBy], [SchoolId])
                                    SELECT
                                        e.StudentId,
                                        @TermIdTo,
                                        @ClassIdTo,
                                        @CreatedBy,
                                        @SchoolID
                                    FROM
                                        Enrollment e
                                    LEFT JOIN Enrollment ex
                                        ON e.StudentId = ex.StudentId
                                        AND ex.TermId = @TermIdTo
                                        AND ex.ClassId = @ClassIdTo
                                        AND ex.SchoolId = @SchoolID
                                    WHERE
                                        (e.TermId = @TermIdFrom AND e.ClassId = @ClassIdFrom AND e.SchoolId = @SchoolId)
                                        AND ex.StudentId IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                        cmd.Parameters.AddWithValue("@ClassIdFrom", ddlSourceClass.SelectedValue);
                        cmd.Parameters.AddWithValue("@ClassIdTo", ddlDestinationClass.SelectedValue);
                        cmd.Parameters.AddWithValue("@TermIdFrom", ddlSourceTerm.SelectedValue);
                        cmd.Parameters.AddWithValue("@TermIdTo", ddlDestinationTerm.SelectedValue);
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);

                        cmd.ExecuteNonQuery();

                        ClearFormFields();
                        lblMessage.Text = "Student Enrolled successfully!";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // SQL error number for unique key violation
                {
                    ErrorMessage.Text = "A student with the same details has already been enrolled.";
                }
                else
                {
                    ErrorMessage.Text = "An error occurred while enrolling the student. Please try again.";
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void ClearFormFields()
        {
            ddlSourceTerm.SelectedIndex = 0;
            ddlSourceClass.SelectedIndex = 0;
            ddlDestinationClass.SelectedIndex = 0;
            ddlDestinationTerm.SelectedIndex = 0;
        }
    }
}
