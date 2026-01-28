using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Students
{

        public partial class RecoverLogin : System.Web.UI.Page
        {
            protected void Page_Load(object sender, EventArgs e)
            {
            }

        protected void btnRecover_Click(object sender, EventArgs e)
        {
            string firstLetter = txtFirstLetter.Text.Trim();
            string lastLetter = txtLastLetter.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();

            // Validate input
            if (string.IsNullOrEmpty(firstLetter) || string.IsNullOrEmpty(lastLetter) || string.IsNullOrEmpty(phoneNumber))
            {
                lblMessage.Text = "Please fill in all fields.";
                lblMessage.Visible = true;
                gvLoginDetails.Visible = false;
                return;
            }

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = @"SELECT FirstName + ' ' + LastName AS FullName, UserName, Password 
                         FROM Student 
                         WHERE Phone = @Phone 
                         AND LEFT(FirstName,1) COLLATE SQL_Latin1_General_CP1_CI_AI = @FirstLetter 
                         AND LEFT(LastName,1) COLLATE SQL_Latin1_General_CP1_CI_AI = @LastLetter";

                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                cmd.Parameters.AddWithValue("@FirstLetter", firstLetter);
                cmd.Parameters.AddWithValue("@LastLetter", lastLetter);

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                // Check if records exist
                if (dt.Rows.Count > 0)
                {
                    gvLoginDetails.DataSource = dt;
                    gvLoginDetails.DataBind();
                    gvLoginDetails.Visible = true;
                    lblMessage.Visible = false;
                }
                else
                {
                    lblMessage.Text = "No student found with these details.";
                    lblMessage.Visible = true;
                    gvLoginDetails.Visible = false;
                }
            }
        }
    }
}