using System.Text.RegularExpressions;

namespace CliHelper;
public interface ITextEdit
{
   string SplitJoinLowerText(string input, string split = " ", string? join = null, bool lower = true);
   string SplitJoinLowerText(string input, char split = ' ', string? join = null, bool lower = true);
   string SplitJoinLowerText(string input, char split = ' ', char join = ' ', bool lower = true);
   string SplitJoinLowerText(string input, Regex split, string? join = null, bool lower = true);
   string SplitJoinLowerText(string input, Regex split, char join = ' ', bool lower = true);
   string[] SplitText(string input, char split);
   string[] SplitText(string input, string split);
   string[] SplitText(string input, Regex split);
   string JoinText(char join, params string[] input);
   string JoinText(string? join = null, params string[] input);
   string LowerText(string input);
}