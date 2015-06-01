using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
  #region Interface
  public interface IBaseList : System.Collections.IList
  {
    string Name { get; }
    IBaseList NewList();
    string ChildName { get; }
    IBaseItem AddNew();
  }
  #endregion Interface

  #region Object
  public abstract class BaseList<T> : List<T>, IBaseList
  {
    public virtual string Name { get { return this.GetType().Name; } }
    public virtual string ChildName { get { return typeof(T).Name; } }

    public abstract IBaseList NewList();
    public abstract IBaseItem AddNew();

    public IBaseList GetFromXml(string path)
    {
      var list = NewList();
      if (path == null || path.Length < 1)
        return list;

      using (var stream = new System.IO.MemoryStream())
      {
        using (var xmlReader = System.Xml.XmlReader.Create(stream))
        {
          XMLManager.ReadFromXml(xmlReader, list);
        }
      }

      return list;
    }
  }
  #endregion Object
}
