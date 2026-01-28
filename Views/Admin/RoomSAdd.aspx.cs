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
    public partial class RoomSAdd : System.Web.UI.Page
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

                if (Request.QueryString["RoomId"] != null)
                {
                    int RoomId;
                    if (int.TryParse(Request.QueryString["RoomId"], out RoomId))
                    {
                        string mode = Request.QueryString["mode"];
                        if (mode == "delete")
                        {
                            DeleteBook(RoomId);
                        }
                        else
                        {
                            LoadRecordData(RoomId);
                        }
                    }
                }
            }
        }

        protected void SetButtonText()
        {
            btnSubmit.Text = Request.QueryString["RoomId"] != null ? "Update" : "Add";
        }

        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string HostelQry = @"SELECT 0 AS HostelId, '-- Select Hostel --' AS HostelName 
                                           UNION  
                                           SELECT HostelId, HostelName FROM Hostels WHERE SchoolId = @SchoolId";


                Con.Open();
                PopulateDropDownList(Con, HostelQry, ddlHostel, "HostelName", "HostelId");
            }
        }

        private void PopulateDropDownList(SqlConnection conn, string query, DropDownList ddl, string textField, string valueField)
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    ddl.DataSource = dr;
                    ddl.DataTextField = textField;
                    ddl.DataValueField = valueField;
                    ddl.DataBind();
                }
            }
        }

        private void LoadRecordData(int RoomId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Rooms WHERE RoomId = @RoomId", Con))
                {
                    cmd.Parameters.AddWithValue("@RoomId", RoomId);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows && dr.Read())
                        {
                            txtRoomNumber.Text = dr["RoomNumber"].ToString();
                            txtRoomDescription.Text = dr["RoomDescription"].ToString();
                            txtCapacity.Text = dr["Capacity"].ToString();
                            ddlHostel.SelectedValue = dr["HostelId"].ToString();
                        }
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["RoomId"] != null)
            {
                int RoomId;
                if (int.TryParse(Request.QueryString["RoomId"], out RoomId))
                {
                    UpdateRecord(RoomId);
                }
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
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"INSERT INTO Rooms (RoomNumber,HostelId,Capacity,RoomDescription, CreatedBy, SchoolId) 
                                     VALUES (@RoomNumber,@HostelId,@Capacity,@RoomDescription, @CreatedBy, @SchoolId)";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@RoomNumber", txtRoomNumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@RoomDescription", txtRoomDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@Capacity", txtCapacity.Text.Trim());
                        cmd.Parameters.AddWithValue("@HostelId", ddlHostel.SelectedValue);
                        cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"] ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"] ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Room added successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error adding book. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void UpdateRecord(int RoomId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    string query = @"UPDATE Rooms 
                                     SET RoomNumber = @RoomNumber, 
                                         RoomDescription = @RoomDescription, 
                                         Capacity = @Capacity, 
                                         HostelId = @HostelId
                                     WHERE RoomId = @RoomId";
                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        cmd.Parameters.AddWithValue("@RoomNumber", txtRoomNumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@RoomDescription", txtRoomDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@Capacity", txtCapacity.Text.Trim());
                        cmd.Parameters.AddWithValue("@HostelId", ddlHostel.SelectedValue);
                        cmd.Parameters.AddWithValue("@RoomId", RoomId);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Record updated successfully!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessModal", "$('#successModal').modal('show');", true);
            }
            catch (SqlException ex)
            {
                lblErrorMessage.Text = "Error updating Room. Please try again. " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorModal", "$('#errorModal').modal('show');", true);
            }
        }

        private void DeleteBook(int RoomId)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM rooms WHERE RoomId = @RoomId", Con))
                {
                    cmd.Parameters.AddWithValue("@RoomId", RoomId);
                    cmd.ExecuteNonQuery();
                }
            }
            Response.Redirect("Rooms.aspx?deleteSuccess=true");
        }

        private void ClearControls()
        {
            txtCapacity.Text = string.Empty;
            txtRoomDescription.Text = string.Empty;
            txtRoomNumber.Text = string.Empty;
            ddlHostel.SelectedIndex = 0;
        }
    }
}
