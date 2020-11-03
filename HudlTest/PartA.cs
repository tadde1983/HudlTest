using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace HudlTest
{
    [TestClass]
    public class PartA
    { 

        protected IWebDriver driver;
        protected WebDriverWait waiter;
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void CreateDriver()
        {
            ChromeOptions chromeoptions = new ChromeOptions();
            chromeoptions.AddArguments("--disable-extensions");
            chromeoptions.AddArguments("--disable-gpu");
            chromeoptions.AddArguments("--disable-infobars");
            chromeoptions.AddArgument("--start-maximized");
            chromeoptions.AddAdditionalCapability("useAutomationExtension", true);
            chromeoptions.AddArguments("--no-sandbox");
            driver = new ChromeDriver(chromeoptions);
            waiter = new WebDriverWait(driver, new TimeSpan(0, 0, 5));
        }

        [TestCleanup]
        public void QuitDriver()
        {
            if (driver != null)
            {
                try
                {
                    driver.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " --- " + " ERROR Closing BROWSER...");
                }
                try
                {
                    driver.Quit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " --- " + " ERROR Quitting BROWSER...");
                }
                try
                {
                    driver.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " --- " + " ERROR Disposing BROWSER...");
                }
                finally
                {
                    driver = null;
                }
            }
        }

        [TestMethod]
        ///<summary>
        /// Verify if a user will be able to login with a valid username and valid password.
        ///</summary>
        public void LoginWithValidCredentials()
        {
            //Go To Login Page hudl.com
            driver.Navigate().GoToUrl("https://www.hudl.com/login");

            //Execute Login Process
            DoLogin("tadde1983@gmail.com", "TestHudl1");
            
            //Waiting for the Avatar element is Displayed (timeout 5s)
            waiter.Until(d => d.FindElements(By.ClassName("uni-avatar__initials")).Count(e => e.Displayed) > 0);

            //Check if the Avatar Initials are correctly ST
            Assert.IsTrue(driver.FindElement(By.ClassName("uni-avatar__initials")).Text.Equals("ST"), "LogIn - Something went wrong during the login process, 'ST' initials are not correctly displayed");

        }

        [TestMethod]
        ///<summary>
        /// Verify if a user cannot login with a valid username and an invalid password.
        ///</summary>
        public void LoginWithInvalidPasswordCredentials()
        {
            //Go To Login Page hudl.com
            driver.Navigate().GoToUrl("https://www.hudl.com/login");

            DoLogin("tadde1983@gmail.com", "TestHudl");


            waiter.Until(d => d.FindElements(By.ClassName("login-error-container")).Count(e => e.Displayed) > 0);

            Assert.IsTrue(driver.FindElement(By.ClassName("login-error-container")).FindElement(By.TagName("p")).Text.StartsWith("We didn't recognize that email and/or password."), " No error Message on Wrong Login credentials");
        }

        [TestMethod]
        ///<summary>
        /// Verify the login page for both, when the field is blank and Login button is clicked.
        ///</summary>
        public void LoginWithNoCredentials()
        {
            //Go To Login Page hudl.com
            driver.Navigate().GoToUrl("https://www.hudl.com/login");

            DoLogin("", "");

            waiter.Until(d => d.FindElements(By.ClassName("login-error-container")).Count(e => e.Displayed) > 0);

            Assert.IsTrue(driver.FindElement(By.ClassName("login-error-container")).FindElement(By.TagName("p")).Text.StartsWith("We didn't recognize that email and/or password."), " No error Message on Wrong Login credentials");
        }

        [TestMethod]
        ///<summary>
        /// Verify the login page when the field password is blank and Login button is clicked.
        ///</summary>
        public void LoginWithNoPassword()
        {
            //Go To Login Page hudl.com
            driver.Navigate().GoToUrl("https://www.hudl.com/login");

            DoLogin("tadde1983@gmail.com", "");

            waiter.Until(d => d.FindElements(By.ClassName("login-error-container")).Count(e => e.Displayed) > 0);

            Assert.IsTrue(driver.FindElement(By.ClassName("login-error-container")).FindElement(By.TagName("p")).Text.StartsWith("We didn't recognize that email and/or password."), " No error Message on Wrong Login credentials");
        }


        private void DoLogin(string username, string password)
        {
            //Waiting for textbox (ID=email) is ready (visible and interactable) in the DOM of the page
            waiter.Until(d => d.FindElements(By.Id("email")).Count(e => e.Displayed) > 0);

            //Clear All Fields
            driver.FindElement(By.Id("email")).Clear();
            driver.FindElement(By.Id("password")).Clear();

            //Write the credentials
            driver.FindElement(By.Id("email")).SendKeys(username);
            driver.FindElement(By.Id("password")).SendKeys(password);


            //Security Check On Password
            if (password.Length > 0)
            {
                var text=((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].value;", driver.FindElement(By.Id("password"))).ToString();
                Assert.IsFalse(text.Equals(driver.FindElement(By.Id("password")).Text), "PASSWORD IS NOT MASQUERADE!");
            }
            //Click on the login button
            driver.FindElement(By.Id("logIn")).Click();
        }
    }
}
