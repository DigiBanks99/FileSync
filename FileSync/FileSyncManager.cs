using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using DDXmlLib;

namespace FileSync
{
  public class FileSyncManager
  {
    #region Public Properties
    public string WatchFilePath
    {
      get;
      set;
    }

    private bool _cancel;
    public bool Cancel
    {
      get { return _cancel; }
      set
      {
        _cancel = value;
        if (!value && OnCancel != null)
          OnCancel(this, new CancelEventArgs(value, "Canceled."));
      }
    }

    public Dictionary<string, List<string>> SyncedFiles { get; set; }
    #endregion Public Properties

    #region Events & Delegates
    public delegate void ProgressChangedHandler(object sender, FileSyncEventArgs e);
    public delegate void ProgressChangingHandler(object sender, FileSyncEventArgs e);
    public delegate void CanceledEventHandler(object sender, CancelEventArgs e);
    public event ProgressChangedHandler OnProgressChanged;
    public event ProgressChangingHandler OnProgressChanging;
    public event CanceledEventHandler OnCancel;
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
      if (watchList == null)
        watchList = ReadWatchList();

      if (watchList == null)
      {
        Logger.Warning("Nothing has been listed for synchronisation");
        return;
      }

      SyncedFiles = new Dictionary<string, List<string>>();
      List<XmlNode> updateNodeList = new List<XmlNode>();

      // Itterate through each watch item and take the appropriate action
      foreach (var watchItem in watchList)
      {
        // If the destination does not exist, create it
        if (!Directory.Exists(watchItem.DestinationPath))
          Directory.CreateDirectory(watchItem.DestinationPath);

        var sourceDirInfo = new DirectoryInfo(watchItem.SourcePath);
        var destDirInfo = new DirectoryInfo(watchItem.DestinationPath);

        // If the last sync date is after the last write time for the source directory, don't take any action
        if (watchItem.LastSyncDate.HasValue && watchItem.LastSyncDate.Value >= sourceDirInfo.LastWriteTime)
        {
          Logger.Info(string.Format("Nothing to sync for {0}", watchItem.Name));
          continue;
        }

        Logger.Info(string.Format("Copying files for {0}...", watchItem.Name));

        watchItem.LastSyncDate = DateTime.Now;
        UpdateWatchItem(watchItem, updateNodeList);

        SyncFolders(watchItem, sourceDirInfo, destDirInfo);
        CopyFilesInDirectory(sourceDirInfo, watchItem, destDirInfo, string.Empty);
      }

      if (updateNodeList.Any())
        UpdateWatchList(updateNodeList);
    }

    public List<XmlNode> UpdateWatchItem(WatchItem watchItem, List<XmlNode> updateNodeList = null)
    {
      if (updateNodeList == null)
        updateNodeList = new List<XmlNode>();

      updateNodeList.Add(DDXmlWriter.CreateXmlNode(watchItem));

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

    public WatchList ReadWatchList()
    {
      var elements = DDXmlReader.ReadElements(WatchFilePath, typeof(WatchItem).Name);
      if (elements == null)
      {
        Logger.Error("Failed to load Watch Items from the file...");
        return null;
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
              watchItem.ExcludeKeyWords = new List<string>();
              foreach (string value in valArray)
              {
                if (val == string.Empty)
                  continue;

                watchItem.ExcludeKeyWords.Add(value);
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

        var skipFile = watch.ExcludeKeyWords.FindAll(s => s.IndexOf(file.Name.ToLower()) >= 0);
        if (skipFile.Count > 0)
          continue;

        if (File.Exists(Path.Combine(destDirInfo.FullName, subDirectory, file.Name)))
          continue;

        if (OnProgressChanging != null)
          OnProgressChanging(this, new FileSyncEventArgs(string.Format("Copying {0}...", file.Name)));

        File.Copy(file.FullName, Path.Combine(destDirInfo.FullName, subDirectory, file.Name));

        string fileName = string.Format("{0}{1}{2}", subDirectory, subDirectory.Length > 0 ? "/" : string.Empty, file.Name);
        if (OnProgressChanged != null)
          OnProgressChanged(this, new FileSyncEventArgs(string.Format("Copied {0}: {1}", file.Name, fileName)));

        // Added synced file to list of synced files
        if (!SyncedFiles.ContainsKey(watch.Name))
          SyncedFiles.Add(watch.Name, new List<string>());

        SyncedFiles[watch.Name].Add(fileName);
      }
    }
    #endregion Private Methods
  }
}
