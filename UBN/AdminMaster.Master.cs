using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.UBN
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
                int roleId = Convert.ToInt32(Session["RoleId"]);

                // Products menu visibility with granular permission checks
                if (userPermissions.Contains("Product_Create") ||
                    userPermissions.Contains("Product_Read") ||
                    userPermissions.Contains("Product_Update") ||
                    userPermissions.Contains("Product_Delete") ||
                    userPermissions.Contains("Category_Manage") ||
                    userPermissions.Contains("Costing_Manage") ||
                    userPermissions.Contains("Pricing_Manage") ||
                    userPermissions.Contains("Inventory_Manage") ||
                    userPermissions.Contains("StockCount_Manage"))
                {
                    pnlProduct.Visible = true;

                    pnlProducts.Visible = userPermissions.Contains("Product_Create") ||
                                          userPermissions.Contains("Product_Read") ||
                                          userPermissions.Contains("Product_Update") ||
                                          userPermissions.Contains("Product_Delete");

                    pnlCategories.Visible = userPermissions.Contains("Category_Manage");
                    pnlCosting.Visible = userPermissions.Contains("Costing_Manage");
                    pnlPricing.Visible = userPermissions.Contains("Pricing_Manage");
                    pnlInventory.Visible = userPermissions.Contains("Inventory_Manage");
                    pnlStockCount.Visible = userPermissions.Contains("StockCount_Manage");
                }


                if (userPermissions.Contains("PurchaseOrder_Manage") ||
                      userPermissions.Contains("Supplier_Manage") ||
                      userPermissions.Contains("ReceivedGoods_Manage"))
                {
                    pnlPurchase.Visible = true;
                    pnlPurchaseOrders.Visible = userPermissions.Contains("PurchaseOrder_Manage");
                    pnlSuppliers.Visible = userPermissions.Contains("Supplier_Manage");
                    pnlReceivedGoods.Visible = userPermissions.Contains("ReceivedGoods_Manage");
                }

                // Sales menu visibility
                if (userPermissions.Contains("SalesInvoice_Manage") ||
                    userPermissions.Contains("Customer_Manage") ||
                    userPermissions.Contains("Receipt_Manage"))
                {
                    pnlSales.Visible = true;
                    pnlSalesInvoices.Visible = userPermissions.Contains("SalesInvoice_Manage");
                    pnlCustomers.Visible = userPermissions.Contains("Customer_Manage");
                    pnlReceipts.Visible = userPermissions.Contains("Receipt_Manage");
                }


                if (userPermissions.Contains("Income_Create") ||
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
                    PnlTeacherAttendance.Visible = true;
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
                   pnlPermissions.Visible = true;
                }

            }
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            string absoluteUrl = Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/UBN/Login.aspx");
            Response.Redirect(absoluteUrl);
        }
    }
}


