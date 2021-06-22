using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Zealous.Models.DB;
using Microsoft.AspNetCore.Session;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;




namespace Zealous.Helper
{
    public static class Helper 
    {
        public static string Encrypt(string plainText)
        {
            string key = "jdsg432387#";
            byte[] EncryptKey = { };
            byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
            EncryptKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByte = Encoding.UTF8.GetBytes(plainText);
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, des.CreateEncryptor(EncryptKey, IV), CryptoStreamMode.Write);
            cStream.Write(inputByte, 0, inputByte.Length);
            cStream.FlushFinalBlock();
            string toBase64 = Convert.ToBase64String(mStream.ToArray());
            return toBase64.Replace("+", "oasxxx");
        }

        public static long Decrypt(string encryptedText)
        {
            try
            {

                encryptedText = encryptedText.Replace("oasxxx", "+");
                string key = "jdsg432387#";
                byte[] DecryptKey = { };
                byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
                byte[] inputByte = new byte[encryptedText.Length];

                DecryptKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByte = Convert.FromBase64String(encryptedText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(DecryptKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByte, 0, inputByte.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return Int64.Parse(encoding.GetString(ms.ToArray()));
            }
            catch (Exception exp)
            {
                return 0;
            }
        }

        public static long CalculateCostOfSeat(long FlightId)
        {

            return 0;
        }

        public static string GetTimeinSecs(DateTime? Fisrt)
        {
            DateTime startTime = Convert.ToDateTime( Fisrt);

            DateTime endTime = DateTime.Now;

            TimeSpan span = endTime.Subtract(startTime);

            if (span.Days != 0)
            {
                return span.Days + "d";
            }
            else if(span.Hours != 0)
            {
                return span.Hours + "h";
            }
            else if (span.Minutes != 0)
            {
                return span.Minutes + "m";
            }
            else 
            {
                return span.Seconds + "s";
            }

        }

        public static int CheckFAA(string UserId)
        {
            ZealousContext zc = new ZealousContext();
            var user = zc.User.Find(Convert.ToInt64(UserId));
            return user.Faa;
        }

        public static int MessageCount(string UserId)
        {
            ZealousContext zc = new ZealousContext();
            long userId = Convert.ToInt64(UserId);
            var n = zc.ChatPermission.Include(c => c.User1Navigation).Include(c => c.User2Navigation).Where(m => m.User1 == userId && m.IsSeen == 0).Count();
            return n;
           
        }
        public static int NotificationCount(string UserId)
        {
            ZealousContext zc = new ZealousContext();
            long userId = Convert.ToInt64(UserId);
            var na = zc.Notifications.Include(n => n.Flight).Include(n => n.Member).Include(n => n.Pilot).Where(m => m.PilotId == userId && m.Status == 0 && m.Flight.DateOfFlight > DateTime.Now).Count();
            return na;
          
        }

        public static bool CheckPayment(long userId)
        {
            ZealousContext zc = new ZealousContext();
           
            var na = zc.Payment.Where(x=>x.UserId==userId && x.FromDate<DateTime.Now && x.ToDate>DateTime.Now).LastOrDefault();
            if (na == null)
            {
                return false;
            }
            else
            {
                return true;
            }
            

        }

        public static string GetFileNameFromPath(string strFileNameWithPath)
        {
            int lastIndex = strFileNameWithPath.LastIndexOf('\\');
            string strFileName = strFileNameWithPath.Substring(lastIndex + 1);
            return strFileName;

        }

        public static string Delay(int time)
        {
            for (int i = 0; i <= time; i++)
            {
                System.Threading.Thread.Sleep(1000);
            }
            return "";
        }
    }
}
