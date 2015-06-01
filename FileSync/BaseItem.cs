using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
  #region Interface
  public interface IBaseItem
  {
    string Name { get; }
    void CreateXmlObject(System.IO.MemoryStream stream, System.Xml.XmlWriter xmlWriter);
  }
  #endregion Interface

  public class BaseItem : IBaseItem
  {
    public virtual string Name { get { return GetType().Name; } }

    public virtual void CreateXmlObject(System.IO.MemoryStream stream, System.Xml.XmlWriter xmlWriter)
    {
      xmlWriter.WriteStartElement(Name);

      var properties = this.GetType().GetProperties();
      foreach (var prop in properties)
      {
        if (prop.PropertyType is IBaseList)
        {
          XMLManager.CreateXmlObject((IBaseList)prop.GetValue(this), stream, xmlWriter);
          continue;
        }

        xmlWriter.WriteStartElement(prop.Name);
        xmlWriter.WriteValue(prop.GetValue(this));
        xmlWriter.WriteEndElement();
      }

      xmlWriter.WriteEndElement();
    }
  }
}
