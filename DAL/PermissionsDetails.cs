using SMSWEBAPP.DAL;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class PermissionsDetails
{
    public static List<string> AllPermissions = new List<string>(); // Store permission names as strings

    // Method to fetch and load permissions
    public void GetPermissions(int userId)
    {
        string ShowData = @"SELECT Permission FROM RolePermission rp 
                            INNER JOIN roles R ON rp.roleid = R.roleid
                            INNER JOIN Permission p ON rp.permissionid = p.permissionid 
                            WHERE rp.roleid = @Condition";

        using (SqlConnection Con = new SqlConnection(AppConnection.GetConnectionString()))
        {
            using (SqlCommand Cmd = new SqlCommand(ShowData, Con))
            {
                Cmd.Parameters.AddWithValue("@Condition",LoggedInUser.RoleId);

                if (Con.State != ConnectionState.Open)
                {
                    Con.Open();
                }

                using (SqlDataReader Sdr = Cmd.ExecuteReader())
                {
                    if (Sdr.HasRows)
                    {
                        AllPermissions.Clear(); // Clear old permissions

                        while (Sdr.Read())
                        {
                            // Add each permission name to the list
                            AllPermissions.Add(Sdr["Permission"].ToString());
                        }
                    }
                }
            }
        }
    }
}
