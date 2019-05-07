using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace NM_Tool.Encryption
{
    /// <summary>
    /// MD5加密
    /// </summary>
   public class MD5
    {
       public static string ToMD5(string value)
       {
           if (value == null || value == "")
           {
               return "";
           }
           byte[] data = System.Text.Encoding.Default.GetBytes(value);
           var md = new MD5CryptoServiceProvider();
           var data2 = md.ComputeHash(data);
           var v = BitConverter.ToString(data2);
           v = v.Replace("-", "");
           v = v.ToLower();
           return v;
       }

       public static string get_md5_vercode(string app_id, long timespan, string mer_no, string soft_key)
       {
           var str = timespan.ToString().Substring(0, 4) + "app_id:" + app_id + "mer_no:" + mer_no + "soft_key:" + soft_key + timespan.ToString().Substring(4);
           string md5_str = ToMD5(str);

           return md5_str;
       }
    }
}
