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
        case (int)CodeTypes.Java:
        case (int)CodeTypes.PHP:
        case (int)CodeTypes.JavaScript:
        case (int)CodeTypes.C:
          supportsCStyleComments = true;
          supportsCPlusPlusStyleComments = true;
          break;
      }
    }
    /// <summary>
    ///   Sets the language of this instance based on it's code type.
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
      switch (fileType.Split(".").Last())// Convert the file suffix to a given language.
      {
        case "cs":// It's C#.
          fileType = "CSharp";
          SetLanguage(0);
          break;
        case "c":// It's C.
        case "h":
          fileType = "C";
          SetLanguage(1);
          break;
        case "java":// It's Java.
          fileType = "Java";
          SetLanguage(2);
          break;
        case "php":// It's PHP.
        case "php5":
        case "php7":
        case "phps":
        case "phtml":
          fileType = "PHP";
          SetLanguage(3);
          break;
        case "js":// It's JavaScript.
        case "mjs":
          fileType = "JavaScript";
          SetLanguage(4);
          break;
        default:// It's not a supported type.
          fileType = "None";
          SetLanguage(5);
          break;
      }
      return fileType;
    }
    /// <summary>
    ///   Gets all head comments from the code and returns them in XML format.
    /// </summary>
    /// <returns>The head comments.</returns>
    public string GetAllHeads()
    {
      string head = "\0";
      string[] file = File.ReadAllLines(codeFilePath);
      if (supportsCStyleComments)
      {// Look for slash-double-star either at start of the program or before instance of line containing "class"
        bool commentStarted = false, commentClosed = false, hasClasses = false, hasHead = false;
        int splitAt = 0;
        for (int count = 0; count <= file.Length;count++)
        {
          string element = file[count];
          if (element.Contains("/**"))// The start of the comment is marked by slash-double-star.
            commentStarted = true;
          if (element.Contains("**/") && commentStarted)// The end of the comment is marked by double-star-slash, iff the comment has already been started.
          {
            commentClosed = true;
            if (splitAt == 0)
              splitAt = count;
          }
          if (element.Contains(" class ") && !element.Contains("/"))// If the element contains the "class" keyword, but not just a reference in a comment.
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
            hasHead = true;// If it doesn't have any classes, but the comment is on the first line of the file, it still has a header comment.
        }
        if (hasHead)
        {
          string[] headComment = new string[splitAt + 1];// The header comment starts at the slash-double-star.
          for (int count = 0; count <= splitAt; count++)
            headComment[count] = file[count];// Add the comment to the queue to be commented.
          List<string> headCommentList = new List<string>(headComment);// Convert the queue array to a List for better manipulation.
          if (hasClasses)
          {
            while (!headCommentList[0].Contains("/**"))
              headCommentList.RemoveAt(0);// If the comment is before a class, rather than at the start, remove any code that lies before the comment.
          }
          //Format as XML.
          if (head == "\0")
            head = "\n     <summary>";
          else
            head += "\n     <summary>";
          string lineOfComment;
          foreach (string element in headCommentList)
          {
            lineOfComment = element.Replace("/**", null);
            lineOfComment = lineOfComment.Replace("**/", null);
            lineOfComment = lineOfComment.Replace(" * ", "       ");
            head += lineOfComment;
          }
          head += "\n     </summary>\n";
        }
      }

      if (supportsCSharpStyleComments)
      {// Look for triple-slash either at start of program or the before instance of line containing "class".
        for (int count = 0; count <= file.Length; count++)
        {
          string element = file[count];
          if (element.Contains(" class ") && !element.Contains("/"))// If you've passed the class and not found a comment, there's no point in looking any further.
            break;
          if (element.Contains("///"))
          {
            if (head == "\0")// If there isn't a header comment yet, write one.
              head = element.Replace("///", "\n     ");
            else// If there's a header comment already, append it.
              head += element.Replace("///", "\n     ");
          }
        }
      }
      if (head == "\0")
        head = "No header information found.";// If there's still no header information, document that.
      return head;
    }
    /// <summary>
    ///   Gets all methods.
    /// </summary>
    /// <returns>The comments about methods.</returns>
    public string GetAllMethods()
    {
      string methodInfo = "\0";
      List<string> file = new List<string>(File.ReadAllLines(codeFilePath));// Save the file contents as a list.
      List<string> commentBuffer = new List<string>();// Create a list for the method-level comments.
      if (supportsCStyleComments)
      {
        List<string> specialCommentDataBuffer = new List<string>();// Create a list for @tags.
        int startPosition = 0;
        if (file[0].Contains("/**"))// Check if there is a head comment from the first line.
        {
          do
            startPosition++;
          while (!file[startPosition].Contains("**/"));
        }
        int lineCount = 0;
        foreach (string line in file)// Check if there is a class.
        {
          if (line.Contains(" class "))
          {
            startPosition = lineCount;
            break;
          }
          lineCount++;
        }
        // Loop through the file array from the end of the head comment block at the start of the file if there isn't a "class", or from the "class" declaration if there is.
        bool commentOpenInBuffer = false;
        for (int count = startPosition; count < file.Count; count++)
        {
          if (file[count].Contains("/**") && !file[count].Contains("**/"))// C-style comment, multi-line.
          {
            commentBuffer.Add(file[count]);// Open the comment.
            commentOpenInBuffer = true;// Tell the program the comment is open
          }
          else if (file[count].Contains("/**") && file[count].Contains("**/"))// C-style comment, single line.
            commentBuffer.Add(file[count]);// Add the comment to the comment buffer list.
          else if (commentOpenInBuffer)// If a comment has been started, but not finished, add every line to the comment buffer.
          {
            commentBuffer.Add(file[count]);
            if (file[count].Contains("**/"))// If the comment has been closed:
              commentOpenInBuffer = false;// Tell the program the comment isn't open. 
          }
          else if (file[count - 1].Contains("**/"))
          {
            if (!string.IsNullOrWhiteSpace(file[count]))// Pass the attached method to the comment data if there is one.
            {
              if (methodInfo == "\0")
                methodInfo = $"\n          <method>\n            <methodCall>{file[count].TrimStart()}</methodCall>";
              else
                methodInfo += $"\n          <method>\n            <methodCall>{file[count].TrimStart()}</methodCall>";
            }
            else
            {
              if (methodInfo == "\0")// If there isn't an attached method, pass that too.
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
                string comment = commentLine.Replace("/**", "\n           ").Replace(" * ", "\n             ").Replace("**/", null);// Clean up the comment contents.
                if (!string.IsNullOrWhiteSpace(comment))
                  methodInfo += comment;// Add the comment data to methodInfo.
              }
            }
            methodInfo += "\n            </summary>";
            if (specialCommentDataBuffer.Any())
            {
              foreach(string line in specialCommentDataBuffer)
              {
                if (line.Contains("@param"))// If the user has added a parameter to the comment, format that correctly.
                {
                  string paramName = line.Replace("*", null).Trim().Split(" ")[1];
                  string paramDescription = line.Replace("*", null).Trim().Split(paramName)[1];
                  methodInfo += $"\n            <param name=\"{paramName}\">{paramDescription}</param>";// Output params.
                }
                else// If the user has added a return value to the comment, format that correctly.
                {
                  string returnData = line.Replace("*", null).Trim().Split("@return ")[1];
                  methodInfo += $"\n            <returns>{returnData}</return>";// Output return.
                }
              }
            }
            methodInfo += "\n          </method>";
            commentBuffer.Clear();// Clear the comment buffer list once we're done with it.
          }
        }
      }
      file = new List<string>(File.ReadAllLines(codeFilePath));// Create a clean copy of file.
      commentBuffer.Clear();
      if (supportsCSharpStyleComments)// If the language you're using can handle C# XML comments.
      {
        // Look for triple-slash either at start of program or the before instance of line containing "class".
        while (!file[0].Contains(" class "))
          file.RemoveAt(0);// Remove anything that's not method level.  We don't need to worry about parsing that.
        for (int count = 0; count < file.Count(); count++)
        {
          if (file[count].Contains("///"))
            commentBuffer.Add(file[count]);// Add any comment to the comment buffer.  It's already XML-formatted.
          else if (count > 0 && file[count - 1].Contains("///"))
          {
            if (!string.IsNullOrWhiteSpace(file[count]))
            {// If there's an associated method call with the comment, add it to the documentation.
              if (methodInfo == "\0")
                methodInfo = $"\n          <method>\n            <methodCall>{file[count].TrimStart()}</methodCall>";
              else
                methodInfo += $"\n          <method>\n            <methodCall>{file[count].TrimStart()}</methodCall>";
            }
            else
            {// If there's no associated method call, add that too.
              if (methodInfo == "\0")
                methodInfo = "\n          <method>\n            <methodCall>Comment is not correctly linked to method.</methodCall>";
              else
                methodInfo += "\n          <method>\n            <methodCall>Comment is not correctly linked to method.</methodCall>";
            }
            foreach (string commentLine in commentBuffer)
              methodInfo += commentLine.Replace("///", "\n           ");// Clean the comment contents to make the C# XML into true XML.
            methodInfo += "\n          </method>";
            commentBuffer.Clear();// Clear the comment buffer list.
          }
        }
      }
      if (methodInfo == "\0")
        methodInfo = "No method information found.";// If there's still no method info whatsoever, pass that message on.
      return methodInfo;
    }
    /// <summary>
    ///   Initializes a new instance of the <see cref="T:QuickDoc.LanguageHandler"/> class.
    /// </summary>
    /// <param name="lCodeFilePath">Code file path.</param>
    public LanguageHandler(string lCodeFilePath)
    {
      codeFilePath = lCodeFilePath;// Assign the local code file path to the instance variable of the code file path.
    }
  }
}