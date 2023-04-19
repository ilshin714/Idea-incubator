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
    class Component_Registration
    {
        [TestFixture]
        public class RegistrationTests
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
            public void Navigation_Registration_Appears_From_Button_Click()
            {
                // Setup - wait commands due to graphic loads
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));

                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");
                // When you click on the profile button
                wait.Until(_driver => _driver.FindElement(By.XPath("/html/body/div[3]/header/div/button[2]")).Displayed);
                IWebElement element = _driver.FindElement(By.XPath("/html/body/div[3]/header/div/button[2]"));
                element.Click();
                // And you click on REGISTER button
                wait.Until(_driver => _driver.FindElement(By.CssSelector("button.mud-button-root:nth-child(2)")).Displayed);
                wait.Until(_driver => _driver.FindElement(By.CssSelector("button.mud-button-root:nth-child(2)")).Enabled);
                element = _driver.FindElement(By.CssSelector("button.mud-button-root:nth-child(2)"));
                element.Click();

                // Then the Registration Component Appears
                wait.Until(_driver => _driver.FindElement(By.CssSelector("h6.mud-typography:nth-child(1)")).Enabled);
                element = _driver.FindElement(By.CssSelector("h6.mud-typography:nth-child(1)"));
                Assert.IsTrue(element.Text.Equals("Register Your Account"));
            }

            [Test]
            public void Registration_Error_If_No_Username()
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
                // And you click on REGISTER button
                wait.Until(_driver => _driver.FindElement(By.CssSelector("button.mud-button-root:nth-child(2)")).Enabled);
                element = _driver.FindElement(By.CssSelector("button.mud-button-root:nth-child(2)"));
                element.Click();

                // And the registration component appears
                wait.Until(_driver => _driver.FindElement(By.CssSelector("h6.mud-typography:nth-child(1)")).Displayed);

                // And you enter in a valid email and password
                element = _driver.FindElement(By.CssSelector(".mud-card-content > div:nth-child(2) > div:nth-child(1) > div:nth-child(1) > input:nth-child(1)"));
                element.Click();
                element.SendKeys("bob@bob.com");

                element = _driver.FindElement(By.CssSelector(".mud-card-content > div:nth-child(3) > div:nth-child(1) > div:nth-child(1) > input:nth-child(1)"));
                element.Click();
                element.SendKeys("12345");

                // And you click "Register"
                element = _driver.FindElement(By.CssSelector("button.mud-button-text:nth-child(2)"));
                element.Click();

                // Then an error message appears
                wait.Until(_driver => _driver.FindElement(By.CssSelector("p.mud-input-error > div > div")).Enabled);
                element = _driver.FindElement(By.CssSelector("p.mud-input-error > div > div"));
                Assert.IsTrue(element.Text.Equals("'User Name' must not be empty."));
            }

            [Test]
            public void Registration_Error_If_No_Email()
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
                // And you click on REGISTER button
                wait.Until(_driver => _driver.FindElement(By.CssSelector("button.mud-button-root:nth-child(2)")).Enabled);
                element = _driver.FindElement(By.CssSelector("button.mud-button-root:nth-child(2)"));
                element.Click();

                // And the registration component appears
                wait.Until(_driver => _driver.FindElement(By.CssSelector("h6.mud-typography:nth-child(1)")).Displayed);

                // And you enter in a valid email and password
                element = _driver.FindElement(By.CssSelector("div.mud-input-control:nth-child(1) > div:nth-child(1) > div:nth-child(1) > input:nth-child(1)"));
                element.Click();
                element.SendKeys("TestingAccount");

                element = _driver.FindElement(By.CssSelector(".mud-card-content > div:nth-child(3) > div:nth-child(1) > div:nth-child(1) > input:nth-child(1)"));
                element.Click();
                element.SendKeys("12345");

                // And you click "Register"
                element = _driver.FindElement(By.CssSelector("button.mud-button-text:nth-child(2)"));
                element.Click();

                // Then an error message appears
                wait.Until(_driver => _driver.FindElement(By.CssSelector("p.mud-input-error > div > div")).Enabled);
                element = _driver.FindElement(By.CssSelector("p.mud-input-error > div > div"));
                Assert.IsTrue(element.Text.Equals("'Email Address' must not be empty."));
            }

            [Test]
            public void Registration_Error_If_No_Password()
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
                // And you click on REGISTER button
                wait.Until(_driver => _driver.FindElement(By.CssSelector("button.mud-button-root:nth-child(2)")).Enabled);
                element = _driver.FindElement(By.CssSelector("button.mud-button-root:nth-child(2)"));
                element.Click();

                // And the registration component appears
                wait.Until(_driver => _driver.FindElement(By.CssSelector("h6.mud-typography:nth-child(1)")).Displayed);

                // And you enter in a valid email and password
                element = _driver.FindElement(By.CssSelector("div.mud-input-control:nth-child(1) > div:nth-child(1) > div:nth-child(1) > input:nth-child(1)"));
                element.Click();
                element.SendKeys("TestingAccount");

                element = _driver.FindElement(By.CssSelector(".mud-card-content > div:nth-child(2) > div:nth-child(1) > div:nth-child(1) > input:nth-child(1)"));
                element.Click();
                element.SendKeys("bob@bob.com");

                // And you click "Register"
                element = _driver.FindElement(By.CssSelector("button.mud-button-text:nth-child(2)"));
                element.Click();

                // Then An error message appears
                wait.Until(_driver => _driver.FindElement(By.CssSelector("div.mud-input-error:nth-child(3) > div:nth-child(2) > p:nth-child(1) > div > div")).Enabled);
                element = _driver.FindElement(By.CssSelector("div.mud-input-error:nth-child(3) > div:nth-child(2) > p:nth-child(1) > div > div"));
                Assert.IsTrue(element.Text.Equals("'Password' must not be empty."));
            }

            [Test]
            public void Registration_Cancel_Does_Not_Submit_Form()
            {
                // Setup - wait commands due to graphic loads
                bool isValid = true;
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));

                // Given When on the homepage
                _driver.Navigate()
                    .GoToUrl("https://localhost:7289");
                // When you click on the profile button
                wait.Until(_driver => _driver.FindElement(By.XPath("/html/body/div[3]/header/div/button[2]")).Enabled);
                IWebElement element = _driver.FindElement(By.XPath("/html/body/div[3]/header/div/button[2]"));
                element.Click();
                // And you click on REGISTER button
                wait.Until(_driver => _driver.FindElement(By.CssSelector("button.mud-button-root:nth-child(2)")).Enabled);
                element = _driver.FindElement(By.CssSelector("button.mud-button-root:nth-child(2)"));
                element.Click();

                // And the registration component appears
                wait.Until(_driver => _driver.FindElement(By.CssSelector("h6.mud-typography:nth-child(1)")).Displayed);

                // And you enter in a valid email and password
                element = _driver.FindElement(By.CssSelector("div.mud-input-control:nth-child(1) > div:nth-child(1) > div:nth-child(1) > input:nth-child(1)"));
                element.Click();
                element.SendKeys("TestingAccount");

                element = _driver.FindElement(By.CssSelector(".mud-card-content > div:nth-child(2) > div:nth-child(1) > div:nth-child(1) > input:nth-child(1)"));
                element.Click();
                element.SendKeys("bob@bob.com");

                element = _driver.FindElement(By.CssSelector(".mud-card-content > div:nth-child(3) > div:nth-child(1) > div:nth-child(1) > input:nth-child(1)"));
                element.Click();
                element.SendKeys("12345");

                // And you click "Cancel"
                element = _driver.FindElement(By.CssSelector(".mud-button-text-default"));
                element.Click();

                // Then no message box appears
                try
                {
                    wait.Until(_driver => _driver.FindElement(By.CssSelector(".mud-snackbar-content-message")).Displayed);
                    isValid = false;
                } catch (WebDriverTimeoutException wdte)
                {
                    // Supposed to happen
                }
                Assert.IsTrue(isValid);
            }

            [TearDown]
            public void CloseBrowser()
            {
                _driver.Quit();
            }
        }

    }
}
