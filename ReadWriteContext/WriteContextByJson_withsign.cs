using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ReadWriteContext
{
    public class WriteContextByJson_withsign : IWriteContext
    {
        private System.Text.StringBuilder sb = new System.Text.StringBuilder();
        private System.Text.StringBuilder sbSign = new StringBuilder();
        /// <summary>    
        /// 转换特殊字符    
        /// </summary>    
        /// <param name="s"></param>      
        private static string ConvertJsonPropertyValue(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        void IWriteContext.Append(string key, string value)
        {
            if (sb.Length != 0)
            {
                sb.Append(",");
            }
            sb.Append("\"" + key + "\"");
            sb.Append(":");
            sb.Append("\"" + ConvertJsonPropertyValue(value) + "\"");
            sbSign.Append(value);
        }

        void IWriteContext.Append(string[] keys, string[] values)
        {
            if (sb.Length != 0)
            {
                sb.Append(",");
            }
            for (int i = 0; i <= keys.Length - 1; i++)
            {
                if (i != 0)
                {
                    sb.Append(",");
                }
                sb.Append("\"" + keys[i] + "\"");
                sb.Append(":");
                sb.Append("\"" + ConvertJsonPropertyValue(values[i]) + "\"");
                sbSign.Append(values[i]);
            }
        }

        void IWriteContext.Append(string key, string[] subkeys, string[] subvalues)
        {
            IWriteContext ins = new WriteContextByJson();
            ins.Append(subkeys, subvalues);
            if (sb.Length != 0)
            {

                sb.Append(",");

            }
            sb.Append("\"" + key + "\":" + ins.ToString());
        }

        void IWriteContext.Append(string key, DataTable dt)
        {
            //
            System.Text.StringBuilder sb2 = new System.Text.StringBuilder();
            sb2.Append("[");
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                if (i != 0)
                {
                    sb2.Append(",");
                }
                DataRow row = dt.Rows[i];
                sb2.Append("{");
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    if (j != 0)
                    {
                        sb2.Append(",");
                    }
                    sb2.Append("\"" + dt.Columns[j].ColumnName + "\"");
                    sb2.Append(":");
                    string value = DataTabelCellToString(row[j], dt.Columns[j].DataType);
                    value = ConvertJsonPropertyValue(value);
                    sb2.Append("\"" + value + "\"");
                    sbSign.Append(value);
                }
                sb2.Append("}");
            }
            sb2.Append("]");
            //
            if (sb.Length != 0)
            {
                sb.Append(",");
            }
            sb.Append("\"" + key + "\":" + sb2.ToString());
        }

        string IWriteContext.ToString()
        {
            IWriteContext ins = this;
            ins.Append("sign", sbSign.ToString());
            return "{" + sb.ToString() + "}";
        }



        void IWriteContext.Clear()
        {
            sb.Clear();
            sbSign.Clear();
        }

        private string DataTabelCellToString(object cellValue, Type cellType)
        {
            if (cellValue == DBNull.Value)
            {
                return "";
            }
            if (cellType == typeof(DateTime))
            {
                DateTime dt = (DateTime)cellValue;
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                return cellValue.ToString();
            }
        }


        void IWriteContext.SetStr(string str)
        {
            throw new NotImplementedException();
        }
    }
}
