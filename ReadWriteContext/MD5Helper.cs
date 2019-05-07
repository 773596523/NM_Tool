using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace ReadWriteContext
{
    public class MD5Helper
    {
        public static string ToMD5(byte[] data)
        {
            var md = new MD5CryptoServiceProvider();
            var data2 = md.ComputeHash(data);
            var v = BitConverter.ToString(data2);
            v = v.Replace("-", "");
            v = v.ToUpper();
            return v;
        }

        public static string ToMD5(string value)
        {
            byte[] data = System.Text.Encoding.Default.GetBytes(value);
            var md = new MD5CryptoServiceProvider();
            var data2 = md.ComputeHash(data);
            var v = BitConverter.ToString(data2);
            v = v.Replace("-", "");
            v = v.ToUpper();
            return v;
        }

        public static string NewMd5(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("X2");

            }
            return pwd;
        }
    }
}
