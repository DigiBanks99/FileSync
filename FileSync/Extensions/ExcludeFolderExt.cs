using System.Collections.Generic;

namespace FileSync
{
  public static class ExcludeFolderExt
  {
    public static bool Contains(this List<ExcludeFolder> list, string value)
    {
      if (list == null)
        return false;

      foreach (var item in list)
      {
        if (value == item.ExcludeFolderName)
          return true;
      }

      return false;
    }
  }
}
