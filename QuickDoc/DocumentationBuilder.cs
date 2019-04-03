using System;
using System.IO;
using System.Linq;

namespace QuickDoc
{
  public class DocumentationBuilder
  {
    private readonly string docFolderPath, codeFilePath;// Create instance variables of the documentation folder and the code file

    public DocumentationBuilder(string ldocFolderPath, string lcodeFilePath)// Constructor assigns the documentation folder and code file the user requires.
    {
      docFolderPath = ldocFolderPath;
      codeFilePath = lcodeFilePath;
    }

    private string[] BuildXML()
    {
      string[] xml = 
      {
        @"<?xml version=""1.0"" encoding=""UTF-8""?>",
        @"<document>",
        @"",
        @"</document>"
      };
      return xml;
    }

    public void CreateDocumentation() 
    {
      string codeFileName = codeFilePath.Split("/").Last();
      File.Create(docFolderPath + "/" + codeFileName + ".xml").Close();
      string[] xml = BuildXML();
      File.WriteAllLines(docFolderPath + "/" + codeFileName + ".xml",xml);
    }
  }
}
