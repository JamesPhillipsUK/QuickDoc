using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace QuickDoc
{
  public class LanguageHandler
  {
    private bool supportsCStyleComments = false;// What comment styles are accepted by the language? - bounded by slashes and stars.
    private bool supportsCPlusPlusStyleComments = false;// - bounded by double-slash and line break.
    private bool supportsCSharpStyleComments = false;// - bounded by triple-slash and line break.
    private enum CodeTypes {CSharp, C, Java, PHP, JavaScript, None}// Languages supported by QuickDoc.
    private string codeFilePath;
    /** This sets which language rules are true for the given language. **/
    private void SetLanguageRules(int codeTypeValue)
    {
      switch (codeTypeValue)
      {
        case (int) CodeTypes.CSharp:
          supportsCStyleComments = true;
          supportsCPlusPlusStyleComments = true;
          supportsCSharpStyleComments = true;
          break;
        case (int) CodeTypes.C:
          supportsCStyleComments = true;
          break;
        case (int) CodeTypes.Java:
        case (int) CodeTypes.PHP:
        case (int) CodeTypes.JavaScript:
          supportsCStyleComments = true;
          supportsCPlusPlusStyleComments = true;
          break;
      }
    }
    /** Sets the language of the instance of this. **/
    private void SetLanguage(int codeType)
    {
      var codeTypeValues = Enum.GetValues(typeof(CodeTypes));
      foreach (int representationOfCodeType in codeTypeValues)
      {
        if (representationOfCodeType == codeType)
          SetLanguageRules(codeType);
      }
    }
    /** Gets the language of a file, and sets the language of this based on that of the file. **/
    public string GetLanguage(string fileType)
    {
      switch (fileType.Split(".").Last())
      {
        case "cs":
          fileType = "CSharp";
          SetLanguage(0);
          break;
        case "c":
          fileType = "C";
          SetLanguage(1);
          break;
        case "java":
          fileType = "Java";
          SetLanguage(2);
          break;
        case "php":
          fileType = "PHP";
          SetLanguage(3);
          break;
        case "js":
          fileType = "JavaScript";
          SetLanguage(4);
          break;
        default:
          fileType = "None";
          SetLanguage(5);
          break;
      }
      return fileType;
    }

    public string GetAllHeads()
    {
      string head = "\0";
      string[] file = File.ReadAllLines(codeFilePath);
      if (supportsCStyleComments)
      {
        //Look for /** ... **/ either at start of the program or before instance of line containing "class"
        bool commentStarted = false, commentClosed = false, hasClasses = false, hasHead = false;
        int splitAt = 0;
        for (int count = 0; count <= file.Length;count++)
        {
          string element = file[count];
          if (element.Contains("/**"))
            commentStarted = true;
          if (element.Contains("**/") && commentStarted)
          {
            commentClosed = true;
            if (splitAt == 0)
              splitAt = count;
          }
          if (element.Contains(" class ") && !element.Contains("/"))
          {
            hasClasses = true;
            break;
          }
        }
        if (commentStarted && commentClosed && hasClasses)
          hasHead = true;
        else if (!hasClasses)
        {
          if (file[0].Contains("/**"))
            hasHead = true;
        }
        if (hasHead)
        {
          string[] headComment = new string[splitAt + 1];
          for (int count = 0; count <= splitAt; count++)
            headComment[count] = file[count];
          List<string> headCommentList = new List<string>(headComment);
          if (hasClasses)
          {
            while (!headCommentList[0].Contains("/**"))
              headCommentList.RemoveAt(0);
          }
          //Format as XML.
          if (head == "\0")
            head = @"<summary>\n";
          else
            head += @"<summary>\n";
          string lineOfComment;
          foreach (string element in headCommentList)
          {
            lineOfComment = element.Replace("/**", null);
            lineOfComment = lineOfComment.Replace("**/", null);
            lineOfComment = lineOfComment.Replace("* ", null);
            head += lineOfComment;
          }
          head += @"\n</summary>";
        }
      }
      if (supportsCPlusPlusStyleComments)
      {
        //Look for blocks of // either at start of the program or before instance of line containing "class"
        //format as XML
      }
      if (supportsCSharpStyleComments)
      {
        //Look for /// either at start of program or the before instance of line containing "class".
      }
      Console.WriteLine(head);
      return head;
    }

    public LanguageHandler(string lCodeFilePath)
    {
      codeFilePath = lCodeFilePath;
    }
  }
}
