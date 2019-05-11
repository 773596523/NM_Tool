using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NM_Tool.Helper
{
    /// <summary>
    /// 操作INI
    /// </summary>
    public static class InI
    {
        public static string filePath = AppSetting.path + "\\IVYTALK.ini";

        /// <summary>
        /// 写入ini配置
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <param name="value">键值</param>
        /// <param name="filePath">路径</param>
        public static void Writue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, filePath);
        }

        /// <summary>
        /// 写入ini配置
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <param name="value">键值</param>
        /// <param name="filePath">路径</param>
        public static void Writue(string section, string key, string value, string filePath)
        {
            WritePrivateProfileString(section, key, value, filePath);
        }

        /// <summary>
        /// 读入
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <param name="filePath">路径</param>
        /// <returns></returns>
        public static string ReadValue(string section, string key)
        {
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", sb, 255, filePath);
            return sb.ToString();
        }

        /// <summary>
        /// 读入
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <param name="filePath">路径</param>
        /// <returns></returns>
        public static string ReadValue(string section, string key, string filePath)
        {
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", sb, 255, filePath);
            return sb.ToString();
        }



        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder Val, int size, string filePath);
    }
}
