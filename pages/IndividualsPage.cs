using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace selenium_example_if
{
    class IndividualsPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public IndividualsPage(IWebDriver driver, WebDriverWait wait)
        {
            this.driver = driver;
            this.wait = wait;
        }

        private IList<IWebElement> policyMenuItems
            => driver.FindElements(By.XPath("//div[starts-with(@class, 'cta-menu-item')]"));
        private IWebElement buyButton
            => driver.FindElement(By.LinkText("Pirkt"));

        public void Navigate(string startUrl)
        {
            driver.Navigate().GoToUrl(startUrl + "/privatpersonam");
        }

        public void ChoosePolicyType(string policyTypeName)
        {
            foreach (IWebElement el in policyMenuItems)
            {
                if (el.Text == policyTypeName)
                {
                    el.Click();
                }
            }

            buyButton.Click();
        }
    }
}
