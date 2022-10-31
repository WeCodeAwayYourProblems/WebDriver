namespace CliHelperClass;
public abstract partial class WebDriverManipulator
{
   // Enums for special types
   public enum ElementType
   {
      Id,
      Name,
      TagName,
      CssSelector,
      ClassName,
      LinkText,
      XPath
   }
}