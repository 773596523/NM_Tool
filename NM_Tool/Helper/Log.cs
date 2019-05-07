using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace NM_Tool.Helper
{
   /// <summary>
   /// 日志
   /// </summary>
    public class Log
    {
        static Log()
        {
            if (Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
        }
        public static string filepath = Application.StartupPath + "\\log\\";
        public static void writeLog(string msg)
        {
            try
            {
                string fileName = DateTime.Now.ToString("yyyyMMddHH") + ".txt";
                List<string> mesList = new List<string>();

                mesList.Add("**************************" + DateTime.Now.ToString() + "*************************");
                mesList.Add(msg);

                File.AppendAllLines(filepath + fileName, mesList, Encoding.UTF8);
            }
            catch (Exception )
            {
                //throw ex;
            }
        }

        public static void writeLog(Exception ex)
        {
            writeLog(ex.ToString());
        }
    }
}
