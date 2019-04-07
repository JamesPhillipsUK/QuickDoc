# QuickDoc #

Works in: 

|C#   |C    |Java |PHP  |JavaScript|
|:---:|:---:|:---:|:---:|:--------:|
|✔    |✔    |✔    |✔    |✔         |

Quick and easy multi-language code documentation engine.

Ever wanted a quicker method of code documentation?  One that works for multiple languages?  Simply write JavaDoc or C# XML comments in your code, and call QuickDoc on the file.

QuickDoc searches your code for these comments and compiles an XML file out of your comments to document your code.  Unlike other documentation engines, it only produces two files, which are small enough to not fill up more of your version contro repositories than your code.  One file is an XML format document detailing your code; the other is a CSS file, which allows it to be presented and colour-coded in any web browser.

## How to use QuickDoc ##

At the start of your code (or before opening a class in the case of OO languages), you can write a comment to explain it's purpose.

``` JAVA
/**
 * You can style your comments like this.
 **/
```

``` C
/** You can style your comments like this. **/
```

```C#
/// <summary>
///   Or you can style your comments like this.
/// </summary>
```

You can also coment any methods, functions, or procedures in your code and what they do.  Do this on the lines before the method/function/procedure.

``` JAVA
/**
 * You can style your comments like this.
 * @param x A parameter of this function.
 * @return the return value of this function.
 **/
public int getX(int x)
{
  return x;
}
```

``` C
/** You can style your comments like this.  It doesn't tell you about parameters or returns though. **/
bool isXTrue(bool x)
{
  return x;
}
```

``` CSHARP
/// <summary>
///   Or you can style your comments like this.
/// </summary>
/// <param name="x">A parameter of this function.</param>
/// <returns>The return value of this function.</returns>
public string GetXOverTwo(float x)
{
  return "answer = " + (x / 2);
}
```

QuickDoc is a command-line utility.  Call it with two parameters, the first is the directory you want to put your documentation in, the second is the file you want to document.

### More From me ###

If you want to see more of what I do, you can visit: [jamesphillipsuk.com](https://jamesphillipsuk.com "My Website!").
If you want to donate to my development efforts, you can send donations via PayPal.Me at [paypal.me/JamesPhillipsUK](https://paypal.me/JamesPhillipsUK "My PayPal.Me").
