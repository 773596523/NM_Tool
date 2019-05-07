using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DB
{
    public class ReflectionHelper
    {

        public static T DataRowToModel<T>(System.Data.DataRow row)
        {
            try
            {

                if (row == null) return default(T);

                var t = typeof(T);
                var pis = t.GetProperties();
                T r = (T)Activator.CreateInstance(t);
                for (int j = 0; j < pis.Length; j++)
                {
                    var pi = pis[j];
                    string colName = pi.Name.ToUpper();
                    if (!row.Table.Columns.Contains(colName)) continue;
                    switch (pi.PropertyType.FullName)
                    {
                        case "System.String":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, row[colName].ToString(), null);
                            break;
                        case "System.Int16":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, (short)row[colName], null);
                            break;
                        case "System.Int32":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, Convert.ToInt32(row[colName]), null);
                            break;
                        case "System.Int64":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, Convert.ToInt64(row[colName]), null);
                            break;
                        case "System.Decimal":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, Convert.ToDecimal(row[colName].ToString().Length == 0 ? 0 : row[colName]), null);
                            break;
                        case "System.Double":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, (double)row[colName], null);
                            break;
                        case "System.Float":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, (float)row[colName], null);
                            break;
                        //case "System.Guid":
                        //    if (row[colName] != DBNull.Value) pi.SetValue(r, (Guid)row[colName], null);
                        //    break;
                        case "System.DateTime":
                            if (row[colName] != DBNull.Value
                                && row[colName] != null
                                && !string.IsNullOrEmpty(row[colName].ToString()))
                                pi.SetValue(r, Convert.ToDateTime(row[colName]), null);
                            else
                                pi.SetValue(r, DateTime.MinValue, null);
                            break;
                        case "System.Char":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, row[colName].ToString().Length == 0 ? char.MinValue : row[colName], null);
                            break;
                        //case "System.Boolean":
                        //    if (row[colName] != DBNull.Value) pi.SetValue(r, (bool)row[colName], null);
                        //    break;
                        //case "System.Byte":
                        //    if (row[colName] != DBNull.Value) pi.SetValue(r, (byte)row[colName], null);
                        //    break;
                    }
                }

                return r;
            }
            catch (Exception ex)
            {
                throw new Exception("1" + ex.Message);
            }

        }

        public static T DataRowToModel2<T>(System.Data.DataRow row, out int errId, out string errMsg)
        {
            try
            {

                if (row == null)
                {
                    errId = 0;
                    errMsg = "";
                    return default(T);
                }

                var t = typeof(T);
                var pis = t.GetProperties();
                T r = (T)Activator.CreateInstance(t);
                for (int j = 0; j < pis.Length; j++)
                {
                    var pi = pis[j];
                    string colName = pi.Name.ToUpper();
                    if (!row.Table.Columns.Contains(colName))
                    {
                        errId = -2;
                        errMsg = colName + " 列名不存在";
                        return default(T);
                    }
                    switch (pi.PropertyType.FullName)
                    {
                        case "System.String":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, row[colName].ToString(), null);
                            break;
                        case "System.Int16":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, (short)row[colName], null);
                            break;
                        case "System.Int32":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, Convert.ToInt32(row[colName]), null);
                            break;
                        case "System.Int64":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, (long)row[colName], null);
                            break;
                        case "System.Decimal":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, Convert.ToDecimal(string.IsNullOrEmpty(row[colName].ToString()) ? "0" : row[colName]), null);
                            break;
                        case "System.Double":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, (double)row[colName], null);
                            break;
                        case "System.Float":
                            if (row[colName] != DBNull.Value) pi.SetValue(r, (float)row[colName], null);
                            break;
                        //case "System.Guid":
                        //    if (row[colName] != DBNull.Value) pi.SetValue(r, (Guid)row[colName], null);
                        //    break;
                        case "System.DateTime":

                            if (row[colName] != DBNull.Value) pi.SetValue(r, Convert.ToDateTime(row[colName]), null);
                            break;

                        //case "System.Boolean":
                        //    if (row[colName] != DBNull.Value) pi.SetValue(r, (bool)row[colName], null);
                        //    break;
                        //case "System.Byte":
                        //    if (row[colName] != DBNull.Value) pi.SetValue(r, (byte)row[colName], null);
                        //    break;
                    }
                }
                errId = 0;
                errMsg = "";
                return r;
            }
            catch (Exception ex)
            {
                errId = -1;
                errMsg = ex.Message;
                return default(T);
            }

        }


        public static System.Data.SqlClient.SqlParameter[] ModelToSqlParameters(object obj)
        {
            List<System.Data.SqlClient.SqlParameter> lst = new List<System.Data.SqlClient.SqlParameter>();
            var t = obj.GetType();
            var pis = t.GetProperties();
            for (int j = 0; j < pis.Length; j++)
            {
                var pi = pis[j];
                string colName = pi.Name.ToUpper();
                System.Data.SqlClient.SqlParameter par = new System.Data.SqlClient.SqlParameter();
                par.ParameterName = "@" + pi.Name;
                switch (pi.PropertyType.FullName)
                {
                    case "System.String":
                        lst.Add(par);
                        par.SqlDbType = System.Data.SqlDbType.VarChar;
                        par.Value = pi.GetValue(obj, null);
                        break;
                    case "System.Int16":
                        lst.Add(par);
                        par.SqlDbType = System.Data.SqlDbType.Int;
                        par.Value = pi.GetValue(obj, null);
                        break;
                    case "System.Int32":
                        lst.Add(par);
                        par.SqlDbType = System.Data.SqlDbType.Int;
                        par.Value = pi.GetValue(obj, null);
                        break;
                    case "System.Int64":
                        lst.Add(par);
                        par.SqlDbType = System.Data.SqlDbType.Int;
                        par.Value = pi.GetValue(obj, null);
                        break;
                    case "System.Decimal":
                        lst.Add(par);
                        par.SqlDbType = System.Data.SqlDbType.Decimal;
                        par.Value = pi.GetValue(obj, null);
                        break;
                    case "System.Double":
                        lst.Add(par);
                        par.SqlDbType = System.Data.SqlDbType.Float;
                        par.Value = pi.GetValue(obj, null);
                        break;
                    case "System.Float":
                        lst.Add(par);
                        par.SqlDbType = System.Data.SqlDbType.Float;
                        par.Value = pi.GetValue(obj, null);
                        break;
                    //case "System.Guid":
                    //    if (row[colName] != DBNull.Value) pi.SetValue(r, (Guid)row[colName], null);
                    //    break;
                    case "System.DateTime":

                        lst.Add(par);
                        par.SqlDbType = System.Data.SqlDbType.DateTime;
                        par.Value = pi.GetValue(obj, null);
                        if ((System.DateTime)par.Value == System.DateTime.MinValue)
                        {
                            par.Value = DBNull.Value;
                        }
                        break;
                    //case "System.Boolean":
                    //    if (row[colName] != DBNull.Value) pi.SetValue(r, (bool)row[colName], null);
                    //    break;
                    //case "System.Byte":
                    //    if (row[colName] != DBNull.Value) pi.SetValue(r, (byte)row[colName], null);
                    //    break;
                    default:
                        lst.Add(par);
                        par.Value = pi.GetValue(obj, null);
                        break;
                }
                if (par.Value == null)
                {
                    par.Value = DBNull.Value;
                }
            }

            return lst.ToArray<System.Data.SqlClient.SqlParameter>();

        }


        public static string GetDataTableNameByModel(object obj)
        {
            string tb = obj.GetType().Name.Split('.')[obj.GetType().Name.Split('.').Length - 1];
            return tb;
        }

        /// <summary>
        /// 还原数据类型 为空数据 赋为控制
        /// </summary>
        /// <typeparam name="T">返回的对象</typeparam>
        /// <param name="robject">传入对象</param>
        /// <returns></returns>
        public static T RestoreObject<T>(T robject)
        {

            var t = typeof(T);
            var pis = t.GetProperties();
            for (int j = 0; j < pis.Length; j++)
            {
                var pi = pis[j];
                string colName = pi.Name.ToUpper();
                switch (pi.PropertyType.FullName)
                {
                    case "System.String":
                        if (pi.GetValue(robject, null) == null) pi.SetValue(robject, "".ToString(), null);
                        break;
                    case "System.Char":
                        if (pi.GetValue(robject, null) == null) pi.SetValue(robject, char.MinValue, null);
                        break;
                }
            }

            return robject;
        }

        public static DataTable GetTBOfObject<T>()
        {
            DataTable tb = new DataTable();

            var t = typeof(T);
            var pis = t.GetProperties();
            for (int j = 0; j < pis.Length; j++)
            {
                var pi = pis[j];
                string colName = pi.Name;

                tb.Columns.Add(
                    colName,
                   pi.PropertyType
                    );
            }

            return tb;
        }

        public static string GetCreateTableSQL<T>()
        {
            string create_sql = "create table ?? (";

            var t = typeof(T);
            var pis = t.GetProperties();

            for (int j = 0; j < pis.Length; j++)
            {
                var pi = pis[j];
                string colName = pi.Name;
                switch (pi.PropertyType.FullName)
                {
                    case "System.String":
                        create_sql += colName + "  varchar(255)";
                        break;
                    case "System.Int16":
                        create_sql += colName + "  int";
                        break;
                    case "System.Int32":
                        create_sql += colName + "  int";
                        break;
                    case "System.Int64":
                        create_sql += colName + "  numeric(18, 0)";
                        break;
                    case "System.Decimal":
                        create_sql += colName + "  numeric(18, 4)";
                        break;
                    case "System.Double":
                        create_sql += colName + "  numeric(18, 2)";
                        break;
                    case "System.Float":
                        create_sql += colName + "  numeric(18, 2)";
                        break;
                    case "System.DateTime":
                        create_sql += colName + "  datetime";
                        break;
                    case "System.Char":
                        create_sql += colName + "  varchar(1)";
                        break;
                }
                if (j < pis.Length - 1)
                    create_sql += ",";
            }

            create_sql += ")";

            return create_sql;
        }

        public static string GetInsertSQL<T>(string key_id, string where_key_id)
        {
            var t = typeof(T);
            var pis = t.GetProperties();

            string sql = @" insert into " + t.Name + " ( ";

            for (int j = 0; j < pis.Length; j++)
            {
                var pi = pis[j];
                string colName = pi.Name;
                sql += colName;

                if (j < pis.Length - 1)
                    sql += ",";
            }
            sql += @") 
select ";
            for (int j = 0; j < pis.Length; j++)
            {
                var pi = pis[j];
                string colName = pi.Name;
                sql += "a." + colName;

                if (j < pis.Length - 1)
                    sql += ",";
            }
            sql += @" from ?? a 
 left join " + t.Name + "  z on  ";

            string[] keys = key_id.Split(',');
            for (int i = 0; i < keys.Length; i++)
            {
                sql += " z." + keys[i] + " = a." + keys[i];
                if (i < keys.Length - 1)
                    sql += " and ";
            }

            sql += " where ";

            keys = where_key_id.Split(',');
            for (int i = 0; i < keys.Length; i++)
            {
                sql += " z." + keys[i] + " is null";
                if (i < keys.Length - 1)
                    sql += " or ";
            }

            return sql;
        }
        public static string GetInsertSQL<T>(string key_id, string where_key_id, string no_insert_key)
        {
            var t = typeof(T);
            var pis = t.GetProperties();

            string sql = @" insert into " + t.Name + " ( ";

            for (int j = 0; j < pis.Length; j++)
            {
                var pi = pis[j];
                string colName = pi.Name;

                if (no_insert_key.Equals(colName)) continue;

                sql += colName;

                if (j < pis.Length - 1)
                    sql += ",";
            }
            sql += @") 
select ";
            for (int j = 0; j < pis.Length; j++)
            {
                var pi = pis[j];
                string colName = pi.Name;

                if (no_insert_key.Equals(colName)) continue;

                sql += "a." + colName;

                if (j < pis.Length - 1)
                    sql += ",";
            }
            sql += @" from ?? a 
 left join " + t.Name + "  z on  ";

            string[] keys = key_id.Split(',');
            for (int i = 0; i < keys.Length; i++)
            {
                sql += " z." + keys[i] + " = a." + keys[i];
                if (i < keys.Length - 1)
                    sql += " and ";
            }

            sql += " where ";

            keys = where_key_id.Split(',');
            for (int i = 0; i < keys.Length; i++)
            {
                sql += " z." + keys[i] + " is null";
                if (i < keys.Length - 1)
                    sql += " or ";
            }

            return sql;
        }


        public static string GetUpdateSQL<T>(string key_id, string where_key_id)
        {
            var t = typeof(T);
            var pis = t.GetProperties();

            string sql = @" update " + t.Name + "  set  ";
            for (int j = 0; j < pis.Length; j++)
            {
                var pi = pis[j];
                string colName = pi.Name;
                sql += t.Name + "." + colName + " = " + "b." + colName;

                if (j < pis.Length - 1)
                    sql += ",";
            }

            sql += @" from ?? b  where 1=1 ";

            foreach (string f in key_id.Split(','))
            {
                sql += " and " + t.Name + "." + f + " = b." + f;
            }

            return sql;
        }
        public static string GetUpdateSQL<T>(string key_id, string where_key_id, string no_update_key)
        {
            var t = typeof(T);
            var pis = t.GetProperties();

            string sql = @" update " + t.Name + "  set  ";
            for (int j = 0; j < pis.Length; j++)
            {
                var pi = pis[j];
                string colName = pi.Name;
                if (no_update_key.Contains(colName)) continue;
                sql += t.Name + "." + colName + " = " + "b." + colName;

                if (j < pis.Length - 1)
                    sql += ",";
            }

            sql += @" from ?? b  where 1=1 ";

            foreach (string f in key_id.Split(','))
            {
                sql += " and " + t.Name + "." + f + " = b." + f;
            }

            return sql;
        }

        public static string GetDeleteSQL<T>(string delete_id, string key_id, string where_key_id)
        {
            var t = typeof(T);
            var pis = t.GetProperties();

            string sql = @" delete " + t.Name + " where 1=1 ";

            string[] keys = key_id.Split(',');
            foreach (string f in delete_id.Split(','))
            {
                sql += " and " + f + "  in  ( ";
                sql += " select  a." + f + "  from " + t.Name + "  a   left join  ??   z  on ";

                for (int i = 0; i < keys.Length; i++)
                {
                    sql += " z." + keys[i] + " = a." + keys[i];
                    if (i < keys.Length - 1)
                        sql += " and ";
                }

                sql += " where 1=1 and   (";

                keys = where_key_id.Split(',');
                for (int i = 0; i < keys.Length; i++)
                {
                    sql += "z." + keys[i] + " is null";
                    if (i < keys.Length - 1)
                        sql += " or ";
                }

                sql += ") ";


                sql += ")  ";
            }



            return sql;
        }


        public static List<T> ExecuteToList<T>(DataTable tb, IDbDataParameter[] pars)
        {
            List<T> lis = new List<T>();

            foreach (DataRow dr in tb.Rows)
            {
                lis.Add(ReflectionHelper.DataRowToModel<T>(dr));
            }
            return lis;
        }

        public static Dictionary<TKey, TValue> ExecuteToDic<TKey, TValue>(DataTable tb, string key_name)
        {
            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();

            foreach (DataRow dr in tb.Rows)
            {
                if (!dic.Keys.Contains((TKey)Convert.ChangeType(dr[key_name], typeof(TKey))))
                    dic.Add(
                        (TKey)Convert.ChangeType(dr[key_name], typeof(TKey)),
                        ReflectionHelper.DataRowToModel<TValue>(dr)
                        );
            }

            return dic;
        }

        public static Dictionary<TKey, TValue> ExecuteToDic<TKey, TValue>(DataTable tb, string key_name, string value_name)
        {

            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();

            foreach (DataRow dr in tb.Rows)
            {
                if (!dic.Keys.Contains((TKey)Convert.ChangeType(dr[key_name], typeof(TKey))))
                    dic.Add(
                        (TKey)Convert.ChangeType(dr[key_name], typeof(TKey)),
                        (TValue)Convert.ChangeType(dr[value_name], typeof(TValue))
                        );
            }

            return dic;
        }

        public static Dictionary<TKey, List<TValue>> ExecuteToDicS<TKey, TValue>(DataTable tb, string key_name, string value_name)
        {
            Dictionary<TKey, List<TValue>> dic = new Dictionary<TKey, List<TValue>>();

            foreach (DataRow dr in tb.Rows)
            {

                var Key = (TKey)Convert.ChangeType(dr[key_name], typeof(TKey));
                var Value = (List<TValue>)Convert.ChangeType(dr[value_name], typeof(TValue));
                if (dic.TryGetValue(Key, out Value))
                {
                    foreach (var item in Value)
                    {
                        dic[Key].Add(item);
                    }
                }
                else
                    dic.Add(Key, Value);
            }

            return dic;
        }



    }
}
