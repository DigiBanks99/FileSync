using System;
using System.Collections.Generic;
using System.Xml;

namespace FileSync
{
  public class WatchItem
  {
    #region Properties
    private string _name;
    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;
      }
    }

    private string _sourcePath;
    public string SourcePath
    {
      get { return _sourcePath; }
      set
      {
        _sourcePath = value;
      }
    }

    private string _destPath;
    public string DestinationPath
    {
      get { return _destPath; }
      set
      {
        _destPath = value;
      }
    }

    private bool _includeSubFolders;
    public bool IncludeSubFolders
    {
      get { return _includeSubFolders; }
      set
      {
        _includeSubFolders = value;
      }
    }

    private List<string> _excludeKeyWords;
    public List<string> ExcludeKeyWords
    {
      get { return _excludeKeyWords; }
      set
      {
        _excludeKeyWords = value;
      }
    }

    private List<string> _excludeFolders;
    public List<string> ExcludeFolders
    {
      get { return _excludeFolders; }
      set
      {
        _excludeFolders = value;
      }
    }

    private DateTime? _lastSyncDate;
    public DateTime? LastSyncDate
    {
      get { return _lastSyncDate; }
      set
      {
        _lastSyncDate = value;
      }
    }
    #endregion Properties
  }
}
