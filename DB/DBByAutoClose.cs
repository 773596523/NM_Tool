using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DB
{
    public class DBByAutoClose : IDB
    {
        private string connection_string = "";
        public DBByAutoClose()
        {
            this.connection_string = System.Configuration.ConfigurationManager.AppSettings["conn"];
        }

        public DBByAutoClose(string connection_string)
        {
            this.connection_string = connection_string;
        }

        private System.Data.SqlClient.SqlConnection GetNewConn()
        {
            return new System.Data.SqlClient.SqlConnection(connection_string);
        }

        public void BulkCopy(System.Data.DataTable dt)
        {
            System.Data.SqlClient.SqlBulkCopy bulk = null;
            try
            {
                bulk = new System.Data.SqlClient.SqlBulkCopy(this.connection_string, System.Data.SqlClient.SqlBulkCopyOptions.UseInternalTransaction);
                bulk.BulkCopyTimeout = 9999;
                bulk.BatchSize = 50000;
                bulk.DestinationTableName = dt.TableName;
                foreach (DataColumn col in dt.Columns)
                {
                    bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                bulk.WriteToServer(dt);
            }
            catch (Exception )
            {
                throw;
            }
            finally
            {
                if (bulk != null)
                {
                    bulk.Close();
                }
            }
        }

        DataTable IDB.ExecuteToTable(string sql)
        {
            IDB db = this;
            return db.ExecuteToTable(sql, null);
        }

        System.Data.DataTable IDB.ExecuteToTable(string sql, System.Data.IDbDataParameter[] pars)
        {
            System.Data.SqlClient.SqlConnection conn = this.GetNewConn();
            try
            {

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 8000;
                System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter();
                da.SelectCommand = cmd;
                cmd.CommandText = sql;


                if (pars != null)
                {
                    cmd.Parameters.AddRange(pars);
                }

                //
                conn.Open();
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception )
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        System.Data.DataTable IDB.ExecuteToTable(string sql, string sort, System.Data.IDbDataParameter[] pars, int pageSize, int pageIndex, out int total)
        {
            System.Data.SqlClient.SqlConnection conn = this.GetNewConn();
            try
            {

                const string str1 = "with cte as (";
                const string str1MuliWith = ", cte as (";
                const string str2 = ")select (select count(*) from cte) as _cnt, * from cte where _rn between (@PIndex-1)*@PSize+1 and @PIndex*@PSize";
                const string str3 = " row_number() over (order by {0}) as _rn,";
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 8000;
                System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter();
                da.SelectCommand = cmd;
                cmd.CommandText = sql;
                if (pars != null)
                {
                    cmd.Parameters.AddRange(pars);
                }

                //先判断是否存在多with语句，如果有多语句，在SQL添加?withPage关键子，该关键字表示当前插入分页的位置
                int widthPageIndex = cmd.CommandText.IndexOf("?withPage", StringComparison.CurrentCultureIgnoreCase);
                if (widthPageIndex != -1)
                {
                    string sqlPre = cmd.CommandText.Substring(0, widthPageIndex);
                    string sqlBack = cmd.CommandText.Substring(widthPageIndex + 9);//?withPage.length = 9;
                    int selectIndex = sqlBack.IndexOf("select", StringComparison.CurrentCultureIgnoreCase);
                    sqlBack = sqlBack.Insert(selectIndex + 6, string.Format(str3, sort));
                    cmd.CommandText = sqlPre + str1MuliWith + sqlBack + str2;
                }
                else
                {
                    int selectIndex = cmd.CommandText.IndexOf("select", StringComparison.CurrentCultureIgnoreCase);
                    cmd.CommandText = str1 + cmd.CommandText.Insert(selectIndex + 6, string.Format(str3, sort)) + str2;
                }

                System.Data.SqlClient.SqlParameter param = cmd.CreateParameter();
                param.ParameterName = "@PSize";
                param.DbType = System.Data.DbType.Int32;
                param.Value = pageSize;
                cmd.Parameters.Add(param);

                param = cmd.CreateParameter();
                param.ParameterName = "@PIndex";
                param.DbType = System.Data.DbType.Int32;
                param.Value = pageIndex;
                cmd.Parameters.Add(param);


                conn.Open();
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);
                total = (dt.Rows.Count > 0 ? (int)dt.Rows[0]["_cnt"] : 0);
                dt.Columns.Remove("_cnt");
                dt.Columns.Remove("_rn");

                return dt;
            }
            catch (Exception )
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        object IDB.ExecuteScalar(string sql)
        {
            IDB db = this;
            return db.ExecuteScalar(sql, null);
        }

        object IDB.ExecuteScalar(string sql, System.Data.IDbDataParameter[] pars)
        {
            System.Data.SqlClient.SqlConnection conn = this.GetNewConn();
            try
            {

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 8000;
                cmd.CommandText = sql;
                if (pars != null)
                {
                    cmd.Parameters.AddRange(pars);
                }
                //
                conn.Open();
                return cmd.ExecuteScalar();
            }
            catch (Exception )
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        object IDB.ExecuteScalar(string sql, System.Data.IDbDataParameter[] pars, CommandType cmdType)
        {
            System.Data.SqlClient.SqlConnection conn = this.GetNewConn();
            try
            {

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 8000;
                cmd.CommandType = cmdType;
                cmd.CommandText = sql;
                if (pars != null)
                {
                    cmd.Parameters.AddRange(pars);
                }
                //
                conn.Open();
                return cmd.ExecuteScalar();

            }
            catch (Exception )
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        T IDB.ExecuteToModel<T>(string sql)
        {
            IDB db = this;
            return db.ExecuteToModel<T>(sql, null);
        }

        T IDB.ExecuteToModel<T>(string sql, System.Data.IDbDataParameter[] pars)
        {
            IDB db = this;
            var dt = db.ExecuteToTable(sql, pars);
            if (dt.Rows.Count == 0)
            {
                return default(T);
            }
            else
            {
                return ReflectionHelper.DataRowToModel<T>(dt.Rows[0]);
            }
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

        Dictionary<TKey, TValue> IDB.ExecuteToDic<TKey, TValue>(string sql, string key_name,
            IDbDataParameter[] pars)
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

        Dictionary<TKey, TValue> IDB.ExecuteToDic<TKey, TValue>(string sql, string key_name,
            string value_name, IDbDataParameter[] pars)
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

        Dictionary<TKey, List<TValue>> IDB.ExecuteToDicS<TKey, TValue>(string sql, string key_name,
            string value_name, IDbDataParameter[] pars)
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

        void IDB.Insert(object obj)
        {
            System.Data.SqlClient.SqlConnection conn = this.GetNewConn();
            try
            {

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = conn;
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
                sql = "insert into " + ReflectionHelper.GetDataTableNameByModel(obj) + "(" + fields + ")values(" + values + ")";
                cmd.CommandText = sql;

                cmd.Parameters.AddRange(ReflectionHelper.ModelToSqlParameters(obj));
                //
                conn.Open();
                cmd.ExecuteScalar();
            }
            catch (Exception )
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public void Insert(string sql, out int rowNum)
        {
            System.Data.SqlClient.SqlConnection conn = this.GetNewConn();
            try
            {

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 8000;
                cmd.CommandText = sql;

                //
                conn.Open();
                rowNum = cmd.ExecuteNonQuery();
            }
            catch (Exception )
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        void IDB.Insert(object obj, string without_fields)
        {
            System.Data.SqlClient.SqlConnection conn = this.GetNewConn();
            try
            {

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = conn;
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
                sql = "insert into " + ReflectionHelper.GetDataTableNameByModel(obj) + "(" + fields + ")values(" + values + ")";
                cmd.CommandText = sql;

                cmd.Parameters.AddRange(ReflectionHelper.ModelToSqlParameters(obj));
                //
                conn.Open();
                cmd.ExecuteScalar();
            }
            catch (Exception )
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        void IDB.Update(object obj, string key_fields)
        {
            System.Data.SqlClient.SqlConnection conn = this.GetNewConn();
            try
            {
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 8000;
                //
                string sql = "";
                string fields = "";
                foreach (System.Reflection.PropertyInfo p in obj.GetType().GetProperties())
                {
                    int flag = 0;
                    foreach (string f in key_fields.Split(','))
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
                sql = "update " + ReflectionHelper.GetDataTableNameByModel(obj) + " set " + fields + " where " + filter;
                cmd.CommandText = sql;

                cmd.Parameters.AddRange(ReflectionHelper.ModelToSqlParameters(obj));
                //
                conn.Open();
                cmd.ExecuteScalar();
            }
            catch (Exception )
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        void IDB.Update(object obj, string key_fields, string update_fields)
        {
            System.Data.SqlClient.SqlConnection conn = this.GetNewConn();
            try
            {
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = conn;
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
                    int flag = 0;
                    foreach (string f in key_fields.Split(','))
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
                sql = "update " + ReflectionHelper.GetDataTableNameByModel(obj) + " set " + fields + " where " + filter;
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(ReflectionHelper.ModelToSqlParameters(obj));
                //
                conn.Open();
                cmd.ExecuteScalar();
            }
            catch (Exception )
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public void UpdateNo(object obj, string key_fields, string no_update_fields)
        {
            System.Data.SqlClient.SqlConnection conn = this.GetNewConn();
            try
            {
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = conn;
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
                cmd.Parameters.AddRange(ReflectionHelper.ModelToSqlParameters(obj));
                //

                cmd.ExecuteScalar();
            }
            catch (Exception )
            {
                throw;
            }
            finally
            {
                conn.Close();
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
