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

    public Dictionary<string, List<string>> SyncedFiles { get; set; }
    #endregion Public Properties

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

    public void Sync()
    {
      var watchList = ReadWatchList();
      if (watchList == null)
      {
        Logger.Warning("Nothing has been listed for synchronisation");
        return;
      }

      SyncedFiles = new Dictionary<string, List<string>>();
      var updateNodeList = new List<XmlNode>();

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
        updateNodeList.Add(DDXmlWriter.CreateXmlNode(watchItem));

        var sourceDirectories = sourceDirInfo.GetDirectories();
        FileInfo[] sourceFiles = null;
        foreach (var dir in sourceDirectories)
        {
          // Don't copy folders that have been marked for exclusion
          if (watchItem.ExcludeFolders != null && watchItem.ExcludeFolders.Contains(dir.Name))
            continue;

          // Create the directory if it does not exist
          if (!Directory.Exists(Path.Combine(destDirInfo.FullName, dir.Name)))
            Directory.CreateDirectory(Path.Combine(destDirInfo.FullName, dir.Name));

          // Get the files in the directory and copy the files
          sourceFiles = dir.GetFiles();
          CopyFilesInDirectory(sourceFiles, watchItem, destDirInfo, dir.Name);
        }

        sourceFiles = sourceDirInfo.GetFiles();
        CopyFilesInDirectory(sourceFiles, watchItem, destDirInfo, string.Empty);
      }

      if (updateNodeList.Any())
        UpdateWatchList(updateNodeList);
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
    #endregion Public Methods

    #region Private Methods
    private void CopyFilesInDirectory(FileInfo[] files, WatchItem watch, DirectoryInfo destDirInfo, string subDirectory)
    {
      if (files == null)
        return;

      foreach (var file in files)
      {
        var skipFile = watch.ExcludeKeyWords.FindAll(s => s.IndexOf(file.Name.ToLower()) >= 0);
        if (skipFile.Count > 0)
          continue;

        if (File.Exists(Path.Combine(destDirInfo.FullName, subDirectory, file.Name)))
          continue;

        File.Copy(file.FullName, Path.Combine(destDirInfo.FullName, subDirectory, file.Name));

        // Added synced file to list of synced files
        if (!SyncedFiles.ContainsKey(watch.Name))
          SyncedFiles.Add(watch.Name, new List<string>());

        SyncedFiles[watch.Name].Add(string.Format("{0}{1}{2}", subDirectory, subDirectory.Length > 0 ? "/" : string.Empty, file.Name));
      }
    }

    private WatchList ReadWatchList()
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
            watchItem.ExcludeKeyWords = new List<string>();

            var val = property.InnerText;
            string[] valArray = val.Split(new string[] { Environment.NewLine, " " }, StringSplitOptions.None);
            if (valArray != null)
            {
              foreach (string value in valArray)
              {
                watchItem.ExcludeKeyWords.Add(value);
              }
            }

            continue;
          }

          if (property.Name == "ExcludeFolders")
          {
            watchItem.ExcludeFolders = new List<string>();

            var val = property.InnerText;
            string[] valArray = val.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (valArray != null)
            {
              foreach (string value in valArray)
              {
                watchItem.ExcludeFolders.Add(value.Trim());
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
    #endregion Private Methods
  }
}
