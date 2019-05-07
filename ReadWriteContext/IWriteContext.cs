using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Text;
using System.Data;

namespace ReadWriteContext
{
    public interface IWriteContext
    {
        void Append(string key, string value);
        void Append(string[] keys, string[] values);
        void Append(string key, string[] subkeys, string[] subvalues);
        void Append(string key, DataTable dt);

        void SetStr(string str);
        string ToString();
        void Clear();
    }

}