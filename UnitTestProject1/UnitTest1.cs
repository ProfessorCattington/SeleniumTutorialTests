using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using OpenQA.Selenium.Interactions;

namespace SeleniumTests
{

    [TestClass]
    public class UnitTest1
    {
        ChromeDriver chromeDriver = new ChromeDriver("C:\\Users\\Mike\\Downloads\\chromedriver_win32");
        string bingURL = "https://www.bing.com/";

        private void OpenURL()
        {
            chromeDriver.Manage().Window.Maximize();
            chromeDriver.Navigate().GoToUrl(bingURL);

        }

        [TestMethod]
        public void TestSearchFieldAppearsOnPageLoad()
        {
            OpenURL();

            IWebElement searchBox = chromeDriver.FindElementByXPath("//div[@class='b_searchboxForm']");

            Assert.IsTrue(searchBox.Enabled);
            Assert.IsTrue(searchBox.Displayed);

            chromeDriver.Quit();
        }

        [TestMethod]
        public void TestSearchFieldIsInFocusOnSiteLoad()
        {
            OpenURL();

            IWebElement currentFocusedElement = chromeDriver.SwitchTo().ActiveElement();

            string focusedClass = currentFocusedElement.GetAttribute("class");

            string expectedClass = "b_searchbox";

            Assert.AreEqual(expectedClass, focusedClass);

            chromeDriver.Quit();

        }
        [TestMethod]
        public void TestSearchFieldAcceptsInput()
        {
            OpenURL();

            string searchString = "Cat Videos";

            IWebElement searchBox = chromeDriver.FindElementByXPath("//input[@id='sb_form_q']");

            searchBox.SendKeys(searchString);

            string searchBoxValue = searchBox.GetAttribute("value");

            Assert.AreEqual(searchString, searchBoxValue);

            chromeDriver.Quit();
        }
        [TestMethod]
        public void TestSearchSuggestionsAppearCorrectly()
        {
            OpenURL();

            IWebElement searchBox = chromeDriver.FindElementById("sb_form_q");
            searchBox.SendKeys("Cat Videos");

            WebDriverWait webDriverWait = new WebDriverWait(chromeDriver, System.TimeSpan.FromSeconds(5));
            //trying out a different method of finding elements. unsure if using class name is better or worse than xpath
            webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("sa_as")));  

            IList<IWebElement> suggestions = chromeDriver.FindElementsByClassName("sa_sg");

            //unsure if there's a pixel scaling method for search suggestions in the list. using a more generic list count test
            int expectedSuggestions = 8;
            int searchSuggestionsTotal = suggestions.Count;

            Assert.AreEqual(expectedSuggestions, searchSuggestionsTotal);

            chromeDriver.Quit();
        }
        [TestMethod]
        public void TestSearchSuggestionsContainSearchText()
        {

            string searchText = "Cat Videos";

            OpenURL();

            IWebElement searchBox = chromeDriver.FindElementById("sb_form_q");
            searchBox.SendKeys(searchText);

            WebDriverWait webDriverWait = new WebDriverWait(chromeDriver, System.TimeSpan.FromSeconds(5));
            webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("sa_sg")));

            IList<IWebElement> suggestions = chromeDriver.FindElementsByClassName("sa_sg");

            for(int i = 0; i < suggestions.Count; i++)
            {
                string suggestionText = suggestions[i].Text;

                //bing suggestions are lower cased. call tolower on our search text
                Assert.IsTrue(suggestionText.Contains(searchText.ToLower()));

            }

            chromeDriver.Quit();
        }
        [TestMethod]
        public void TestSearchFieldButtonRedirectsUser()
        {
            OpenURL();

            string searchText = "Cat Videos";

            IWebElement searchBox = chromeDriver.FindElementById("sb_form_q");
            searchBox.SendKeys(searchText);

            IWebElement searchButton = chromeDriver.FindElementByXPath("//input[@type='submit']");

            Actions action = new Actions(chromeDriver);

            action.MoveToElement(searchButton).Click().Build().Perform();

            string currentURL = chromeDriver.Url;

            //take the url and see if it contains the search term with '+' operators where spaces were

            string[] searchTermArray = searchText.Split(' ');

            string newSearchTerm = "";
            for(int i = 0; i < searchTermArray.Length; i++)
            {
                if(i < searchTermArray.Length -1)
                {
                    newSearchTerm += searchTermArray[i] + "+";
                }
                else
                {
                    newSearchTerm += searchTermArray[i];
                }
            }

            bool currentURLContainsSearch = currentURL.Contains(newSearchTerm) ? true : false;

            Assert.IsTrue(currentURLContainsSearch);

            chromeDriver.Quit();
        }

        [TestMethod]
        public void TestBlankSearchFieldDisplaysTrendingTopics()
        {
            OpenURL();

            IWebElement searchBox = chromeDriver.FindElementById("sb_form_q");

            Actions action = new Actions(chromeDriver);
            action.MoveToElement(searchBox).Click().Perform();

            WebDriverWait webDriverWait = new WebDriverWait(chromeDriver, System.TimeSpan.FromSeconds(5));
            webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("sa_as")));

            IWebElement trendingHeader = chromeDriver.FindElementByClassName("sa_hd");
            IList<IWebElement> trendingTopics = chromeDriver.FindElementsByClassName("sa_sg");

            Assert.IsTrue(trendingHeader.Text == "Trending now");

            int expectedTrendingTopics = 8;

            Assert.AreEqual(expectedTrendingTopics, trendingTopics.Count);

            for(int i  = 0; i < trendingTopics.Count; i++)
            {

                Assert.IsTrue(trendingTopics[i].Text != "");
            }

            chromeDriver.Quit();
        }


        //Extra test while learning to use selenium.
        //does not pass. MoveToElement on the office online does not perform mouseover behavior like other pages
        //tested pages; gameranx -> platform header control . browserstack -> sign up for free button
        //i will need a more experienced person to explain what's wrong here.
        public void TestMouseOverOfficeOnlineRevealsControls()
        {
            chromeDriver.Manage().Window.Maximize();
            chromeDriver.Navigate().GoToUrl("https://www.bing.com/");

            WebDriverWait webDriverWait = new WebDriverWait(chromeDriver, System.TimeSpan.FromSeconds(5));

            IWebElement officeOnline = chromeDriver.FindElementByXPath("//li[@id='office']");

            Actions action = new Actions(chromeDriver);

            action.MoveToElement(officeOnline).Perform();

            IWebElement controls = chromeDriver.FindElementByXPath("//div[@id='off_menu_cont']");

            bool isDisplayed = controls.Displayed;

            Assert.IsTrue(isDisplayed);
        }
    }
}
