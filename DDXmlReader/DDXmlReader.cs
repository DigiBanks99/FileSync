using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace DDXmlLib
{
  public class DDXmlReader : IDisposable
  {
    #region Properties
    private Stream _stream;
    public Stream Stream
    {
      get { return _stream; }
      set { _stream = value; }
    }
    #endregion Properties

    #region Enums
    public enum StreamCloseResult
    {
      Success,
      Busy,
      Failed
    }
    #endregion Enums

    #region Stream Connection
    public Stream OpenStream(string filePath)
    {
      Stream = File.Open(filePath, FileMode.Open);
      return Stream;
    }

    public StreamCloseResult CloseStream()
    {
      try
      {
        if (Stream != null)
        {
          Stream.Close();
          Stream.Flush();
          Stream = null;
        }
      }
      catch (IOException)
      {
        return StreamCloseResult.Failed;
      }

      return StreamCloseResult.Success;
    }
    #endregion Stream Connection

    public static IEnumerable<XmlNode> ReadElements(string filePath, string elementName)
    {
      var document = new XmlDocument();
      var elements = new List<XmlNode>();

      //using (XmlReader xmlReader = XmlReader.Create(filePath))
      //{
      //  xmlReader.MoveToContent();

      //  while (xmlReader.Read())
      //  {
      //    if (xmlReader.Name == elementName && xmlReader.NodeType == XmlNodeType.Element)
      //    {
      //      var node = document.CreateNode(XmlNodeType.Element, xmlReader.Name, string.Empty);
      //      node.InnerXml = xmlReader.ReadInnerXml();
      //      node.InnerText = node.InnerText.Trim();
      //      elements.Add(node);
      //    }
      //  }
      //}

      document.Load(filePath);
      var childNodes = document.ChildNodes;
      GetMatchingNode(elementName, elements, childNodes);

      return elements;
    }

    private static void GetMatchingNode(string elementName, List<XmlNode> elements, XmlNodeList childNodes)
    {
      foreach (XmlNode node in childNodes)
      {
        if (node.NodeType == XmlNodeType.Element && node.Name == elementName)
        {
          elements.Add(node);
          continue;
        }

        if (node.ChildNodes != null)
        {
          GetMatchingNode(elementName, elements, node.ChildNodes);
        }
      }
    }

    #region IDisposable Members

    public void Dispose()
    {
      CloseStream();
    }

    #endregion
  }
}
