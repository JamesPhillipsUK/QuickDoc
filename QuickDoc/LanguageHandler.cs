using System;
using System.Linq;

namespace QuickDoc
{
  public class LanguageHandler
  {
    private bool supportsCStyleComments = false;// What comment styles are accepted by the language? - bounded by slashes and stars.
    private bool supportsCPlusPlusStyleComments = false;// - bounded by double-slash and line break.
    private bool supportsCSharpStyleComments = false;// - bounded by triple-slash and line break.
    private enum CodeTypes {CSharp, C, Java, PHP, JavaScript, None}// Languages supported by QuickDoc.
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

    public LanguageHandler()
    {
    }
  }
}
