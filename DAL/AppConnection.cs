

using System;
using System.Configuration;

namespace SMSWEBAPP.DAL
{
    internal class AppConnection
    {
        public static string GetConnectionString(string connectionStringName = "DefaultConnection")
        {
            return ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }
    }
}