using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SMSWEBAPP.DAL
{
    public class SecureData
    {
        public static string EncryptData(string simplestring)
        {
            MD5 mD5 = new MD5CryptoServiceProvider();
            byte[] PasswordHash = Encoding.UTF8.GetBytes(simplestring);
            return Encoding.UTF8.GetString(mD5.ComputeHash(PasswordHash));



        }
    }
}