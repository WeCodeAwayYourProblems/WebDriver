namespace CliHelper;
public interface IHelper
{
   string AskUserForInput(string prompt, bool extraSpace = true);
   void InformUser(string information);
   void PauseUntilKeyStroke(string prompt);
   bool RequestBooleanValue(int repeatRequestOnFailure, string? prompt = null, params string[] option);
   bool RequestBooleanValue(string firstOptionAsBooleanStatement, string secondOptionAsBooleanStatement, int repeatRequestOnFailure);
   string RequestFileNameLocation(string requestedFile);
   string RequestFileNameLocation(string requestedFile, int attempts);
   void IntCannotBeNegativeError(int attempts);
   int ConvertStringToInt(string input);
   bool ConvertStringToBool(string input);
   IEnumerable<string> AggregateFiles(string downloadLocation, string nameOfFileAggregator, string nameOfRecord, string sourceLocation, string properFormattingOfFileName, string fileExtension);
   System.Collections.Generic.List<string> Affirmative { get; set; }
   System.Collections.Generic.List<string> Negative { get; set; }
   System.Collections.Generic.List<string> SkipList { get; set; }
}