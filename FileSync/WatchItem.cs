<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;

namespace FileSync
{
  public class WatchItem
  {
    #region Properties
    private string _name;
    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    private string _sourcePath;
    public string SourcePath
    {
      get { return _sourcePath; }
      set { _sourcePath = value; }
    }

    private string _destPath;
    public string DestinationPath
    {
      get { return _destPath; }
      set { _destPath = value; }
    }

    private bool _includeSubFolders;
    public bool IncludeSubFolders
    {
      get { return _includeSubFolders; }
      set { _includeSubFolders = value; }
    }

    private List<string> _excludeKeyWords;
    public List<string> ExcludeKeyWords
    {
      get { return _excludeKeyWords; }
      set { _excludeKeyWords = value; }
    }

    private DateTime _lastSyncDate;
    public DateTime LastSyncDate
    {
      get { return _lastSyncDate; }
      set { _lastSyncDate = value; }
    }
    #endregion Properties
  }
}
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
  public class WatchItem
  {
    #region Constructor
    public WatchItem(bool isNew = false)
    {
      if (isNew)
        InsertDateAdded();
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
    private string _directoryPath;
    public string DirectoryPath
    {
      get { return _directoryPath; }
      set { _directoryPath = value; }
    }

    [NonSerialized]
    private WatchLocationList _watchLocations;
    public WatchLocationList WatchLocations
    {
      get { return _watchLocations; }
      set { _watchLocations = value; }
    }

    [NonSerialized]
    private string _genre;
    public string Genre
    {
      get { return _genre; }
      set { _genre = value; }
    }

    [NonSerialized]
    private DateTime _dateAdded;
    public DateTime DateAdded
    {
      get { return _dateAdded; }
      set { _dateAdded = value; }
    }
    #endregion Properties

    #region Methods
    public void AddWatchLocation(string storePath, string description = "")
    {
      var watchLocationItem = _watchLocations.AddNew();
      watchLocationItem.Directory = storePath;
      if (description != null && description != "")
        watchLocationItem.Description = description;
      else
      {
        watchLocationItem.Description = System.IO.Path.GetDirectoryName(storePath);
      }

    }

    public void RemoveWatchLocation(int index)
    {
      try
      {
        _watchLocations.RemoveAt(index);
      }
      catch (IndexOutOfRangeException ex)
      {
        Console.WriteLine("The list could not find the item specified.\n" + ex.Message);
        return;
      }
      catch (NullReferenceException ex)
      {
        Console.WriteLine("The watchlist has not been properly initialised. Contact the developer.\n" + ex.Message);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    public void RemoveWatchLocation(string storePath)
    {
      try
      {
        var watchLocationItem = _watchLocations.First<WatchLocation>(t => t.Directory == storePath);
        if (watchLocationItem == null)
        {
          Console.WriteLine("WARNING: The item to be removed could not be found in the list.");
          return;
        }

        _watchLocations.Remove(watchLocationItem);
      }
      catch (IndexOutOfRangeException ex)
      {
        Console.WriteLine("The list could not find the item specified.\n" + ex.Message);
        return;
      }
      catch (NullReferenceException ex)
      {
        Console.WriteLine("The watchlist has not been properly initialised. Contact the developer.\n" + ex.Message);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    private void InsertDateAdded()
    {
      DateAdded = DateTime.Now;
    }
    #endregion Methods
  }
}
>>>>>>> 520300d9827f3b6b839a4567026727f1e0c68051
