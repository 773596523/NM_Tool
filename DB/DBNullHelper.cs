using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DB
{
    public class DBNullHelper
    {
      public static  object DateTimeForDB(DateTime Value)
      {
          if (Value == DateTime.MinValue)
              return DBNull.Value;
          else
              return DateTime.Parse(Value.ToString("yyyy-MM-dd HH:mm:ss"));
      }

      public static object DateForDB(DateTime Value)
      {
          if (Value == DateTime.MinValue)
              return DBNull.Value;
          else
              return DateTime.Parse(Value.ToString("yyyy-MM-dd"));
      }

      public static object StringForDB(string value)
      {
          if (value == null)
          {
              return DBNull.Value;
          }
          else
          {
              return value;
          }
      }

    }
}
