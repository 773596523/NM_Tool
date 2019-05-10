using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DB
{
    public class SQLiteByHandClose : DB.IDB
    {
        private System.Data.SQLite.SQLiteConnection conn = null;
        private System.Data.SQLite.SQLiteTransaction tran = null;


        public SQLiteByHandClose(string connection_string)
        {
            conn = new System.Data.SQLite.SQLiteConnection(connection_string);
        }

        public void Open()
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

        }

        public void Close()
        {
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }

        }



        public void BeginTran()
        {
            tran = conn.BeginTransaction();
        }

        public void CommitTran()
        {
            if (tran != null)
            {
                tran.Commit();
                tran = null;
            }

        }

        public void RollBackTran()
        {
            if (tran != null)
            {
                tran.Rollback();
                tran = null;
            }

        }

        DataTable IDB.ExecuteToTable(string sql)
        {
            IDB db = this;
            return db.ExecuteToTable(sql, null);
        }

        System.Data.DataTable DB.IDB.ExecuteToTable(string sql, System.Data.IDbDataParameter[] pars)
        {
            System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();
            cmd.Connection = conn;
            cmd.Transaction = tran;
            cmd.CommandTimeout = 8000;
            System.Data.SQLite.SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter();
            da.SelectCommand = cmd;
            cmd.CommandText = sql;
            if (pars != null)
            {
                cmd.Parameters.AddRange(pars);
            }

            //

            System.Data.DataTable dt = new System.Data.DataTable();
            da.Fill(dt);
            return dt;
        }

        System.Data.DataTable DB.IDB.ExecuteToTable(string sql, string sort, System.Data.IDbDataParameter[] pars, int pageSize, int pageIndex, out int total)
        {
            throw new NotImplementedException();
        }

        object IDB.ExecuteScalar(string sql)
        {
            IDB db = this;
            return db.ExecuteScalar(sql, null);
        }

        object DB.IDB.ExecuteScalar(string sql, System.Data.IDbDataParameter[] pars)
        {
            System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();
            cmd.Connection = conn;
            cmd.Transaction = tran;
            cmd.CommandTimeout = 8000;
            cmd.CommandText = sql;
            if (pars != null)
            {
                cmd.Parameters.AddRange(pars);
            }
            //

            return cmd.ExecuteScalar();
        }

        object DB.IDB.ExecuteScalar(string sql, System.Data.IDbDataParameter[] pars, System.Data.CommandType cmdType)
        {
            System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();
            cmd.Connection = conn;
            cmd.Transaction = tran;
            cmd.CommandTimeout = 8000;
            cmd.CommandType = cmdType;
            cmd.CommandText = sql;
            if (pars != null)
            {
                cmd.Parameters.AddRange(pars);
            }
            //

            return cmd.ExecuteScalar();
        }

        T IDB.ExecuteToModel<T>(string sql)
        {
            IDB db = this;
            return db.ExecuteToModel<T>(sql, null);
        }

        T DB.IDB.ExecuteToModel<T>(string sql, System.Data.IDbDataParameter[] pars)
        {
            DB.IDB db = this;
            var dt = db.ExecuteToTable(sql, pars);
            if (dt.Rows.Count == 0) return default(T); else return DB.ReflectionHelper.DataRowToModel<T>(dt.Rows[0]);
        }

        List<T> IDB.ExecuteToList<T>(string sql)
        {
            IDB db = this;
            return db.ExecuteToList<T>(sql, null);
        }

        List<T> IDB.ExecuteToList<T>(string sql, IDbDataParameter[] pars)
        {
            IDB db = this;
            var dt = db.ExecuteToTable(sql, pars);
            List<T> lis = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                lis.Add(ReflectionHelper.DataRowToModel<T>(dr));
            }
            return lis;
        }

        Dictionary<TKey, TValue> IDB.ExecuteToDic<TKey, TValue>(string sql, string key_name)
        {
            IDB db = this;
            var dt = db.ExecuteToTable(sql, null);

            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();

            foreach (DataRow dr in dt.Rows)
            {
                if (!dic.Keys.Contains((TKey)Convert.ChangeType(dr[key_name], typeof(TKey))))
                    dic.Add(
                        (TKey)Convert.ChangeType(dr[key_name], typeof(TKey)),
                        ReflectionHelper.DataRowToModel<TValue>(dr)
                        );
            }

            return dic;
        }

        Dictionary<TKey, TValue> IDB.ExecuteToDic<TKey, TValue>(string sql, string key_name, IDbDataParameter[] pars)
        {
            IDB db = this;
            var dt = db.ExecuteToTable(sql, pars);

            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();

            foreach (DataRow dr in dt.Rows)
            {
                if (!dic.Keys.Contains((TKey)Convert.ChangeType(dr[key_name], typeof(TKey))))
                    dic.Add(
                       (TKey)Convert.ChangeType(dr[key_name], typeof(TKey)),
                        ReflectionHelper.DataRowToModel<TValue>(dr)
                        );
            }

            return dic;
        }

        Dictionary<TKey, TValue> IDB.ExecuteToDic<TKey, TValue>(string sql, string key_name, string value_name)
        {
            IDB db = this;
            return db.ExecuteToDic<TKey, TValue>(sql, key_name, value_name, null);
        }

        Dictionary<TKey, TValue> IDB.ExecuteToDic<TKey, TValue>(string sql, string key_name, string value_name, IDbDataParameter[] pars)
        {
            IDB db = this;
            var dt = db.ExecuteToTable(sql, pars);

            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();

            foreach (DataRow dr in dt.Rows)
            {
                if (!dic.Keys.Contains((TKey)Convert.ChangeType(dr[key_name], typeof(TKey))))
                    dic.Add(
                          (TKey)Convert.ChangeType(dr[key_name], typeof(TKey)),
                        (TValue)Convert.ChangeType(dr[value_name], typeof(TValue))
                        );
            }

            return dic;
        }

        Dictionary<TKey, List<TValue>> IDB.ExecuteToDicS<TKey, TValue>(string sql, string key_name, string value_name)
        {
            IDB db = this;
            return db.ExecuteToDicS<TKey, TValue>(sql, key_name, value_name, null);
        }

        Dictionary<TKey, List<TValue>> IDB.ExecuteToDicS<TKey, TValue>(string sql, string key_name, string value_name, IDbDataParameter[] pars)
        {
            IDB db = this;
            var dt = db.ExecuteToTable(sql, pars);

            Dictionary<TKey, List<TValue>> dic = new Dictionary<TKey, List<TValue>>();

            foreach (DataRow dr in dt.Rows)
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


        void DB.IDB.Insert(object obj)
        {
            System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();
            cmd.Connection = conn;
            cmd.Transaction = tran;
            cmd.CommandTimeout = 8000;
            //
            string sql = "";
            string fields = "";
            string values = "";
            foreach (System.Reflection.PropertyInfo p in obj.GetType().GetProperties())
            {
                if (fields == "")
                {
                    fields += p.Name;
                    values += "@" + p.Name;
                }
                else
                {
                    fields += "," + p.Name;
                    values += "," + "@" + p.Name;
                }
            }
            sql = "insert into " + DB.ReflectionHelper.GetDataTableNameByModel(obj) + "(" + fields + ")values(" + values + ")";
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(ModelToSqlParameters(obj));
            //

            cmd.ExecuteScalar();
        }

        public void Insert(string sql, out int rowNum)
        {
            System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();
            cmd.Connection = conn;
            cmd.Transaction = tran;
            cmd.CommandTimeout = 8000;
            cmd.CommandText = sql;

            //
            rowNum= cmd.ExecuteNonQuery();
        }

        void IDB.Insert(object obj, string without_fields)
        {
            System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();
            cmd.Connection = conn;
            cmd.Transaction = tran;
            cmd.CommandTimeout = 8000;
            //
            string sql = "";
            string fields = "";
            string values = "";
            foreach (System.Reflection.PropertyInfo p in obj.GetType().GetProperties())
            {
                int flag = 0;
                foreach (string f in without_fields.Split(','))
                {
                    if (p.Name.ToLower() == f.ToLower())
                    {
                        flag = 1;
                    }
                }
                if (flag == 1)
                {
                    continue;
                }
                if (fields == "")
                {
                    fields += p.Name;
                    values += "@" + p.Name;
                }
                else
                {
                    fields += "," + p.Name;
                    values += "," + "@" + p.Name;
                }
            }
            sql = "insert into " + DB.ReflectionHelper.GetDataTableNameByModel(obj) + "(" + fields + ")values(" + values + ")";
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(ModelToSqlParameters(obj));
            //

            cmd.ExecuteScalar();
        }

        void DB.IDB.Update(object obj, string key_fields)
        {
            System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();
            cmd.Connection = conn;
            cmd.Transaction = tran;
            cmd.CommandTimeout = 8000;
            //
            string sql = "";
            string fields = "";
            foreach (System.Reflection.PropertyInfo p in obj.GetType().GetProperties())
            {
                if (fields == "")
                {
                    fields += p.Name + "=" + "@" + p.Name;
                }
                else
                {
                    fields += "," + p.Name + "=" + "@" + p.Name;
                }
            }
            string filter = "";
            if (key_fields.Contains(",") == false)
            {
                filter = key_fields + "=" + "@" + key_fields;
            }
            else
            {
                foreach (string field in key_fields.Split(','))
                {
                    if (filter == "")
                    {
                        filter += field + "=" + "@" + field;
                    }
                    else
                    {
                        filter += " and " + field + "=" + "@" + field;
                    }
                }
            }
            sql = "update " + DB.ReflectionHelper.GetDataTableNameByModel(obj) + " set " + fields + " where " + filter;
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(ModelToSqlParameters(obj));
            //
            cmd.ExecuteScalar();
        }

        void DB.IDB.Update(object obj, string key_fields, string update_fields)
        {
            System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();
            cmd.Connection = conn;
            cmd.Transaction = tran;
            cmd.CommandTimeout = 8000;
            //
            string sql = "";
            string fields = "";
            List<string> lst = new List<string>();
            if (update_fields.Contains(",") == false)
            {
                lst.Add(update_fields.ToLower());
            }
            else
            {
                foreach (string field in update_fields.Split(','))
                {
                    lst.Add(field.ToLower());
                }
            }
            foreach (System.Reflection.PropertyInfo p in obj.GetType().GetProperties())
            {
                if (lst.Contains(p.Name.ToLower()) == false)
                {
                    continue;
                }
                if (fields == "")
                {
                    fields += p.Name + "=" + "@" + p.Name;
                }
                else
                {
                    fields += "," + p.Name + "=" + "@" + p.Name;
                }
            }
            string filter = "";
            if (key_fields.Contains(",") == false)
            {
                filter = key_fields + "=" + "@" + key_fields;
            }
            else
            {
                foreach (string field in key_fields.Split(','))
                {
                    if (filter == "")
                    {
                        filter += field + "=" + "@" + field;
                    }
                    else
                    {
                        filter += " and " + field + "=" + "@" + field;
                    }
                }
            }
            sql = "update " + DB.ReflectionHelper.GetDataTableNameByModel(obj) + " set " + fields + " where " + filter;
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(ModelToSqlParameters(obj));
            //

            cmd.ExecuteScalar();
        }

        void DB.IDB.UpdateNo(object obj, string key_fields, string no_update_fields)
        {
            System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();
            cmd.Connection = conn;
            cmd.Transaction = tran;
            cmd.CommandTimeout = 8000;
            //
            string sql = "";
            string fields = "";
            List<string> lst = new List<string>();
            if (no_update_fields.Contains(",") == false)
            {
                lst.Add(no_update_fields.ToLower());
            }
            else
            {
                foreach (string field in no_update_fields.Split(','))
                {
                    lst.Add(field.ToLower());
                }
            }
            foreach (System.Reflection.PropertyInfo p in obj.GetType().GetProperties())
            {
                if (lst.Contains(p.Name.ToLower()))
                {
                    continue;
                }
                if (fields == "")
                {
                    fields += p.Name + "=" + "@" + p.Name;
                }
                else
                {
                    fields += "," + p.Name + "=" + "@" + p.Name;
                }
            }
            string filter = "";
            if (key_fields.Contains(",") == false)
            {
                filter = key_fields + "=" + "@" + key_fields;
            }
            else
            {
                foreach (string field in key_fields.Split(','))
                {
                    if (filter == "")
                    {
                        filter += field + "=" + "@" + field;
                    }
                    else
                    {
                        filter += " and " + field + "=" + "@" + field;
                    }
                }
            }
            sql = "update " + DB.ReflectionHelper.GetDataTableNameByModel(obj) + " set " + fields + " where " + filter;
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(ModelToSqlParameters(obj));
            //

            cmd.ExecuteScalar();
        }

        public static System.Data.SQLite.SQLiteParameter[] ModelToSqlParameters(object obj)
        {
            List<System.Data.SQLite.SQLiteParameter> lst = new List<System.Data.SQLite.SQLiteParameter>();
            var t = obj.GetType();
            var pis = t.GetProperties();
            for (int j = 0; j < pis.Length; j++)
            {
                var pi = pis[j];
                string colName = pi.Name.ToUpper();
                System.Data.SQLite.SQLiteParameter par = new System.Data.SQLite.SQLiteParameter();
                par.ParameterName = "@" + pi.Name;
                switch (pi.PropertyType.FullName)
                {
                    case "System.String":
                        lst.Add(par);
                        par.DbType = DbType.String;
                        par.Value = pi.GetValue(obj, null);
                        break;
                    case "System.Int16":
                        lst.Add(par);
                        par.DbType = DbType.Int16;
                        par.Value = pi.GetValue(obj, null);
                        break;
                    case "System.Int32":
                        lst.Add(par);
                        par.DbType = DbType.Int32;
                        par.Value = pi.GetValue(obj, null);
                        break;
                    case "System.Int64":
                        lst.Add(par);
                        par.DbType = DbType.Int64;
                        par.Value = pi.GetValue(obj, null);
                        break;
                    case "System.Decimal":
                        lst.Add(par);
                        par.DbType = DbType.Decimal;
                        par.Value = pi.GetValue(obj, null);
                        break;
                    case "System.Double":
                        lst.Add(par);
                        par.DbType = DbType.Double;
                        par.Value = pi.GetValue(obj, null);
                        break;
                    case "System.Float":
                        lst.Add(par);
                        par.DbType = DbType.Double;
                        par.Value = pi.GetValue(obj, null);
                        break;
                    //case "System.Guid":
                    //    if (row[colName] != DBNull.Value) pi.SetValue(r, (Guid)row[colName], null);
                    //    break;
                    case "System.DateTime":

                        lst.Add(par);
                        par.DbType = DbType.DateTime;
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

            return lst.ToArray<System.Data.SQLite.SQLiteParameter>();

        }


        public static void CreateFile(string file)
        {
            System.Data.SQLite.SQLiteConnection.CreateFile(file);
        }

        public bool ExistTable(string tb)
        {
            string sql = "select count(*) from sqlite_master where type='table' and name=@tb";
            System.Data.SQLite.SQLiteParameter[] pars = new System.Data.SQLite.SQLiteParameter[]
            {
                new System.Data.SQLite.SQLiteParameter("@tb",DbType.String)
            };
            pars[0].Value = tb;
            DB.IDB db = this;

            var obj = db.ExecuteScalar(sql, pars);

            if (obj == null)
            {
                return false;
            }
            else
            {
                int cnt = 0;
                int.TryParse(obj.ToString(), out cnt);
                if (cnt == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }

        public bool ExistField(string tb, string field)
        {
            try
            {
                string sql = "select " + field + " from " + tb + " Limit 1";
                DB.IDB db = this;
                db.ExecuteScalar(sql, null);
                return true;
            }
            catch (Exception )
            {
                return false;
            }

        }

        bool IDB.ExistsTable(string tableName)
        {
            string sql = "SELECT count(*) FROM sysobjects WHERE name='" + tableName + "'";
            IDB db = this;
            string count = db.ExecuteScalar(sql).ToString();

            int cou = 0;
            Int32.TryParse(count, out cou);

            if (cou == 0)
                return false;
            else
                return true;

        }




    }
}
