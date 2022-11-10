using OpenQA.Selenium.Chrome;
using System;
using NUnit;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Threading;
using System.Collections.Generic;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;

namespace selenium_example_if
{
    public class TravelPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public TravelPage(IWebDriver driver, WebDriverWait wait)
        {
            this.driver = driver;
            this.wait = wait;
        }

        private IList<IWebElement> destinationMenuEls
            => driver.FindElements(By.CssSelector(".destination.btn.btn-default"));
        private IList<IWebElement> dateInputEls
            => driver.FindElements(By.CssSelector(".form-control.hasDatepicker"));
        private IList<IWebElement> optionsEls
            => driver.FindElements(By.CssSelector(".col-xs-12.col-sm-5.col-lg-4"));
        private IWebElement priceDisplay
            => driver.FindElement(By.ClassName("hk-premium"));
        private IList<IWebElement> optionsDropdowns
            => driver.FindElements(By.ClassName("dropdown-menu"));

        //would like to avoid using xpath like this, given more time there is a better solution probably

        private IWebElement numberOfChildInput
            => driver.FindElement(By.XPath("//*[@id='travelTravellersStep']/div[2]/div[1]/div[2]/div[1]/div[2]/div/input"));
        private IWebElement numberOfAdultInput
            => driver.FindElement(By.XPath("//*[@id='travelTravellersStep']/div[2]/div[1]/div[2]/div[4]/div[2]/div/input"));
        private IWebElement calculatePriceButton
            => driver.FindElement(By.XPath("//*[@id='travelTravellersStep']/div[2]/div[1]/div[2]/div[7]/ul/li/input"));
        private IWebElement liablityAndLegalButton
            => driver.FindElement(By.XPath("//*[@id='travelCoverageStep']/div[2]/div[1]/div[2]/div[2]/div[1]/div[1]/span"));
        private IWebElement accidentsButton
            => driver.FindElement(By.XPath("//*[@id='travelCoverageStep']/div[2]/div[1]/div[2]/div[2]/div[3]/div[1]/span"));
        private IWebElement proceedButton
            => driver.FindElement(By.XPath("//*[@id='travelInformationStep']/div[2]/div[1]/div[2]/div[2]/ul/li/input"));


        public void SelectDestination(string destinationName)
        {
            WaitOverlayFade();

            foreach (IWebElement el in destinationMenuEls)
            {
                if (el.Text == destinationName)
                {
                    IJavaScriptExecutor ex = (IJavaScriptExecutor)driver;
                    ex.ExecuteScript("arguments[0].click();", el);//workaround for normal click not working with chrome for this button
                }
            }
        }

        public void AddToAndFromDates(string startDate, string endDate)
        {
            WaitOverlayFade();

            dateInputEls[0].Click();
            dateInputEls[0].SendKeys(startDate);

            dateInputEls[1].Click();
            dateInputEls[1].Clear();

            dateInputEls[1].SendKeys(endDate.Remove(2, 8));//workaround for weird sendkeys behavior with firefox for this input
            dateInputEls[1].SendKeys(endDate.Remove(0, 2));

            WaitOverlayFade();

            proceedButton.Click();
        }

        public void AddTravelers()
        {
            WaitOverlayFade();

            numberOfChildInput.Clear();
            numberOfChildInput.SendKeys("2");

            numberOfAdultInput.Clear();
            numberOfAdultInput.SendKeys("2");

            calculatePriceButton.Click();
        }

        public void WaitOverlayFade()
        {
            wait.Until(d => d.FindElements(By.CssSelector(".loading-background-overlay.hk-fade")).Count == 0);
        }

        public string GetFinalPrice()
        {
            return driver.FindElement(By.ClassName("main-number")).Text + "." + driver.FindElement(By.ClassName("leftover-number")).Text;
        }

        public void ChooseCoverage(string medicalAmmount, string baggageAmount, string changesAmount)
        {
            WaitOverlayFade();

            optionsEls[0].Click();
            optionsDropdowns[0].FindElement(By.PartialLinkText(medicalAmmount)).Click();

            WaitOverlayFade();

            optionsEls[1].Click();
            optionsDropdowns[1].FindElement(By.PartialLinkText(baggageAmount)).Click();

            WaitOverlayFade();

            optionsEls[2].Click();
            optionsDropdowns[2].FindElement(By.PartialLinkText(changesAmount)).Click();

            WaitOverlayFade();

            liablityAndLegalButton.Click();

            WaitOverlayFade();

            accidentsButton.Click();

            WaitOverlayFade();

            IJavaScriptExecutor ex = (IJavaScriptExecutor)driver;
            ex.ExecuteScript("arguments[0].scrollIntoView({block: 'center', inline: 'nearest'})", priceDisplay);
        }
    }
}
