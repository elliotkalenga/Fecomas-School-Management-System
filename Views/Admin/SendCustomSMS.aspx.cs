using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Windows.Forms;
using System.Net.Http;
using System.Threading.Tasks;

namespace SMSWEBAPP.Views.Admin
{
    public partial class SendCustomSMS : System.Web.UI.Page
    {

        // Your WhatsApp Business API details
        private static readonly string accessToken = "8e4836d3d59226fd798d2e448124475a"; // Replace with your access token
        private static readonly string phoneNumberId = "+265993189671"; // Replace with your phone number ID
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                PopulateDropDownLists();
                if (Request.QueryString["StudentID"] != null)
                {
                    int studentID;
                    if (int.TryParse(Request.QueryString["StudentID"], out studentID))
                    {
                        string mode = Request.QueryString["mode"];
                        if (mode == "delete")
                        {
                        }
                        else
                        {
                            LoadStudentData(studentID);
                        }
                    }
                }
            }
        }





        private void PopulateDropDownLists()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
            }
        }

        private void LoadStudentData(int studentID)
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Student WHERE StudentID = @StudentID", Con);
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtFirstName.Text = dr["FirstName"].ToString();
                    txtLastName.Text = dr["LastName"].ToString();
                    txtGuardian.Text = dr["Guardian"].ToString();
                    txtPhone.Text = dr["Phone"].ToString();
                }
                dr.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string phone = txtPhone.Text.Trim();

            // Ensure phone number format (remove spaces and ensure country code if needed)
            phone = phone.Replace(" ", "").Replace("+", "");

            // Predefined message
            string message = $"Dear" +" *"+ txtFirstName.Text+"* "+txtLastName.Text +"*,"+"\n\n"
                              + "Congratulations! We are pleased to inform you that you have successfully passed the entrance examination and have been offered admission to "+ $"*{Session["SchoolName"]}*" + " for the upcoming academic year.\n\n"
                              + "Your outstanding performance in the entrance exam demonstrates your potential, and we are excited to welcome you into our school community, where excellence, discipline, and character development are our core values.\n\n"
                              + "Next Steps:\n"
                              + "1. Acceptance of Admission: Kindly confirm your acceptance within 3 days.\n"
                              + "2. School Fees Payment: A detailed breakdown of tuition and other fees will be provided.\n"
                              + "3. Resumption of Classes: Classes to begin on 8 September 2025.\n"
                              + "At "+ $"*{Session["SchoolName"]}*" + ", we are committed to providing a world-class education in a nurturing and disciplined environment. We look forward to seeing you grow academically, socially, and morally as part of our school family.\n\n"
                              + "For any further inquiries, please feel free to contact our admissions office.\n\n"
                              + "Best Regards,\n"
                              + "Admissions Office\n"
                              + $"*{Session["SchoolName"]}*";
            // Encode the message for the URL
            string encodedMessage = Uri.EscapeDataString(message);

            // Construct WhatsApp URL
            string whatsappUrl = $"https://wa.me/{phone}?text={encodedMessage}";

            // Assign the hyperlink
            whatsappLink.NavigateUrl = whatsappUrl;
        }



        private void ClearFormFields()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtGuardian.Text = "";
            txtPhone.Text = "";
        }
    }
}
