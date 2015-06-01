using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
  public class WatchLocationList : BaseList<WatchLocation>
  {
    #region Constructor
    public WatchLocationList()
    {

    }
    #endregion Constructor

    #region Overrides
    public virtual WatchLocation AddNew()
    {
      var watchLocation = new WatchLocation();
      this.Add(watchLocation);
      watchLocation.DateAdded = DateTime.Now;
      return watchLocation;
    }
    #endregion Overrides

    #region IBaseList
    public WatchLocationList NewList()
    {
      return new WatchLocationList();
    }
    #endregion IBaseList
  }
}
