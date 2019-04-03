using System;

namespace QuickDoc
{
  class Program
  {
    private static void HandleArgs(string[] args)
    {
      if (args.Length != 2)
        Console.WriteLine("Please pass a documentation location and a file to document.");
      else
      {
        if (System.IO.File.Exists(args[0]))
          Console.WriteLine("Docs File Exists.");
        else
          Console.WriteLine("Docs File Doesn't Exist.");

        if (System.IO.File.Exists(args[1]))
          Console.WriteLine("Code File Exists.");
        else
          Console.WriteLine("Code File Doesn't Exist.");
      }
    }

    static void Main(string[] args)
    {
      HandleArgs(args);
      Console.ReadLine();
    }
  }
}
