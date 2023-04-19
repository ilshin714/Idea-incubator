using NUnit.Framework;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using OpenQA.Selenium.Support;
using System;
using System.Diagnostics;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace IdeaIncubator.Tests.Selenium
{
    class Page_Login
    {
        [TestFixture]
        public class LoginTests
        {
            private IWebDriver _driver;
            [SetUp]
            public void Setup()
            {
                var firefoxOptions = new FirefoxOptions();
                firefoxOptions.AcceptInsecureCertificates = true;
                firefoxOptions.BrowserExecutableLocation = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
                _driver = new FirefoxDriver(firefoxOptions);
            }

            [Test]
            public void Navigation_Click_Profile_Login_Is_Default()
            {
                // Setup - wait commands due to graphic loads
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));

                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");
                // When you click on the profile button
                wait.Until(_driver => _driver.FindElement(By.XPath("/html/body/div[3]/header/div/button[2]")).Enabled);
                IWebElement element = _driver.FindElement(By.XPath("/html/body/div[3]/header/div/button[2]"));
                element.Click();
                // And username element is on the page
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".testTxtLoginUserName > div > div > input:nth-child(1)")).Enabled);
                // Then the URL is at the /login path
                Assert.AreEqual(_driver.Url, "https://localhost:7289/login");
            }

            [Test]
            public void Login_Initial_Username_is_blank()
            {
                // Setup - wait commands due to graphic loads
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));

                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");
                
                // When you click on the profile button
                IWebElement element = _driver.FindElement(By.XPath("/html/body/div[3]/header/div/button[2]"));
                element.Click();

                // And username element is on the page
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".testTxtLoginUserName > div > div > input:nth-child(1)")).Enabled);

                // And you check the username
                element = _driver.FindElement(By.CssSelector(".testTxtLoginUserName > div > div > input:nth-child(1)"));
                
                // Then the username is blank
                Assert.AreEqual(element.Text, "");
            }

            [Test]
            public void Login_Initial_Password_is_blank()
            {
                // Setup - wait commands due to graphic loads
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));

                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");

                // When you click on the profile button
                IWebElement element = _driver.FindElement(By.XPath("/html/body/div[3]/header/div/button[2]"));
                element.Click();

                // And username element is on the page
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".testTxtLoginPassword > div > div > input:nth-child(1)")).Enabled);

                // And you check the password
                element = _driver.FindElement(By.CssSelector(".testTxtLoginPassword > div > div > input:nth-child(1)"));

                // Then the password is blank
                Assert.AreEqual(element.Text, "");
            }

            [Test]
            public void Login_Blank_Username_Password_Invalid()
            {
                // Setup - wait commands due to graphic loads
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));

                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");

                // When you click on the profile button
                IWebElement element = _driver.FindElement(By.XPath("/html/body/div[3]/header/div/button[2]"));
                element.Click();

                // And the login button is on the page
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".testButtonLogin")).Enabled);

                // And you click Login
                element = _driver.FindElement(By.CssSelector(".testButtonLogin"));
                element.Click();

                // Then receive a warning message about invalid login
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".mud-snackbar-content-message")).Displayed);
                element = _driver.FindElement(By.CssSelector(".mud-snackbar-content-message"));
                Assert.AreEqual(element.Text, "Login is invalid. Please try again.");
            }

            [Test]
            public void Login_Username_Case_Sensitive()
            {
                // Setup - wait commands due to graphic loads
                bool testValid = true;
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));

                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");

                // And you click on the profile button
                IWebElement element = _driver.FindElement(By.XPath("/html/body/div[3]/header/div/button[2]"));
                element.Click();

                // When you enter "testaccount" as a username and enter it's password
                // Wait for UserName label to appear, then click it
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".testTxtLoginUserName > div > div > input:nth-child(1)")).Enabled);
                element = _driver.FindElement(By.CssSelector(".testTxtLoginUserName > div > div > input:nth-child(1)"));
                element.Click();

                // Enter in "testaccount"
                element.SendKeys("testaccount");

                // Click the User Password label
                element = _driver.FindElement(By.CssSelector(".testTxtLoginPassword > div > div > input:nth-child(1)"));
                element.Click();

                // Enter in 12345
                element.SendKeys("12345");

                // And the login button is on the page
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".testButtonLogin")).Enabled);

                // And you click Login
                element = _driver.FindElement(By.CssSelector(".testButtonLogin"));
                element.Click();

                // And receive a warning message about invalid login
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".mud-snackbar-content-message")).Displayed);
                element = _driver.FindElement(By.CssSelector(".mud-snackbar-content-message"));
                testValid = element.Text.Equals("Login is invalid. Please try again.");

                // And you enter in "TestAccount" in Username
                try
                {
                    wait.Until(_driver => !(_driver.FindElement(By.CssSelector(".mud-snackbar-content-message")).Displayed));
                } catch (WebDriverTimeoutException wdte)
                {
                    // Happens because as the element disappears the check can't find the element anymore
                }
                element = _driver.FindElement(By.CssSelector(".testTxtLoginUserName > div > div > input:nth-child(1)"));
                element.Clear();
                element.Click();
                element.SendKeys("TestAccount");

                // And you click Login
                element = _driver.FindElement(By.CssSelector(".testButtonLogin"));
                element.Click();

                // Then you are logged in (sent to the main page)
                wait.Until(_driver => _driver.FindElement(By.CssSelector("div.mud-grid:nth-child(2)")).Displayed);
                try
                {
                    element = _driver.FindElement(By.CssSelector("div.mud-grid:nth-child(2)"));
                } catch (Exception ex)
                {
                    testValid = false;
                }
                Assert.IsTrue(testValid);
            }

            [Test]
            public void Login_UserId_Set_After_Login()
            {
                // Setup - wait commands due to graphic loads
                bool testValid = true;
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));

                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");

                // And you click on the profile button
                IWebElement element = _driver.FindElement(By.XPath("/html/body/div[3]/header/div/button[2]"));
                element.Click();

                // When you enter "testaccount" as a username and enter it's password
                // Wait for UserName label to appear, then click it
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".testTxtLoginUserName > div > div > input:nth-child(1)")).Enabled);
                element = _driver.FindElement(By.CssSelector(".testTxtLoginUserName > div > div > input:nth-child(1)"));
                element.Click();

                // Enter in "testaccount"
                element.SendKeys("TestAccount");

                // Click the User Password label
                element = _driver.FindElement(By.CssSelector(".testTxtLoginPassword > div > div > input:nth-child(1)"));
                element.Click();

                // Enter in 12345
                element.SendKeys("12345");

                // And the login button is on the page
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".testButtonLogin")).Enabled);

                // And you click Login
                element = _driver.FindElement(By.CssSelector(".testButtonLogin"));
                element.Click();

                // And you are sent to the main page
                wait.Until(_driver => _driver.FindElement(By.CssSelector("div.mud-grid:nth-child(2)")).Displayed);
                try
                {
                    element = _driver.FindElement(By.CssSelector("div.mud-grid:nth-child(2)"));
                }
                catch (Exception ex)
                {
                    testValid = false;
                }

                // Then the username label appears in the upper right (only set if userId is set)
                wait.Until(_driver => _driver.FindElement(By.CssSelector("h6.mud-typography")).Displayed);
                element = _driver.FindElement(By.CssSelector("h6.mud-typography"));
                testValid = testValid && element.Text.Equals("TestAccount");
                Assert.IsTrue(testValid);
            }

            [TearDown]
            public void CloseBrowser()
            {
                _driver.Quit();
            }
        }

    }
}
