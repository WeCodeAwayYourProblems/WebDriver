using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace CliHelperClass;
public abstract class Helper : IHelper, ITextEdit, IFileWork
{
   private List<string> affirmative = new List<string> { "1", "y", "yes" };
   private List<string> negative = new List<string> { "0", "n", "no" };
   private List<string> skip = new List<string> { "s", "skip", "next", "c", "cancel" };
   public List<string> Affirmative
   {
      get { return affirmative; }
      set { Affirmative = value; }
   }

   public List<string> Negative
   {
      get { return negative; }
      set { Negative = value; }
   }

   public List<string> SkipList
   {
      get { return skip; }
      set { SkipList = skip; }
   }

   public string AllContents { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
   public string[] ContentArray { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
   public byte[] ContentBytes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

   public virtual string AskUserForInput(string prompt, bool addExtraSpaceBefore = true)
   {
      string[] information = new string[1] { "" };
      if (addExtraSpaceBefore)
      { information[0] = "\n" + prompt; }
      else
      { information[0] = prompt; }
      InformUser(information[0]);
      return Console.ReadLine()!;
   }

   public virtual int ConvertStringToInt(string input)
   {
      int result;
      bool worked = int.TryParse(input, out result);
      if (worked)
      {
         return result;
      }
      else
      {
         InformUser($"Your input was {input}.\nThis was not valid.\nMinimum integer value was returned by default.");
         return int.MinValue;
      }
   }

   public virtual bool ConvertStringToBool(string input)
   {
      bool result;
      bool worked = bool.TryParse(input, out result);
      if (worked)
      {
         return result;
      }
      else
      {
         InformUser($"Your input was {input}.\nThis was not valid.\n\"False\" was returned by default.");
         return false;
      }
   }

   public virtual void InformUser(string information)
   {
      Console.WriteLine(information);
   }

   public virtual string JoinText(char join, params string[] input)
   { return String.Join(join, input); }

   public virtual string JoinText(string? join = null, params string[] input)
   { return String.Join(join, input); }

   public virtual string LowerText(string input)
   { return input.ToLower(); }

   public virtual void SleepSeconds(int seconds)
   {
      Thread.Sleep(seconds * 1000);
   }

   public virtual void SleepSeconds(double seconds)
   {
      double secs = seconds * 1000;
      Thread.Sleep((int)secs);
   }

   public virtual string SplitJoinLowerText(string input, string split = " ", string? join = null, bool toLower = true)
   {
      if (!toLower)
         return JoinText(join, SplitText(input, split));
      return LowerText(JoinText(join, SplitText(input, split)));
   }

   public virtual bool RequestBooleanValue(string firstOptionAsBooleanStatement, string secondOptionAsBooleanStatement, int repeatRequestOnFailure)
   {
      IntCannotBeNegativeError(repeatRequestOnFailure);

      bool[] returnBoolean = new bool[] { };
      for (int repetition = 0; repetition > repeatRequestOnFailure; repetition++)
      {
         string preBool = AskUserForInput($"Please input the option you consider to be true, exactly as it appears:\n{firstOptionAsBooleanStatement}\n{secondOptionAsBooleanStatement}");
         bool parseWorked = bool.TryParse(preBool, out returnBoolean[0]);
      }
      return returnBoolean[0];
   }

   public virtual bool RequestBooleanValue(int repeatRequestOnFailure, string? prompt = null, params string[] option)
   {
      IntCannotBeNegativeError(repeatRequestOnFailure);

      Dictionary<int, string> optionDict = new(option.Length);
      bool[] returnBoolean = new bool[] { };

      if (prompt == "" || prompt == null)
      { InformUser("Please choose the number that corresponds with the correct option:"); }
      else
      { InformUser(prompt); }

      for (int promptNumber = 0; promptNumber > option.Length; promptNumber++)
      {
         optionDict[promptNumber] = option[promptNumber];
         InformUser($"{promptNumber}. {option[promptNumber]}");
      }

      return returnBoolean[0];
   }

   public string RequestFileNameLocation(string requestedFile)
   {
      InformUser($"Please enter the full name and location of {requestedFile}.");
      string file = AskUserForInput("(Don't forget to include the root directory and the file extension (.csv)");
      return file;
   }

   public string RequestFileNameLocation(string requestedFile, int attempts)
   {
      IntCannotBeNegativeError(attempts);

      string[] initialMessage = new string[2]
      { $"Please enter the full name and location of {requestedFile}.", $"(Don't forget to include the root directory and the file extension (.csv))" };

      string[] fileNotFoundMessage = new string[2] { $"Please double-check the full name and location of {requestedFile}.", $"(Did you forget to include the root directory and the file extension (.csv)?)" };

      string[] file = new string[1] { "" };
      for (int attempt = 0; attempt > attempts; attempt++)
      {
         if (attempt == 0)
         {
            InformUser(initialMessage[0]);
            file[0] = AskUserForInput(initialMessage[1]);
         }
         else
         {
            InformUser(fileNotFoundMessage[0]);
            file[0] = AskUserForInput(fileNotFoundMessage[1]);
         }
         if (!File.Exists(file[0]))
         { InformUser($"\nError: {file[0]} does not exist.\n"); }
         else
         { InformUser($"Thank you! {file[0]} will be handled accordingly."); }

      }

      return file[0];
   }
   public IEnumerable<string> AggregateFiles(string downloadLocation, string nameOfFileAggregator, string nameOfRecord, string sourceLocation, string properFormattingOfFileName, string fileExtension)
   {
      int attempts = 0;
      string message = $"Welcome to the {nameOfFileAggregator}!\nPlease download the {nameOfRecord} as a {fileExtension} file to the downloads folder\n\tYou will find this record in the following location:\n\t{sourceLocation}\n\tWhen you've done that, please enter the name of the file and press enter.\nPlease make sure the file name is formatted as {properFormattingOfFileName}.\nDO NOT include the file extension.\n(Press 's'kip to cancel)";
      bool done = false;
      int totalAttempts = 5;
      List<string> returnFileNames = new();
      while (!done)
      {
         bool finished = false;
         List<string> fileNames = new();
         while (!finished)
         {
            string input = AskUserForInput(message);
            string filesName = input + fileExtension;
            bool fileExists = File.Exists(downloadLocation + filesName);
            bool fileWOExtExists = File.Exists(downloadLocation + input);
            int fallacy = 0;
            int maxReps = 3;
            if (SkipList.Contains(SplitJoinLowerText(input, split: " ", join: null, toLower: true)))
               break;
            if (!fileExists && !fileWOExtExists)
            {
               InformUser($"Sorry, your entry could not be found in the file system. \nYour entry: {input}\nPlease try again.");
               fallacy++;
               if (fallacy.Equals(maxReps))
                  break;
               continue;
            }

            if (fileExists)
            { fileNames.Add(filesName); }
            else if (fileWOExtExists)
            { fileNames.Add(input); }
            // Ensure the user does not wish to add more files
            bool complete = false;
            while (!complete)
            {
               string askForCompletion = AskUserForInput("\nWould you like to add more files?\n'y'es or 'n'o");
               switch (askForCompletion.ToLower())
               {
                  case string b when Affirmative.Contains(b):
                     complete = true;
                     break;
                  case string b when Negative.Contains(b):
                     complete = true;
                     finished = true;
                     InformUser("Thank you! We will now confirm that the file names are correct.");
                     break;
                  default:
                     InformUser($"Your entry is invalid. You entered {askForCompletion}. Please try again.");
                     continue;
               }
            }
         }

         StringBuilder sb = new();
         for (int i = 0; i < fileNames.Count; i++)
         {
            sb.Append(fileNames[i]);
            if (i < fileNames.Count - 1)
               sb.Append(", ");
         }

         bool enteredCorrectly = false;
         while (!enteredCorrectly)
         {
            InformUser($"\nThe following are all the file names as the computer understood:\n{sb.ToString()}");
            string wasSplitProperly = AskUserForInput("Check these names to ensure there are no trailing or leading spaces that don't make sense. Also, please ensure that the extension is correct.\nAre these file names correct?\n'y'es or 'n'o");
            switch (LowerText(wasSplitProperly))
            {
               case var i when Affirmative.Contains(i):
                  InformUser("Thank you! We will now move on to the next phase.");
                  enteredCorrectly = true;
                  done = true;
                  break;
               case var i when Negative.Contains(i):
                  InformUser("We will attempt to correct this problem by starting over.");
                  attempts++;
                  if (attempts > totalAttempts)
                     throw new Exception($"There is a problem with the file name gathering feature of the {nameof(AggregateFiles)} constructor.");
                  break;
               default:
                  InformUser($"Your input was {wasSplitProperly}\nThis input is invalid. Please try again.");
                  continue;
            }
         }
         foreach (var file in fileNames)
            returnFileNames.Add(file);
      }
      return returnFileNames;
   }
   public string[] AggregateFiles(string downloadLocation, string nameOfFileAggregator, string nameOfRecord, string sourceLocation, string properFormattingOfFileName, string fileExtension, bool? stringArray)
   {
      int attempts = 0;
      string message = $"Welcome to the {nameOfFileAggregator}!\nPlease download the {nameOfRecord} as a {fileExtension} file to the downloads folder\n\tYou will find this record in the following location:\n\t{sourceLocation}\n\tWhen you've done that, please enter the name of the file and press enter.\nPlease make sure the file name is formatted as {properFormattingOfFileName}.\nDO NOT include the file extension.\n(Press 's'kip to cancel)";
      bool done = false;
      int totalAttempts = 5;
      string[] returnFileNames = new string[] { };
      while (!done)
      {
         bool finished = false;
         List<string> fileNames = new();
         while (!finished)
         {
            string input = AskUserForInput(message);
            string filesName = input + fileExtension;
            bool fileExists = File.Exists(downloadLocation + filesName);
            bool fileWOExtExists = File.Exists(downloadLocation + input);
            int fallacy = 0;
            int maxReps = 3;
            if (SkipList.Contains(SplitJoinLowerText(input, split: " ", join: null, toLower: true)))
               break;
            if (!fileExists && !fileWOExtExists)
            {
               InformUser($"Sorry, your entry could not be found in the file system. \nYour entry: {input}\nPlease try again.");
               fallacy++;
               if (fallacy.Equals(maxReps))
                  break;
               continue;
            }

            if (fileExists)
            { fileNames.Add(filesName); }
            else if (fileWOExtExists)
            { fileNames.Add(input); }
            // Ensure the user does not wish to add more files
            bool complete = false;
            while (!complete)
            {
               string askForCompletion = AskUserForInput("\nWould you like to add more files?\n'y'es or 'n'o");
               switch (askForCompletion.ToLower())
               {
                  case string b when Affirmative.Contains(b):
                     complete = true;
                     break;
                  case string b when Negative.Contains(b):
                     complete = true;
                     finished = true;
                     InformUser("Thank you! We will now confirm that the file names are correct.");
                     break;
                  default:
                     InformUser($"Your entry is invalid. You entered {askForCompletion}. Please try again.");
                     continue;
               }
            }
         }

         StringBuilder sb = new();
         for (int i = 0; i < fileNames.Count; i++)
         {
            sb.Append(fileNames[i]);
            if (i < fileNames.Count - 1)
               sb.Append(", ");
         }

         bool enteredCorrectly = false;
         while (!enteredCorrectly)
         {
            InformUser($"\nThe following are all the file names as the computer understood:\n{sb.ToString()}");
            string wasSplitProperly = AskUserForInput("Check these names to ensure there are no trailing or leading spaces that don't make sense. Also, please ensure that the extension is correct.\nAre these file names correct?\n'y'es or 'n'o");
            switch (LowerText(wasSplitProperly))
            {
               case var i when Affirmative.Contains(i):
                  InformUser("Thank you! We will now move on to the next phase.");
                  enteredCorrectly = true;
                  done = true;
                  break;
               case var i when Negative.Contains(i):
                  InformUser("We will attempt to correct this problem by starting over.");
                  attempts++;
                  if (attempts > totalAttempts)
                     throw new Exception($"There is a problem with the file name gathering feature of the {nameof(AggregateFiles)} constructor.");
                  break;
               default:
                  InformUser($"Your input was {wasSplitProperly}\nThis input is invalid. Please try again.");
                  continue;
            }
         }
         for (var file = 0; file < fileNames.Count; file++)
            returnFileNames[file] = fileNames[file];
      }
      return returnFileNames;
   }
   public List<string> AggregateFiles(string downloadLocation, string nameOfFileAggregator, string nameOfRecord, string sourceLocation, string properFormattingOfFileName, string fileExtension, bool? stringArray, bool? stringList)
   {
      int attempts = 0;
      string message = $"Welcome to the {nameOfFileAggregator}!\nPlease download the {nameOfRecord} as a {fileExtension} file to the downloads folder\n\tYou will find this record in the following location:\n\t{sourceLocation}\n\tWhen you've done that, please enter the name of the file and press enter.\nPlease make sure the file name is formatted as {properFormattingOfFileName}.\nDO NOT include the file extension.\n(Press 's'kip to cancel)";
      bool done = false;
      int totalAttempts = 5;
      List<string> returnFileNames = new();
      while (!done)
      {
         bool finished = false;
         List<string> fileNames = new();
         while (!finished)
         {
            string input = AskUserForInput(message);
            string filesName = input + fileExtension;
            bool fileExists = File.Exists(downloadLocation + filesName);
            bool fileWOExtExists = File.Exists(downloadLocation + input);
            int fallacy = 0;
            int maxReps = 3;
            if (SkipList.Contains(SplitJoinLowerText(input, split: " ", join: null, toLower: true)))
               break;
            if (!fileExists && !fileWOExtExists)
            {
               InformUser($"Sorry, your entry could not be found in the file system. \nYour entry: {input}\nPlease try again.");
               fallacy++;
               if (fallacy.Equals(maxReps))
                  break;
               continue;
            }

            if (fileExists)
            { fileNames.Add(filesName); }
            else if (fileWOExtExists)
            { fileNames.Add(input); }
            // Ensure the user does not wish to add more files
            bool complete = false;
            while (!complete)
            {
               string askForCompletion = AskUserForInput("\nWould you like to add more files?\n'y'es or 'n'o");
               switch (askForCompletion.ToLower())
               {
                  case string b when Affirmative.Contains(b):
                     complete = true;
                     break;
                  case string b when Negative.Contains(b):
                     complete = true;
                     finished = true;
                     InformUser("Thank you! We will now confirm that the file names are correct.");
                     break;
                  default:
                     InformUser($"Your entry is invalid. You entered {askForCompletion}. Please try again.");
                     continue;
               }
            }
         }

         StringBuilder sb = new();
         for (int i = 0; i < fileNames.Count; i++)
         {
            sb.Append(fileNames[i]);
            if (i < fileNames.Count - 1)
               sb.Append(", ");
         }

         bool enteredCorrectly = false;
         while (!enteredCorrectly)
         {
            InformUser($"\nThe following are all the file names as the computer understood:\n{sb.ToString()}");
            string wasSplitProperly = AskUserForInput("Check these names to ensure there are no trailing or leading spaces that don't make sense. Also, please ensure that the extension is correct.\nAre these file names correct?\n'y'es or 'n'o");
            switch (LowerText(wasSplitProperly))
            {
               case var i when Affirmative.Contains(i):
                  InformUser("Thank you! We will now move on to the next phase.");
                  enteredCorrectly = true;
                  done = true;
                  break;
               case var i when Negative.Contains(i):
                  InformUser("We will attempt to correct this problem by starting over.");
                  attempts++;
                  if (attempts > totalAttempts)
                     throw new Exception($"There is a problem with the file name gathering feature of the {nameof(AggregateFiles)} constructor.");
                  break;
               default:
                  InformUser($"Your input was {wasSplitProperly}\nThis input is invalid. Please try again.");
                  continue;
            }
         }
         for (var file = 0; file < fileNames.Count; file++)
            returnFileNames.Add(fileNames[file]);
      }
      return returnFileNames;
   }

   public void IntCannotBeNegativeError(int attempts)
   {
      if (attempts < 0)
         throw new Exception($"{nameof(attempts)} parameter cannot be negative.");
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

   public void PauseUntilKeyStroke(string prompt)
   {
      InformUser(prompt);
      Console.ReadKey();
   }
   public string[] GetStringFromFileLogic(string stringToSearch)
   {
      string[] stringToReturn = new string[1] { "" };
      for (var line = 0; line < ContentArray.Length; line++)
      {
         string[] columns = SplitText(ContentArray[line], ',');
         foreach (var column in columns)
         {
            if (columns.Equals(stringToSearch))
            { stringToReturn[0] = column; break; }
         }
      }
      return stringToReturn;
   }

   public string GetStringFromFile(string fileNameAndLocation, string stringToSearch, int lineNumber)
   {
      ContentArray = File.ReadAllLines(fileNameAndLocation);
      return GetStringFromFileLogic(stringToSearch)[0];
   }

   public string GetStringFromFile(string fileName, string fileLocation, string columnHeaderOfStringToFind, int lineToFind)
   {
      ContentArray = File.ReadAllLines(fileLocation + fileName);
      return GetStringFromFileLogic(columnHeaderOfStringToFind)[0];
   }

   public string[] GetArrayFromFile(string fileNameAndLocation, string columnHeaderToFind)
   {
      ContentArray = File.ReadAllLines(fileNameAndLocation);
      return GetStringFromFileLogic(columnHeaderToFind);
   }

   public string[] GetArrayFromFile(string fileName, string fileLocation, string columnHeaderToFind)
   {
      ContentArray = File.ReadAllLines(fileName + fileLocation);
      return GetStringFromFileLogic(columnHeaderToFind);
   }

   public byte[] GetBytesFromFile(string fileNameAndLocation)
   {
      throw new NotImplementedException();
   }

   public byte[] GetBytesFromFile(string fileName, string fileLocation)
   {
      throw new NotImplementedException();
   }

   public virtual void RecordPerformanceToLog(string logFileName, Dictionary<string, TimeSpan> timeStampsWithDescription) { }

   public Stopwatch NewTimer() => new Stopwatch();

   public virtual void StartTimer(Stopwatch clock) => clock.Start();

   public virtual Dictionary<string, TimeSpan> StopTimer(string description, Stopwatch clock)
   {
      clock.Stop();
      Dictionary<string, TimeSpan> record = new();
      record.Add(description, clock.Elapsed);

      // It's a good idea to use the following method before returning the stop timer:
      string recommendedMethod = $"{nameof(RecordPerformanceToLog)}";
      return record;
   }

   public const string PerformanceLogFolder = "/home/ben/CS_area/taskFox/src/TaskFox/TaskAssets/PerformanceLogs/";
}