using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileSync
{
  public class XMLManager
  {
    public static void SaveToXML(IBaseList list)
    {
      var stream = new MemoryStream();
      var xmlWriter = XmlWriter.Create(stream);
      xmlWriter.WriteStartDocument();
      CreateXmlObject(list, stream, xmlWriter);
      xmlWriter.Flush();
      stream.Flush();
      stream.Close();
    }

    public static void CreateXmlObject(IBaseList list, MemoryStream stream, XmlWriter xmlWriter)
    {
      xmlWriter.WriteStartElement(list.GetType().Name);

      foreach (var item in list)
      {
        ((IBaseItem)item).CreateXmlObject(stream, xmlWriter);
      }
      xmlWriter.WriteEndElement();
    }

    public static IBaseItem ReadFromXml(System.Xml.XmlReader xmlReader, IBaseItem item)
    {
      xmlReader.MoveToContent();
      if (xmlReader.IsEmptyElement)
      {
        xmlReader.Read();
        return item;
      }

      xmlReader.Read();
      while (!xmlReader.EOF)
      {
        if (xmlReader.IsStartElement())
        {
          if (!string.IsNullOrEmpty(xmlReader.Name) && item.GetType().GetProperty(xmlReader.Name).GetValue(item, null) == null)
            break;
          else
          {
            string val = xmlReader.ReadElementContentAsString();
            if (!string.IsNullOrEmpty(val))
              item.GetType().GetProperty(xmlReader.Name).SetValue(item, val);
          }
        }
        else
        {
          xmlReader.Read();
          break;
        }
      }
      return item;
    }

    public static IBaseList ReadFromXml(System.Xml.XmlReader xmlReader, IBaseList list)
    {
      xmlReader.MoveToContent();
      if (xmlReader.IsEmptyElement)
      {
        xmlReader.Read();
        return list;
      }

      xmlReader.Read();
      while (!xmlReader.EOF)
      {
        if (xmlReader.IsStartElement())
        {
          if (xmlReader.Name == list.Name)
          {
            ReadFromXml(xmlReader, list);
          }
          else if (xmlReader.Name == list.ChildName)
          {
            var item = list.AddNew();
            ReadFromXml(xmlReader, item);
          }
          else
            xmlReader.Skip();
        }
        else
        {
          xmlReader.Read();
          break;
        }
      }
      return list;
    }
  }
}
