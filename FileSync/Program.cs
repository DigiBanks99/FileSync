using System;
using DDXmlLib.ExceptionHandling;

namespace FileSync
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var manager = new FileSyncManager();
      manager.WatchFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FileSync", "WatchList.dfs");

      var watchItem = new WatchItem();
      watchItem.Name = "Testing 1";
      watchItem.SourcePath = @"C:\Test";
      watchItem.DestinationPath = @"C:\TestDest";
      watchItem.LastSyncDate = DateTime.Now;
      watchItem.IncludeSubFolders = true;

      try
      {
        manager.Sync();

        Logger.Info(string.Empty);
        if (manager.SyncedFiles.Keys.Count == 0)
        {
          Logger.Info("All files up to date.");
        }
        else
        {
          foreach (var key in manager.SyncedFiles.Keys)
          {
            Logger.Success(string.Format("Added the following files for {0}:", key));
            foreach (var file in manager.SyncedFiles[key])
              Logger.Success(string.Format("\t{0}", file));
          }
        }
      }
      catch (System.IO.IOException ex)
      {
        Logger.Error(ex.Message);
      }
      Logger.Info(string.Empty);

      Console.Write("Press any key to continue...");
      Console.ReadKey();
    }
  }
}