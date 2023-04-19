using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using IdeaIncubatorBlazor.Services.Ideas;
using IdeaIncubatorBlazor.Services.Users;

namespace IdeaIncubator.Tests.Selenium
{
    class Page_Idea
    {
        [TestFixture]
        public class IdeaTests
        {
            private IWebDriver _driver;
            private WebDriverWait wait;
            [SetUp]
            public void Setup()
            {
                var firefoxOptions = new FirefoxOptions();
                firefoxOptions.AcceptInsecureCertificates = true;
                firefoxOptions.BrowserExecutableLocation = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
                _driver = new FirefoxDriver(firefoxOptions);
                wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));
            }

            [Test]
            public void Navigation_IdeaPage_Accessible()
            {
                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");
                // When you click on an Idea
                wait.Until(_driver => _driver.FindElement(By.CssSelector("div.mud-container-maxwidth-lg:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3)")).Enabled);
                IWebElement element = _driver.FindElement(By.CssSelector("div.mud-container-maxwidth-lg:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3)"));
                element.Click();

                // Then the idea appears
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".mb-2")).Enabled);
                bool ideaLoaded = true;
                try
                {
                    element = _driver.FindElement(By.CssSelector(".mb-2"));
                } catch (Exception ex)
                {
                    ideaLoaded = false;
                }
                Assert.AreEqual(ideaLoaded, true);
            }

            [Test]
            public void Click_On_Idea_Opens_That_Idea()
            {
                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");
                // When you click on the first Idea
                wait.Until(_driver => _driver.FindElement(By.CssSelector("div.mud-container-maxwidth-lg:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > h4:nth-child(1)")).Enabled);
                IWebElement element = _driver.FindElement(By.CssSelector("div.mud-container-maxwidth-lg:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > h4:nth-child(1)"));
                String elementText = element.Text;
                element.Click();

                // Then that idea appears
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".mb-2")).Enabled);
                bool ideaLoaded = true;
                try
                {
                    element = _driver.FindElement(By.CssSelector(".mb-2"));
                }
                catch (Exception ex)
                {
                    ideaLoaded = false;
                }
                
                Assert.AreEqual(ideaLoaded && element.Text.Equals(elementText), true);
            }

            [Test]
            public void AnonymousUser_Cannot_Join_Idea()
            {
                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");
                // When you click on the first Idea
                wait.Until(_driver => _driver.FindElement(By.CssSelector("div.mud-container-maxwidth-lg:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > h4:nth-child(1)")).Enabled);
                IWebElement element = _driver.FindElement(By.CssSelector("div.mud-container-maxwidth-lg:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > h4:nth-child(1)"));
                element.Click();

                // Then that idea appears
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".mb-2")).Enabled);
                bool ideaLoaded = true;
                try
                {
                    element = _driver.FindElement(By.CssSelector(".mb-2"));
                    wait.Until(_driver => !(_driver.FindElement(By.CssSelector("button.mud-button-filled:nth-child(1) > span:nth-child(1)")).Displayed));
                    element = _driver.FindElement(By.CssSelector("button.mud-button-filled:nth-child(1) > span:nth-child(1)"));
                }
                catch (Exception ex)
                {
                    ideaLoaded = false;
                }
                Assert.AreEqual(ideaLoaded && element.Text.Equals("JOIN IDEA") && element.Displayed, false);
            }

            [Test]
            public void Anonymous_Cannot_See_Contents()
            {
                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");
                // When you click on the first Idea
                wait.Until(_driver => _driver.FindElement(By.CssSelector("div.mud-container-maxwidth-lg:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > h4:nth-child(1)")).Enabled);
                IWebElement element = _driver.FindElement(By.CssSelector("div.mud-container-maxwidth-lg:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > h4:nth-child(1)"));
                element.Click();

                // Then that idea appears
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".mb-2")).Enabled);
                bool ideaLoaded = true;
                try
                {
                    element = _driver.FindElement(By.CssSelector(".mb-2"));
                    wait.Until(_driver => _driver.FindElement(By.CssSelector("div.mud-grid-item-xs-6:nth-child(2)")).Enabled);
                    element = _driver.FindElement(By.CssSelector("div.mud-grid-item-xs-6:nth-child(2)"));
                }
                catch (Exception ex)
                {
                    ideaLoaded = false;
                }
                Assert.AreEqual(ideaLoaded, false);
            }

            [Test]
            public void Anonymous_Cannot_See_Comments()
            {
                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");
                // When you click on the first Idea
                wait.Until(_driver => _driver.FindElement(By.CssSelector("div.mud-container-maxwidth-lg:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > h4:nth-child(1)")).Enabled);
                IWebElement element = _driver.FindElement(By.CssSelector("div.mud-container-maxwidth-lg:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > h4:nth-child(1)"));
                element.Click();

                // Then that idea appears
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".mb-2")).Enabled);
                bool ideaLoaded = true;
                try
                {
                    element = _driver.FindElement(By.CssSelector(".mb-2"));
                    wait.Until(_driver => _driver.FindElement(By.CssSelector("div.mud-grid-item-xs-6:nth-child(3)")).Enabled);
                    element = _driver.FindElement(By.CssSelector("div.mud-grid-item-xs-6:nth-child(3)"));
                }
                catch (Exception ex)
                {
                    ideaLoaded = false;
                }
                Assert.AreEqual(ideaLoaded, false);
            }

            [Test]
            public void Idea_Click_On_Members_Shows_Tab()
            {
                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");
                // When you click on the first Idea
                wait.Until(_driver => _driver.FindElement(By.CssSelector("div.mud-container-maxwidth-lg:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > h4:nth-child(1)")).Enabled);
                IWebElement element = _driver.FindElement(By.CssSelector("div.mud-container-maxwidth-lg:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > h4:nth-child(1)"));
                element.Click();

                // And that idea appears
                wait.Until(_driver => _driver.FindElement(By.CssSelector(".mb-2")).Enabled);
                
                // And you click on Members
                bool ideaLoaded = true;
                try
                {
                    wait.Until(_driver => !(_driver.FindElement(By.CssSelector("div.mud-grid-item-xs-6:nth-child(3)")).Enabled));
                } catch (WebDriverTimeoutException ex)
                {
                    // Do nothing, all good
                }

                element = _driver.FindElement(By.CssSelector(".mud-tabs-toolbar-wrapper > div:nth-child(2) > div:nth-child(1)"));
                element.Click();
                wait.Until(_driver => _driver.FindElement(By.CssSelector("th.mud-table-cell:nth-child(1) > span:nth-child(1)")).Displayed);
                try
                {
                    element = _driver.FindElement(By.CssSelector("th.mud-table-cell:nth-child(1) > span:nth-child(1)"));
                }
                catch (Exception ex)
                {
                    ideaLoaded = false;
                }
                Assert.AreEqual(ideaLoaded && element.Text.Equals("User Name"), true);
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
