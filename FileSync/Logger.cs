using System;

namespace FileSync
{
  public class Logger
  {
    [ThreadStatic]
    [NonSerialized]
    private static bool _surpressLogging = false;
    public static bool SurpressLogging
    {
      get { return _surpressLogging; }
      set { _surpressLogging = value; }
    }

    public static void Error(string message)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.Error.WriteLine(message);
      Console.ForegroundColor = ConsoleColor.White;
    }

    public static void Warning(string message)
    {
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.Error.WriteLine(message);
      Console.ForegroundColor = ConsoleColor.White;
    }

    public static void Info(string message)
    {
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine(message);
    }

    public static void Success(string message)
    {
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine(message);
      Console.ForegroundColor = ConsoleColor.White;
    }
  }
}
