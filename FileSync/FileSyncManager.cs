using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileSync
{
  public static class FileSyncManager
  {
    private static string _syncFileLocation = null;
    public static string SyncFileLocation
    {
      get { return _syncFileLocation; }
      set { _syncFileLocation = value; }
    }

    private static string[] _originPaths = null;
    public static string[] OriginPaths
    {
      get { return _originPaths; }
      set { _originPaths = value; }
    }

    private static string[] _destinationPaths;
    public static string[] DestinationPaths
    {
      get { return _destinationPaths; }
      set { _destinationPaths = value; }
    }

    public void LoadSyncFile()
    {
      if (Utils.IsStringNullOrEmpty(SyncFileLocation))
        throw new Exception("The path to your Sync File is invalid. Did you specify the correct path?");

      string[] lines = File.ReadAllLines(SyncFileLocation);
      OriginPaths = new string[lines.Length];
      DestinationPaths = new string[lines.Length];

      int cnt = 0;
      foreach (var line in lines)
      {
        OriginPaths[cnt] = GetOriginPath(line);
        DestinationPaths[cnt] = GetDestinationPath(line);
        cnt++;
      }
    }

    public static string GetOriginPath(string line)
    {
      return null;
    }

    public static string GetDestinationPath(string line)
    {
      return null;
    }
  }
}
