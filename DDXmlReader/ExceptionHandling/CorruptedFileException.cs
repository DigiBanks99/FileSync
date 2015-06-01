using System;

namespace DDXmlLib.ExceptionHandling
{
  public class CorruptedFileException : Exception
  {
    #region Privates
    private string _message = null;
    #endregion Privates

    public CorruptedFileException()
    {
      _message = string.Empty;
    }

    public CorruptedFileException(string message)
    {
      _message = message;
    }

    public override string Message
    {
      get
      {
        return "Corrupted File: " + _message;
      }
    }
  }
}
