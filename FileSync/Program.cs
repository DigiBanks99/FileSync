using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileSync
{
  class Program
  {
    public static string  RemotePath = string.Empty,
                          LocalPath = string.Empty,
                          AppData = string.Empty,
                          WatchFile = @"FileSync\watchlist",
                          Splitter = "\t\t\t";

    public static void Main( string[] args )
    {
      Console.WriteLine();
      try
      {
        AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        WatchFile = string.Format(@"{0}\{1}", AppData, WatchFile);
        if (!args.Any())
        {
          string[] watchDirs = ReadWatchDirs();
          foreach (var watchDir in watchDirs)
          {
            if (string.IsNullOrEmpty(watchDir))
              continue;
            string[] dirs = SeperateStrings(watchDir);
            if (dirs[0] == null || dirs[1] == null)
            {
              Console.WriteLine(string.Format("ERROR: Watchdir is not valid. Please correct them according to the previous errors and try again."));
              break;
            }
            CreateFiles(dirs[0], dirs[1]);
          }
        }
        else
        {
          switch (args[0])
          {
            case "add":
              if (args.Count() < 3)
              {
                Console.WriteLine("Usage: FileSync add <Path-To-Watch> <CopyToPath>");
                return;
              }
              var pathString = string.Format("{0}{1}{2}", args[1], Splitter, args[2]);
              AddWatchDir(pathString);
              break;
            default:
              RemotePath = args[0];
              LocalPath = args[1];
              CreateFiles(RemotePath, LocalPath);
              break;
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("ERROR: " + ex.Message);
      }

      Console.Write("Press any key...");
      Console.ReadKey();
      Console.WriteLine();
    }

    public static void AddWatchDir(string pathPair)
    {
      try
      {
        string[] watches = null;
        StreamWriter streamWriter = null;
        if (!Directory.Exists(WatchFile.Substring(0, WatchFile.LastIndexOf('\\'))))
          Directory.CreateDirectory(WatchFile.Substring(0, WatchFile.LastIndexOf('\\')));
        if (File.Exists(WatchFile))
          watches = File.ReadAllLines(WatchFile);
        else
        {
          streamWriter = File.CreateText(WatchFile);
          Console.WriteLine("Adding path pair...");
          streamWriter.Write(pathPair);
          streamWriter.Close();
          Console.WriteLine("Added!");
          return;
        }

        streamWriter = File.AppendText(WatchFile);
        if (!watches.Contains(pathPair))
        {
          Console.WriteLine("Adding path pair...");
          streamWriter.Write("\n" + pathPair);
          Console.WriteLine("Added!");
        }
        streamWriter.Close();
      }
      catch (IOException ex)
      {
        Console.WriteLine("ERROR: " + ex.Message);
      }
    }

    public static string[] ReadWatchDirs()
    {
      try
      {
        string[] watches = File.ReadAllLines(WatchFile);
        return watches;
      }
      catch (IOException ex)
      {
        Console.WriteLine("ERROR: " + ex.Message);
      }

      return null;
    }

    public static string[] SeperateStrings(string watch)
    {
      if (string.IsNullOrEmpty(watch))
        return null;

      var pathPair = new string[2];
      var indexOfSplitter = watch.IndexOf(Splitter, StringComparison.Ordinal);
      if (indexOfSplitter < 1)
      {
        Console.WriteLine(string.Format("ERROR: The path line '{0}', does not have the correct splitter ('{1}').", watch, Splitter));
        return pathPair;
      }
      pathPair[0] = watch.Substring(0, indexOfSplitter);
      pathPair[1] = watch.Substring(indexOfSplitter + Splitter.Length);
      return pathPair;
    }

    public static void CreateFiles(string remotePath, string localPath)
    {
      try
      {
        string[] filesRemote = Directory.GetFiles(remotePath, "*", SearchOption.AllDirectories);
        string[] filesLocal = Directory.GetFiles(localPath, "*", SearchOption.AllDirectories);

        var remoteMangedFileList = new List<IFile>();
        var localMangedFileList = new List<IFile>();

        foreach (var file in filesRemote)
        {
          var fi = new FileInfo(file);
          if (fi.Name.Equals("Thumbs.db"))
            continue;
          var managedFileRemote = new ManagedFile(file);
          managedFileRemote.Size = fi.Length;
          managedFileRemote.DateCreated = fi.CreationTime >= fi.LastAccessTime ? fi.CreationTime : fi.LastAccessTime;
          remoteMangedFileList.Add(managedFileRemote);
        }

        foreach (var file in filesLocal)
        {
          var fi = new FileInfo(file);
          if (fi.Name.Equals("Thumbs.db"))
            continue;
          var managedFileLocal = new ManagedFile(file);
          managedFileLocal.Size = fi.Length;
          managedFileLocal.DateCreated = fi.CreationTime >= fi.LastAccessTime ? fi.CreationTime : fi.LastAccessTime;
          localMangedFileList.Add(managedFileLocal);
        }

        Console.WriteLine("Copying Files...");
        CopyNewFiles(remoteMangedFileList, localMangedFileList, localPath);
        Console.WriteLine("Files copied successfully!!!");
      }
      catch (Exception ex)
      {
        Console.WriteLine("\nERROR: " + ex.Message);
      }
    }

    public static void CopyNewFiles(List<IFile> remoteList, List<IFile> localList, string localPath)
    {
      if (remoteList == null)
        return;

      foreach (var file in remoteList.Where(file => CheckForNewFiles(file, localList) == FileConsts.ERROR))
        file.Copy(file, localPath);
    }

    public static int CheckForNewFiles(IFile fileRemote, List<IFile> localList)
    {
      return localList.Any(fileLocal => fileRemote.Compare(fileLocal) == FileConsts.ERROR) ? FileConsts.ERROR : FileConsts.NOACTION;
    }
  }
}
