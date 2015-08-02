using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using DDXmlLib;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FileSync
{
  public class FileSyncManager
  {

    #region Constructor
    private FileSyncManager()
    {
      _context = SynchronizationContext.Current ?? new SynchronizationContext();
    }
    #endregion Constructor

    #region Private Members
    private SynchronizationContext _context;
    [NonSerialized]
    private string _watchFilePath = string.Empty;
    [NonSerialized]
    private bool _cancel;
    [NonSerialized]
    private static FileSyncManager _manager = null;
    private static object objLock = new object();
    #endregion Private Members

    #region Public Properties
    public static FileSyncManager Manager
    {
      get
      {
        if (_manager != null)
          return _manager;

        lock (objLock)
        {
          if (_manager != null)
            return _manager;
          return _manager = new FileSyncManager();
        }
      }
    }

    public string WatchFilePath
    {
      get
      {
        return _watchFilePath;
      }
      set
      {
        _watchFilePath = value;
      }
    }

    public bool Cancel
    {
      get { return _cancel; }
      set
      {
        _cancel = value;
        RaiseCancelInContext(this, value);
      }
    }

    public Dictionary<string, List<string>> SyncedFiles { get; set; }
    #endregion Public Properties

    #region Events & Delegates
    public delegate void FileCopiedEventHandler(object sender, FileSyncEventArgs e);
    public delegate void FileCopyingEventHandler(object sender, FileSyncEventArgs e);
    public delegate void CanceledEventHandler(object sender, CancelEventArgs e);
    public delegate void ProgressChangedEventHandler(object sender, int newValue);
    public event FileCopiedEventHandler OnFileCopied;
    public event FileCopyingEventHandler OnFileCopying;
    public event CanceledEventHandler OnCancel;
    public event ProgressChangedEventHandler OnProgressChanged;
    #endregion Events & Delegates

    #region Public Methods
    public void AddNewWatch(WatchItem watchItem)
    {
      var watchList = ReadWatchList();

      if (watchList != null)
      {
        foreach (var existingWatchItem in watchList)
        {
          if (existingWatchItem.Name == watchItem.Name)
          {
            Logger.Error("The watch you are adding already exists.");
            return;
          }
        }
      }

      var xmlNode = DDXmlWriter.CreateXmlNode(watchItem);
      List<XmlNode> nodes = new List<XmlNode>();
      nodes.Add(xmlNode);

      DDXmlWriter.WriteElements(WatchFilePath, typeof(WatchList).Name, typeof(WatchItem).Name, nodes);
    }

    public void Sync(WatchList watchList = null)
    {
      var updateNodeList = DoSynchronization();

      if (updateNodeList != null && updateNodeList.Any())
        UpdateWatchList(updateNodeList);
    }

    public async Task SyncAsync(WatchList watchList = null)
    {
      var updateNodeList = DoSynchronization();

      if (updateNodeList != null && updateNodeList.Any())
        await UpdateWatchListAsync(updateNodeList);
    }

    public List<XmlNode> UpdateWatchItem(WatchItem watchItem, List<XmlNode> updateNodeList = null)
    {
      if (updateNodeList == null)
        updateNodeList = new List<XmlNode>();

      updateNodeList.Add(DDXmlWriter.CreateXmlNode(watchItem));

      return updateNodeList;
    }

    public void UpdateWatchList(List<XmlNode> updateNodeList)
    {
      try
      {
        DDXmlWriter.UpdateElements(WatchFilePath, "Name", updateNodeList);
      }
      catch (IOException ex)
      {
        Logger.Error("Update Watch List Error: " + ex.Message);
      }
    }

    public async Task UpdateWatchListAsync(List<XmlNode> updateNodeList)
    {
      try
      {
        await DDXmlWriter.UpdateElementsAsync(WatchFilePath, "Name", updateNodeList);
      }
      catch (IOException ex)
      {
        Logger.Error("Update Watch List Error: " + ex.Message);
      }
    }

    public WatchList ReadWatchList()
    {
      var elements = DDXmlReader.ReadElements(WatchFilePath, typeof(WatchItem).Name);
      if (elements == null)
      {
        Logger.Error("Failed to load Watch Items from the file...");
        return new WatchList();
      }

      WatchList list = new WatchList();

      foreach (XmlElement element in elements)
      {
        var watchItem = new WatchItem();

        foreach (XmlNode property in element.ChildNodes)
        {
          if (property.Name == "ExcludeKeyWords")
          {
            var val = property.InnerText;
            string[] valArray = val.Split(new string[] { Environment.NewLine, " " }, StringSplitOptions.None);
            if (valArray != null && valArray.Length > 0)
            {
              watchItem.ExcludeKeyWords = new List<ExcludeKeyWord>();
              foreach (string value in valArray)
              {
                if (val == string.Empty)
                  continue;

                watchItem.ExcludeKeyWords.Add(new ExcludeKeyWord() { KeyWord = value });
              }
            }

            continue;
          }

          if (property.Name == "ExcludeFolders")
          {
            var val = property.InnerText;
            string[] valArray = val.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (valArray != null && valArray.Length > 0)
            {
              watchItem.ExcludeFolders = new List<ExcludeFolder>();
              foreach (string value in valArray)
              {
                if (val == string.Empty)
                  continue;

                watchItem.ExcludeFolders.Add(new ExcludeFolder() { ExcludeFolderName = value.Trim() });
              }
            }

            continue;
          }

          var propertyInfo = watchItem.GetType().GetProperty(property.Name);
          var propertyVal = property.InnerText;
          if (propertyVal == null || propertyVal == string.Empty)
          {
            propertyInfo.SetValue(watchItem, null);
            continue;
          }

          if (typeof(bool).IsAssignableFrom(propertyInfo.PropertyType) ||
              typeof(Nullable<bool>).IsAssignableFrom(propertyInfo.PropertyType))
          {
            var textVal = propertyVal.ToLower();
            bool boolVal = false;
            switch (textVal)
            {
              case "t":
              case "true":
              case "1":
              case "yes":
              case "y":
                boolVal = true;
                break;
              case "f":
              case "false":
              case "0":
              case "no":
              case "n":
                boolVal = true;
                break;
            }
            propertyInfo.SetValue(watchItem, boolVal);
          }
          else if (typeof(short).IsAssignableFrom(propertyInfo.PropertyType) ||
                   typeof(Nullable<short>).IsAssignableFrom(propertyInfo.PropertyType))
          {
            propertyInfo.SetValue(watchItem, XmlConvert.ToInt16(property.InnerText));
          }
          else if (typeof(int).IsAssignableFrom(propertyInfo.PropertyType) ||
                   typeof(Nullable<int>).IsAssignableFrom(propertyInfo.PropertyType))
          {
            propertyInfo.SetValue(watchItem, XmlConvert.ToInt32(property.InnerText));
          }
          else if (typeof(long).IsAssignableFrom(propertyInfo.PropertyType) ||
                   typeof(Nullable<long>).IsAssignableFrom(propertyInfo.PropertyType))
          {
            propertyInfo.SetValue(watchItem, XmlConvert.ToInt64(property.InnerText));
          }
          else if (typeof(float).IsAssignableFrom(propertyInfo.PropertyType) ||
                   typeof(Nullable<float>).IsAssignableFrom(propertyInfo.PropertyType))
          {
            propertyInfo.SetValue(watchItem, XmlConvert.ToSingle(property.InnerText));
          }
          else if (typeof(decimal).IsAssignableFrom(propertyInfo.PropertyType) ||
                   typeof(Nullable<decimal>).IsAssignableFrom(propertyInfo.PropertyType))
          {
            propertyInfo.SetValue(watchItem, XmlConvert.ToDecimal(property.InnerText));
          }
          else if (typeof(double).IsAssignableFrom(propertyInfo.PropertyType) ||
                   typeof(Nullable<double>).IsAssignableFrom(propertyInfo.PropertyType))
          {
            propertyInfo.SetValue(watchItem, XmlConvert.ToDouble(property.InnerText));
          }
          else if (typeof(DateTime).IsAssignableFrom(propertyInfo.PropertyType) ||
                   typeof(Nullable<DateTime>).IsAssignableFrom(propertyInfo.PropertyType))
          {
            propertyInfo.SetValue(watchItem, Convert.ToDateTime(property.InnerText));
          }
          else if (typeof(Guid).IsAssignableFrom(propertyInfo.PropertyType) ||
                   typeof(Nullable<Guid>).IsAssignableFrom(propertyInfo.PropertyType))
          {
            propertyInfo.SetValue(watchItem, XmlConvert.ToGuid(property.InnerText));
          }
          else
          {
            propertyInfo.SetValue(watchItem, propertyVal);
          }
        }

        list.Add(watchItem);
      }

      return list;
    }
    #endregion Public Methods

    #region Private Methods
    private List<XmlNode> DoSynchronization(WatchList watchList = null)
    {
      if (watchList == null)
        watchList = ReadWatchList();

      if (watchList == null)
      {
        Logger.Warning("Nothing has been listed for synchronisation");
        return null;
      }

      SyncedFiles = new Dictionary<string, List<string>>();
      List<XmlNode> updateNodeList = new List<XmlNode>();

      var cnt = 0;
      // Itterate through each watch item and take the appropriate action
      foreach (var watchItem in watchList)
      {
        if (Cancel)
          return null;

        // If the destination does not exist, create it
        if (!Directory.Exists(watchItem.DestinationPath))
          Directory.CreateDirectory(watchItem.DestinationPath);

        var sourceDirInfo = new DirectoryInfo(watchItem.SourcePath);
        var destDirInfo = new DirectoryInfo(watchItem.DestinationPath);

        // If the last sync date is after the last write time for the source directory, don't take any action
        if (watchItem.LastSyncDate.HasValue && watchItem.LastSyncDate.Value >= sourceDirInfo.LastWriteTime)
        {
          Logger.Info(string.Format("Nothing to sync for {0}", watchItem.Name));
          RaiseProgressChangedInContext(this, ++cnt);
          continue;
        }

        Logger.Info(string.Format("Copying files for {0}...", watchItem.Name));

        SyncFolders(watchItem, sourceDirInfo, destDirInfo);
        CopyFilesInDirectory(sourceDirInfo, watchItem, destDirInfo, string.Empty);

        watchItem.LastSyncDate = DateTime.Now;
        UpdateWatchItem(watchItem, updateNodeList);
        RaiseProgressChangedInContext(this, ++cnt);
      }

      return updateNodeList;
    }

    private void SyncFolders(WatchItem watchItem, DirectoryInfo sourceDirInfo, DirectoryInfo destDirInfo)
    {
      var sourceDirectories = sourceDirInfo.GetDirectories();
      foreach (var dir in sourceDirectories)
      {
        // Don't copy folders that have been marked for exclusion
        if (watchItem.ExcludeFolders != null && watchItem.ExcludeFolders.Contains(dir.Name))
          continue;

        // Create the directory if it does not exist
        if (!Directory.Exists(Path.Combine(destDirInfo.FullName, dir.Name)))
          Directory.CreateDirectory(Path.Combine(destDirInfo.FullName, dir.Name));

        SyncFolders(watchItem, dir, new DirectoryInfo(Path.Combine(destDirInfo.FullName, dir.Name)));

        // Get the files in the directory and copy the files
        CopyFilesInDirectory(dir, watchItem, destDirInfo, dir.Name);
      }
    }

    private void CopyFilesInDirectory(DirectoryInfo sourceDir, WatchItem watch, DirectoryInfo destDirInfo, string subDirectory)
    {
      if (sourceDir == null)
        throw new ArgumentException("sourceDir");

      var files = sourceDir.GetFiles();
      if (files == null)
        return;

      foreach (var file in files)
      {
        if (Cancel)
          return;

        var skipFile = watch.ExcludeKeyWords.FindAll(s => file.Name.ToLower()?.IndexOf(s.KeyWord?.ToLower()) >= 0);
        if (skipFile.Count > 0)
          continue;

        if (File.Exists(Path.Combine(destDirInfo.FullName, subDirectory, file.Name)))
          continue;

        string fileName = $"{subDirectory}{(subDirectory.Length > 0 ? "," : string.Empty)}{file.Name}";

        RaiseOnCopyingFileInContext(this, $"Copying {file.Name}...");
        File.Copy(file.FullName, Path.Combine(destDirInfo.FullName, subDirectory, file.Name));
        RaiseOnFileCopiedInContext(this, $"Copied {file.Name}: {fileName}");

        // Added synced file to list of synced files
        if (!SyncedFiles.ContainsKey(watch.Name))
          SyncedFiles.Add(watch.Name, new List<string>());

        SyncedFiles[watch.Name].Add(fileName);
      }
    }

    private void RaiseOnFileCopiedInContext(object sender, string message)
    {
      if (OnFileCopied == null)
        return;

      _context?.Post(changed => OnFileCopied(this, new FileSyncEventArgs(message)), sender);
    }

    private void RaiseOnCopyingFileInContext(object sender, string message)
    {
      if (OnFileCopying == null)
        return;

      _context?.Post(changing => OnFileCopying(this, new FileSyncEventArgs(message)), sender);
    }

    private void RaiseCancelInContext(object sender, bool value)
    {
      if (OnCancel == null)
        return;

      _context?.Post(cancelled => OnCancel(this, new CancelEventArgs(value, "Cancelling...")), value);
    }

    private void RaiseProgressChangedInContext(object sender, int newVal)
    {
      if (OnProgressChanged == null)
        return;

      _context?.Post(changed => OnProgressChanged(this, newVal), this);
    }
    #endregion Private Methods
  }
}
