using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using DDXmlLib.ExceptionHandling;
using System.Threading.Tasks;

namespace DDXmlLib
{
  public class DDXmlWriter
  {
    public static XmlNode CreateXmlNode(object item, XmlDocument document = null, List<string> excludeProperties = null, string name = "")
    {
      if (item == null)
        return null;

      if (document == null)
        document = new XmlDocument();

      var type = item.GetType();

      if (name == string.Empty)
        name = type.Name;

      XmlNode node = document.CreateElement(name);

      if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type)) // TODO: test if nullables work.
      {
        foreach (var collectionItem in (System.Collections.IEnumerable)item)
        {
          var collectionChildNode = CreateXmlNode(collectionItem, document, excludeProperties, collectionItem.GetType().Name);
          if (collectionChildNode != null && collectionChildNode.InnerXml != null && collectionChildNode.InnerXml != string.Empty)
            node.AppendChild(collectionChildNode);
        }
        return node;
      }

      foreach (var property in type.GetProperties())
      {
        // Exclude some properties
        if (excludeProperties != null)
        {
          var foundItems = excludeProperties.FindAll(s => s.IndexOf(property.Name, StringComparison.OrdinalIgnoreCase) >= 0);
          if (foundItems != null || foundItems.Any())
            continue;
        }

        var propertyNode = document.CreateNode(XmlNodeType.Element, property.Name, string.Empty);

        // Check if property is a generic collection of some sort and return it as an element
        if (typeof(System.Collections.ICollection).IsAssignableFrom(property.PropertyType))
        {
          XmlNode newNode = CreateXmlNode(property.GetValue(item), document, name: property.Name);
          if (newNode != null)
            propertyNode = newNode;
        }
        else // add the value as inner text
        {
          var value = property.GetValue(item);
          if (value != null)
            propertyNode.InnerText = value.ToString();
          else
            continue;
        }

        node.AppendChild(propertyNode);
      }

      return node;
    }

    public static void WriteElements(string filePath, string rootElement, string elementName, IEnumerable<XmlNode> elements)
    {
      if (elements == null)
        throw new ArgumentNullException("elements");

      if (!elements.Any())
        return;

      XmlDocument document = new XmlDocument();

      if (!File.Exists(filePath))
      {
        using (var xmlWriter = XmlWriter.Create(filePath))
        {
          xmlWriter.WriteStartElement(rootElement);
          WriteChildElement(elementName, elements, xmlWriter);
          xmlWriter.WriteEndElement();
        }
      }
      else
      {
        try
        {
          document.Load(filePath);
        }
        catch (XmlException ex)
        {
          if (ex.Message.StartsWith("Root element is missing"))
          {
            try
            {
              document.AppendChild(document.CreateElement(rootElement));
              document.Save(filePath);
              document.Load(filePath);
            }
            catch
            {
              throw new CorruptedFileException(string.Format("Failed to find a '{0}' element in the document.", rootElement));
            }
          }
        }
        var rootListElems = document.GetElementsByTagName(rootElement);
        if (rootListElems == null || rootListElems.Count == 0)
          throw new CorruptedFileException(string.Format("Failed to find a '{0}' element in the document.", rootElement));

        if (rootListElems.Count > 1)
          throw new CorruptedFileException(string.Format("There are more than one '{0}' elements in this document. Please ensure that there is only one before trying again.", rootElement));

        var rootElem = rootListElems[0];
        foreach (XmlNode node in elements)
        {
          var found = false;
          foreach (XmlNode existingNode in rootElem.ChildNodes)
          {
            if (existingNode.Name == node.Name && existingNode.InnerXml == node.InnerXml)
            {
              found = true;
              break;
            }
          }

          if (!found)
            rootElem.AppendChild(document.ImportNode(node, true));
        }

        document.Save(filePath);
      }
    }

    private static void WriteChildElement(string elementName, IEnumerable<XmlNode> elements, XmlWriter xmlWriter)
    {
      foreach (var node in elements)
      {
        xmlWriter.WriteStartElement(elementName);

        if (node.ChildNodes != null && node.ChildNodes.Count > 0)
        {
          foreach (XmlNode childNode in node.ChildNodes)
          {
            if (childNode.NodeType == XmlNodeType.Text || childNode.NodeType == XmlNodeType.Whitespace || childNode.NodeType == XmlNodeType.None)
            {
              xmlWriter.WriteValue(childNode.Value);
              continue;
            }

            WriteChildElement(childNode.Name, node.ChildNodes.GetList(), xmlWriter);
          }
        }
        else
        {
          xmlWriter.WriteValue(node.InnerText);
        }

        xmlWriter.WriteEndElement();
      }
    }

    public static void UpdateElements(string filePath, string identifierName, IEnumerable<XmlNode> updateElements)
    {
      XmlDocument document = null;
      try
      {
        document = DDXmlReader.ReadDocument(filePath);
      }
      catch (IOException ex)
      {
        throw ex;
      }

      if (document == null)
        return; // TODO: handle a document that cannot be read

      foreach (XmlElement updateElement in updateElements)
      {
        // Find the identifier element
        XmlElement identifier = updateElement[identifierName];
        if (identifier == null)
          continue; // TODO: handle elements that don't have a valid identifier when updating the watch list

        if (document.HasChildNodes)
          UpdateChildElements(updateElement, identifier, document.ChildNodes, document);

        document.Save(filePath);
      }
    }

    public async static Task UpdateElementsAsync(string filePath, string identifierName, IEnumerable<XmlNode> updateElements)
    {
      XmlDocument document = null;
      try
      {
        document = await DDXmlReader.ReadDocumentAsync(filePath);
      }
      catch (IOException ex)
      {
        throw ex;
      }

      if (document == null)
        return; // TODO: handle a document that cannot be read

      foreach (XmlElement updateElement in updateElements)
      {
        // Find the identifier element
        XmlElement identifier = updateElement[identifierName];
        if (identifier == null)
          continue; // TODO: handle elements that don't have a valid identifier when updating the watch list

        if (document.HasChildNodes)
          UpdateChildElements(updateElement, identifier, document.ChildNodes, document);

        await Task.Factory.StartNew(() => document.Save(filePath));
      }
    }

    public static void UpdateChildElements(XmlElement updateElement, XmlElement identifier, XmlNodeList childNodes, XmlDocument document)
    {
      foreach (XmlElement element in childNodes)
      {
        if (element.Name != updateElement.Name)
        {
          if (!element.HasChildNodes)
            continue;

          UpdateChildElements(updateElement, identifier, element.ChildNodes, document);
          continue;
        }

        if (element[identifier.Name] == null || element[identifier.Name].InnerXml != updateElement[identifier.Name].InnerXml)
          continue;

        foreach (XmlElement updateProperty in updateElement.ChildNodes)
        {
          if (element[updateProperty.Name] == null)
          {
            var node = document.CreateElement(updateProperty.Name);
            element.AppendChild(node);
          }

          element[updateProperty.Name].InnerXml = updateProperty.InnerXml;
        }

        for (int i = element.ChildNodes.Count - 1; i >= 0; i--)
        {
          var property = element.ChildNodes[i];

          if (updateElement[property.Name] == null)
            element.RemoveChild(property);
        }
      }
    }
  }

  public static class XmlNodeListExt
  {
    public static List<XmlNode> GetList(this XmlNodeList list)
    {
      var nodes = new List<XmlNode>();
      for (var i = 0; i < list.Count; i++)
        nodes.Add(list[i]);

      return nodes;
    }
  }
}
