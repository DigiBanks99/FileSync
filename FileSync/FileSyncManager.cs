using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using DDXmlLib;

namespace FileSync
{
  public class FileSyncManager
  {
    public string WatchFilePath
    {
      get;
      set;
    }

    public Dictionary<string, List<string>> SyncedFiles { get; set; }

    public void AddNewWatch(WatchItem watchItem)
    {
      var xmlNode = DDXmlWriter.CreateXmlNode(watchItem);
      List<XmlNode> nodes = new List<XmlNode>();
      nodes.Add(xmlNode);

      DDXmlWriter.WriteElements(WatchFilePath, typeof(WatchList).Name, typeof(WatchItem).Name, nodes);
    }

    public void Sync()
    {
      var watchList = ReadWatchList();
      SyncedFiles = new Dictionary<string, List<string>>();

      if (watchList == null)
      {
        Logger.Warning("Nothing has been listed for synchronisation");
        return;
      }

      foreach (var watch in watchList)
      {
        if (!Directory.Exists(watch.DestinationPath))
          Directory.CreateDirectory(watch.DestinationPath);

        var sourceDirInfo = new DirectoryInfo(watch.SourcePath);
        var destDirInfo = new DirectoryInfo(watch.DestinationPath);

        if (watch.LastSyncDate >= sourceDirInfo.LastWriteTime)
        {
          Logger.Info(string.Format("Nothing to sync for {0}", watch.Name));
          continue;
        }

        Logger.Info(string.Format("Copying files for {0}...", watch.Name));

        watch.LastSyncDate = DateTime.Now;

        var sourceDirectories = sourceDirInfo.GetDirectories();
        FileInfo[] sourceFiles = null;
        foreach (var dir in sourceDirectories)
        {
          if (!Directory.Exists(Path.Combine(destDirInfo.FullName, dir.Name)))
            Directory.CreateDirectory(Path.Combine(destDirInfo.FullName, dir.Name));

          sourceFiles = dir.GetFiles();
          CopyFilesInDirectory(sourceFiles, watch, destDirInfo, dir.Name);
        }

        sourceFiles = sourceDirInfo.GetFiles();
        CopyFilesInDirectory(sourceFiles, watch, destDirInfo, string.Empty);
      }
    }

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

    #region Private Methods
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
            string[] valArray = val.Split(' ', '\n', '\r');
            if (valArray != null)
            {
              foreach (string value in valArray)
              {
                watchItem.ExcludeKeyWords.Add(value);
              }
            }

            continue;
          }

          var propertyInfo = watchItem.GetType().GetProperty(property.Name);
          var propertyVal = property.InnerText;
          if (propertyVal == null)
          {
            propertyInfo.SetValue(watchItem, null);
            continue;
          }

          if (propertyInfo.PropertyType == typeof(bool))
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
          else if (propertyInfo.PropertyType == typeof(int))
          {
            propertyInfo.SetValue(watchItem, XmlConvert.ToInt32(property.InnerText));
          }
          else if (propertyInfo.PropertyType == typeof(decimal))
          {
            propertyInfo.SetValue(watchItem, XmlConvert.ToDecimal(property.InnerText));
          }
          else if (propertyInfo.PropertyType == typeof(DateTime))
          {
            propertyInfo.SetValue(watchItem, Convert.ToDateTime(property.InnerText));
          }
          else if (propertyInfo.PropertyType == typeof(Guid))
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
