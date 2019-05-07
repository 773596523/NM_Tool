using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace ReadWriteContext
{
    public class WriteContextByXML : IWriteContext
    {

        private System.Text.StringBuilder sb = new System.Text.StringBuilder();


        void IWriteContext.Append(string key, string value)
        {
            if (sb.Length != 0)
            {
                sb.Append("\r\n");
            }
            value = value.Replace("<", "!@#").Replace(">", "#@!");
            sb.Append("<" + key + ">");
            sb.Append(value);
            sb.Append("</" + key + ">");
        }

        void IWriteContext.Append(string[] keys, string[] values)
        {
            if (sb.Length != 0)
            {
                sb.Append("\r\n");
            }
            for (int i = 0; i <= keys.Length - 1; i++)
            {
                if (i != 0)
                {
                    sb.Append("\r\n");
                }
                sb.Append("<" + keys[i] + ">");
                sb.Append(values[i]);
                sb.Append("</" + keys[i] + ">");
            }
        }

        void IWriteContext.Append(string key, string[] subkeys, string[] subvalues)
        {
            System.Data.DataTable dt = new DataTable();

            for (int i = 0; i <= subkeys.Length - 1; i++)
            {
                dt.Columns.Add(subkeys[i]);
            }
            System.Data.DataRow row = dt.NewRow();
            for (int i = 0; i <= subvalues.Length - 1; i++)
            {
                row[i] = subvalues[i];
            }
            dt.Rows.Add(row);
            //
            IWriteContext ins = this;
            ins.Append(key, dt);
        }

        void IWriteContext.Append(string key, DataTable dt)
        {
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                DataRow row = dt.Rows[i];
                if (sb.Length != 0)
                {
                    sb.Append("\r\n");
                }
                sb.Append("<" + key + ">");
                sb.Append("\r\n");
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    sb.Append("<" + dt.Columns[j].ColumnName + ">");
                    sb.Append(DataTabelCellToString(row[j], dt.Columns[j].DataType));
                    sb.Append("</" + dt.Columns[j].ColumnName + ">");
                    sb.Append("\r\n");
                }
                sb.Append("</" + key + ">");
            }
        }

        string IWriteContext.ToString()
        {
            return "<xml>" + "\r\n" + sb.ToString() + "\r\n" + "</xml>";
        }

        void IWriteContext.Clear()
        {
            sb.Clear();
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
            this.sb = new System.Text.StringBuilder(str);
        }
    }
}