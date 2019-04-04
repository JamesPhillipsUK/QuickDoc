using System;
using System.IO;
using System.Linq;

namespace QuickDoc
{
  public class DocumentationBuilder
  {
    private readonly string docFolderPath, codeFilePath;// Create instance variables of the documentation folder and the code file.
    private readonly LanguageHandler language;// 

    public DocumentationBuilder(string ldocFolderPath, string lcodeFilePath)// Constructor assigns the documentation folder and code file the user requires.
    {
      docFolderPath = ldocFolderPath;
      codeFilePath = lcodeFilePath;
      language = new LanguageHandler(codeFilePath);
    }
    /** Builds the XML for the documentation. **/
    private string[] BuildXML(string codeFileName)
    {
      string[] xml =
      {
        @"<?xml version=""1.0"" encoding=""UTF-8""?>",
        @"<document>",
        @"  <file name=""" + codeFileName + @""" codeType=""" + language.GetLanguage(codeFileName) + @""">",
        @"    <header>",
        @"      <sectionTitle>Header Information:</sectionTitle>",
        @"      <fileName>File Name: " + codeFileName + @"</fileName>",
        @"      <fileType>File Type: " + language.GetLanguage(codeFileName) + @"</fileType>",
        @"      <documentedInfo>Developer-Documented Information: ",
        language.GetAllHeads(),
        @"      </documentedInfo>",
        @"    </header>",
        @"    <body>",
        @"      <methods>",
        @"        <sectionTitle>Methods, Functions, and Procedures</sectionTitle>",
        @"        <documentedInfo>Developer-Documented Information: ",
        language.GetAllMethods(),
        @"        </documentedInfo>",
        @"      </methods>",
        @"    </body>",
        @"  </file>",
        @"</document>"
      };
      return xml;
    }

    /** Creates the documentation file, and populates it accordingly. **/
    public void CreateDocumentation() 
    {
      string codeFileName = codeFilePath.Split("/").Last();
      File.Create(docFolderPath + "/" + codeFileName + ".xml").Close();// Create the file.
      string[] xml = BuildXML(codeFileName);
      File.WriteAllLines(docFolderPath + "/" + codeFileName + ".xml",xml);// Populate the file.
    }
  }
}
