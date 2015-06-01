using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
  public class WatchLocation
  {
    #region Constructor
    public WatchLocation()
    {

    }
    #endregion Constructor

    #region Properties
    [NonSerialized]
    private string _description;
    public string Description
    {
      get { return _description; }
      set { _description = value; }
    }

    [NonSerialized]
    private string _directory;
    public string Directory
    {
      get { return _directory; }
      set 
      {
        if (_directory == value)
          return;

        _directory = value;
        DirectoryChanged();
      }
    }

    [NonSerialized]
    private bool _isLocal;
    public bool IsLocal
    {
      get { return _isLocal; }
      set { _isLocal = value; }
    }

    [NonSerialized]
    private DateTime _dateAdded;
    public DateTime DateAdded
    {
      get { return _dateAdded; }
      set { _dateAdded = value; }
    }

    [NonSerialized]
    private DateTime _lastActivated;
    public DateTime LastActivated
    {
      get { return _lastActivated; }
      set { _lastActivated = value; }
    }
    #endregion Properties

    #region Methods
    public void UpdateLastActivated()
    {
      LastActivated = DateTime.Now;
    }

    private void DirectoryChanged()
    {
      if (Directory.Length < 3)
        return;

      if (Directory.Substring(0, 3).Contains(@"\\"))
        IsLocal = false;

      IsLocal = true;
    }
    #endregion Methods
  }
}
