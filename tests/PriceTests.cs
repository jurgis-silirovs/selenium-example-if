using OpenQA.Selenium.Chrome;
using System;
using NUnit;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.IO;
using System.Collections.Generic;

[assembly: Parallelizable(ParallelScope.Fixtures)]

namespace selenium_example_if
{
    //this is just a quick way to run same test on multiple browsers(from google), there are probably better ways to do it

    [TestFixture(typeof(EdgeDriver))]
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(FirefoxDriver))]

    public class PriceTests<TWebDriver> where TWebDriver : IWebDriver, new()
    {
        public IWebDriver driver;
        public WebDriverWait wait;

        [SetUp]
        public void SetUp()
        {
            try
            {
                InitDriver();
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception("Browser is not installed! " + ex);
            }

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }

        private void InitDriver()
        {
            switch (typeof(TWebDriver).Name)
            {
                case ("ChromeDriver"):
                    ChromeOptions options = new ChromeOptions();
                    options.AddArguments("--headless");
                    driver = new ChromeDriver(options);
                    break;
                case ("EdgeDriver"):
                    EdgeOptions edgeOptions = new EdgeOptions();
                    edgeOptions.AddArguments("--headless");
                    driver = new EdgeDriver(edgeOptions);
                    break;
                case ("FirefoxDriver"):
                    FirefoxOptions firefoxOptions = new FirefoxOptions();
                    firefoxOptions.AddArguments("--headless");
                    driver = new FirefoxDriver(firefoxOptions);
                    break;
                default:
                    break;
            }
        }

        [TearDown]
        public void TearDown()
        {
            SaveScreenshot();

            driver.Quit();
            driver.Dispose();
        }

        [Test]
        public void ShouldGetPrice()
        {
            //pageobject simple example,
            //normally would not write it exactly like this unless actually re-using in multiple tests
            //to further improve it in a similar way, inputs, expected outputs could come from testcase attributes

            var IndividualsPage = new IndividualsPage(driver, wait);
            var TravelPage = new TravelPage(driver, wait);

            var startUrl = "https://if.lv";

            var coverageInput = new Dictionary<string, string>(){
                {"MedicalCoverage", "300 000"},
                {"BaggageCoverage", "600"},
                {"ChangeCoverage", "Nevēlos"},
                {"Price", "28.24 €" }
            };

            var startDate = DateTime.Today.AddDays(1).ToString("dd.MM.yyyy");
            var endDate = DateTime.Today.AddDays(4).ToString("dd.MM.yyyy");

            IndividualsPage.Navigate(startUrl);
            IndividualsPage.ChoosePolicyType("Ceļojumam");

            TravelPage.SelectDestination("Eiropa");
            TravelPage.AddToAndFromDates(startDate, endDate);
            TravelPage.AddTravelers();
            TravelPage.ChooseCoverage(coverageInput["MedicalCoverage"], coverageInput["BaggageCoverage"], coverageInput["ChangeCoverage"]);

            Assert.AreEqual(coverageInput["Price"], TravelPage.GetFinalPrice());
        }

        public void SaveScreenshot()
        {
            var browserName = driver.GetType().ToString().Remove(0, 16);
            var dir = Directory.CreateDirectory(Environment.CurrentDirectory + "\\Screenshots");

            string path = Path.Combine(Environment.CurrentDirectory, @"Screenshots\", browserName + ".png");

            ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(path);

            Console.WriteLine("Screenshots saved in: " + dir);
        }
    }
}
