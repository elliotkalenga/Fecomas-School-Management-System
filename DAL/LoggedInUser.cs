using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMSWEBAPP.DAL
{
    public class LoggedInUser
    {
        public static string Username { get; set; }
        public static string FirstName { get; set; }
        public static string LastName { get; set; }
        public static string SchoolName { get; set; }
        public static string StudentId { get; set; }
        public static string StudentNo { get; set; }
        public static string SchoolCode { get; set; }
        public static string RoleTitle { get; set; }
        public static string UserId { get; set; }
        public static string SystemId { get; set; }
        public static int RoleId { get; set; }
        public static int LicensedDay { get; set; }
        public static int UsedDays { get; set; }
        public static int RemainingDays { get; set; }
        public static string LicenseStatus { get; set; }
        public static DateTime StartDate { get; set; }
        public static DateTime EndDate{ get; set; }
        public static string SchoolId { get; set; }
        public static string Address { get; set; }
        public static List<string> Permissions { get; set; }
    }
}