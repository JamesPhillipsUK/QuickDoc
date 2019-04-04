using System;
using System.IO;

namespace QuickDoc
{
  /// <summary>
  /// In C# format.  This is the main controller for QuickDoc.  Call this, and it'll handle everything else.
  /// </summary>
  class Program
  {
    private static void HandleArgs(string[] args)
    {
      string docFolderPath, codeFilePath;

      if (args.Length != 2)// If there's too many or too few arguments:
      {
        Console.WriteLine("Please pass a documentation location and a file to document.");// Pass it to the user.
        Environment.Exit(1);// Exit with a non-zero exit code (failure).
      }
      else
      {
        docFolderPath = args[0];
        if (!Directory.Exists(args[0]))// If the directory doesn't exist, let's create it.
          Directory.CreateDirectory(docFolderPath);

        if (!File.Exists(args[1]))
        {
          Console.WriteLine("Code File Doesn't Exist.");// If there's nothing to document, throw it back to the user.
          Environment.Exit(1);
        }
        codeFilePath = args[1];// Code file exists, let's use it.

        var document = new DocumentationBuilder(docFolderPath, codeFilePath);// Create a new Documentation Builder for the required documentation.
        document.CreateDocumentation();
      }
    }

    static void Main(string[] args)
    {
      HandleArgs(args);
      Console.WriteLine("Documentation written!");
    }
  }
}
