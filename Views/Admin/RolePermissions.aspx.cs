using SMSWEBAPP.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class RolePermissions : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            string systemId = Session["SystemId"] as string;

            if (!string.IsNullOrEmpty(systemId))
            {
                switch (systemId)
                {
                    case "1":
                        this.MasterPageFile = "~/Views/Admin/AdminMaster.Master";
                        break;
                    case "2":
                        this.MasterPageFile = "~/POS/AdminMaster.Master";
                        break;
                    case "3":
                        this.MasterPageFile = "~/AMS/AdminMaster.Master";
                        break;
                    case "4":
                        this.MasterPageFile = "~/CMS/AdminMaster.Master";
                        break;
                    case "6":
                        this.MasterPageFile = "~/UBN/AdminMaster.Master";
                        break;

                    default:
                        this.MasterPageFile = "~/POS/AdminMaster.Master";
                        break;
                }
            }
            else
            {
                // Optionally redirect to login page if session is missing
                this.MasterPageFile = "~/POS/AdminMaster.Master";
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRoles();
                LoadModules(); // Load modules sorted by permission count
            }
        }

        private void LoadRoles()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT RoleID, RoleTitle FROM Roles Where School=@SchoolId", Con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                SqlDataReader dr = cmd.ExecuteReader();
                ddlRoles.DataSource = dr;
                ddlRoles.DataTextField = "RoleTitle";
                ddlRoles.DataValueField = "RoleID";
                ddlRoles.DataBind();
            }
            ddlRoles.Items.Insert(0, new ListItem("-- Select Role --", "0"));
        }

        private void LoadModules()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                // Get modules sorted by number of permissions in descending order
                string query = @"
                    SELECT m.ModuleID, m.ModuleName, COUNT(p.PermissionID) AS PermissionCount
FROM Module m
JOIN Permission p ON m.ModuleID = p.ModuleID
inner join Systems s on M.SystemId=s.SystemId
where (m.systemid=5 or m.SystemId=@SystemId) and m.ModuleId !=9
GROUP BY m.ModuleID, m.ModuleName
ORDER BY COUNT(p.PermissionID) DESC";  // Sorting by permission count

                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@SystemId", Session["SystemId"]);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptModules.DataSource = dt;
                rptModules.DataBind();
            }
        }

        protected void rptModules_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                int moduleID = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "ModuleID"));
                CheckBoxList cblPermissions = (CheckBoxList)e.Item.FindControl("cblModulePermissions");

                using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT PermissionID, Permission FROM Permission WHERE ModuleID = @ModuleID", Con);
                    cmd.Parameters.AddWithValue("@ModuleID", moduleID);

                    SqlDataReader dr = cmd.ExecuteReader();

                    cblPermissions.DataSource = dr;
                    cblPermissions.DataTextField = "Permission";
                    cblPermissions.DataValueField = "PermissionID";
                    cblPermissions.DataBind();
                }
            }
        }

        protected void ddlRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlRoles.SelectedValue != "0")
            {
                LoadModules(); // Reload modules
                MarkAssignedPermissions();
            }
        }

        private void MarkAssignedPermissions()
        {
            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("SELECT PermissionID FROM RolePermission WHERE RoleID = @RoleID", Con);
                cmd.Parameters.AddWithValue("@RoleID", ddlRoles.SelectedValue);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    foreach (RepeaterItem item in rptModules.Items)
                    {
                        CheckBoxList cblPermissions = (CheckBoxList)item.FindControl("cblModulePermissions");
                        ListItem permissionItem = cblPermissions.Items.FindByValue(dr["PermissionID"].ToString());
                        if (permissionItem != null)
                        {
                            permissionItem.Selected = true;
                        }
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ddlRoles.SelectedValue == "0")
            {
                return; // Ensure a role is selected
            }

            int roleID = int.Parse(ddlRoles.SelectedValue);

            using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                Con.Open();
                SqlTransaction transaction = Con.BeginTransaction();

                try
                {
                    // Clear existing permissions for the role
                    SqlCommand cmdDelete = new SqlCommand("DELETE FROM RolePermission WHERE RoleID = @RoleID", Con, transaction);
                    cmdDelete.Parameters.AddWithValue("@RoleID", roleID);
                    cmdDelete.ExecuteNonQuery();

                    // Insert new permissions
                    foreach (RepeaterItem item in rptModules.Items)
                    {
                        CheckBoxList cblPermissions = (CheckBoxList)item.FindControl("cblModulePermissions");

                        foreach (ListItem permission in cblPermissions.Items)
                        {
                            if (permission.Selected)
                            {
                                SqlCommand cmdInsert = new SqlCommand("INSERT INTO RolePermission (RoleID, PermissionID, SchoolId, CreatedBy) VALUES (@RoleID, @PermissionID, @SchoolId, @CreatedBy)", Con, transaction);
                                cmdInsert.Parameters.AddWithValue("@RoleID", roleID);
                                cmdInsert.Parameters.AddWithValue("@PermissionID", permission.Value);
                                cmdInsert.Parameters.AddWithValue("@CreatedBy", Session["Username"]);
                                cmdInsert.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                                cmdInsert.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();

                    // Show success message
                    ScriptManager.RegisterStartupScript(this, GetType(), "SuccessMessage", "alert('Permission assignment saved successfully!');", true);
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }
    }
}
