using System.Text.RegularExpressions;

namespace CliHelper;

public abstract class TextEdit : ITextEdit
{

   public virtual string JoinText(char join, params string[] input)
   { return String.Join(join, input); }

   public virtual string JoinText(string? join = null, params string[] input)
   { return String.Join(join, input); }

   public virtual string LowerText(string input)
   { return input.ToLower(); }

   public virtual string SplitJoinLowerText(string input, string split = " ", string? join = null, bool toLower = true)
   {
      if (!toLower)
         return JoinText(join, SplitText(input, split));
      return LowerText(JoinText(join, SplitText(input, split)));
   }

   public string SplitJoinLowerText(string input, char split = ' ', string? join = null, bool lower = true)
   {
      if (!lower)
         return JoinText(join, SplitText(input, split));
      return LowerText(JoinText(join, SplitText(input, split)));
   }

   public string SplitJoinLowerText(string input, char split = ' ', char join = ' ', bool lower = true)
   {
      if (!lower)
         return JoinText(join, SplitText(input, split));
      return LowerText(JoinText(join, SplitText(input, split)));
   }

   public string SplitJoinLowerText(string input, Regex split, string? join = null, bool lower = true)
   {
      if (!lower)
         return JoinText(join, SplitText(input, split));
      return LowerText(JoinText(join, SplitText(input, split)));
   }

   public string SplitJoinLowerText(string input, Regex split, char join = ' ', bool lower = true)
   {
      if (!lower)
         return JoinText(join, SplitText(input, split));
      return LowerText(JoinText(join, SplitText(input, split)));
   }

   public string[] SplitText(string input, char split)
   { return input.Split(split); }

   public string[] SplitText(string input, string split)
   { return input.Split(split); }

   public string[] SplitText(string input, Regex split)
   { return split.Split(input); }
}