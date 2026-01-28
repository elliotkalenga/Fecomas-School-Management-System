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
    public partial class LibraryInventoryAdd : System.Web.UI.Page
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
                    if (Request.QueryString["InventoryId"] != null)
                    {
                        int InventoryId = int.Parse(Request.QueryString["InventoryId"]);
                        string mode = Request.QueryString["mode"];
                        if (mode == "delete")
                        {
                            DeleteStudentData(InventoryId);
                        }
                        else
                        {
                            LoadInvoiceData(InventoryId);
                            // Load the student data if needed
                        }
                    }


                }

            }

            protected void SetButtonText()
            {
                if (Request.QueryString["InventoryId"] != null)
                {
                    btnSubmit.Text = "Update";
                }
                else
                {
                    btnSubmit.Text = "Add";
                }
            }
            private void DeleteStudentData(int InventoryId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM LibraryInventory WHERE InventoryId = @InventoryId", Con);
                    cmd.Parameters.AddWithValue("@InventoryId", InventoryId);
                    cmd.ExecuteNonQuery();
                    // Set a query parameter to indicate successful deletion
                    Response.Redirect("LibraryInventory.aspx?deleteSuccess=true");
                }

                // Redirect back to the students page after deletion
                Response.Redirect("LibraryInventory.aspx");
            }

            private void PopulateDropDownLists()
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    string BookQry = @"SELECT 0 as  BookId,'---- Select Book Details -----' AS  Book
                                        UNION 
                                        select B.BookId, B.BookTitle +' (A: '+B.Author+' P: '+B.Publisher+'/'+S.SubjectName+'/ '+B.ISBN+')'  as Book from  Books B
                                        inner join Subject S on B.SubjectId=S.SubjectId
                                    
                                    Where B.SchoolId=@SchoolId";


                    string LocationQry = @"SELECT 0 as LocationId,'---- Select Location-----' AS  Location
                                            UNION 
                                        SELECT LocationId, Location FROM Location WHERE SchoolId=@SchoolId";




                    Con.Open();

                    PopulateDropDownList(Con, BookQry, ddlBook, "Book", "BookId");
                    PopulateDropDownList(Con, LocationQry, ddlLocation, "Location", "LocationId");
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

            private void LoadInvoiceData(int InventoryId)
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM LibraryInventory WHERE InventoryId = @InventoryId", Con);
                    cmd.Parameters.AddWithValue("@InventoryId", InventoryId);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {

                            // Set values to dropdown lists
                            ddlBook.SelectedValue = dr["BookId"].ToString();
                        ddlLocation.SelectedValue = dr["LocationId"].ToString();
                        txtBarcode.Text = dr["Barcode"].ToString();


                    }
                }
                    else
                    {
                        // Debug: Log if no rows found
                        System.Diagnostics.Debug.WriteLine("No rows found for the given InventoryId.");
                    }

                    dr.Close();
                }
            }

            protected void btnSubmit_Click(object sender, EventArgs e)
            {

                if (Request.QueryString["InventoryId"] != null)
                {
                    int InventoryId = int.Parse(Request.QueryString["InventoryId"]);
                    UpdateStudentData(InventoryId);
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
                // Check if the user has selected a value from each dropdown
                if (string.IsNullOrEmpty(ddlBook.SelectedValue) || ddlBook.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select Book Details.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                if (string.IsNullOrEmpty(ddlLocation.SelectedValue) || ddlLocation.SelectedValue == "0")
                {
                    ErrorMessage.Text = "Please select Library Location.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                // Ensure session values are not null before using them
                string createdBy = Session["Username"]?.ToString();
                string schoolId = Session["SchoolId"]?.ToString();
                string schoolCode = Session["SchoolCode"]?.ToString();

                if (string.IsNullOrEmpty(createdBy) || string.IsNullOrEmpty(schoolId) || string.IsNullOrEmpty(schoolCode))
                {
                    ErrorMessage.Text = "Session expired. Please log in again.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                    return;
                }

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();

                    string query = @"
                INSERT INTO LibraryInventory
                (BookId, LocationId, SchoolId, SchoolCode, BookStatus, CreatedBy,Barcode)
                VALUES
                (@BookId, @LocationId, @SchoolId, @SchoolCode, @BookStatus, @CreatedBy,@Barcode)";

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@Barcode", txtBarcode.Text.Trim());
                        cmd.Parameters.AddWithValue("@BookId", ddlBook.SelectedValue);
                        cmd.Parameters.AddWithValue("@LocationId", ddlLocation.SelectedValue);
                        cmd.Parameters.AddWithValue("@SchoolId", schoolId);
                        cmd.Parameters.AddWithValue("@SchoolCode", schoolCode);
                        cmd.Parameters.AddWithValue("@BookStatus", "Available");
                        cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                        cmd.ExecuteNonQuery();
                    }
                }

                ClearFormFields();
                lblMessage.Text = "Book Inventory Created Successfully In the Library.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // SQL error number for unique key violation
                {
                    ErrorMessage.Text = "Duplicate Entry: A book with the same details already exists.";
                }
                else
                {
                    ErrorMessage.Text = $"An error occurred while adding the book inventory. SQL Error: {ex.Number}. Please try again.";
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = "An unexpected error occurred: " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }


        private void UpdateStudentData(int InventoryId)
            {
                try
                {
                    using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        Con.Open();
                        string query = "UPDATE LibraryInventory SET LocationId = @LocationId, Barcode=@Barcode " +
                                       "WHERE InventoryId = @InventoryId";
                        SqlCommand cmd = new SqlCommand(query, Con);
                        cmd.Parameters.AddWithValue("@LocationId", ddlLocation.SelectedValue);
                        cmd.Parameters.AddWithValue("@Barcode", txtBarcode.Text.Trim());
                        cmd.Parameters.AddWithValue("@InventoryId", InventoryId);

                        // Debug: Check values being passed to the query

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ClearFormFields();
                            SetButtonText();
                            lblMessage.Text = "Book Inventory updated successfully!";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
                        }
                        else
                        {
                            ErrorMessage.Text = "No record was updated. Please check the InventoryId.";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Detailed error logging
                    System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Number} - {ex.Message}");
                    ErrorMessage.Text = "Duplicate barcode detected";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
                }
            }


            private void ClearFormFields()
            {
                ddlBook.SelectedIndex = 0;
                ddlLocation.SelectedIndex = 0;
            }
        }
    }
