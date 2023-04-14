namespace CliHelper;
public interface IFileWork
{
   string AllContents { get; set; }
   string[] ContentArray { get; set; }
   byte[] ContentBytes { get; set; }
   string GetStringFromFile(string fileNameAndLocation, string columnHeaderOfStringToFind, int lineToFind);
   string GetStringFromFile(string fileName, string fileLocation, string columnHeaderOfStringToFind, int columnToFind);
   string[] GetStringFromFileLogic(string stringToSearch);
   string[] GetArrayFromFile(string fileNameAndLocation, string columnHeaderToFind);
   string[] GetArrayFromFile(string fileName, string fileLocation, string columnHeaderToFind);
   byte[] GetBytesFromFile(string fileNameAndLocation);
   byte[] GetBytesFromFile(string fileName, string fileLocation);
}