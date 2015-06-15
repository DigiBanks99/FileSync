using System;
using DDXmlLib.ExceptionHandling;

namespace FileSync
{
  public class Program
  {
    private static FileSyncManager _manager;

    public static void Main(string[] args)
    {
      _manager = new FileSyncManager();
      _manager.WatchFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FileSync", "WatchList.dfs");

      if (args.Length > 0)
      {
        if (args[0].ToLower() == "add")
        {
          if (args.Length < 4)
          {
            Logger.Info("Usage for add is: add <name> <sourcePath> <destinationPath> [-e]");
            return;
          }

          var name = args[1];
          var source = args[2];
          var dest = args[3];
          var includeSubFolders = true;
          if (args.Length > 4)
            includeSubFolders = args[4] == "-e" ? false : true;

          AddWatch(name, source, dest, includeSubFolders);
        }
        else if (args[0].ToLower() == "rem" ||
                args[0].ToLower() == "remove" ||
                args[0].ToLower() == "del" ||
                args[0].ToLower() == "delete")
        {
          // TODO: implement the delete feature
        }
        else if (args[0].ToLower() == "sync")
        {
          Sync();

          Console.Write("Press any key to continue...");
          Console.ReadKey();
        }
        else if (args[0].ToLower() == "help" ||
                args[0].ToLower() == "?")
        {
          ShowHelp();
        }
      }
      else
      {
        ShowHelp();
      }
    }

    private static void ShowHelp()
    {
      Logger.Info("Usage: filesync [sync] [add add <name> <sourcePath> <destinationPath>] [del] [help]");
      Logger.Info(string.Empty);
      Logger.Info("The most commonly used FileSync commands are:");
      Logger.Info("\tsync\tSyncs files as defined in the watch file");
      Logger.Info("\thelp\tShows the help");
      Logger.Info("\tdel\tDeletes a watch according to name specified as the second parameter");
      Logger.Info("\tadd\tAdds a new watch. Must be followed by [Name of watch] and [Source Path] and [Destination Path]. OPTIONAL: -e to copy only to the first level");
    }

    private static void Sync()
    {
      try
      {
        _manager.Sync();

        Logger.Info(string.Empty);
        if (_manager.SyncedFiles.Keys.Count == 0)
        {
          Logger.Info("All files up to date.");
        }
        else
        {
          foreach (var key in _manager.SyncedFiles.Keys)
          {
            Logger.Success(string.Format("Added the following files for {0}:", key));
            foreach (var file in _manager.SyncedFiles[key])
              Logger.Success(string.Format("\t{0}", file));
          }
        }
      }
      catch (System.IO.IOException ex)
      {
        Logger.Error(ex.Message);
      }
      Logger.Info(string.Empty);
    }

    private static void AddWatch(string name, string sourcePath, string destPath, bool includeSubFolders = true)
    {
      var watchItem = new WatchItem();
      watchItem.Name = name;
      watchItem.SourcePath = sourcePath;
      watchItem.DestinationPath = destPath;
      watchItem.LastSyncDate = DateTime.Now;
      watchItem.IncludeSubFolders = includeSubFolders;

      _manager.AddNewWatch(watchItem);
      Logger.Info(string.Format("Added watch for {0}", watchItem.Name));
    }
  }
}
