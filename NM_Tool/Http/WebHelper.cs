using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReadWriteContext;
using System.Text;
using System.Data;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NM_Tool.Helper;
using NM_Tool.Extension;

namespace NM_Tool.Http
{
    public class WebHelper
    {
        public WebHelper()
        {
            ProcessRequest(System.Web.HttpContext.Current);
        }
        public WebHelper(HttpContext context)
        {
            ProcessRequest(context);
        }
        public WebHelper(out byte[] bytes)
        {
            ProcessRequest(System.Web.HttpContext.Current, out bytes);
        }
        private Encoding coding = Encoding.UTF8;
        private Encoding CreateCoding(HttpContext context)
        {
            return Encoding.UTF8;
        }

        public Dictionary<string, object> kv = new Dictionary<string, object>();


        private void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            coding = CreateCoding(context);
            ReadContext = CreateReadContext(context);
            //
            kv = ReadContext.ToDictionary();
        }

        private void ProcessRequest(HttpContext context, out byte[] bytes)
        {
            context.Response.ContentType = "text/plain";
            coding = CreateCoding(context);
            bytes = CreateReadByte(context);
        }

        public IReadContext ReadContext = null;

        private IReadContext CreateReadContext(HttpContext context)
        {
            var istrm = context.Request.InputStream;
            var ibuf = new byte[istrm.Length];
            istrm.Read(ibuf, 0, ibuf.Length);
            var str = coding.GetString(ibuf, 0, ibuf.Length);
            return new ReadContextByJson(str);
        }

        private byte[] CreateReadByte(HttpContext context)
        {
            var istrm = context.Request.InputStream;
            var ibuf = new byte[istrm.Length];
            istrm.Read(ibuf, 0, ibuf.Length);
            return ibuf.Decompress();
        }

        private IWriteContext WriteContext = new WriteContextByJson();
        public IWriteContext CreateWriteContext()
        {
            if (WriteContext == null)
            {
                WriteContext = new WriteContextByJson();
            }
            return WriteContext;
        }

        /// <summary>
        /// 写入json
        /// </summary>
        /// <param name="flag">标记</param>
        /// <param name="message">信息</param>
        public void Write(string flag, string message)
        {
            WriteContext.Append("Flag", flag);
            WriteContext.Append("Message", message);
        }
        public void Write(string Key, DataTable dt)
        {
            WriteContext.Append(Key, dt);
        }
        public void Write(DataTable dt)
        {
            WriteContext.Append("data", dt);
        }
        public void Write(string[] keys, string[] values)
        {
            WriteContext.Append("data", keys, values);
        }
        public void Write(string key, string[] keys, string[] values)
        {
            WriteContext.Append(key, keys, values);
        }
        public void WriteObject(string Key, string Value)
        {
            WriteContext.Append(Key, Value);
        }
        public void WriteObject(string Key, int Value)
        {
            WriteContext.Append(Key, Value.ToString());
        }
        public void WriteObject(string Key, object Value)
        {
            if (null == Value) WriteContext.Append(Key, "");
            else WriteContext.Append(Key, Value.ToString());
        }
        public void WriteResult(string value)
        {
            if (null == value) WriteContext.Append("result", "");
            else WriteContext.Append("result", value);
        }
        public void WriteResult(object value)
        {
            if (null == value) WriteContext.Append("result", "");
            else WriteContext.Append("result", value.ToString());
        }
        public void WriteSuccess(string message)
        {
            WriteContext.Append("Flag", "1");
            WriteContext.Append("Message", message);
        }
        public void WriteSuccess()
        {
            WriteContext.Append("Flag", "1");
            WriteContext.Append("Message", "成功");
        }
        public void WriteInvalidParameters(string message)
        {
            WriteContext.Append("Flag", "-4");
            WriteContext.Append("Message", message);
        }
        public void WriteError(string msg)
        {
            WriteContext.Append("Flag", "-1");
            WriteContext.Append("Message", msg);
        }
        public void WriteError(Exception ex)
        {
            Log.writeLog(ex.ToString());

            WriteContext.Append("Flag", "-1");
            WriteContext.Append("Message", ex.Message);
        }
        public void WriteStrings(string[] keys, string[] values)
        {
            WriteContext.Append(keys, values);
        }
        public void Write<T>(T t)
        {
            var ts = typeof(T);
            var pis = ts.GetProperties();

            for (int j = 0; j < pis.Length; j++)
            {
                var pi = pis[j];
                string colName = pi.Name.ToUpper();
                object obj = pi.GetValue(t, null);
                string value = obj == null ? "" : obj.ToString();

                WriteObject(colName, value);

            }
        }
        public void Write<T>(List<T> lis)
        {
            var ts = typeof(T);
            var pis = ts.GetProperties();

            DataTable dt = new DataTable();

            for (int j = 0; j < pis.Length; j++)
            {
                var pi = pis[j];
                string colName = pi.Name.ToUpper();

                dt.Columns.Add(colName);
            }


            foreach (T t in lis)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < pis.Length; j++)
                {
                    var pi = pis[j];
                    string colName = pi.Name.ToUpper();
                    object obj = pi.GetValue(t, null);
                    dr[j] = obj == null ? null : obj.ToString();
                }
                dt.Rows.Add(dr);
            }

            Write(dt);

        }
        public void Write<T>(Dictionary<string, T> dic)
        {
            var ts = typeof(T);
            var pis = ts.GetProperties();

            DataTable dt = new DataTable();
            dt.Columns.Add("key");
            for (int j = 0; j < pis.Length; j++)
            {
                var pi = pis[j];
                string colName = pi.Name.ToUpper();
                dt.Columns.Add(colName);
            }


            foreach (string key in dic.Keys)
            {
                DataRow dr = dt.NewRow();
                dr["key"] = key;
                for (int j = 0; j < pis.Length; j++)
                {
                    var pi = pis[j];
                    string colName = pi.Name.ToUpper();

                    dr[j + 1] = pi.GetValue(dic[key], null).ToString(); ;
                }
                dt.Rows.Add(dr);
            }

            Write(dt);

        }
        public void Write<T1, T2>(Dictionary<T1, T2> dic)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("key", typeof(T1));
            dt.Columns.Add("value", typeof(T2));

            foreach (T1 key in dic.Keys)
            {
                DataRow dr = dt.NewRow();
                dr["key"] = key;
                dr["value"] = dic[key];
                dt.Rows.Add(dr);
            }

            Write(dt);

        }

        /// <summary>
        /// 模型 获取DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DataTable GetDataTable(DataTable dt)
        {
            List<string> colName = new List<string>();


            foreach (IReadContext r in ReadList("data"))
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string key = dt.Columns[i].ColumnName;
                    dr[i] = Read(r, key);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public DataTable GetDataTable()
        {
            DataTable dt = new DataTable();

            foreach (IReadContext r in ReadList("data"))
            {
                Dictionary<string, object> dic = r.ToDictionary();

                if (dt.Columns.Count < 1)
                {
                    foreach (string key in dic.Keys)
                    {
                        dt.Columns.Add(key);
                    }
                }

                DataRow dr = dt.NewRow();
                foreach (string key in dic.Keys)
                {
                    int i = dt.Columns.IndexOf(key);
                    dr[i] = dic[key];
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public Dictionary<string, T> GetDic<T>()
        {
            var t = typeof(T);
            var pis = t.GetProperties();
            Dictionary<string, T> dic = new Dictionary<string, T>();

            foreach (IReadContext r in ReadList("data"))
            {
                string key = r.Read("key");
                T nm = GetObject<T>(r);//创建对象

                dic.Add(key, nm);
            }

            return dic;
        }
        public Dictionary<T1, T2> GetDic<T1, T2>()
        {
            Dictionary<T1, T2> dic = new Dictionary<T1, T2>();

            foreach (IReadContext r in ReadList("data"))
            {
                T1 t1 = (T1)(object)r.Read("key");

                T2 t2 = (T2)(object)Convert.ToDecimal(r.Read("value"));
                dic.Add(t1, t2);
            }

            return dic;
        }

        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetList<T>()
        {
            var t = typeof(T);
            var pis = t.GetProperties();
            List<T> nmlis = new List<T>();

            foreach (IReadContext r in ReadList("data"))
            {
                T nm = GetObject<T>(r);//创建对象

                nmlis.Add(nm);
            }

            return nmlis;
        }

        /// <summary>
        /// 返回json
        /// </summary>
        /// <returns></returns>
        public string NmJson()
        {
            return WriteContext.ToString();
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        public T GetObject<T>()
        {
            var t = typeof(T);
            var pis = t.GetProperties();
            T nm = (T)Activator.CreateInstance(t);
            for (int i = 0; i < pis.Length; i++)
            {
                var pi = pis[i];
                string colName = pi.Name.ToUpper();

                switch (pi.PropertyType.FullName)
                {
                    case "System.String":
                        pi.SetValue(nm, ObjectToString(colName), null);
                        break;
                    case "System.Int16":
                        pi.SetValue(nm, (short)ObjectToObject(colName), null);
                        break;
                    case "System.Int32":
                        pi.SetValue(nm, ObjectToInt(colName), null);
                        break;
                    case "System.Int64":
                        pi.SetValue(nm, (long)ObjectToObject(colName), null);
                        break;
                    case "System.Decimal":
                        pi.SetValue(nm, ObjectToDecimal(colName), null);
                        break;
                    case "System.Double":
                        pi.SetValue(nm, (double)ObjectToObject(colName), null);
                        break;
                    case "System.Float":
                        pi.SetValue(nm, (float)ObjectToObject(colName), null);
                        break;
                    //case "System.Guid":
                    //    if (row[colName] != DBNull.Value) pi.SetValue(r, (Guid)row[colName], null);
                    //    break;
                    case "System.DateTime":
                        pi.SetValue(nm, ObjectToDate(colName), null);
                        break;
                    case "System.Char":
                        pi.SetValue(nm, ObjectToChar(colName), null);
                        break;
                        //case "System.Boolean":
                        //    if (row[colName] != DBNull.Value) pi.SetValue(r, (bool)row[colName], null);
                        //    break;
                        //case "System.Byte":
                        //    if (row[colName] != DBNull.Value) pi.SetValue(r, (byte)row[colName], null);
                        //    break;
                }
            }

            return nm;
        }

        public object GetObject(object obj)
        {
            var t = obj.GetType();
            var pis = t.GetProperties();
            for (int i = 0; i < pis.Length; i++)
            {
                var pi = pis[i];
                string colName = pi.Name.ToUpper();

                switch (pi.PropertyType.FullName)
                {
                    case "System.String":
                        pi.SetValue(obj, ObjectToString(colName), null);
                        break;
                    case "System.Int16":
                        pi.SetValue(obj, (short)ObjectToObject(colName), null);
                        break;
                    case "System.Int32":
                        pi.SetValue(obj, ObjectToInt(colName), null);
                        break;
                    case "System.Int64":
                        pi.SetValue(obj, (long)ObjectToObject(colName), null);
                        break;
                    case "System.Decimal":
                        pi.SetValue(obj, ObjectToDecimal(colName), null);
                        break;
                    case "System.Double":
                        pi.SetValue(obj, (double)ObjectToObject(colName), null);
                        break;
                    case "System.Float":
                        pi.SetValue(obj, (float)ObjectToObject(colName), null);
                        break;
                    //case "System.Guid":
                    //    if (row[colName] != DBNull.Value) pi.SetValue(r, (Guid)row[colName], null);
                    //    break;
                    case "System.DateTime":
                        pi.SetValue(obj, ObjectToDate(colName), null);
                        break;
                    case "System.Char":
                        pi.SetValue(obj, ObjectToChar(colName), null);
                        break;
                        //case "System.Boolean":
                        //    if (row[colName] != DBNull.Value) pi.SetValue(r, (bool)row[colName], null);
                        //    break;
                        //case "System.Byte":
                        //    if (row[colName] != DBNull.Value) pi.SetValue(r, (byte)row[colName], null);
                        //    break;
                }
            }

            return obj;
        }
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        public T GetObject<T>(IReadContext r)
        {
            var t = typeof(T);
            var pis = t.GetProperties();
            T nm = (T)Activator.CreateInstance(t);
            for (int i = 0; i < pis.Length; i++)
            {
                var pi = pis[i];
                string colName = pi.Name.ToUpper();

                switch (pi.PropertyType.FullName)
                {
                    case "System.String":
                        pi.SetValue(nm, Read(r, colName), null);
                        break;
                    case "System.Int16":
                        pi.SetValue(nm, Read(r, colName).ToInt16(), null);
                        break;
                    case "System.Int32":
                        pi.SetValue(nm, Read(r, colName).ToInt32(), null);
                        break;
                    case "System.Int64":
                        pi.SetValue(nm, Read(r, colName).ToInt64(), null);
                        break;
                    case "System.Decimal":
                        pi.SetValue(nm, Read(r, colName).ToDecimal(), null);
                        break;
                    case "System.Double":
                        pi.SetValue(nm, Read(r, colName).ToDouble(), null);
                        break;
                    case "System.Float":
                        pi.SetValue(nm, Read(r, colName).ToSingle(), null);
                        break;
                    case "System.DateTime":
                        pi.SetValue(nm, Read(r, colName).ToDateTime(), null);
                        break;
                    case "System.Char":
                        pi.SetValue(nm, Read(r, colName).ToChar(), null);
                        break;
                }
            }

            return nm;
        }

        public string Read(string key)
        {
            return this.ReadContext.Read(key);
        }
        public List<IReadContext> ReadList(string key)
        {
            return ReadContext.ReadList(key);
        }
        public string Read(IReadContext r, string key)
        {
            return r.Read(key);
        }

        /// <summary>
        /// 把字节数组反序列化成对象
        /// </summary>
        public object DeserializeObject(byte[] bytes)
        {
            object obj = null;
            if (bytes == null)
                return obj;
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Binder = new UBinder();

            try
            {
                obj = formatter.Deserialize(ms);
            }
            catch (Exception)
            {
                throw;
            }
            ms.Close();
            return obj;
        }

        /// <summary>
        /// 按类型名获取对象
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public object GetObject()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string classname = ObjectToString("classname");
            dynamic obj = asm.CreateInstance(" KDMIS.Model." + classname);

            return GetObject(obj);
        }
        /// <summary>
        /// 按类型名获取对象
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public object GetObject(string classname)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            dynamic obj = asm.CreateInstance(" KDMIS.Model." + classname);

            return obj;
        }
        /// <summary>
        /// 是否包含某些键
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="keys"></param>
        public bool ExistsKeys(params string[] keys)
        {
            for (var i = 0; i < keys.Length; i++)
            {
                if (!kv.ContainsKey(keys[i])) return false;
            }
            return true;
        }

        public object ObjectToObject(string k)
        {
            if (!kv.ContainsKey(k) || kv[k] == null) return null;
            var v = kv[k];
            if (v == null || v.ToString() == string.Empty) v = null;
            return v;
        }

        public string ObjectToString(string k)
        {
            if (!kv.ContainsKey(k) || kv[k] == null) return "";
            var v = kv[k].ToString();
            if (v == null || v == string.Empty) v = "";
            return v;
        }

        public char ObjectToChar(string k)
        {
            if (!kv.ContainsKey(k) || kv[k] == null) return char.MinValue;
            var s = kv[k].ToString();
            if (s.Length == 0) return char.MinValue;
            else if (s.Length == 1) return s[0];
            else return char.MinValue;
        }

        public int ObjectToInt(string k)
        {
            if (!kv.ContainsKey(k) || kv[k] == null) return 0;
            if (kv[k] is int) return (int)kv[k];
            if (kv[k] is long) return (int)(long)kv[k];

            int i;
            int.TryParse(kv[k].ToString(), out i);
            return i;
        }

        public decimal ObjectToDecimal(string k)
        {
            if (!kv.ContainsKey(k) || kv[k] == null) return 0;
            if (kv[k] is Decimal) return (decimal)kv[k];
            if (kv[k] is Double || kv[k] is Int64 || kv[k] is Int32 || kv[k] is Int16 || kv[k] is Single || kv[k] is String)
            {
                decimal d;
                decimal.TryParse(kv[k].ToString(), out d);
                return d;
            }
            return 0;
        }

        public DateTime ObjectToDate(string k)
        {
            if (!kv.ContainsKey(k) || kv[k] == null) return DateTime.MinValue;
            if (kv[k] is DateTime) return (DateTime)kv[k];

            if ((kv[k] is String) && (string)kv[k] == "") return DateTime.MinValue;
            else if ((kv[k] is String) && (string)kv[k] != "") return DateTime.Parse((string)kv[k]);
            return DateTime.Parse((string)kv[k]);
        }

    }
}