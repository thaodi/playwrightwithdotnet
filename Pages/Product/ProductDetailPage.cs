using Microsoft.Playwright;
using PlaywrightTests.Core.Base;
using PlaywrightTests.Core.Config;
using System.Text.RegularExpressions;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests.Pages.Product
{
    public class ProductDetailPage : BasePage
    {
        public ProductDetailPage(IPage page) : base(page)
        {
        }

        public async Task<List<string>> GetAllColorOnDetailProductItemAsync()
        {
            var colorProductItemLocator = Page.Locator("button[title]");
            var count = await colorProductItemLocator.CountAsync();
            var colorProductItemTexts = new List<string>();
            for (int i = 0; i < count; i++)
            {
                var text = await colorProductItemLocator.Nth(i).GetAttributeAsync("title");
                colorProductItemTexts.Add(text.Trim());
            }

            return colorProductItemTexts;
        }

        public async Task<decimal> GetPriceOnDetailProductItemAsync()
        {
            var priceProductItemLocator = Page.Locator("h1 + div span").First;
            var priceProductItemText = await priceProductItemLocator.InnerTextAsync();
            var match = Regex.Match(priceProductItemText, @"[\d.]+");
            if (match.Success && decimal.TryParse(match.Value, out decimal price))
            {
                return price;
            }
            return 0;
        }

        public async Task<string> GetDescriptionOnDetailProductItemAsync()
        {
            var descriptionLocator = Page.Locator("h2 + div").First;
            var descriptionText = await descriptionLocator.InnerTextAsync();
            return descriptionText.Trim();
        }

        public async Task VerifyProductDetailDisplayedCorrectly(string expectedProductName, decimal expectedProductPrice, string expectedAvailability, string[] expectedColors, string expectedDescription, Dictionary<string, string> expectedProperties)
        {
            var expectedSlug = expectedProductName.ToLower().Replace(" ", "-");
            await Expect(Page).ToHaveURLAsync($"{ConfigReader.BaseUrl}/products/{expectedSlug}");

            //Verify product name, price, and availability
            var productName = await Page.GetByRole(AriaRole.Heading, new() { Name = $"{expectedProductName}" }).First.InnerTextAsync();
            Assert.That(productName, Is.EqualTo(expectedProductName), $"Expected product name: {expectedProductName}, but got: {productName}");

            var productPrice = await GetPriceOnDetailProductItemAsync();
            Assert.That(productPrice, Is.EqualTo(expectedProductPrice), $"Expected price: {expectedProductPrice}, but got: {productPrice}");

            var productAvailability = await GetAvailabilityOnDetailProductItemAsync();
            Assert.That(productAvailability, Is.EqualTo(expectedAvailability), $"Expected availability: {expectedAvailability}, but got: {productAvailability}");

            var productColor = await GetAllColorOnDetailProductItemAsync();
            Assert.That(productColor, Is.EquivalentTo(expectedColors), $"Expected colors: {string.Join(", ", expectedColors)}, but got: {string.Join(", ", productColor)}");

            var productDescription = await GetDescriptionOnDetailProductItemAsync();
            Assert.That(productDescription, Does.Contain(expectedDescription), $"Expected description: {expectedDescription}, but got: {productDescription}");

            foreach (var prop in expectedProperties)
            {
                var propKeyLocator = Page.Locator($"text={prop.Key}").First;
                var propValueLocator = Page.Locator($"text={prop.Value}").First;

                await propKeyLocator.ScrollIntoViewIfNeededAsync();
                await Expect(propKeyLocator).ToBeVisibleAsync();
                await Expect(propValueLocator).ToBeVisibleAsync();
            }
        }

        public async Task SelectColorOnDetailProductItemAsync(string color)
        {
            var colorProductItemLocator = Page.Locator($"button[title='{color}']");
            await ClickAsync(colorProductItemLocator);
        }

        public async Task<string> GetAvailabilityOnDetailProductItemAsync()
        {
            var availabilityProductItemLocator = Page.Locator("h1 + div + div span").First;
            var availabilityProductItemText = await availabilityProductItemLocator.InnerTextAsync();
            return availabilityProductItemText.Trim();
        }
    }
}
