using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace FileSync
{
  public class WatchItem : INotifyPropertyChanged
  {
    #region Properties
    private string _name;
    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;
        NotifyPropertyChanged("Name");
      }
    }

    private string _sourcePath;
    public string SourcePath
    {
      get { return _sourcePath; }
      set
      {
        _sourcePath = value;
        NotifyPropertyChanged("SourcePath");
      }
    }

    private string _destPath;
    public string DestinationPath
    {
      get { return _destPath; }
      set
      {
        _destPath = value;
        NotifyPropertyChanged("DestinationPath");
      }
    }

    private bool _includeSubFolders;
    public bool IncludeSubFolders
    {
      get { return _includeSubFolders; }
      set
      {
        _includeSubFolders = value;
        NotifyPropertyChanged("IncludeSubFolders");
      }
    }

    private List<string> _excludeKeyWords;
    public List<string> ExcludeKeyWords
    {
      get { return _excludeKeyWords; }
      set
      {
        _excludeKeyWords = value;
        NotifyPropertyChanged("ExcludeKeyWords");
      }
    }

    private List<ExcludeFolder> _excludeFolders;
    public List<ExcludeFolder> ExcludeFolders
    {
      get { return _excludeFolders; }
      set
      {
        _excludeFolders = value;
        NotifyPropertyChanged("ExcludeFolders");
      }
    }

    private DateTime? _lastSyncDate;
    public DateTime? LastSyncDate
    {
      get { return _lastSyncDate; }
      set
      {
        _lastSyncDate = value;
        NotifyPropertyChanged("LastSyncDate");
      }
    }
    #endregion Properties

    #region Events and Delegates
    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion Events and Delegates
  }
}
