using Microsoft.Reporting.WebForms;
using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SMSWEBAPP.Views.Admin.Enrollment;

namespace SMSWEBAPP.Views.Admin
{
    public partial class FeesCollectionOthersAdd : System.Web.UI.Page
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
                SetButtonText();
                PopulateDropDownLists();
                if (Request.QueryString["FeesCollectionId"] != null)
                {
                    int FeesCollectionId = int.Parse(Request.QueryString["FeesCollectionId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteReverseTransactionData(FeesCollectionId);
                    }
                    else
                    {
                        LoadFeesCollectionData(FeesCollectionId);
                        // Load the student data if needed
                    }
                }


            }

        }

        protected void SetButtonText()
        {
            if (Request.QueryString["FeesCollectionId"] != null)
            {
                btnSubmit.Text = "Update";
            }
            else
            {
                btnSubmit.Text = "Add";
            }
        }
        private void DeleteReverseTransactionData(int FeesCollectionId)
        {

            if (Session["Permissions"] != null && Session["RoleId"] != null)
            {
                List<string> userPermissions = (List<string>)Session["Permissions"];
                int roleId = Convert.ToInt32(Session["RoleId"]);


                if (userPermissions.Contains("FeesCollection_Reversal"))
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        SqlTransaction transaction = Con.BeginTransaction(); // Begin transaction

                        try
                        {
                            // Step 1: Reverse the transaction
                            string reverseQuery = @"
                INSERT INTO feescollection
                (referenceNo, EnrollmentId Amount, PaymentMethod, CreatedBy, SchoolId)
                SELECT referenceNo+'(Rvs)', InvoiceId, (-1)*Amount, PaymentMethod, @CreatedBy, SchoolId 
                FROM feescollection 
                WHERE FeescollectionId = @FeescollectionId";

                            using (SqlCommand cmdReverse = new SqlCommand(reverseQuery, Con, transaction))
                            {
                                cmdReverse.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                                cmdReverse.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                                cmdReverse.Parameters.AddWithValue("@FeescollectionId", FeesCollectionId);
                                cmdReverse.ExecuteNonQuery();
                            }

                            // Step 2: Compute new total fees collection amount for the term
                            string getAmountQuery = @"
                SELECT
    ISNULL((
        SELECT SUM(FC.Amount)
        FROM FeesCollectionOthers FC
        INNER JOIN Enrollment En ON FC.EnrollmentId = En.EnrollmentId
        INNER JOIN Term T ON En.TermId = T.TermId
        WHERE T.Status = 2 AND FC.SchoolId = @SchoolId
    ), 0)
    +
    ISNULL((
        SELECT SUM(FC.Amount)
        FROM FeesCollection FC
        INNER JOIN StudentInvoice SI ON FC.InvoiceID = SI.InvoiceId
        INNER JOIN Enrollment En ON SI.EnrollmentId = En.EnrollmentId
        INNER JOIN Term T ON En.TermId = T.TermId
        WHERE T.Status = 2 AND FC.SchoolId = @SchoolId
    ), 0)
    AS TotalCollectedAmount;
";

                            decimal totalAmount = 0;

                            using (SqlCommand cmdGetAmount = new SqlCommand(getAmountQuery, Con, transaction))
                            {
                                cmdGetAmount.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                                var result = cmdGetAmount.ExecuteScalar();
                                if (result != DBNull.Value && result != null)
                                {
                                    totalAmount = Convert.ToDecimal(result);
                                }
                            }

                            // Step 3: Check if a record exists in Income table for this term and source
                            string checkExistingQuery = @"
                SELECT IncomeID FROM Income 
                WHERE TermID = (SELECT TOP 1 TermId FROM Term WHERE Status = 2) 
                AND Source = 1 
                AND SchoolId = @SchoolId";

                            object existingIncomeId = null;

                            using (SqlCommand cmdCheck = new SqlCommand(checkExistingQuery, Con, transaction))
                            {
                                cmdCheck.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                                existingIncomeId = cmdCheck.ExecuteScalar();
                            }

                            if (existingIncomeId != null)
                            {
                                // Step 4: If record exists, update it
                                string updateQuery = @"
                    UPDATE Income 
                    SET Amount = @Amount 
                    WHERE IncomeID = @IncomeID";

                                using (SqlCommand cmdUpdate = new SqlCommand(updateQuery, Con, transaction))
                                {
                                    cmdUpdate.Parameters.AddWithValue("@Amount", totalAmount);
                                    cmdUpdate.Parameters.AddWithValue("@IncomeID", existingIncomeId);
                                    cmdUpdate.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Step 5: If no record exists, insert a new record
                                string insertIncomeQuery = @"
                    INSERT INTO Income (TermID, Amount, Source, Purpose, Description, CreatedBy, CreatedDate, SchoolId)
                    VALUES ((SELECT TOP 1 TermId FROM Term WHERE Status = 2), @Amount, 1, 'School Fees Collection', 'Fees received', @CreatedBy, GETDATE(), @SchoolId)";

                                using (SqlCommand cmdInsert = new SqlCommand(insertIncomeQuery, Con, transaction))
                                {
                                    cmdInsert.Parameters.AddWithValue("@Amount", totalAmount);
                                    cmdInsert.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                                    cmdInsert.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                                    cmdInsert.ExecuteNonQuery();
                                }
                            }

                            // Commit transaction if everything is successful
                            transaction.Commit();

                            ClearFormFields();
                            lblMessage.Text = "Transaction has been reversed successfully!";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);

                            // Redirect back to the FeesCollection page
                            Response.Redirect("FeesCollection.aspx");
                        }
                        catch (Exception ex)
                        {
                            // Rollback transaction in case of error
                            transaction.Rollback();
                            ErrorMessage.Text = "An error occurred: " + ex.Message;
                            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                        }
                    }

                }
                else
                {
                    ErrorMessage.Text = "ACCESS DENIED! YOU DO NOT HAVE PERMISSION TO PERFORM THIS ACTION ";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }
        }

        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string StudentQry = @"SELECT 0 as EnrollmentId,'---- Select Student -----' AS  Student UNION SELECT  
                                    EnrollmentId,
                                    Student +' '+ ClassName + ' Term '+ TermNumber+ '('+FinancialYear +')'  as Student
                                    FROM 
                                        FeesCollectionSummary
                                    WHERE 
                                        Status=2 and Schoolid=@Schoolid order by Student";


                string PaymentMethodtQry = @"SELECT 0 as PaymentMethodId,'---- Select Payment Method-----' AS  PaymentMethod UNION SELECT PaymentmethodId, PaymentMethod from paymentmethod";

                string Feesquery = @"SELECT FeesId, FeesName FROM (
    SELECT 0 AS FeesId, '---- Select Purpose For Payment -----' AS FeesName, 0 AS Amount
    UNION
    SELECT FeesId, FeesName, Amount
    FROM FeesConfiguration
    WHERE Status = 2 AND SchoolId = @SchoolId
) AS Combined
ORDER BY 
    CASE WHEN FeesId = 0 THEN 0 ELSE 1 END,  
    Amount;
";
                Con.Open();

                PopulateDropDownList(Con, Feesquery, ddlPaidFor, "FeesName", "FeesId");
                PopulateDropDownList(Con, StudentQry, ddlStudent, "Student", "EnrollmentId");
                PopulateDropDownList(Con, PaymentMethodtQry, ddlPaymentMethod, "PaymentMethod", "PaymentMethodId");
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

        private void LoadFeesCollectionData(int FeesCollectionId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();

                string receiptquery = @"SELECT 
    fc.feescollectionId, 
    fc.ReferenceNo,
    fcs.Student,
    fcs.InvoiceId,
    fc.PaymentMethod,
    fcs.FeesName + ' (' + CONVERT(VARCHAR, fcs.TotalFees) + ')' AS Fees, 
    fc.Amount AS ReceiptAmount, 
    fcs.TotalCollected AS Cum_Collection, 
    fcs.balance, 
    fcs.InvoiceStatus  
FROM FeesCollectionSummary fcs
INNER JOIN Feescollection fc 
    ON fc.invoiceid = fcs.invoiceid
WHERE fc.FeesCollectionId = @FeesCollectionId;
";
                SqlCommand cmd = new SqlCommand(receiptquery, Con);
                cmd.Parameters.AddWithValue("@FeesCollectionId", FeesCollectionId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        // Debug: Check values being retrieved
                        var feescollectionId = dr["FeescollectionId"].ToString();
                        var invoiceId = dr["InvoiceId"].ToString();
                        var PaymentMethod = dr["PaymentMethod"].ToString();
                        var amount = decimal.Parse(dr["ReceiptAmount"].ToString());

                        // Set values to dropdown lists
                        ddlStudent.SelectedValue = invoiceId;
                        ddlPaymentMethod.SelectedValue = PaymentMethod;
                        ddlPaymentMethod.SelectedValue = feescollectionId;
                        txtAmount.Text = amount.ToString();

                        // Debug: Log or break to verify values

                        // Handle other fields if any
                    }
                }
                else
                {
                    // Debug: Log if no rows found
                    System.Diagnostics.Debug.WriteLine("No rows found for the given FeesCollectionId.");
                }

                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            if (Request.QueryString["FeesCollectionId"] != null)
            {
                int FeesCollectionId = int.Parse(Request.QueryString["FeesCollectionId"]);
                ClearFormFields();
            }
            else
            {
                AddFeesCollection();

            }
        }


        private void AddFeesCollection()
        {
            try
            {
                // Check if the user has selected a value from each dropdown
                if (ddlStudent.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select Student";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }


                if (ddlPaidFor.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select Purpose for payment";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (ddlPaymentMethod.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select Payment Method.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    SqlTransaction transaction = Con.BeginTransaction();

                    try
                    {
                        // Step 1: Insert into FeesCollection
                        string insertFeesCollectionQuery = @"
                         INSERT INTO feescollectionOthers
                         (EnrollmentId, Amount, PaymentMethod, CreatedBy, SchoolId,SchoolCode,FeesId,Description)
                         VALUES
                         (@EnrollmentId, @Amount, @PaymentMethod, @CreatedBy, @SchoolId,@SchoolCode,@FeesId,@Description)";

                        SqlCommand cmd = new SqlCommand(insertFeesCollectionQuery, Con, transaction);
                        string RandomString = RandomStringGenerator.GenerateRandomString();

                        cmd.Parameters.AddWithValue("@EnrollmentId", ddlStudent.SelectedValue);
                        cmd.Parameters.AddWithValue("@PaymentMethod", ddlPaymentMethod.SelectedValue);
                        cmd.Parameters.AddWithValue("@FeesId", ddlPaidFor.SelectedValue);
                        cmd.Parameters.AddWithValue("@Description", txtDescription.Text); 
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(txtAmount.Text)); 
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                        cmd.Parameters.AddWithValue("@SchoolCode", Session["SchoolCode"]);

                        cmd.ExecuteNonQuery();

                        // Step 2: Compute total fees collection amount for the term
                        string getAmountQuery = @"
                    SELECT SUM(FC.Amount) 
                    FROM FeesCollection FC
                    INNER JOIN StudentInvoice SI ON FC.InvoiceID = SI.InvoiceId
                    INNER JOIN Enrollment En ON SI.EnrollmentId = En.EnrollmentId
                    INNER JOIN Term T ON En.TermId = T.TermId
                    WHERE T.Status = 2 AND FC.SchoolId = @SchoolId";

                        decimal totalAmount = 0;

                        using (SqlCommand cmdGetAmount = new SqlCommand(getAmountQuery, Con, transaction))
                        {
                            cmdGetAmount.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                            var result = cmdGetAmount.ExecuteScalar();
                            if (result != DBNull.Value && result != null)
                            {
                                totalAmount = Convert.ToDecimal(result);
                            }
                        }

                        // Step 3: Check if a record exists in Income table for this term and source
                        string checkExistingQuery = @"
                    SELECT IncomeID FROM Income 
                    WHERE TermID = (SELECT TOP 1 TermId FROM Term WHERE Status = 2) 
                    AND Source = 1 
                    AND SchoolId = @SchoolId";

                        object existingIncomeId = null;

                        using (SqlCommand cmdCheck = new SqlCommand(checkExistingQuery, Con, transaction))
                        {
                            cmdCheck.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                            existingIncomeId = cmdCheck.ExecuteScalar();
                        }

                        if (existingIncomeId != null)
                        {
                            // Step 4: If record exists, update it
                            string updateQuery = @"
                        UPDATE Income 
                        SET Amount = @Amount 
                        WHERE IncomeID = @IncomeID";

                            using (SqlCommand cmdUpdate = new SqlCommand(updateQuery, Con, transaction))
                            {
                                cmdUpdate.Parameters.AddWithValue("@Amount", totalAmount);
                                cmdUpdate.Parameters.AddWithValue("@IncomeID", existingIncomeId);
                                cmdUpdate.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Step 5: If no record exists, insert a new record
                            string insertIncomeQuery = @"
                        INSERT INTO Income (TermID, Amount, Source, Purpose, Description, CreatedBy, CreatedDate, SchoolId)
                        VALUES ((SELECT TOP 1 TermId FROM Term WHERE Status = 2), @Amount, 1, 'School Fees Collection', 'Fees received', @CreatedBy, GETDATE(), @SchoolId)";

                            using (SqlCommand cmdInsert = new SqlCommand(insertIncomeQuery, Con, transaction))
                            {
                                cmdInsert.Parameters.AddWithValue("@Amount", totalAmount);
                                cmdInsert.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                                cmdInsert.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                                cmdInsert.ExecuteNonQuery();
                            }
                        }

                        // Commit transaction if everything is successful
                        transaction.Commit();

                        ClearFormFields();
                        lblMessage.Text = "Transaction Created successfully!";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction in case of error
                        transaction.Rollback();
                        ErrorMessage.Text = "An error occurred: " + ex.Message;
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that may have occurred
                ErrorMessage.Text = "An error occurred: " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }



        private void ClearFormFields()
        {
            ddlStudent.SelectedIndex = 0;
            ddlPaymentMethod.SelectedIndex = 0;
            txtAmount.Text = "";
        }



    }
}
