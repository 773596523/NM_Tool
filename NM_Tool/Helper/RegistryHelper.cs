using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace NM_Tool.Helper
{
    /// <summary>
    /// 注册表
    /// </summary>
    public class RegistryHelper
    {
        /// <summary>
        /// 创建一个注册表项
        /// </summary>
        public void CreateRegistFile(string RegistFileName, string ParentName = "SOFTWARE")
        {
            //SOFTWARE在LocalMachine分支下
            RegistryKey key = Registry.LocalMachine.OpenSubKey(ParentName, true);
            RegistryKey software = key.CreateSubKey(RegistFileName);
        }

        /// <summary>
        /// 删除注册表
        /// </summary>
        public void DelRegistFile(string RegistFileName, string ParentName = "SOFTWARE")
        {
            RegistryKey key = Registry.LocalMachine;
            RegistryKey software = key.OpenSubKey(ParentName, true);
            software.DeleteSubKey(RegistFileName, true);
        }

        /// <summary>
        /// 添加键和值
        /// </summary>
        public void WriteToRegistFile(Dictionary<string, string> dic,
            string RegistFileName, string ParentName = "SOFTWARE")
        {
            RegistryKey software = Registry.LocalMachine.OpenSubKey(ParentName, true);
            RegistryKey test = software.OpenSubKey(RegistFileName, true);

            foreach (string str in dic.Keys)
            {
                test.SetValue(str, dic[str]);
            }
        }

        /// <summary>
        /// 读取键和值
        /// </summary>
        public string ReadToRegistFile(string keys,
            string RegistFileName, string ParentName = "SOFTWARE")
        {
            RegistryKey software = Registry.LocalMachine.OpenSubKey(ParentName, true);
            RegistryKey test = software.OpenSubKey(RegistFileName, true);

            string str = test.GetValue(keys).ToString();
            return str;
        }

        /// <summary>
        /// 删除键和值
        /// </summary>
        public void DeleteToResgist(string RegistFileName, string key, string ParentName = "SOFTWARE")
        {
            RegistryKey software = Registry.LocalMachine.OpenSubKey(ParentName, true);
            RegistryKey test = software.OpenSubKey(RegistFileName, true);

            test.DeleteValue(key);
        }

        /// <summary>
        /// 判断注册表项是否存在
        /// </summary>
        /// <returns>bool</returns>
        public bool IsRegeditItemExist(string RegistFileName, string ParentName = "SOFTWARE")
        {
            string[] subkeyNames;
            RegistryKey key = Registry.LocalMachine;
            RegistryKey software = key.OpenSubKey(ParentName);

            subkeyNames = software.GetSubKeyNames();

            foreach (string keyName in subkeyNames)
            {
                if (keyName == RegistFileName)
                {
                    key.Close();
                    return true;
                }
            }
            key.Close();
            return false;
        }

        /// <summary>
        /// 判断该路径是否已经存在
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public bool IsRegeditKeyExit(string RegistFileName, string name, string ParentName = "SOFTWARE")
        {
            string[] saveSubkeyNames;

            RegistryKey key = Registry.LocalMachine;
            RegistryKey software = key.OpenSubKey(ParentName, true);
            RegistryKey test = software.OpenSubKey(RegistFileName, true);

            //获取该子项下的所有键值的名称saveSubkeyNames 
            saveSubkeyNames = test.GetValueNames();

            foreach (string keyName in saveSubkeyNames)
            {
                if (keyName == name)
                {
                    key.Close();
                    return true;
                }
            }
            key.Close();
            return false;
        }
    }
}
