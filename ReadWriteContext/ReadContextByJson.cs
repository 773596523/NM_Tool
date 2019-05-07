using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ReadWriteContext
{
   public  class ReadContextByJson:IReadContext 
    {
       protected string context;
       public ReadContextByJson(string context)
       {
           this.context = context;
       }

       bool IReadContext.Exist(string path)
       {
               string endPath ;
               if (path.Contains("/") == false)
               {
                   endPath = path;
               }
               else
               {
                   endPath = path.Split( '/' )[path.Split( '/' ).Length - 1];
               }
               IReadContext red = ToEndNode(this.context,path);
               ReadContextByJson rJson =(ReadContextByJson) red;
               JObject obj =(JObject) Newtonsoft.Json.JsonConvert.DeserializeObject(rJson.context);
               if (obj.GetValue(endPath) == null)
               {
                   return false;
               }
               else
               {
                   return true;
               }
       }

       private IReadContext ToEndNode(string context, string path)
       {
           if (path.Contains("/") == false)
           {
               return new ReadContextByJson(context);
           }
           else
           {
               string firstPath = path.Split('/')[0];
               
               JObject obj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(context);
               if (obj.GetValue(firstPath) == null)
               {
                   return new ReadContextByJson(context);
               }
               else
               {
                   var con = obj.GetValue(firstPath).ToString();
                   return ToEndNode(con, path.Substring(firstPath.Length));
               }
              
           }
       }

       string IReadContext.Read(string path)
       {
           string endPath;
           if (path.Contains("/") == false)
           {
               endPath = path;
           }
           else
           {
               endPath = path.Split('/')[path.Split('/').Length - 1];
           }
           IReadContext red = ToEndNode(this.context,path);
           ReadContextByJson rJson = (ReadContextByJson)red;
           JObject obj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(rJson.context);
           if (obj.GetValue(endPath) == null)
           {
               return "";
           }
           else
           {
               return obj.GetValue(endPath).ToString().Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
           }
       }

       List<IReadContext> IReadContext.ReadList(string path)
       {
           List<IReadContext> lst = new List<IReadContext>();
           //
               IReadContext red = this;
               var json = red.Read(path);
               JArray obj = (JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(json);
               for (int i = 0; i < obj.Count; i++)
               {
                   lst.Add(new ReadContextByJson(obj[i].ToString()));
               }
           //
           return lst;
       }

       Dictionary<string, object> IReadContext.ToDictionary()
       {
           Dictionary<string, object> kv;
           if (string.IsNullOrEmpty(context))
           {
               kv = new Dictionary<string, object>();
           }
           else
           {
               kv = ToDictionary(context);
           }
           return kv;
           
       }

       private Dictionary<string, object> ToDictionary(string json)
       {
           try
           {
               return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
           }
           catch
           {
               return new Dictionary<string, object>();
           }
       }


    }
}
