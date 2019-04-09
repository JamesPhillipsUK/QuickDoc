using System;
namespace QuickDoc
{
  /// <summary>
  /// CSS Builder builds the CSS stylesheet file used by QuickDoc.
  /// </summary>
  public class CSSBuilder
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:QuickDoc.CSSBuilder"/> class.
    /// </summary>
    public CSSBuilder()
    {
    }
    /// <summary>
    ///   Gets the CSS for the stylesheet file.
    /// </summary>
    /// <returns>The css.</returns>
    public string[] GetCSS()
    {
      string[] css =
        {
          @"/** Stylesheet for QuickDoc documents. **/",
          @"*{display:block;}",
          @"document{background:white;width:100%;}",
          @"file{width:95%;margin:2.5vw auto; padding:1.25vw 0;}",
          @"file[codeType = CSharp]{background:lightskyblue;}",
          @"file[codeType = C]{background:springgreen;}",
          @"file[codeType = Java]{background:pink;}",
          @"file[codeType = PHP]{background:cornsilk;}",
          @"file[codeType = JavaScript]{background:lavender;}",
          @"file[codeType = None]{background:lightgray;}",
          @"header,methods{width:95%;margin:1.25vw auto; border:1px solid black;}",
          @"documentedInfo,method{width:95%;margin:0.825vw auto; border:1px solid black;}",
          @"methodCall:before{content:""METHOD / FUNCTION / PROCEDURE: "";}",
          @"param:before{content:""PARAMETER: "";}",
          @"method summary:before{content:""SUMMARY: "";}",
          @"return:before{content:""RETURN VALUE: "";}",
          @"see{content:attr(cref) "" "";display:inline;}",
        };
      return css;
    }
  }
}
