using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Text;
using System.Data;

namespace ReadWriteContext
{
  public interface IReadContext
    {

      bool Exist(string path);
      string Read(string path);
      List<IReadContext> ReadList(string path);
      Dictionary<string, object> ToDictionary();

    }
}
