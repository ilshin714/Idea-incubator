using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using IdeaIncubatorBlazor.Services.Ideas;
using IdeaIncubatorBlazor.Services.Users;

namespace IdeaIncubator.Tests.Selenium
{
    class Page_Index
    {
        [TestFixture]
        public class IndexTests
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
            public void Navigation_Homepage_Accessible()
            {
                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");
                // Then the URL is at the / path
                Assert.AreEqual(_driver.Url, "https://localhost:7289/");
            }

            [Test]
            public void SearchBar_Prompt_Disappears()
            {
                // Setup
                bool isVisible = true;
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));
                
                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");
                // When the searchbar is loaded
                wait.Until(_driver => _driver.FindElement(By.CssSelector("input.mud-input-root-outlined")).Enabled);

                // Then it does not disappear again
                try
                {
                    wait.Until(_driver => !_driver.FindElement(By.CssSelector("input.mud-input-root-outlined")).Displayed);
                    isVisible = false;
                } catch (WebDriverTimeoutException wdte)
                {
                    // Good if this happens
                }
                Assert.IsTrue(isVisible);
            }

            [Test]
            public void Index_Valid_Login_Causes_Username_To_Appear()
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

                // Then the username label appears in the upper right
                wait.Until(_driver => _driver.FindElement(By.CssSelector("h6.mud-typography")).Displayed);
                element = _driver.FindElement(By.CssSelector("h6.mud-typography"));
                testValid = testValid && element.Text.Equals("TestAccount");
                Assert.IsTrue(testValid);
            }

            [Test]
            public void Index_Login_Status_Not_Maintained_Between_Sessions()
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

                // Get a new session
                var firefoxOptions = new FirefoxOptions();
                firefoxOptions.AcceptInsecureCertificates = true;
                firefoxOptions.BrowserExecutableLocation = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
                IWebDriver _driver2 = new FirefoxDriver(firefoxOptions);
                _driver.Close();
                _driver2.Navigate()
                    .GoToUrl("https://localhost:7289");

                // Then the username label is blank
                wait.Until(_driver => _driver2.FindElement(By.CssSelector("h6.mud-typography")).Enabled);
                element = _driver2.FindElement(By.CssSelector("h6.mud-typography"));
                testValid = testValid && element.Text.Equals("");
                _driver2.Quit();
                Assert.IsTrue(testValid);
            }

            [Test]
            public void Index_Ideas_Are_Displayed_When_Supposed_To()
            {
                // Setup - wait commands due to graphic loads

                IdeaIncubatorBlazor.Models.IdeaIncubatorDbContext dbContext;
                IRoleService roleService;
                IUserService userService;
                IUserIdeaRoleService userIdeaRoleService;
                IIdeaService ideaService;
                dbContext = new IdeaIncubatorBlazor.Models.IdeaIncubatorDbContext();
                roleService = new RoleService(dbContext, null);
                userService = new UserService(dbContext, null, roleService);
                userIdeaRoleService = new UserIdeaRoleService(dbContext, null);
                ideaService = new IdeaService(dbContext, userService, roleService, userIdeaRoleService, null);

                bool testValid = true;
    
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));

                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");

                // When you count the ideas
                wait.Until(_driver => _driver.FindElement(By.CssSelector("div.mud-container-maxwidth-lg")).Displayed);
                int numberOfIdeas = _driver.FindElements(By.CssSelector("div.mud-container-maxwidth-lg")).Count();
                
                // And you get the number from the database
                int databaseIdeas = ideaService.GetIdeas().Count();

                Assert.That(numberOfIdeas, Is.EqualTo(databaseIdeas));
            }

            [TearDown]
            public void CloseBrowser()
            {
                if (_driver != null)
                {
                    _driver.Quit();
                }
            }
        }
    }
}
