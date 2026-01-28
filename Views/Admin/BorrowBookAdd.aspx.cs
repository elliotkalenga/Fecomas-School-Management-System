using SMSWEBAPP.DAL;
using System;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class BorrowBookAdd : System.Web.UI.Page
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
                if (Request.QueryString["BorrowingId"] != null)
                {
                    int BorrowingId = int.Parse(Request.QueryString["BorrowingId"]);
                    string mode = Request.QueryString["mode"];
                    if (mode == "delete")
                    {
                        DeleteBorrowingRecord(BorrowingId);
                    }
                    else
                    {
                        LoadRecordData(BorrowingId);
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            if (Request.QueryString["BorrowingId"] != null)
            {
                btnSubmit.Text = "Return A Book";
            }
            else
            {
                btnSubmit.Text = "Borrow A Book";
            }
        }

        private void DeleteBorrowingRecord(int BorrowingId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM BorrowBook WHERE BorrowingId = @BorrowingId", Con);
                cmd.Parameters.AddWithValue("@BorrowingId", BorrowingId);
                cmd.ExecuteNonQuery();
                Response.Redirect("BorrowBook.aspx?deleteSuccess=true");
            }
        }

        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string BookQry = @"SELECT '0' as Barcode, '---- Select Book Details -----' AS Book
                                    UNION
                                    SELECT Li.Barcode, Li.Barcode + '/' + B.BookTitle + ' (A: ' + B.Author + ' P: ' + B.Publisher + '/' + S.SubjectName + '/ ' + B.ISBN + ')' as book
                                    FROM LibraryInventory LI
                                    INNER JOIN Books B ON LI.BookId = B.BookId
                                    INNER JOIN Subject S ON B.SubjectId = S.SubjectId
                                    WHERE LI.SchoolId = @SchoolId and LI.BookStatus='Available'";

                string MemberQry = @"SELECT 0 as MemberId, '---- Select Library Member -----' AS Member
                                     UNION
                                     SELECT MemberId, Member
                                     FROM LibraryMember
                                     WHERE SchoolId = @SchoolId AND Status = 2";

                Con.Open();
                PopulateDropDownList(Con, BookQry, ddlBook, "Book", "Barcode");
                PopulateDropDownList(Con, MemberQry, ddlMember, "Member", "MemberId");
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

        private void LoadRecordData(int BorrowingId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT BorrowingId,BookNo as Barcode,MemberId,Numberofdays FROM BorrowBook WHERE BorrowingId = @BorrowingId", Con);
                cmd.Parameters.AddWithValue("@BorrowingId", BorrowingId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        ddlBook.SelectedValue = dr["Barcode"].ToString();
                        ddlMember.SelectedValue = dr["MemberId"].ToString();
                        txtDays.Text = dr["NumberofDays"].ToString();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No rows found for the given BorrowingId.");
                }

                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["BorrowingId"] != null)
            {
                int BorrowingId = int.Parse(Request.QueryString["BorrowingId"]);
                UpdateRecordsData(BorrowingId);
                ClearFormFields();
            }
            else
            {
                AddNewRecord();
            }
        }

        private void AddNewRecord()
        {
            try
            {
                if (string.IsNullOrEmpty(ddlBook.SelectedValue) || ddlBook.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select Book Details.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (string.IsNullOrEmpty(ddlMember.SelectedValue) || ddlMember.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select Library Member.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                string createdBy = Session["Username"]?.ToString();
                string schoolId = Session["SchoolId"]?.ToString();

                if (string.IsNullOrEmpty(createdBy) || string.IsNullOrEmpty(schoolId))
                {
                    ErrorMessage.Text = "Session expired. Please log in again.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();

                    using (SqlTransaction transaction = Con.BeginTransaction())
                    {
                        try
                        {
                            string insertQuery = @"INSERT INTO BorrowBook (BookNo, MemberId, NumberofDays, SchoolId, BookStatus, CreatedBy)
                                                   VALUES (@BookNo, @MemberId, @NumberofDays, @SchoolId, @BookStatus, @CreatedBy)";

                            using (SqlCommand cmd = new SqlCommand(insertQuery, Con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@BookNo", ddlBook.SelectedValue);
                                cmd.Parameters.AddWithValue("@MemberId", ddlMember.SelectedValue);
                                cmd.Parameters.AddWithValue("@NumberofDays", txtDays.Text.Trim());
                                cmd.Parameters.AddWithValue("@SchoolId", schoolId);
                                cmd.Parameters.AddWithValue("@BookStatus", "Not Returned");
                                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                                cmd.ExecuteNonQuery();
                            }

                            string updateQuery = @"UPDATE LibraryInventory SET BookStatus = 'Not Available' WHERE Barcode = @BookNo";

                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, Con, transaction))
                            {
                                updateCmd.Parameters.AddWithValue("@BookNo", ddlBook.SelectedValue);
                                updateCmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            ClearFormFields();
                            lblMessage.Text = "Book Borrowed Successfully from the Library.";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    ErrorMessage.Text = "This book is not available for Borrowing.";
                }
                else
                {
                    ErrorMessage.Text = $"An error occurred while adding the book record. SQL Error: {ex.Number}. Please try again.";
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = "An unexpected error occurred: " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void UpdateRecordsData(int BorrowingId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();

                    using (SqlTransaction transaction = Con.BeginTransaction())
                    {
                        try
                        {
                            string updateBorrowBookQuery = @"UPDATE BorrowBook
                                                            SET ActualReturnDate = GETDATE(),
                                                                BookStatus = 'Returned'
                                                            WHERE BorrowingId = @BorrowingId";

                            SqlCommand cmdUpdateBorrow = new SqlCommand(updateBorrowBookQuery, Con, transaction);
                            cmdUpdateBorrow.Parameters.AddWithValue("@BorrowingId", BorrowingId);

                            int rowsAffected = cmdUpdateBorrow.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                string selectBookNoQuery = "SELECT BookNo FROM BorrowBook WHERE BorrowingId = @BorrowingId";
                                SqlCommand cmdSelectBookNo = new SqlCommand(selectBookNoQuery, Con, transaction);
                                cmdSelectBookNo.Parameters.AddWithValue("@BorrowingId", BorrowingId);
                                object bookNoObj = cmdSelectBookNo.ExecuteScalar();

                                if (bookNoObj != null)
                                {
                                    string bookNo = bookNoObj.ToString();

                                    string updateInventoryQuery = @"UPDATE LibraryInventory
                                                                    SET BookStatus = 'Available'
                                                                    WHERE Barcode = @BookNo";

                                    SqlCommand cmdUpdateInventory = new SqlCommand(updateInventoryQuery, Con, transaction);
                                    cmdUpdateInventory.Parameters.AddWithValue("@BookNo", bookNo);
                                    cmdUpdateInventory.ExecuteNonQuery();
                                }

                                transaction.Commit();
                                ClearFormFields();
                                SetButtonText();
                                lblMessage.Text = "Book Returned successfully!";
                                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                            }
                            else
                            {
                                transaction.Rollback();
                                ErrorMessage.Text = "No record was updated. Please check the BorrowingId.";
                                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Number} - {ex.Message}");
                ErrorMessage.Text = "An error occurred while updating the record. Please try again.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void ClearFormFields()
        {
            ddlBook.SelectedIndex = 0;
            ddlMember.SelectedIndex = 0;
            txtDays.Text = string.Empty;
        }
    }
}
