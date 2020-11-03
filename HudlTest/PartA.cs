using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HudlTest
{
    [TestClass]
    public class PartA
    { 

        IWebDriver driver;
        WebDriverWait wait;
        ChromeOptions chromeoptions;
        public TestContext TestContext { get; set; }

        [ClassInitialize]

        public static void ClassInit(TestContext context)
        {
            //Delete Temp browser directory
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + "tempdata"))
                Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + "tempdata", true);
        }

        [TestInitialize]
        public void CreateDriver()
        {
            chromeoptions = new ChromeOptions();
            chromeoptions.AddArguments("--disable-extensions");
            chromeoptions.AddArguments("--disable-gpu");
            chromeoptions.AddArguments("--disable-infobars");
            chromeoptions.AddArgument("--start-maximized");
            chromeoptions.AddAdditionalCapability("useAutomationExtension", true);
            chromeoptions.AddArguments("--user-data-dir=" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + "tempdata");
            chromeoptions.AddArguments("--no-sandbox");
            driver = new ChromeDriver(chromeoptions);
            wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));
            driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 10);
            driver.Manage().Timeouts().PageLoad = new TimeSpan(0, 0, 10);
        }

        [TestCleanup]
        public void QuitDriver()
        {
            
            DisposeDriver();

            ClearTempdir();
        }

        private void DisposeDriver()
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

        private void ClearTempdir()
        {
           
            Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + "tempdata",true);
        }

        [TestMethod]
        ///<summary>
        /// Verify if a user will be able to login with a valid username and valid password.
        ///</summary>
        public void LoginCorrectCredentials()
        {
            //Execute Login Process
            DoLogin("tadde1983@gmail.com", "TestHudl1");

            //waiting for the avatar element
            wait.Until(w => w.FindElements(By.ClassName("uni-avatar__initials")).Count(e=>e.Displayed)> 0);

            //Check if the Avatar Initials are correctly ST
            Assert.IsTrue(driver.FindElement(By.ClassName("uni-avatar__initials")).Text.Equals("ST"), "LogIn - Something went wrong during the login process, 'ST' initials are not correctly displayed");

        }


        [TestMethod]
        ///<summary>
        /// Verify if a user cannot login with a valid username and an invalid password.
        ///</summary>
        public void LoginInvalidPasswordCredentials()
        {
            //Execute Login Process
            DoLogin("tadde1983@gmail.com", "TestHudl");

            //waiting for the error message
            wait.Until(w => w.FindElements(By.XPath("//div[@class='login-error-container']/p")).Count(e => e.Displayed && e.Text.Length>0) > 0);

            //Check Login Error Message
            Assert.IsTrue(driver.FindElement(By.XPath("//div[@class='login-error-container']/p")).Text.StartsWith("We didn't recognize that email and/or password."), " No error Message on Wrong Login credentials");
        }

        [TestMethod]
        ///<summary>
        /// Verify the login page for both, when the field is blank and Login button is clicked.
        ///</summary>
        public void LoginNoCredentials()
        {
            //Execute Login Process
            DoLogin("", "");

            //waiting for the error message
            wait.Until(w => w.FindElements(By.XPath("//div[@class='login-error-container']/p")).Count(e => e.Displayed && e.Text.Length > 0) > 0);

            //Check Login Error Message
            Assert.IsTrue(driver.FindElement(By.XPath("//div[@class='login-error-container']/p")).Text.StartsWith("We didn't recognize that email and/or password."), " No error Message on Wrong Login credentials");
        }

        [TestMethod]
        ///<summary>
        /// Verify the login page when the field password is blank and Login button is clicked.
        ///</summary>
        public void LoginNoPassword()
        {
            //Execute Login Process
            DoLogin("tadde1983@gmail.com", "");

            //waiting for the error message
            wait.Until(w => w.FindElements(By.XPath("//div[@class='login-error-container']/p")).Count(e => e.Displayed && e.Text.Length > 0) > 0);

            //Check Login Error Message
            Assert.IsTrue(driver.FindElement(By.XPath("//div[@class='login-error-container']/p")).Text.StartsWith("We didn't recognize that email and/or password."), " No error Message on Wrong Login credentials");
        }

        [TestMethod]
        ///<summary>
        /// Verify the login page when the field password is blank and Login button is clicked.
        ///</summary>
        public void LoginVerifyRememberMeCredentials()
        {
            //Execute Login Process
            DoLogin("tadde1983@gmail.com", "TestHudl1" ,true);

            //Check if the Avatar Initials are correctly ST
            Assert.IsTrue(driver.FindElement(By.ClassName("uni-avatar__initials")).Text.Equals("ST"), "LogIn - Something went wrong during the login process, 'ST' initials are not correctly displayed");

            DisposeDriver();

            CreateDriver();

            GoToHudlPage();

            //Check if the Avatar Initials are correctly ST
            Assert.IsTrue(driver.FindElement(By.ClassName("uni-avatar__initials")).Text.Equals("ST"), "LogIn 'Remember me' functionality is not working properly");

        }

        private void GoToHudlPage()
        {
            driver.Navigate().GoToUrl("https://www.hudl.com");
            
        }

        public void GoToLoginPage()
        {
            driver.FindElements(By.TagName("a")).Where(e => e.Displayed && e.Text.Equals("Log in")).First().Click();
        }

        private void DoLogin(string username, string password, bool RememberCredentials=false)
        {
            //Go To Hudl Home Page
            GoToHudlPage();

            //Go To Hudl Login Page
            GoToLoginPage();

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

            if (RememberCredentials)
            {
               
                ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].click();", driver.FindElement(By.Id("remember-me")));
            }
            //Click on the login button
            driver.FindElement(By.Id("logIn")).Click();

            
        }
    }
}
