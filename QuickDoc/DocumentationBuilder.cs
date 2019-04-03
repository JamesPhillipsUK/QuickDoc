using System;
namespace QuickDoc
{
  public class DocumentationBuilder
  {
    private string docFolderPath, codeFilePath;// Create instance variables of the documentation folder and the code file

    public DocumentationBuilder(string ldocFolderPath, string lcodeFilePath)// Constructor assigns the documentation folder and code file the user requires.
    {
      docFolderPath = ldocFolderPath;
      codeFilePath = lcodeFilePath;
    }
  }
}
