using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;

namespace Selenium_Testing
{
    [TestFixture]
    public class OrderService
    {
        private IWebDriver _webDriver;
        private WebDriverWait _webDriverWait;

        [SetUp]
        public void Setup()
        {
            var options = new EdgeOptions();
            options.AddArgument("--headless");
            this._webDriver = new EdgeDriver(options);
            this._webDriverWait = new WebDriverWait(this._webDriver, TimeSpan.FromSeconds(20));
        }

        [Test]
        public void TestAddProductsToCartWithLoggedInUser()
        {
            this._webDriver.Navigate().GoToUrl("http://localhost/cart/products");

            // Wait for the add to cart button to be clickable
            var addButton = _webDriverWait.Until(d =>
            {
                try
                {
                    var element = d.FindElement(By.ClassName("btn--e-brand"));
                    return element.Enabled ? element : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });

            if (addButton != null)
            {
                // Use JavaScript to click the button and trigger the onclick function
                ((IJavaScriptExecutor)_webDriver).ExecuteScript("arguments[0].click();", addButton);

                // Wait for the element indicating the product is added to the cart
                var response = _webDriverWait.Until(d =>
                {
                    try
                    {
                        return d.FindElement(By.ClassName("product-m__add-cart"));
                    }
                    catch (NoSuchElementException)
                    {
                        return null;
                    }
                });

                Assert.IsNotNull(response, "The product was not successfully added to the cart.");
                Console.WriteLine("The product was successfully added to the cart.");
            }
            else
            {
                Assert.Fail("The add to cart button was not found or not clickable.");
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (_webDriver != null)
            {
                _webDriver.Quit();
                _webDriver.Dispose();
            }
        }
    }
}
