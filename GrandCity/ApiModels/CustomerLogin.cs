using MeherEstateDevelopers.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MeherEstateDevelopers.ApiModels
{

    public class CustomerLogin
    {
        private Grand_CityEntities db = new Grand_CityEntities();
        public string Name { get; set; }
        public string Father_Husband { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CNIC { get; set; }
        public string Password { get; set; }


        public bool Add(string Name, string FatherHusband, string Email, string Phone, string Cnic, string Password)
        {
            if (!string.IsNullOrEmpty(Password))
            {

                db.Add_CustomerLogin(Name, FatherHusband, Email, Phone, Cnic, Password);
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool Register(string Name, string FatherHusband, string Email, string Phone, string Cnic, string Password)
        {
            if (!string.IsNullOrEmpty(Password))
            {
                Password = Encrypt(Password);
                int otpnumber = OTP_Number();
                db.Add_CustomerRegisteration(Name, FatherHusband, Email, Phone, Cnic, Password, otpnumber);
                string otp = otpnumber.ToString();
                string message = "This is an automated SMS from website / app url Your verification pin is "+otp+".Please enter the PIN for instant verification. Thank you!";
                SmsService s = new SmsService();
                s.SendMsg(message, Phone);
                return true;
            }
            else
            {
                return false;
            }

        }
        public int OTP_Number()
        {
            Random generator = new Random();
            int number = Convert.ToInt32(generator.Next(1, 10000).ToString("D4"));
            return number;
        }
        public static string Encrypt(string plainText)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(plainText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    plainText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return plainText;
        }
        public static string Dcrypt(string cipherText)
        {
            string EncryptionKey = "abc123";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {

                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

    }
}
