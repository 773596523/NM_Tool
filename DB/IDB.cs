using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DB
{
    public interface IDB
    {
        System.Data.DataTable ExecuteToTable(string sql);
        System.Data.DataTable ExecuteToTable(string sql, System.Data.IDbDataParameter[] pars);
        System.Data.DataTable ExecuteToTable(string sql, string sort, System.Data.IDbDataParameter[] pars, int pageSize, int pageIndex, out int total);

        object ExecuteScalar(string sql);
        object ExecuteScalar(string sql, System.Data.IDbDataParameter[] pars);
        object ExecuteScalar(string sql, System.Data.IDbDataParameter[] pars, CommandType cmdType);

        T ExecuteToModel<T>(string sql);
        T ExecuteToModel<T>(string sql, System.Data.IDbDataParameter[] pars);

        List<T> ExecuteToList<T>(string sql);
        List<T> ExecuteToList<T>(string sql, System.Data.IDbDataParameter[] pars);

        Dictionary<TKey, TValue> ExecuteToDic<TKey, TValue>(string sql, string key_name);
        Dictionary<TKey, TValue> ExecuteToDic<TKey, TValue>(string sql, string key_name, System.Data.IDbDataParameter[] pars);

        Dictionary<TKey, TValue> ExecuteToDic<TKey, TValue>(string sql, string key_name, string value_name);
        Dictionary<TKey, TValue> ExecuteToDic<TKey, TValue>(string sql, string key_name, string value_name, System.Data.IDbDataParameter[] pars);

        Dictionary<TKey, List<TValue>> ExecuteToDicS<TKey, TValue>(string sql, string key_name, string value_name);
        Dictionary<TKey, List<TValue>> ExecuteToDicS<TKey, TValue>(string sql, string key_name, string value_name, System.Data.IDbDataParameter[] pars);

        void Insert(object obj);
        void Insert(string sql, out int rowNum);
        void Insert(object obj, string without_fields);

        void Update(object obj, string key_fields);
        void Update(object obj, string key_fields, string update_fields);
        void UpdateNo(object obj, string key_fields, string no_update_fields);


        bool ExistsTable(string tableName);
    }
}
