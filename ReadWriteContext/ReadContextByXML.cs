using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ReadWriteContext
{
   public  class ReadContextByXML:IReadContext 
    {

       protected string context = "";
       XmlDocument  doc =null;
       public ReadContextByXML(string context)
       {
           this.context = context;
           doc = new XmlDocument();
           doc.LoadXml(context);
       }

        bool IReadContext.Exist(string path)
        {
            if (doc.SelectSingleNode("xml/"+path) == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        string IReadContext.Read(string path)
        {
            XmlNode node = doc.SelectSingleNode("xml/" + path);
            if (node == null)
            {
                return "";
            }
            {
                return node.InnerXml.Replace("!@#", "<").Replace("#@!", ">");
            }
        }

        List<IReadContext> IReadContext.ReadList(string path)
        {
            List<IReadContext> lst = new List<IReadContext>();
            //
            XmlNodeList nlst = doc.SelectNodes("xml/" + path);
            foreach (XmlNode node in nlst)
            {
                string xml = "<xml>" + "\r\n" + node.InnerXml + "\r\n" + "</xml>";
                lst.Add(new ReadContextByXML(xml));
            }
            //
            return lst;
        }

        Dictionary<string, object> IReadContext.ToDictionary()
        {
            XmlNode node = doc.SelectSingleNode("xml");
            if (node == null)
            {
                return new Dictionary<string, object>();
            }
            else
            {
                return ToDictionary(node);
            }
        }

        private Dictionary<string, object> ToDictionary(XmlNode node)
        {
            if (node.ChildNodes.Count == 0)
            {
                return new Dictionary<string, object>();
            }
            else
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (XmlNode n in node.ChildNodes)
                {
                    if (dic.ContainsKey(n.Name) == true)
                    {
                        continue;
                    }
                 
                    dic.Add(n.Name, n.InnerXml.Replace("!@#", "<").Replace("#@!", ">"));
                }
                return dic;
            }
        }

    }
}
