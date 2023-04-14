using CliHelper;
namespace WebDriver_Custom;

public class WebDriverParent : TextEdit, ITime
{
   public void SleepSeconds(int seconds)
   {
      Thread.Sleep(seconds * 1000);
   }

   public void SleepSeconds(double seconds)
   {
      double secs = seconds * 1000;
      Thread.Sleep((int)secs);
   }

}