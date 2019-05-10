using NM_Tool.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace NM_Tool.Extension
{
    /// <summary>
    /// 常用转换
    /// </summary>
    public static class ConvExtension
    {

        public static Int16 ToInt16(this object obj)
        {
            Int16 result = 0;
            if (obj == null)
            {
                result = 0;
            }
            else
            {
                if (!Int16.TryParse(obj.ToString(), out result))
                {
                    decimal dec = obj.ToDecimal();

                    result = Decimal.ToInt16(dec);
                }
            }
            return result;
        }

        public static Int32 ToInt32(this object obj)
        {
            Int32 result = 0;

            if (obj == null)
            {
                result = 0;
            }
            else
            {
                if (!Int32.TryParse(obj.ToString(), out result))
                {
                    decimal dec = obj.ToDecimal();

                    result = Decimal.ToInt32(dec);
                }
            }

            return result;
        }

        public static Int64 ToInt64(this object obj)
        {
            Int64 result = 0;

            if (obj == null)
            {
                result = 0;
            }
            else
            {
                if (!Int64.TryParse(obj.ToString(), out result))
                {
                    decimal dec = obj.ToDecimal();

                    result = dec.ToInt64();
                }
            }


            return result;
        }

        public static decimal ToDecimal(this object obj)
        {
            decimal result = 0;

            if (obj == null)
            {
                result = 0;
            }
            else
            {
                decimal.TryParse(obj.ToString(), out result);
            }

            return result;
        }

        public static double ToDouble(this object obj)
        {
            double value = 0.0;
            if (obj == null)
            {
                value = 0.0;
            }
            else
            {
                double.TryParse(obj.ToString(), out value);
            }
            return value;
        }

        public static float ToSingle(this object obj)
        {
            float value = 0;
            if (obj == null)
            {
                value = 0;
            }
            else
            {
                float.TryParse(obj.ToString(), out value);
            }
            return value;
        }

        public static char ToChar(this object obj)
        {
            char result = ' ';

            if (obj == null)
            {
                result = ' ';
            }
            else
            {
                if (string.IsNullOrEmpty(obj.ToString()))
                {
                    result = ' ';
                }
                else
                {
                    result = Convert.ToChar(obj.ToString());
                }
            }

            return result;
        }

        public static bool ToBool(this object obj)
        {
            bool flag = false;

            if (obj == null)
            {
                flag = false;
            }
            else
            {
                if (string.IsNullOrEmpty(obj.ToString()))
                {
                    flag = false;
                }
                else
                {
                    bool.TryParse(obj.ToString(), out flag);
                }
            }

            return flag;
        }

        public static DateTime ToDateTime(this object obj)
        {
            DateTime time = DateTime.MinValue;

            if (obj == null)
            {
                time = DateTime.MinValue;
            }
            else
            {
                if (!DateTime.TryParse(obj.ToString(), out time))
                {
                    time = Convert.ToDateTime(obj.ToString());
                }
            }

            return time;
        }

        public static string Toyyyy_MM_dd_HH_mm_ss(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string Toyyyy_MM_dd(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 压缩字节
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] CompressBytes(this byte[] bytes)
        {
            using (MemoryStream compressStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(compressStream, CompressionMode.Compress))
                    zipStream.Write(bytes, 0, bytes.Length);
                return compressStream.ToArray();
            }
        }

        /// <summary>
        /// 解压缩字节
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Decompress(this byte[] bytes)
        {
            using (var compressStream = new MemoryStream(bytes))
            {
                using (var zipStream = new GZipStream(compressStream, CompressionMode.Decompress))
                {
                    using (var resultStream = new MemoryStream())
                    {
                        zipStream.CopyTo(resultStream);
                        return resultStream.ToArray();
                    }
                }
            }
        }

        #region 序列化
        /// <summary>
        /// 序列化称成文件 保存本地
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="name">文件名称</param>
        public static void SerializableObject<T>(this T obj, string name)
        {
            if (!Directory.Exists(Application.StartupPath + "\\Resource\\"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\Resource\\");
            }
            string file = Application.StartupPath + "\\Resource\\" + name + ".obj";
            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, obj);
            }
        }

        /// <summary>
        /// 从本地文件里反序列成文件
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        public static T DeSerializableObject<T>(this T obj, string name)
        {
            if (!Directory.Exists(Application.StartupPath + "\\Resource\\"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\Resource\\");
            }
            string file = Application.StartupPath + "\\Resource\\" + name + ".obj";
            if (File.Exists(file))
            {
                //不会锁文件
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    obj = (T)bf.Deserialize(fs);
                }
            }
            return obj;

        }

        #endregion

        #region 格式化字符串
        public static string FormatWith(this string format, params object[] objs)
        {
            return string.Format(format, objs);
        }
        public static string FormatWith(this string format, IFormatProvider fp, params object[] objs)
        {
            return string.Format(fp, format, objs);
        }
        #endregion

    }
}
