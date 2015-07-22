using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
  public class ExcludeKeyWord
  {
    private string _keyWord;
    public string KeyWord
    {
      get { return _keyWord; }
      set { _keyWord = value; }
    }
  }
}
