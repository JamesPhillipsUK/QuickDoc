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
    private readonly string codeFilePath;
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
          supportsCPlusPlusStyleComments = true;
          break;
        case (int) CodeTypes.Java:
        case (int) CodeTypes.PHP:
        case (int) CodeTypes.JavaScript:
          supportsCStyleComments = true;
          supportsCPlusPlusStyleComments = true;
          break;
      }
    }
    /// <summary>
    /// Sets the language of this instance based on it's code type.
    /// </summary>
    /// <param name="codeType">Code type.</param>
    private void SetLanguage(int codeType)
    {
      var codeTypeValues = Enum.GetValues(typeof(CodeTypes));
      foreach (int representationOfCodeType in codeTypeValues)
      {
        if (representationOfCodeType == codeType)
          SetLanguageRules(codeType);
      }
    }
    /// <summary>
    ///   Gets the language of a file, and sets the language of this based on that of the file.
    /// </summary>
    /// <returns>The language.</returns>
    /// <param name="fileType">File type.</param>
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

    /// <summary>
    ///   Gets all head comments.
    /// </summary>
    /// <returns>The head comments.</returns>
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
            head = "\n<summary>";
          else
            head += "\n<summary>";
          string lineOfComment;
          foreach (string element in headCommentList)
          {
            lineOfComment = element.Replace("/**", null);
            lineOfComment = lineOfComment.Replace("**/", null);
            lineOfComment = lineOfComment.Replace("* ", null);
            head += lineOfComment;
          }
          head += "\n</summary>\n";
        }
      }

      if (supportsCSharpStyleComments)
      {
        //Look for /// either at start of program or the before instance of line containing "class".
        for (int count = 0; count <= file.Length; count++)
        {
          string element = file[count];
          if (element.Contains(" class "))
            break;
          if (element.Contains("///"))
          {
            if (head == "\0")
              head = element.Replace("///", "\n     ");
            else
              head += element.Replace("///", "\n     ");
          }
        }
      }
      if (head == "\0")
        head = "No header information found.";
      return head;
    }

    /// <summary>
    ///   Gets all methods.
    /// </summary>
    /// <returns>The comments about methods.</returns>
    public string GetAllMethods()
    {
      string methodInfo = "\0";
      List<string> file = new List<string>(File.ReadAllLines(codeFilePath));
      List<string> commentBuffer = new List<string>();
      if (supportsCStyleComments)
      {
        List<string> specialCommentDataBuffer = new List<string>();
        int startPosition = 0;
        //check if there is a head comment from the first line.
        if (file[0].Contains("/**"))
        {
          do
            startPosition++;
          while (!file[startPosition].Contains("**/"));
        }
        //check if there is a class.
        int lineCount = 0;
        foreach (string line in file)
        {
          if (line.Contains(" class "))
          {
            startPosition = lineCount;
            break;
          }
          lineCount++;
        }
        //loop through the file array from the end of the head comment block at the start of the file if there isn't a class
        //or from the class declaration if there is
        bool commentOpenInBuffer = false;
        for (int count = startPosition; count < file.Count; count++)
        {
          if (file[count].Contains("/**") && !file[count].Contains("**/"))
          {
            commentBuffer.Add(file[count]);
            commentOpenInBuffer = true;
          }
          else if (file[count].Contains("/**") && file[count].Contains("**/"))
            commentBuffer.Add(file[count]);
          else if (commentOpenInBuffer)
          {
            commentBuffer.Add(file[count]);
            if (file[count].Contains("**/"))
              commentOpenInBuffer = false;
          }
          else if (file[count - 1].Contains("**/"))
          {
            //Build the XML
            if (!string.IsNullOrWhiteSpace(file[count]))
            {
              if (methodInfo == "\0")
                methodInfo = $"\n          <method>\n            <methodCall>{file[count].TrimStart()}</methodCall>";
              else
                methodInfo += $"\n          <method>\n            <methodCall>{file[count].TrimStart()}</methodCall>";
            }
            else
            {
              if (methodInfo == "\0")
                methodInfo = "\n          <method>\n            <methodCall>Comment is not correctly linked to method.</methodCall>";
              else
                methodInfo += "\n          <method>\n            <methodCall>Comment is not correctly linked to method.</methodCall>";
            }
            methodInfo += "\n            <summary>";
            foreach (string commentLine in commentBuffer)
            {
              if (commentLine.Contains("@param"))
                specialCommentDataBuffer.Add(commentLine);// Keep params safe elsewhere.
              else if (commentLine.Contains("@return"))
                specialCommentDataBuffer.Add(commentLine);// Keep return safe elsewhere.
              else
              {
                string comment = commentLine.Replace("/**", "\n           ").Replace(" * ", "\n             ").Replace("**/", null);
                if (!string.IsNullOrWhiteSpace(comment))
                  methodInfo += comment;
              }
            }
            methodInfo += "\n            </summary>";
            if (specialCommentDataBuffer.Any())
            {
              foreach(string line in specialCommentDataBuffer)
              {
                if (line.Contains("@param"))
                {
                  string paramName = line.Replace("*", null).Trim().Split(" ")[1];
                  string paramDescription = line.Replace("*", null).Trim().Split(paramName)[1];
                  methodInfo += $"\n            <param name=\"{paramName}\">{paramDescription}</param>";// Output params.
                }
                else
                {
                  string returnData = line.Replace("*", null).Trim().Split("@return ")[1];
                  methodInfo += $"\n            <returns>{returnData}</return>";// Output return.
                }
              }
            }
            methodInfo += "\n          </method>";
            commentBuffer.Clear();
          }
        }
      }

      file = new List<string>(File.ReadAllLines(codeFilePath));
      commentBuffer.Clear();

      if (supportsCSharpStyleComments)
      {
        //Look for /// either at start of program or the before instance of line containing "class".

        while (!file[0].Contains(" class "))
          file.RemoveAt(0);
        for (int count = 0; count < file.Count(); count++)
        {
          if (file[count].Contains("///"))
            commentBuffer.Add(file[count]);
          else if (count > 0 && file[count - 1].Contains("///"))
          {
            if (!string.IsNullOrWhiteSpace(file[count]))
            {
              if (methodInfo == "\0")
                methodInfo = $"\n          <method>\n            <methodCall>{file[count].TrimStart()}</methodCall>";
              else
                methodInfo += $"\n          <method>\n            <methodCall>{file[count].TrimStart()}</methodCall>";
              foreach (string commentLine in commentBuffer)
                methodInfo += commentLine.Replace("///", "\n           ");
              methodInfo += "\n          </method>";
            }
            else
            {
              if (methodInfo == "\0")
                methodInfo = "\n          <method>\n            <methodCall>Comment is not correctly linked to method.</methodCall>";
              else
                methodInfo += "\n          <method>\n            <methodCall>Comment is not correctly linked to method.</methodCall>";
              foreach (string commentLine in commentBuffer)
                methodInfo += commentLine.Replace("///", "\n           ");
              methodInfo += "\n          </method>";
            }
            commentBuffer.Clear();
          }
        }
      }

      if (methodInfo == "\0")
        methodInfo = "No method information found.";
      return methodInfo;
    }

    public LanguageHandler(string lCodeFilePath)
    {
      codeFilePath = lCodeFilePath;
    }
  }
}
