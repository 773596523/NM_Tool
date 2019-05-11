using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
        public static string filepath = AppSetting.path + "\\log\\";

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
            catch (Exception)
            {
                //throw ex;
            }
        }
        public static void writeLog(string title, string msg)
        {
            try
            {
                string fileName = DateTime.Now.ToString("yyyyMMddHH") + ".txt";
                List<string> mesList = new List<string>();

                mesList.Add("**************************" + title + "   " + DateTime.Now.ToString() + "*************************");
                mesList.Add(msg);

                File.AppendAllLines(filepath + fileName, mesList, Encoding.UTF8);
            }
            catch (Exception)
            {
                //throw ex;
            }
        }
        public static void writeLog(string position, string message, params string[] paras)
        {
            string fileName = DateTime.Now.ToString("yyyyMMddHH") + ".txt";
            List<string> mesList = new List<string>();
            mesList.Add("");
            mesList.Add("");
            mesList.Add("**************************" + position + DateTime.Now.ToString() + "*************************");
            if (paras != null)
            {
                for (var i = 0; i < paras.Length; i++)
                {
                    mesList.Add("Paramter " + i.ToString() + ":" + paras[i]);
                }
            }
            mesList.Add(message);
            mesList.Add("**************************************************************************************");

            File.AppendAllLines(filepath + fileName, mesList, Encoding.UTF8);
        }
        public static void writeLog(Exception ex)
        {
            writeLog(ex.ToString());
        }
    }
}
