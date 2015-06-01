using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
  public static class Utils
  {
    public static bool IsStringNullOrEmpty(string value)
    {
      if (value == null || value == string.Empty)
        return true;
      return false;
    }
  }
}
