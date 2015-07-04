using System;

namespace FileSync
{
  public class FileSyncEventArgs : EventArgs
  {
    public FileSyncEventArgs()
    {
      Message = null;
    }

    public FileSyncEventArgs(string message)
    {
      Message = message;
    }

    private string _message;

    public string Message
    {
      get { return _message; }
      set { _message = value; }
    }
  }

  public class CancelEventArgs : FileSyncEventArgs
  {
    public CancelEventArgs(bool cancelled)
    {
      Cancelled = cancelled;
    }

    public CancelEventArgs(bool cancelled, string message)
    {
      Cancelled = cancelled;
      Message = message;
    }

    private bool _cancelled;

    public bool Cancelled
    {
      get { return _cancelled; }
      set { _cancelled = value; }
    }

  }
}
