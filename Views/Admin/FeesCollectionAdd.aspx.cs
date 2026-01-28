using Microsoft.Reporting.WebForms;
using Newtonsoft.Json.Linq;
using SMSWEBAPP.DAL;
using SMSWEBAPP.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SMSWEBAPP.Views.Admin.Enrollment;
namespace SMSWEBAPP.Views.Admin
{
    public partial class FeesCollectionAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.ForeColor = Color.Green;
            lblMessage.Text = "Transaction Proccessed Sucessfully";

            if (Session["User"] == null)
            {
                // Redirect to login page
                Response.Redirect("UserLogin.aspx");
            }


            if (!IsPostBack)
            {
                SetButtonText();
                PopulateDropDownLists();
                string RandomString = RandomStringGenerator.GenerateRandomString();
                string referenceno = "REF / " + ddlStudent.SelectedValue + "." + RandomString + "." + " / " + Session["SchoolCode"];
                txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtReference.Text=referenceno;
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
                txtAmount.Enabled = false;
                txtDate.Enabled = false;
                ddlPaymentMethod.Enabled = false;
                ddlStudent.Enabled = false;
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
                (referenceNo, InvoiceId, Amount, PaymentMethod, CreatedBy, SchoolId)
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
WHERE TermID = (SELECT TOP 1 TermId FROM Term WHERE Status = 2 AND SchoolId = @schoolid) 
AND Source = 1 ";

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
VALUES (
    (SELECT TOP 1 TermId FROM Term WHERE Status = 2 AND SchoolId = @SchoolId),
    @Amount,
    1,
    'School Fees Collection',
    'Fees received',
    @CreatedBy,
    GETDATE(),
    @SchoolId
);
";

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
                string StudentQry = @"SELECT 
    0 AS InvoiceId,
    '---- Select Invoice to start Collecting Fees -----' AS Invoice

UNION ALL

SELECT  
    InvoiceId,
    CONCAT(
        Student, 
        ' (', FeesName, 
        ' Balance : MK',
        FORMAT(
            ISNULL(Balance, 0) + ISNULL(PreviousTermBalance, 0),
            'N0'
        ),
        ')'
    ) AS Invoice
FROM FeesCollectionSummary
WHERE 
    (ISNULL(Balance, 0) + ISNULL(PreviousTermBalance, 0)) > 0
    AND Status = 2
    AND SchoolId = @SchoolId

ORDER BY Invoice;
";


                string PaymentMethodtQry = @"SELECT 0 as PaymentMethodId,'---- Select Payment Method-----' AS  PaymentMethod UNION SELECT PaymentmethodId, PaymentMethod from paymentmethod";


                Con.Open();

                PopulateDropDownList(Con, StudentQry, ddlStudent, "Invoice", "InvoiceId");
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
                        txtReference.Text = dr["ReferenceNo"].ToString();

                        ddlStudent.SelectedValue = invoiceId;
                        ddlPaymentMethod.SelectedValue = PaymentMethod;
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


        protected async void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["FeesCollectionId"] != null)
            {
                int FeesCollectionId = int.Parse(Request.QueryString["FeesCollectionId"]);

                await UpdateFeesCollection(FeesCollectionId);  // <-- CALL UPDATE HERE
            }
            else
            {
                await AddFeesCollection(); // For inserts
            }

            lblMessage.Visible = true;
        }


        private async Task UpdateFeesCollection(int FeesCollectionId)
        {
            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                await con.OpenAsync();

                string updateQuery = @"
            UPDATE FeesCollection 
            SET referenceNo = @ReferenceNo,
                Amount = @Amount,
                PaymentMethod = @PaymentMethod,
                PaidDate = @PaidDate
            WHERE FeesCollectionId = @FeesCollectionId";

                SqlCommand cmd = new SqlCommand(updateQuery, con);
                cmd.Parameters.AddWithValue("@ReferenceNo", txtReference.Text);
                cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(txtAmount.Text));
                cmd.Parameters.AddWithValue("@PaymentMethod", ddlPaymentMethod.SelectedValue);

                DateTime paidDate = DateTime.TryParse(txtDate.Text, out DateTime dt)
                                    ? dt
                                    : DateTime.Now;
                cmd.Parameters.AddWithValue("@PaidDate", paidDate);

                cmd.Parameters.AddWithValue("@FeesCollectionId", FeesCollectionId);

                await cmd.ExecuteNonQueryAsync();
            }

            lblMessage.Text = "Record updated successfully!";
        }

        private SmsService _smsService = new SmsService();

        private async Task AddFeesCollection()
        {
            
            try
            {
                // Check if the user has selected a value from each dropdown
                if (ddlStudent.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select Invoice.";
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
                INSERT INTO feescollection
                (referenceNo, InvoiceId, Amount, PaymentMethod, CreatedBy, SchoolId,PaidDate)
                VALUES
                (@ReferenceNo, @InvoiceId, @Amount, @PaymentMethod, @CreatedBy, @SchoolId,@PaidDate)";

                        SqlCommand cmd = new SqlCommand(insertFeesCollectionQuery, Con, transaction);

                        cmd.Parameters.AddWithValue("@ReferenceNo", txtReference.Text.ToString());
                        cmd.Parameters.AddWithValue("@InvoiceId", ddlStudent.SelectedValue);
                        cmd.Parameters.AddWithValue("@PaymentMethod", ddlPaymentMethod.SelectedValue);
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(txtAmount.Text)); // Convert txtAmount.Text to decimal
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                        cmd.Parameters.Add("@PaidDate", SqlDbType.Date).Value = txtDate.Text.ToString();


                        cmd.ExecuteNonQuery();
                        string RandomString = RandomStringGenerator.GenerateRandomString();
                        string referenceno = "REF / " + ddlStudent.SelectedValue + "." + RandomString + "." + " / " + Session["SchoolCode"];
                        txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                        txtReference.Text = referenceno;
                        // Commit transaction if everything is successful
                        transaction.Commit();
                        // Get phone number and student details
                        string getPhoneNumberQuery = @"SELECT 
      EnrollmentID,
      Phone,
      Email,
      SchoolId,
      Paidstatus,
      Guardian,
      InvoiceId,
      InvoiceNo,
      Student,
      ClassName,
      StudentNO,

      FeesName,

      /* 🔹 Current + Previous Term Balance */
      (ISNULL(TotalFees, 0) + ISNULL(PreviousTermBalance, 0)) AS TotalFees,

      /* Paid */
      TotalCollected,

      /* 🔹 Balance including Previous Term Balance */
      (ISNULL(TotalFees, 0) + ISNULL(PreviousTermBalance, 0)) 
          - ISNULL(TotalCollected, 0) AS Balance,

      TermNumber + ' (' + FinancialYear + ')' AS Term,

      Status,
      CreatedBy,
      CreatedDate,

      SchoolName,
      SchoolCode,
      Logo,
      Logoid,
      Address,

      ParentSchoolCode,
      ParentSchoolName,
      ParentSchoolId,

      TermId,
      SmsStatus,

      /* ✅ Invoice status based on FULL amount */
      CASE 
          WHEN ISNULL(TotalCollected, 0) = 0 THEN 'NOT PAID'
          WHEN ISNULL(TotalCollected, 0) 
               < (ISNULL(TotalFees, 0) + ISNULL(PreviousTermBalance, 0))
               THEN 'PARTLY PAID'
          WHEN ISNULL(TotalCollected, 0) 
               = (ISNULL(TotalFees, 0) + ISNULL(PreviousTermBalance, 0))
               THEN 'PAID IN FULL'
          ELSE 'OVER PAID'
      END AS InvoiceStatus

FROM dbo.FeesCollectionSummary




                                               WHERE InvoiceId = @InvoiceId and SmsStatus=1";

                        using (SqlCommand phoneCmd = new SqlCommand(getPhoneNumberQuery, Con))
                        {
                            phoneCmd.Parameters.AddWithValue("@InvoiceId", ddlStudent.SelectedValue);
                            phoneCmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                            phoneCmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                            SqlDataReader reader = phoneCmd.ExecuteReader();
                            if (reader.Read())
                            {
                                string phoneNumber = reader["phone"].ToString();
                                string studentName = reader["Student"].ToString();
                                string term = reader["Term"].ToString();
                                decimal Balance = decimal.Parse(reader["balance"].ToString());
                                string School = reader["SchoolName"].ToString();
                                string Class = reader["ClassName"].ToString();
                                string FeesName = reader["FeesName"].ToString();
                                reader.Close();

                                // Step 2: Compute total fees collection amount for the term
                                string getAmountQuery = @"
                        SELECT SUM(FC.Amount) 
                        FROM FeesCollection FC
                        INNER JOIN StudentInvoice SI ON FC.InvoiceID = SI.InvoiceId
                        INNER JOIN Enrollment En ON SI.EnrollmentId = En.EnrollmentId
                        INNER JOIN Term T ON En.TermId = T.TermId
                        WHERE T.Status = 2 AND FC.SchoolId = @SchoolId";

                                decimal totalAmount = 0;

                                using (SqlCommand cmdGetAmount = new SqlCommand(getAmountQuery, Con))
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
                        WHERE TermID = (SELECT TOP 1 TermId FROM Term WHERE Status = 2 AND SchoolId = @SchoolId) 
                        AND Source = 1 
                        ";

                                object existingIncomeId = null;

                                using (SqlCommand cmdCheck = new SqlCommand(checkExistingQuery, Con))
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

                                    using (SqlCommand cmdUpdate = new SqlCommand(updateQuery, Con))
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

                                    using (SqlCommand cmdInsert = new SqlCommand(insertIncomeQuery, Con))
                                    {
                                        cmdInsert.Parameters.AddWithValue("@Amount", totalAmount);
                                        cmdInsert.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                                        cmdInsert.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                                        cmdInsert.ExecuteNonQuery();
                                    }
                                }

                                // Send SMS
                                decimal amount = decimal.Parse(txtAmount.Text);
                                string formattedAmount = amount.ToString("N2"); // Formats as #,##0.00
                                string formattedbalance = Balance.ToString("N2"); // Formats as #,##0.00

                                string message = $"{School} has received K{formattedAmount} as Term {term} {FeesName}\n" +
                                                 $"for {studentName} of {Class}\n" +
                                                 $"Balance: K{formattedbalance}\n" +
                                                 $"THANK YOU";
                                string token = await _smsService.GetAccessTokenAsync();
                                if (!string.IsNullOrEmpty(token))
                                {
                                    string result = await _smsService.SendSmsAsync(token, new string[] { phoneNumber }, message);
                                    try
                                    {
                                        var jsonData = JObject.Parse(result);
                                        if (jsonData["payload"].First["status"].ToString() == "submitted")
                                        {
                                            await InsertSmsLog(phoneNumber, message, studentName);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Error parsing JSON: " + ex.Message);
                                    }
                                }



                                ClearFormFields();


                            }
                            else
                            {
                                reader.Close();
                                transaction.Rollback();
                                ErrorMessage.Text = "An error occurred: Student details not found.";
                                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                            }
                        }
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

        private async Task InsertSmsLog(string phone,string Message, string Student)
        {
            using (SqlConnection conn = new SqlConnection(AppConnection.GetConnectionString()))
            {
                await conn.OpenAsync();

                string tariffQuery = "SELECT TOP 1 TariffId FROM smsTariff WHERE Status = 1";
                int tariffId = 0;
                using (SqlCommand tariffCmd = new SqlCommand(tariffQuery, conn))
                {
                    object tariffResult = await tariffCmd.ExecuteScalarAsync();
                    if (tariffResult != null)
                    {
                        tariffId = Convert.ToInt32(tariffResult);
                    }
                    else
                    {
                        throw new Exception("No active tariff found.");
                    }
                }

                string query = "INSERT INTO SmsLog (Phone,Message, CreatedDate, CreatedBy, SchoolId, TariffId,Student) VALUES (@Phone,@Message, GETDATE(), @CreatedBy, @SchoolId, @TariffId,@Student)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Message", Message);
                    cmd.Parameters.AddWithValue("@Student", Student);
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                    cmd.Parameters.AddWithValue("@TariffId", tariffId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }



        private void ClearFormFields()
        {
            ddlStudent.SelectedIndex = 0;
           ddlPaymentMethod.SelectedIndex = 0;
            txtAmount.Text = "";
        }

        protected void ddlStudent_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
