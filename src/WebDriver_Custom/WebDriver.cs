using System.Drawing;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace WebDriver_Custom;
public class WebDriverManipulator : WebDriverParent
{
   public const double ImplicitWait = 10.0;
   public const double ImplicitWaitDefault = 0.0;
   public const int TotalAttemptsDefault = 5;
   public const int WindowWidth = 1536 / 2;
   public const int WindowHeight = 960;
   public const bool TMaxMin = true;
   public const bool FMaxMin = false;
   public ChromeDriver? Chrome { get; private set; }
   public WebDriverWait? Wait { get; private set; }

   // Chrome methods
   public string GetCurrentUrl() => Chrome!.Url;

   public void StartChrome(string loginURL, bool openSecondTab, bool openThirdTab = false, bool maximizeWindow = false, bool minimizeWindow = false, int windowWidth = WindowWidth, int windowHeight = WindowHeight, bool pause = true)
   {
      // Instantiate ChromeDriver and resize Chrome window
      InstantiateChrome(loginURL, openTwoTabs: openSecondTab);
      ResizeWindow(maximizeWindow, minimizeWindow, windowWidth, windowHeight);
      if (pause)
         SleepSeconds(5);
      AdjustImplicitWait(ImplicitWait);

      if (openSecondTab)
         OpenNewTab();
      if (openThirdTab)
         OpenNewTab();

      NavigateToUrl(loginURL, usualWay: false);
   }
   public void InstantiateChrome(string loginUrl, bool openTwoTabs = true)
   {
      Chrome = new ChromeDriver();
      Wait = new(Chrome, TimeSpan.FromSeconds(10));
      Wait.IgnoreExceptionTypes(typeof(NoSuchElementException));

      if (openTwoTabs)
         OpenNewTab();
   }
   public void CloseChrome(bool tryAltOnFailure = true)
   {
      if (tryAltOnFailure)
      {
         try
         { handlesMethod(); }
         catch
         { tryMethod(); }
      }
      else
      { tryMethod(); }
      // End of method
      // Start of local function
      void tryMethod()
      {
         bool finished = false;
         while (!finished)
         {
            try
            { Chrome!.Close(); }
            catch
            { finished = true; }
            try
            { Chrome!.SwitchTo().Window(Chrome!.WindowHandles[0]); }
            catch
            { finished = true; }
         }
      }
      // End of local function tryMethod
      // Start of local function
      void handlesMethod()
      {
         var handles = Chrome!.WindowHandles;
         bool closed = false;
         while (!closed)
         {
            for (int handle = 0; handle < handles.Count; handle++)
            {
               Chrome!.Close();
               Chrome!.SwitchTo().Window(Chrome!.WindowHandles[0]);
            }
         }
      }
      // End of local function handlesMethod
   }
   public void AdjustImplicitWait(double shortenImplicitWaitBy)
   { Chrome!.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(shortenImplicitWaitBy); }
   public void RefreshChrome() => Chrome!.Navigate().Refresh();
   public void NavigateChromeToUrl(string url, int maxAttempts = 5)
   {
      int counter = 0;
      while (Chrome!.Url != url)
      {
         Chrome!.Navigate().GoToUrl(url);
         if (Chrome!.Url != url)
            Chrome.Url = url;
         counter++;
         if (counter == maxAttempts)
            break;
      }
   }


   // FindElement() methods
   public IWebElement FindElement(By by, bool adjustWindow, bool multipleTries = true, double shortenImplicitWaitBy = ImplicitWaitDefault) =>
   FindElementInternally(adjustWindow, multipleTries, by, shortenImplicitWaitBy);
   public IWebElement FindElement(ElementType by, string element, bool adjustWindow, bool multipleTries = true, double shortenImplicitWaitBy = ImplicitWaitDefault)
   {
      By type = ConvertElementTypeToByType(by, element);
      return FindElementInternally(adjustWindow, multipleTries, type, shortenImplicitWaitBy);
   }
   public IWebElement FindElement(IWebElement superElement, ElementType by, string element, bool multipleTries = true, int maxTries = 5, double shortenImplicitWaitBy = ImplicitWaitDefault)
   {
      By type = ConvertElementTypeToByType(by, element);
      AdjustImplicitWait(shortenImplicitWaitBy);
      List<IWebElement> returnValue = new(1);

      bool[] breakValue = new bool[1] { false };
      if (multipleTries)
      {
         for (int tries = 1; tries <= maxTries; tries++)
         {
            LocalFuncDoubleFindElementMethod(superElement, type, returnValue, breakValue);
            if (breakValue[0])
               break;
         }
      }
      else
      { LocalFuncDoubleFindElementMethod(superElement, type, returnValue, breakValue); }

      AdjustImplicitWait(ImplicitWait);
      return returnValue[0];

      // Start Local Functions
      void LocalFuncDoubleFindElementMethod(IWebElement superElement, By type, List<IWebElement> returnValue, bool[] breakValue)
      {
         try
         {
            returnValue.Add(superElement.FindElement(type));
            breakValue[0] = true;
         }
         catch
         {
            try
            {
               string superElementAttribute = ConvertAttributeTypeToString(AttributeType.CssSelector);
               string superElementSelector = superElement.GetAttribute(superElementAttribute);
               By superElementBy = ConvertElementTypeToByType(ElementType.CssSelector, superElementSelector);
               returnValue.Add(Wait!.Until(Chrome => Chrome!.FindElement(superElementBy)));
               breakValue[0] = true;
            }
            catch { }
         }
      }
      // End Local Functions
   }
   public IWebElement FindElement(ElementType bySE, string superElement, ElementType by, string element, bool multipleTries = true, int maxTries = 5, double shortenImplicitWaitBy = ImplicitWaitDefault)
   {
      By typeSE = ConvertElementTypeToByType(bySE, superElement);
      By type = ConvertElementTypeToByType(by, element);
      AdjustImplicitWait(shortenImplicitWaitBy);
      List<IWebElement> returnValue = new(1) { Chrome!.FindElement(typeSE) };

      bool[] breakValue = new bool[1] { false };
      if (multipleTries)
      {
         for (int tries = 1; tries <= maxTries; tries++)
         {
            LocalFuncDoubleFindElementMethod(typeSE, type, returnValue, breakValue);
            if (breakValue[0])
               break;
         }
      }
      else
      { LocalFuncDoubleFindElementMethod(typeSE, type, returnValue, breakValue); }

      AdjustImplicitWait(ImplicitWait);
      return returnValue[0];

      // Start Local Functions
      void LocalFuncDoubleFindElementMethod(By typeSE, By type, List<IWebElement> returnValue, bool[] breakValue)
      {
         try
         {
            returnValue.Add(Wait!.Until(Chrome => Chrome!.FindElement(typeSE).FindElement(type)));
            breakValue[0] = true;
         }
         catch
         {
            try
            {
               returnValue.Add(Wait!.Until(Chrome => Chrome!.FindElement(typeSE).FindElement(type)));
               breakValue[0] = true;
            }
            catch { }
         }
      }
      // End Local Functions
   }
   public IWebElement FindElementWithManyPossibleStrings(ElementType by, bool adjustWindow, bool multipleTries = true, double shortenImplicitWaitBy = ImplicitWaitDefault, params string[] elements)
   {
      IWebElement[] returnElement = new IWebElement[] { };
      foreach (var element in elements)
      {
         By type = ConvertElementTypeToByType(by, element);
         try
         {
            returnElement[0] = FindElementInternally(adjustWindow, multipleTries, type, shortenImplicitWaitBy);
            break;
         }
         catch { continue; }
      }
      return returnElement[0];
   }
   private IWebElement FindElementInternally(bool adjustWindow, bool multipleTries, By byType, double shortenImplicitWaitBy)
   {
      AdjustImplicitWait(shortenImplicitWaitBy);
      if (!multipleTries)
      {
         IWebElement valuer = Wait!.Until(Chrome => Chrome!.FindElement(byType));
         AdjustImplicitWait(ImplicitWait);
         return valuer;
      }
      else
      {
         try
         {
            IWebElement valuer = Wait!.Until(Chrome => Chrome!.FindElement(byType));
            AdjustImplicitWait(ImplicitWait);
            return valuer;
         }
         catch
         {
            RefreshChrome();
            try
            {
               if (adjustWindow)
                  ResizeWindow(TMaxMin, FMaxMin);
               Wait!.Until(Chrome => Chrome!.FindElement(byType));
               if (adjustWindow)
                  ResizeWindow(FMaxMin, FMaxMin);
               IWebElement valuer = Wait!.Until(Chrome => Chrome!.FindElement(byType));
               AdjustImplicitWait(ImplicitWait);
               return valuer;
            }
            catch
            {
               if (adjustWindow)
                  ResizeWindow(FMaxMin, FMaxMin);
               IWebElement valuer = Wait!.Until(Chrome => Chrome!.FindElement(byType));
               AdjustImplicitWait(ImplicitWait);
               return valuer;
            }
         }
      }
   }
   public ReadOnlyCollection<IWebElement> FindAllElements(By by, bool adjustWindow, bool multipleTries = true, double shortenImplicitWaitBy = ImplicitWaitDefault) => FindAllElements(adjustWindow, byType: by, multipleTries: multipleTries);
   public ReadOnlyCollection<IWebElement> FindAllElements(ElementType by, string element, bool adjustWindow, bool multipleTries = true, double shortenImplicitWaitBy = ImplicitWaitDefault)
   {
      By type = ConvertElementTypeToByType(by, element);
      return FindAllElements(adjustWindow, byType: type, multipleTries: multipleTries);
   }
   public ReadOnlyCollection<IWebElement> FindAllElementsWithManyPossibleStrings(ElementType by, bool adjustWindow, bool multipleTries = true, int maxTries = 5, double shortenImplicitWaitBy = ImplicitWaitDefault, params string[] elements)
   {
      AdjustImplicitWait(shortenImplicitWaitBy);
      List<ReadOnlyCollection<IWebElement>> returnArray = new(1);
      foreach (var element in elements)
      {
         By type = ConvertElementTypeToByType(by, element);
         if (!multipleTries)
         {
            try
            { returnArray.Add(Wait!.Until(Chrome => Chrome!.FindElements(type))); }
            catch { }
         }
         else
         {
            for (int trial = 1; trial <= maxTries; trial++)
            {
               try
               {
                  returnArray.Add(Wait!.Until(Chrome => Chrome!.FindElements(type)));
                  break;
               }
               catch
               {
                  RefreshChrome();
                  if (adjustWindow)
                     ResizeWindow(TMaxMin, FMaxMin);
                  try
                  {
                     returnArray.Add(Wait!.Until(Chrome => Chrome!.FindElements(type)));
                     break;
                  }
                  catch { }
               }
            }
         }
      }
      AdjustImplicitWait(ImplicitWait);
      return returnArray[0];
   }
   public List<string> GetAllElementsAttribute(ElementType by, string element, AttributeType typeToReturn, bool multipleTries = true, int tries = 5, double shortenImplicitWaitBy = ImplicitWaitDefault)
   {
      By elementBy = ConvertElementTypeToByType(by, element);
      string attribute = ConvertAttributeTypeToString(typeToReturn);
      List<string> elementsToReturn = new();
      if (!multipleTries)
      { AddElementAttributeToList(elementBy, attribute, elementsToReturn); }
      else
      {
         for (int attempt = 0; attempt < tries; attempt++)
         {
            try
            {
               AddElementAttributeToList(elementBy, attribute, elementsToReturn);
               break;
            }
            catch { }
         }
      }

      return elementsToReturn;

      // Start Local Function
      void AddElementAttributeToList(By elementBy, string attribute, List<string> elementsToReturn)
      {
         foreach (var iElement in Wait!.Until(Chrome => Chrome!.FindElements(elementBy)))
            elementsToReturn.Add(iElement.GetAttribute(attribute));
      }
      // End Local Function
   }
   private string ConvertAttributeTypeToString(AttributeType typeToReturn) =>
      typeToReturn switch
      {
         AttributeType.Id => "Id",
         AttributeType.CssSelector => "CssSelector",
         AttributeType.AriaLabel => "aria-label",
         _ =>
            throw new Exception($"{nameof(AttributeType)} {typeToReturn} has not been implemented in the switch conversion inside the {nameof(ConvertAttributeTypeToString)} method in {nameof(WebDriverManipulator)} class."),
      };

   public ReadOnlyCollection<IWebElement> FindAllElements(IWebElement superElement, ElementType by, string element, bool multipleTries = true, int tries = 5, double shortenImplicitWaitBy = ImplicitWaitDefault)
   {
      By type = ConvertElementTypeToByType(by, element);
      return FindElementsBySuperElement(superElement, type, multipleTries, tries, shortenImplicitWaitBy);

      // Start Local Function
      ReadOnlyCollection<IWebElement> FindElementsBySuperElement(IWebElement superElement, By type, bool multipleTries, int tries, double shortenImplicitWaitBy)
      {
         AdjustImplicitWait(shortenImplicitWaitBy);
         for (int reps = 0; reps < tries; reps++)
         {
            if (!multipleTries || reps.Equals(tries - 1))
               break;

            try
            {
               superElement.FindElements(type);
               break;
            }
            catch
            { }
         }
         AdjustImplicitWait(ImplicitWait);
         return superElement.FindElements(type);
      }
   }
   public ReadOnlyCollection<IWebElement> FindAllElements(bool adjustWindow, bool multipleTries, By byType)
   {
      if (!multipleTries)
      { return Wait!.Until(Chrome => Chrome!.FindElements(byType)); }
      else
      {
         try
         { return Wait!.Until(Chrome => Chrome!.FindElements(byType)); }
         catch
         {
            RefreshChrome();
            try
            {
               if (adjustWindow)
                  ResizeWindow(TMaxMin, FMaxMin);
               Wait!.Until(Chrome => Chrome!.FindElement(byType));
               if (adjustWindow)
                  ResizeWindow(FMaxMin, FMaxMin);
               return Wait!.Until(Chrome => Chrome!.FindElements(byType));
            }
            catch { return Wait!.Until(Chrome => Chrome!.FindElements(byType)); }
         }
      }
   }

   // Click() and SendKeys() methods
   public void ClickOnElement(By by, bool adjustWindow, bool multipleTries = true, int maxAttempts = 5, double shortenImplicitWaitTo = ImplicitWaitDefault)
   {
      bool clicked = false;
      int attempts = 1;
      AdjustImplicitWait(shortenImplicitWaitTo);
      if (!multipleTries)
         FindElement(by, adjustWindow, multipleTries).Click();
      else
      {
         while (!clicked)
         {
            clicked = attempts > maxAttempts ? true : false;
            if (clicked)
               break;
            clicked = ClickedOnElement(by, adjustWindow, multipleTries, shortenImplicitWaitTo);
            attempts++;
         }
      }
      AdjustImplicitWait(ImplicitWait);
   }
   public void ClickOnElement(ElementType by, string element, bool adjustWindow, bool multipleTries = true, int maxAttempts = 5, double shortenImplicitWaitTo = ImplicitWaitDefault)
   {
      bool clicked = false;
      int attempts = 1;
      AdjustImplicitWait(shortenImplicitWaitTo);
      if (!multipleTries)
      { FindElement(by, element, adjustWindow, multipleTries).Click(); }
      else
      {
         while (!clicked)
         {
            clicked = attempts > maxAttempts ? true : false;
            if (clicked)
               break;
            clicked = ClickedOnElement(by, adjustWindow, multipleTries, shortenImplicitWaitTo, element);
            attempts++;
         }
      }
      AdjustImplicitWait(ImplicitWait);
   }
   public void ClickOnElement(IWebElement element, bool multipleTries = true, int maxAttempts = 5, double shortenImplicitWaitBy = ImplicitWaitDefault)
   {
      Actions actions = new(Chrome);
      AdjustImplicitWait(shortenImplicitWaitBy);

      if (!multipleTries)
      { TryToClickWithTwoMethods(element, actions); }
      else
      {
         for (int attempt = 1; attempt <= maxAttempts; attempt++)
         {
            try
            {
               TryToClickWithTwoMethods(element, actions, attempt);
               break;
            }
            catch
            {
               try
               {
                  TryToClickWithTwoMethods(element, actions, attempt);
                  break;
               }
               catch
               { }
            }
         }
      }
      AdjustImplicitWait(ImplicitWait);

      // Start Local Functions
      static void TryToClickWithTwoMethods(IWebElement element, Actions actions, int attempt = 0)
      {
         try
         { element.Click(); }
         catch
         { actions.Click(element).Perform(); }
      }
      // End Local Functions
   }
   public void ClickOnElementWithManyPossibleStrings(ElementType by, bool adjustWindow, bool multipleTries = true, int maxAttempts = 4, double shortenImplicitWaitBy = ImplicitWaitDefault, params string[] elements)
   {
      AdjustImplicitWait(shortenImplicitWaitBy);
      bool clicked = false;
      int attempts = 0;

      // This number ensures that we get one attempt only
      if (!multipleTries)
         maxAttempts = 0;

      while (!clicked)
      {
         clicked = attempts > maxAttempts ? true : false;
         clicked = ClickedOnElement(by, adjustWindow: adjustWindow, multipleTries: multipleTries, shortenImplicitWaitBy, elements);
         attempts++;
      }
      AdjustImplicitWait(ImplicitWait);
   }
   public bool ClickedOnElement(By by, bool adjustWindow, bool multipleTries, double shortenImplicitWaitBy, params string[] elements)
   {
      bool[] clicked = new bool[1] { false };
      foreach (var element in elements)
      {
         if (!multipleTries)
         {
            bool breakLoop = LocalFuncSimpleBreakLoop(by, element, adjustWindow, multipleTries, out clicked[0]);
            if (breakLoop)
               break;
         }
         else
         {
            try
            {
               bool breakLoop = LocalFuncSimpleBreakLoop(by, element, adjustWindow, multipleTries, out clicked[0]);
               if (breakLoop)
                  break;
            }
            catch (ElementClickInterceptedException)
            {
               bool breakLoop = LocalFuncComplexBreakLoop(by, adjustWindow, multipleTries, out clicked[0]);
               if (breakLoop)
                  break;
            }
            catch (StaleElementReferenceException)
            {
               bool breakLoop = LocalFuncComplexBreakLoop(by, adjustWindow, multipleTries, out clicked[0]);
               if (breakLoop)
                  break;
            }
            catch (NoSuchElementException)
            {
               bool breakLoop = LocalFuncComplexBreakLoop(by, adjustWindow, multipleTries, out clicked[0]);
               if (breakLoop)
                  break;
            }
            catch (ElementNotInteractableException)
            {
               bool breakLoop = LocalFuncComplexBreakLoop(by, adjustWindow, multipleTries, out clicked[0]);
               if (breakLoop)
                  break;
            }
         }
      }
      return clicked[0];

      // Start Local Function
      bool LocalFuncComplexBreakLoop(By by, bool adjustWindow, bool multipleTries, out bool clicked)
      {
         bool breakout;
         try
         {
            clicked = RefreshResizeClick(by, adjustWindow, multipleTries);
            breakout = true;
         }
         catch
         {
            if (adjustWindow)
               ResizeWindow(FMaxMin, FMaxMin);
            breakout = false;
            clicked = false;
         }

         return breakout;
      }
      // Start Local Function
      bool LocalFuncSimpleBreakLoop(By by, string element, bool adjustWindow, bool multipleTries, out bool clicked)
      {
         FindElement(by, adjustWindow, multipleTries: multipleTries).Click();
         clicked = true;
         return true;
      }
      // End All Local Functions
   }

   public bool ClickedOnElement(ElementType by, bool adjustWindow, bool multipleTries, double shortenImplicitWaitBy, params string[] elements)
   {
      bool[] clicked = new bool[1] { false };
      foreach (var element in elements)
      {
         if (!multipleTries)
         {
            bool breakLoop = LocalFuncSimpleBreakLoop(by, element, adjustWindow, multipleTries, out clicked[0]);
            if (breakLoop)
               break;
         }
         else
         {
            try
            {
               bool breakLoop = LocalFuncSimpleBreakLoop(by, element, adjustWindow, multipleTries, out clicked[0]);
               if (breakLoop)
                  break;
            }
            catch (ElementClickInterceptedException)
            {
               bool breakLoop = LocalFuncComplexBreakLoop(by, element, adjustWindow, multipleTries, out clicked[0]);
               if (breakLoop)
                  break;
            }
            catch (StaleElementReferenceException)
            {
               bool breakLoop = LocalFuncComplexBreakLoop(by, element, adjustWindow, multipleTries, out clicked[0]);
               if (breakLoop)
                  break;
            }
            catch (NoSuchElementException)
            {
               bool breakLoop = LocalFuncComplexBreakLoop(by, element, adjustWindow, multipleTries, out clicked[0]);
               if (breakLoop)
                  break;
            }
            catch (ElementNotInteractableException)
            {
               bool breakLoop = LocalFuncComplexBreakLoop(by, element, adjustWindow, multipleTries, out clicked[0]);
               if (breakLoop)
                  break;
            }
         }
      }
      return clicked[0];

      // Start Local Function
      bool LocalFuncComplexBreakLoop(ElementType by, string element, bool adjustWindow, bool multipleTries, out bool clicked)
      {
         bool breakout;
         try
         {
            clicked = RefreshResizeClick(by, adjustWindow, element, multipleTries);
            breakout = true;
         }
         catch
         {
            if (adjustWindow)
               ResizeWindow(FMaxMin, FMaxMin);
            breakout = false;
            clicked = false;
         }

         return breakout;
      }
      // Start Local Function
      bool LocalFuncSimpleBreakLoop(ElementType by, string element, bool adjustWindow, bool multipleTries, out bool clicked)
      {
         FindElement(by, element, adjustWindow, multipleTries: multipleTries).Click();
         clicked = true;
         return true;
      }
      // End All Local Functions
   }
   public bool RefreshResizeClick(By by, bool adjustWindow, bool multipleTries)
   {
      bool clicked = false;
      RefreshChrome();
      if (adjustWindow)
         ResizeWindow(TMaxMin, FMaxMin);
      try
      {
         FindElement(by, multipleTries).Click();
         clicked = true;
      }
      catch
      {
         Actions actions = new(Chrome);
         actions.MoveToElement(FindElement(by, multipleTries)).Click(FindElement(by, multipleTries)).Perform();
         clicked = true;
      }
      if (adjustWindow)
         ResizeWindow(FMaxMin, FMaxMin);
      return clicked;
   }

   public bool RefreshResizeClick(ElementType by, bool adjustWindow, string element, bool multipleTries)
   {
      bool clicked = false;
      RefreshChrome();
      if (adjustWindow)
         ResizeWindow(TMaxMin, FMaxMin);
      try
      {
         FindElement(by, element, multipleTries).Click();
         clicked = true;
      }
      catch
      {
         Actions actions = new(Chrome);
         actions.MoveToElement(FindElement(by, element, multipleTries
         )).Click(FindElement(by, element, multipleTries)).Perform();
         clicked = true;
      }
      if (adjustWindow)
         ResizeWindow(FMaxMin, FMaxMin);
      return clicked;
   }
   public By StringToBy(string by, string element) =>
   ConvertElementTypeToByType
   (
      ConvertStringToElementType(by),
      element
   );
   public By ConvertElementTypeToByType(ElementType by, string element) =>
   by switch
   {
      ElementType.ClassName => By.ClassName(element),
      ElementType.CssSelector => By.CssSelector(element),
      ElementType.Id => By.Id(element),
      ElementType.LinkText => By.LinkText(element),
      ElementType.Name => By.Name(element),
      ElementType.TagName => By.TagName(element),
      ElementType.XPath => By.XPath(element),
      _ => throw new Exception($"{nameof(ElementType)} {by} has not been implemented in the switch conversion inside the {nameof(ConvertElementTypeToByType)} method in {nameof(WebDriverManipulator)} class."),
   };

   public ElementType ConvertStringToElementType(string type) =>
   SplitJoinLowerText(type, split: " ") switch
   {
      "classname" or "class" => ElementType.ClassName,
      "cssselector" or "selector" => ElementType.CssSelector,
      "id" => ElementType.Id,
      "linktext" or "link" => ElementType.LinkText,
      "name" => ElementType.Name,
      "tagname" or "tag" => ElementType.TagName,
      "xpath" => ElementType.XPath,
      _ => throw new Exception($"The paramter {nameof(type)} has been input as \"{type}\". This is either not implemented by the {nameof(ElementType)} item or has been input erroneously."),
   };

   public void SendKeysToElement(By by, KeysEnum message, bool multipleTries = true)
   {
      FindElement(by, multipleTries).SendKeys(ConvertKeys(message));
   }
   public void SendKeysToElement(By by, string message, bool multipleTries = true)
   {
      if (message != null)
         FindElement(by, multipleTries).SendKeys(message);
      else
         throw new System.Exception($"{nameof(message)} parameter cannot be null.");
   }
   public void SendKeysToElement(ElementType by, string element, string message, bool multipleTries = true)
   {
      if (message != null)
         FindElement(by, element, multipleTries).SendKeys(message);
      else
         throw new System.Exception($"{nameof(message)} parameter cannot be null.");
   }


   // Find text methods
   public string FindElementsText(ElementType by, bool adjustWindow, bool multipleTries = false, double shortenImplicitWaitBy = ImplicitWaitDefault, params string[] elements)
   {
      string[] text = new string[1] { "" };
      int attempts = 0;
      AdjustImplicitWait(shortenImplicitWaitBy);
      foreach (string element in elements)
      {
         if (attempts > 4)
            break;
         if (attempts > 1)
            RefreshChrome();

         try
         {
            text[0] = FindElementText(by, element: element, adjustWindow, multipleTries: multipleTries);
            break;
         }
         catch { }

         if (text[0] != "")
         { break; }
      }
      AdjustImplicitWait(ImplicitWait);

      return text[0];
   }
   public string FindElementText(ElementType by, string element, bool adjustWindow, bool multipleTries = true, double shortenImplicitWaitBy = ImplicitWaitDefault, int totalAttempts = TotalAttemptsDefault)
   {
      string[] text = new string[1] { "" };
      int attempts = 0;
      AdjustImplicitWait(shortenImplicitWaitBy);
      if (!multipleTries)
      { text[0] = FindElement(by, element, multipleTries).Text; }
      else
      {
         while (text[0] == "")
         {
            if (attempts.Equals(totalAttempts))
            {
               text[0] = FindElement(by, element, adjustWindow, multipleTries: false).Text;
               break; // We want to throw exceptions, if possible
            }
            if (attempts > 0)
               RefreshChrome();

            try
            {
               // Regular text method
               text[0] = FindElement(by, element, adjustWindow, multipleTries, shortenImplicitWaitBy).Text;
               break;
            }
            catch
            {
               if (attempts > totalAttempts)
                  FindElement(by, element, adjustWindow, multipleTries: false);
            }
            attempts++;
         }
      }
      AdjustImplicitWait(ImplicitWait);
      return text[0];
   }


   // Get Attribute methods
   public string GetElementType(ElementType by, string element, AttributeType attributeType)
   {
      By type = ConvertElementTypeToByType(by, element);
      return GetElementType(type, attributeType);
   }
   public string GetElementTypeWithManyPossibleStrings(ElementType by, AttributeType attributeType, params string[] elements)
   {
      string[] returner = new string[1] { "" };
      foreach (var element in elements)
      {
         By type = ConvertElementTypeToByType(by, element);

         try { returner[0] = GetElementType(type, attributeType); }
         catch { };
      }
      return returner[0];
   }
   private string GetElementType(By type, AttributeType attributeType)
   => Wait!.Until(Chrome => Chrome!.FindElement(type)).GetAttribute(ConvertAttributeTypeToString(attributeType));


   // Window Manipulation methods
   public void ResizeWindow(bool maximize, bool minimize, int width = 0, int height = 0)
   {
      if (maximize && minimize)
         throw new ArgumentException($"Parameter arguments {nameof(maximize)} and {nameof(minimize)} cannot both be true in {nameof(ResizeWindow)}.\nArg {nameof(maximize)} input: {maximize}.\nArg {nameof(minimize)} input: {minimize}");
      if (minimize)
         Chrome!.Manage().Window.Minimize();
      if (maximize)
         Chrome!.Manage().Window.Maximize();

      if (width > 0 && height > 0)
         LocalResizeWindow(width, height);
      SleepSeconds(0.5);

      void LocalResizeWindow(int width, int height) => Chrome!.Manage().Window.Size = new Size(width, height);
   }
   public void OpenNewTab() => ExecuteAScript(JavaScriptOpenNewTab);
   public void ExecuteAScript(string script) => ((IJavaScriptExecutor)Chrome!).ExecuteScript(script);
   public void SwitchToWindow(string window) => Chrome!.SwitchTo().Window(window);
   public void SwitchToWindow(int windowNum) => Chrome!.SwitchTo().Window(Chrome!.WindowHandles[windowNum]);
   public void NavigateToUrl(string url, bool usualWay)
   {
      if (usualWay)
      {
         try { Chrome!.Navigate().GoToUrl(url); }
         catch (WebDriverException)
         {
            try { RefreshChrome(); }
            catch
            {
               SleepSeconds(10.5);
               Chrome!.Navigate().GoToUrl(url);
            }
         }
      }
      else
      {
         try { Chrome!.Url = url; }
         catch (WebDriverException)
         { SleepSeconds(10.5); Chrome!.Url = url; }
      }
   }
   public void ScrollToElement(ElementType by, string element, bool adjustWindow, bool multipleTries = true, double shortenImplicitWaitTo = 0, int numberOfTries = 5)
   {
      Actions actions = new(Chrome);
      for (int tries = 1; tries <= numberOfTries; tries++)
      {
         try
         {
            actions.MoveToElement(FindElement(by, element, adjustWindow, multipleTries, shortenImplicitWaitTo)).Perform();
            break;
         }
         catch
         { }
      }
   }
   public void ScrollToElement(IWebElement element, bool adjustWindow, bool multipleTries = true, double shortenImplicitWaitBy = ImplicitWaitDefault, int numberOfTries = 5)
   {
      AdjustImplicitWait(shortenImplicitWaitBy);
      Actions actions = new(Chrome);
      if (multipleTries)
      {
         for (int tries = 1; tries <= numberOfTries; tries++)
         {
            try
            {
               actions.MoveToElement(element).Perform();
               break;
            }
            catch
            {
               RefreshChrome();
               if (adjustWindow)
                  ResizeWindow(TMaxMin, FMaxMin);
               try
               {
                  actions.MoveToElement(element).Perform();
                  break;
               }
               catch { }
            }
         }
      }
      else
      { actions.MoveToElement(element).Perform(); }
      AdjustImplicitWait(ImplicitWait);
   }
   public void ScrollToElementWithManyPossibleStrings(ElementType by, bool adjustWindow, bool multipleTries = true, double shortenImplicitWaitBy = 0, int numberOfTries = 5, params string[] elements)
   {
      Actions actions = new(Chrome);
      for (int tries = 1; tries <= numberOfTries; tries++)
      {
         foreach (var element in elements)
         {
            try
            {
               actions.MoveToElement(FindElement(by, element, adjustWindow, multipleTries, shortenImplicitWaitBy)).Perform();
               break;
            }
            catch
            { }
         }
      }
   }
   private const string JavaScriptOpenNewTab = "window.open('about:blank', '_blank');";


   // Constants: Element strings -- USED IN ANY AND ALL TASKS
   public const string TagElement1 = "form-inline";
   public const string TagElement2 = "select2-choices";
   public const string NotesSection = "notes-section";
   public const string NotesBoxesElem = "box";
   public const string SmallText1 = "small";
   public const string SmallText2 = "this call";
   public const string NotesText1 = "comment-col-note";
   public const string NotesText2 = "comment-display";
   public const string User_LoginElem = "user_login";
   public const string User_PasswordElem = "user_password";
   public const string LoginButtonSelector = "#signin > input.button.callout.bigbutton";
   public const string AccountNumberSelector = "#body > div.sidenav-container.container-fluid > div > div.col-md-10.pageform > div > div:nth-child(1) > div > span";
   public const string MenuLabelSelector = "#ctm-main-nav > ul.nav.navbar-nav.navbar-right.ctm-outer-hr.ctm-settings-hr > li.dropdown.nav-recent-accounts > a > span > span";
   public const string Edit = "Edit";
   public const string LinkSelector = "#body > div.standard-table-container > div.standard-table-parent.container-fluid > div:nth-child(9) > div > table > tbody > tr > td:nth-child(1) > a";
   public const string MainList = "main-list";
   public const string TrElem = "tr";
   public const string TdElem = "td";
   public const string H5Elem = "h5";
   public const string Duration = "duration";
   public const string BusinessUserElem = "business_user_username";
   public const string BusinessPassElem = "business_user_password";
   public const string Button = "button";
   public const string CallsSearchResults = "calls_search_results";
   public const string TrTagName = "tr";
   public const string PaneBodyCell = "pane--body-cell";
   public const string DateRangeSelector = "#account_main > div > div:nth-child(2) > div > div:nth-child(2) > div > span";
   public const string NumberOfPagesSelector = "#account_main > div > div:nth-child(3) > div > div.calls.clearfix > div > div.container > div > div > ul";
   public const string CalendarSelector = "#account_main > div > div:nth-child(2) > div > div:nth-child(2) > div > i";
   public const string CalendarSelector2 = "#account_main > div > div:nth-child(2) > div > div:nth-child(2) > div";
   public Exception KeyException = new("The given parameter does not match the appropriate key.");
   public string ConvertStringToKeyString(string key) =>
   SplitJoinLowerText(key, " ") switch
   {
      "enter" => Keys.Enter,
      "clear" => Keys.Clear,
      "return" => Keys.Return,
      "escape" => Keys.Escape,
      _ => throw KeyException,
   };
   public KeysEnum ConvertFromStringToKeyEnum(string key) =>
      SplitJoinLowerText(key, " ") switch
      {
         "enter" => KeysEnum.Enter,
         "clear" => KeysEnum.Clear,
         "return" => KeysEnum.Return,
         "escape" => KeysEnum.Escape,
         _ => throw KeyException,
      };
   public string ConvertKeys(KeysEnum keys) =>
      keys switch
      {
         KeysEnum.Enter => Keys.Enter,
         KeysEnum.Clear => Keys.Clear,
         KeysEnum.Return => Keys.Return,
         KeysEnum.Escape => Keys.Escape,
         _ => throw KeyException,
      };
}