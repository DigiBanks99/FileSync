using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
  public class WatchItemList : BaseList<WatchItem>
  {
    #region Constructor
    public WatchItemList()
    {

    }
    #endregion Constructor

    #region Overrides
    public virtual WatchItem AddNew()
    {
      var watchItem = new WatchItem(true);
      this.Add(watchItem);
      return watchItem;
    }
    #endregion Overrides

    #region IBaseList
    public WatchItemList NewList()
    {
      return new WatchItemList();
    }
    #endregion IBaseList

    #region Public Methods
    public WatchItemList GetFromXml(string path)
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
    #endregion Public Methods
  }
}
