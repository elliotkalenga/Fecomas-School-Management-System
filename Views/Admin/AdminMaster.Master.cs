using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class AdminMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                if (Session["User"] != null)
                {
                    loggedinUser.InnerText = $"Hi {Session["FirstName"]} {Session["LastName"]}";
                    lblBrand.Text = $"{Session["SchoolName"]} ({Session["SchoolCode"]})";
                    SetUserPermissions();
                    LoadLogo();

                }

            }
        }

        private void LoadLogo()
        {
            string imageName = "";

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                string query = "SELECT Logoname FROM School S INNER JOIN Logo L ON S.Logoid = L.Id WHERE S.Schoolid = @SchoolId";

                using (SqlCommand cmd = new SqlCommand(query, Con))
                {
                    // Pass the parameter value
                    cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);

                    Con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        imageName = result.ToString();
                    }
                }
            }

            if (!string.IsNullOrEmpty(imageName))
            {
                // Full physical path (not usable in `ImageUrl`)
                string fullPath = @"C:\inetpub\wwwroot\SMSWEBAPP\StudentImages\" + imageName;

                // Convert full path to a proper web URL
                string webPath = "https://fecomas.com/StudentImages/" + imageName;

                imgLogo.ImageUrl = webPath; // Use the web-accessible URL
            }
            else
            {
                imgLogo.ImageUrl = "https://fecomas.com/images/default-logo.png"; // Default logo
            }
        }
        private void SetUserPermissions()
        {
            if (Session["Permissions"] != null && Session["RoleId"] != null)
            {
                List<string> userPermissions = (List<string>)Session["Permissions"];
                int schoolId = Convert.ToInt32(Session["SchoolId"]);

                if (schoolId == 18)
                {
                    pnlSchools.Visible = true;
                    pnlLicenses.Visible = true;
                    PnlVisitors.Visible = true;
                    pnlInquiry.Visible = true;
                    pnlInvoiceDetails.Visible = true;

                }

                if (userPermissions.Contains("SendSMS"))
                {
                    pnlSettings.Visible = pnlSms.Visible = pnlSmsLogs.Visible = pnlSendSms.Visible = true;

                }


                if (userPermissions.Contains("Manage_Hostels"))
                {
                    pnlRooms.Visible =pnlHostels.Visible = pnlManageHostels.Visible = pnlHostelAllocation.Visible = pnlHostelAllocationReport.Visible = true;

                }

                if (userPermissions.Contains("Manage_Assets"))
                {
                    pnlAssets.Visible = pnlAssetAllocation.Visible = pnlAssetCategory.Visible = pnlAssetManagement.Visible= pnlAssetReport.Visible = true;

                }

                if (userPermissions.Contains("Student_Create") ||
                    userPermissions.Contains("Student_Read") ||
                    userPermissions.Contains("Student_Update") ||
                    userPermissions.Contains("Student_Delete") ||
                    userPermissions.Contains("Enrollment_Create") ||
                    userPermissions.Contains("Enrollment_Read") ||
                    userPermissions.Contains("Enrollment_Update") ||
                    userPermissions.Contains("Enrollment_Delete") ||
                    userPermissions.Contains("Enrollment_Reports"))
                {
                    pnlRegistration.Visible = true;
                    pnlEnrollment.Visible = true;
                    pnlBulkEnrollment.Visible = true;
                    pnlStudents.Visible = true;
                    pnlEnrollmentReport.Visible = true;
                }


                if (userPermissions.Contains("Manage_Schemes_Of_Work") )
                {
                    pnlSchemesOfWork.Visible = true;
                    pnlTeaching.Visible = true;
                }

                if (userPermissions.Contains("Approve_Lesson_Plan"))
                {
                    pnlApproveScheme.Visible = true;
                    pnlLessonApproval.Visible = true;
                    pnlTeaching.Visible = true;
                }

                if (userPermissions.Contains("Manage_Lesson_Plan") )
                 {
                    pnlLessonPlans.Visible = true;
                    pnlTeaching.Visible = true;
                }


                if (
                   userPermissions.Contains("Manage_Period_Register") )
                {
                    pnlPeriodRegister.Visible = true;
                    pnlTeaching.Visible = true;
                }

                if (
                    userPermissions.Contains("Manage_Performance_Appraisal"))
                {
                    pnlPerformanceAppraisal.Visible = true;
                    pnlTeaching.Visible = true;
                }
                if (userPermissions.Contains("Candidate_Management") ||
                    userPermissions.Contains("Entrace_Exam_Management"))
                {
                    pnlCandidate.Visible = true;
                    pnlEntrance.Visible = true;
                    pnlEntranceExam.Visible = true;
                }

                if ( userPermissions.Contains("Income_Create") ||
                     userPermissions.Contains("Income_Read") ||
                     userPermissions.Contains("Income_Update") ||
                     userPermissions.Contains("Income_Delete") ||
                     userPermissions.Contains("Budget_Create") ||
                     userPermissions.Contains("Budget_Read") ||
                     userPermissions.Contains("Budget_Update") ||
                     userPermissions.Contains("Budget_Delete") ||
                     userPermissions.Contains("BudgetItems_Create") ||
                     userPermissions.Contains("BudgetItems_Read") ||
                     userPermissions.Contains("BudgetItems_Update") ||
                     userPermissions.Contains("BudgetItems_Delete") ||
                     userPermissions.Contains("Requisition_Create") ||
                     userPermissions.Contains("Requisition_Read") ||
                     userPermissions.Contains("Requisition_Update") ||
                     userPermissions.Contains("Requisition_Delete") ||
                     userPermissions.Contains("Expenses_Create") ||
                     userPermissions.Contains("Expenses_Read") ||
                     userPermissions.Contains("Expenses_Update") ||
                     userPermissions.Contains("Expenses_Delete"))
                {
                    pnlAccounting.Visible = true;
                    pnlBudgeting.Visible = true;
                    pnlIncome.Visible = true;
                    PnlRequisitions.Visible = true;
                    pnlExpenses.Visible = true;
                }


                if (userPermissions.Contains("TimeTable_Create") ||
                    userPermissions.Contains("TimeTable_Read"))
                {
                    //pnlTimeTable.Visible = true;
                    //pnlExamTimeTable.Visible = true;
                    //pnlTeachingTimeTable.Visible = true;
                }

                if (userPermissions.Contains("StudentsAttendance_Mark") ||
                   userPermissions.Contains("TeachersAttendance_Mark"))
                {
                    PnlAttendance.Visible = true;
                    PnlStudentAttendance.Visible = true;
                    AttendanceReport.Visible = true;
                }
                if (userPermissions.Contains("Exam_Create") ||
                    userPermissions.Contains("Exam_Read") ||
                    userPermissions.Contains("Exam_Update") ||
                    userPermissions.Contains("Exam_Delete") ||
                    userPermissions.Contains("Subject_Create") ||
                    userPermissions.Contains("Subject_Read") ||
                    userPermissions.Contains("Subject_Update") ||
                    userPermissions.Contains("Subject_Delete") ||
                    userPermissions.Contains("SubjectAllocation_Create") ||
                    userPermissions.Contains("SubjectAllocation_Read") ||
                    userPermissions.Contains("SubjectAllocation_Update") ||
                    userPermissions.Contains("Subject_Allocation_Reports") ||
                    userPermissions.Contains("SubjectAllocation_Delete"))
                {
                    pnlExam.Visible = true;
                    pnlSubjects.Visible = true;
                    pnlSubjectAllocation.Visible = true;
                    pnlSchoolReports.Visible = true;
                    pnlExamManagement.Visible = true;
                }

                if (userPermissions.Contains("Manage_Scores") ||
                    userPermissions.Contains("Manage_Assessments"))
                {
                    pnlExam.Visible = true;
                    pnlGradeManagement.Visible = true;
                    pnlAssessment.Visible = true;
                }


                if (userPermissions.Contains("Release_Exam_Results") ||
                    userPermissions.Contains("Generate_SchoolReports"))
                {
                    pnlExam.Visible = true;
                    pnlExamManagement.Visible = true;
                    pnlSchoolReports.Visible = true;
                }


                // Library Permissions
                if (userPermissions.Contains("Book_Create") ||
                    userPermissions.Contains("Book_Read") ||
                    userPermissions.Contains("Book_Update") ||
                    userPermissions.Contains("Book_Update") ||
                    userPermissions.Contains("Library_Reports") ||
                    userPermissions.Contains("Book_Return") ||
                    userPermissions.Contains("Shelf_Create") ||
                    userPermissions.Contains("Book_Borrow") ||
                    userPermissions.Contains("Book_Delete"))
                {
                    pnlBooksManagement.Visible = true;
                    pnlBorrowBooks.Visible = true;
                    pnlLibrary.Visible = true;
                    pnlLibraryInventory.Visible = true;
                    pnlLibraryStockCount.Visible = true;
                    pnlLocations.Visible = true;
                    pnlBarcode.Visible = true;
                    pnlbookcategory.Visible = true;
                }

                if (userPermissions.Contains("FeesCollection_Create") ||
                    userPermissions.Contains("FeesCollection_Read") ||
                    userPermissions.Contains("FeesCollection_Update") ||
                    userPermissions.Contains("FeesCollection_Delete") ||
                    userPermissions.Contains("FeesCollection_Reports") ||
                    userPermissions.Contains("FeesCollection_Reversal"))
                {
                    pnlFees.Visible = true;
                    pnlFeesCollectionReports.Visible = true;
                    pnlInvoices.Visible = true;
                    pnlBulkInvoices.Visible = true;
                    pnlPrintInvoices.Visible = true;
                    pnlFeesCollection.Visible = true;
                    pnlFeesCollectionOthers.Visible = true;
                }

                if (userPermissions.Contains("Users_Create") ||
                    userPermissions.Contains("Users_Read") ||
                    userPermissions.Contains("Users_Update") ||
                    userPermissions.Contains("Users_Delete") ||
                    userPermissions.Contains("Role_Create") ||
                    userPermissions.Contains("Role_Read") ||
                    userPermissions.Contains("Role_Update") ||
                    userPermissions.Contains("Role_Delete") ||
                    userPermissions.Contains("Permission_Create") ||
                    userPermissions.Contains("Permission_Read") ||
                    userPermissions.Contains("Permission_Update") ||
                    userPermissions.Contains("Permission_Delete") ||
                    userPermissions.Contains("Reset_Password") ||
                    userPermissions.Contains("Reset_Student_Password"))
                {
                    pnlUser.Visible = pnlUsers.Visible = pnlRoles.Visible = pnlPasswordReset.Visible =
                    pnlStudentPasswordReset.Visible = pnlPermissions.Visible = true;
                }

                if (userPermissions.Contains("Term_Create") ||
                    userPermissions.Contains("Term_Read") ||
                    userPermissions.Contains("Term_Update") ||
                    userPermissions.Contains("Term_Delete") ||
                    userPermissions.Contains("AcademicYear_Create") ||
                    userPermissions.Contains("AcademicYear_Read") ||
                    userPermissions.Contains("AcademicYear_Update") ||
                    userPermissions.Contains("AcademicYear_Delete") ||
                    userPermissions.Contains("Class_Create") ||
                    userPermissions.Contains("Class_Read") ||
                    userPermissions.Contains("Class_Update") ||
                    userPermissions.Contains("Class_Delete") ||
                    userPermissions.Contains("TermNumber_Create") ||
                    userPermissions.Contains("TermNumber_Read") ||
                    userPermissions.Contains("TermNumber_Update") ||
                    userPermissions.Contains("TermNumber_Delete"))
                {
                    pnlSettings.Visible = pnlClasses.Visible = pnlTerms.Visible =pnlClassStream.Visible=
                    pnlAcademicYears.Visible = pnlGradingSystem.Visible= true;
                }


                if (userPermissions.Contains("GradingSystem_Management") ||

                    userPermissions.Contains("GradingSystem_Management"))
                {
                    pnlSettings.Visible  = pnlGradingSystem.Visible = true;
                }


                if (userPermissions.Contains("FeesStructure_Create") ||
    userPermissions.Contains("FeesStructure_Read") ||
    userPermissions.Contains("FeesStructure_Update") ||
    userPermissions.Contains("FeesStructure_Delete"))
                {
                    pnlSettings.Visible = pnlFeesStructure.Visible = true;
                }


                if (userPermissions.Contains("GradingSystem_Update") ||
                    userPermissions.Contains("GradingSystem_Create") ||
                    userPermissions.Contains("GradingSystem_Read") ||
                    userPermissions.Contains("GradingSystem_Delete"))
                {
                    pnlSettings.Visible = pnlGradingSystem.Visible = true;
                }
            }
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            string absoluteUrl = Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/Views/Admin/UserLogin.aspx");
            Response.Redirect(absoluteUrl);
        }
    }
}


