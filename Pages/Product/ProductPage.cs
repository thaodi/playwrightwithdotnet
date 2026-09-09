using Microsoft.Playwright;
using PlaywrightTests.Core.Base;
using PlaywrightTests.Core.Config;
using PlaywrightTests.Components.Header;
using static Microsoft.Playwright.Assertions;
using System.Text.RegularExpressions;

namespace PlaywrightTests.Pages.Product
{
    public class ProductPage : BasePage
    {
        public HeaderComponent Header { get; }
        public ProductPage(IPage page) : base(page)
        {
            Header = new HeaderComponent(page);
        }

        private ILocator _shopAllBtn => Page.GetByRole(AriaRole.Link, new() { Name = "Shop All" });
        private ILocator _forkOnGithubBtn => Page.GetByRole(AriaRole.Link, new() { Name = "Fork on GitHub" });
        private ILocator _quickstartGuide => Page.GetByRole(AriaRole.Link, new() { Name = "Quickstart Guide→" });
        private ILocator _viewAllLink => Page.GetByRole(AriaRole.Link, new() { Name = "View all →" });
        private ILocator _allProductLbl => Page.GetByRole(AriaRole.Heading, new() { Name = "All Products" });

        private ILocator _colorBtn => Page.GetByRole(AriaRole.Button, new() { Name = "Color" });
        private ILocator _priceBtn => Page.GetByRole(AriaRole.Button, new() { Name = "Price" });
        private ILocator _availabilityBtn => Page.GetByRole(AriaRole.Button, new() { Name = "Availability" });
        private ILocator _sortBtn => Page.GetByRole(AriaRole.Button, new() { Name = "Sort" });
        private ILocator _productItems => Page.Locator("//div[contains(@class, 'grid')]//div[contains(@class, 'group relative')]");

        public async Task NavigateToProductPageAsync()
        {
            await NavigateToAsync("");
        }

        public async Task ViewAllProduct()
        {
            await ClickAsync(_viewAllLink);
        }

        public async Task VerifyAllProductIsDisplayedAsync(string url, int numberOfProducts)
        {
            await Expect(Page).ToHaveURLAsync($"{ConfigReader.BaseUrl}/{url}");
            await Expect(_allProductLbl).ToBeVisibleAsync();
            await Expect(_colorBtn).ToBeVisibleAsync();
            await Expect(_priceBtn).ToBeVisibleAsync();
            await Expect(_availabilityBtn).ToBeVisibleAsync();
            await Expect(_sortBtn).ToBeVisibleAsync();
            await Expect(_productItems.First).ToBeVisibleAsync();
            await Expect(Page.GetByText($"{numberOfProducts} products")).ToBeVisibleAsync();
        }
        public async Task ClickSortButtonAsync()
        {
            await ClickAsync(_sortBtn);
        }

        public async Task ClickSortOptionAsync(string sortOption)
        {
            var sortOptionLocator = Page.GetByRole(AriaRole.Menuitemradio, new() { Name = sortOption });
            await ClickAsync(sortOptionLocator);
        }

        public async Task<int> GetSortOptionCountAsync()
        {
            var sortOptionItems = Page.GetByRole(AriaRole.Menuitemradio);
            return await sortOptionItems.CountAsync();
        }

        public async Task<List<decimal>> GetAllProductPricesAsync()
        {
            var priceLocators = _productItems.Locator("//span");
            var count = await priceLocators.CountAsync();
            var prices = new List<decimal>();

            for (int i = 0; i < count; i++)
            {
                var priceText = await priceLocators.Nth(i).InnerTextAsync();
                if (decimal.TryParse(priceText.Replace("$", "").Trim(), out decimal price))
                {
                    prices.Add(price);
                }
            }
            return prices;
        }

        public async Task<List<string>> GetAllProductNamesAsync()
        {
            var nameLocators = _productItems.Locator("//h3");
            var count = await nameLocators.CountAsync();
            var names = new List<string>();

            for (int i = 0; i < count; i++)
            {
                var nameText = await nameLocators.Nth(i).InnerTextAsync();
                names.Add(nameText);
            }
            return names;
        }

        public async Task<int> OpenSortAndSelectOptionAsync(string sortOption)
        {
            await NavigateToProductPageAsync();
            await ViewAllProduct();
            await ClickSortButtonAsync();

            var count = await GetSortOptionCountAsync();
            await ClickSortOptionAsync(sortOption);

            return count;
        }

        public async Task ClickFilterColorButtonAsync()
        {
            await ClickAsync(_colorBtn);
        }

        public async Task ClickColorOptionAsync(params string[] colorOption)
        {
            foreach (var color in colorOption)
            {
                var colorOptionLocator = Page.GetByRole(AriaRole.Menuitemcheckbox, new() { Name = color });
                await ClickAsync(colorOptionLocator);
            }
        }

        public async Task<int> GetColorOptionCountAsync()
        {
            var colorOptionItems = Page.GetByRole(AriaRole.Menuitemcheckbox);
            return await colorOptionItems.CountAsync();
        }

        public async Task<int> GetSelectedColorCountAsync()
        {
            var colorQuantityButton = Page.GetByRole(AriaRole.Button).Filter(new() { HasText = "Color" });
            var text = await colorQuantityButton.InnerTextAsync();
            var quantityText = text.Split("\n");
            return int.Parse(quantityText[1]);
        }

        public async Task<List<string>> GetSelectedColorTagTextsAsync(params string[] selectedColors)
        {
            var tagTexts = new List<string>();
            foreach (var color in selectedColors)
            {
                var colorTagLocator = Page.GetByText($"Color: {color}");
                tagTexts.Add(await colorTagLocator.InnerTextAsync());
            }
            return tagTexts;
        }

        public async Task<int> GetProductResultCountAsync()
        {
            return await _productItems.CountAsync();
        }

        public async Task ClickProductItemByIndexAsync(int index)
        {
            var productItemLocator = _productItems.Nth(index);
            await ClickAsync(productItemLocator);
        }

        public async Task ClickProductItemByNameAsync(string nameOfProduct)
        {
            var productItemLocator = _productItems.GetByRole(AriaRole.Link, new() { Name = nameOfProduct }).First;
            int maxScrolls = 20;
            int currentScroll = 0;

            while (currentScroll < maxScrolls)
            {
                if (await productItemLocator.CountAsync() > 0 && await productItemLocator.IsVisibleAsync())
                {
                    break;
                }

                await Page.EvaluateAsync("window.scrollBy(0, 600);");
                await Page.WaitForTimeoutAsync(1000);

                currentScroll++;
            }
            await ClickAsync(productItemLocator);
        }

        public async Task<List<string>> GetAllColorOnDetailProductItemAsync()
        {
            var colorProductItemLocator = Page.Locator("button[title]");
            var count = await colorProductItemLocator.CountAsync();
            var colorProductItemTexts = new List<string>();
            for(int i = 0; i < count; i++)
            {
                var text = await colorProductItemLocator.Nth(i).GetAttributeAsync("title");
                colorProductItemTexts.Add(text.Trim());
            }
            
            return colorProductItemTexts;
        }

        public async Task ClickPriceButtonAsync()
        {
            await ClickAsync(_priceBtn);
        }

        public async Task ClickPriceOptionAsync(string priceOption)
        {
            var priceOptionLocator = Page.GetByRole(AriaRole.Menuitemradio, new() { Name = priceOption });
            await ForceClickAsync(priceOptionLocator);
        }

        public async Task<int> GetPriceOptionCountAsync()
        {
            var priceOptionItems = Page.GetByRole(AriaRole.Menuitemradio);
            return await priceOptionItems.CountAsync();
        }

        public async Task CloseMenuAsync()
        {
            await Page.Keyboard.PressAsync("Escape");
        }

        public async Task<int> GetSelectedPriceCountAsync()
        {
            var priceQuantityButton = Page.GetByRole(AriaRole.Button).Filter(new() { HasText = "Price" });
            var text = await priceQuantityButton.InnerTextAsync();
            var quantityText = text.Split("\n");
            return int.Parse(quantityText[1]);
        }

        public async Task<string> GetSelectedPriceTagTextsAsync(string selectedPriceRanges)
        {
            var priceTagLocator = Page.GetByText($"Price: {selectedPriceRanges}");
            return await priceTagLocator.InnerTextAsync();
        }

        public async Task<decimal> GetPriceOnDetailProductItemAsync()
        {
            var priceProductItemLocator = Page.Locator("h1 + div span").First;
            var priceProductItemText = await priceProductItemLocator.InnerTextAsync();
            var match = Regex.Match(priceProductItemText, @"[\d.]+");
            if(match.Success && decimal.TryParse(match.Value, out decimal price))
            {
                return price;
            }
            return 0;
        }

        #region Availability Filter
        public async Task ClickAvailabilityButtonAsync()
        {
            await ClickAsync(_availabilityBtn);
        }
        public async Task<int> GetAvailabilityOptionCountAsync()
        {
            var availabilityOptionItems = Page.GetByRole(AriaRole.Menuitemradio);
            return await availabilityOptionItems.CountAsync();
        }

        public async Task ClickAvailabilityOptionAsync(string availabilityOption)
        {
            var availabilityOptionLocator = Page.GetByRole(AriaRole.Menuitemradio, new() { Name = availabilityOption });
            await ClickAsync(availabilityOptionLocator);
        }

        public async Task<int> GetQuantityOfProductInSelectedAvailabilityOptionTextAsync(string availabilityOption)
        {
            var availabilityOptionLocator = Page.GetByRole(AriaRole.Menuitemradio, new() { Name = availabilityOption });
            var quantityText = await availabilityOptionLocator.InnerTextAsync();
            var match = Regex.Match(quantityText, @"[\d]+");
            if(match.Success && int.TryParse(match.Value, out int quantity))
            {
                return quantity;
            }
            return 0;
        }

        public async Task<int> GetSelectedAvailabilityCountAsync()
        {
            var availabilityQuantityButton = Page.GetByRole(AriaRole.Button).Filter(new() { HasText = "Availability" });
            var text = await availabilityQuantityButton.InnerTextAsync();
            var quantityText = text.Split("\n");
            return int.Parse(quantityText[1]);
        }

        public async Task<string> GetSelectedAvailabilityTagTextsAsync(string selectedAvailability)
        {
            var availabilityTagLocator = Page.GetByText(selectedAvailability).First;
            return await availabilityTagLocator.InnerTextAsync();
        }

        public async Task<string> GetAvailabilityOnDetailProductItemAsync()
        {
            var availabilityProductItemLocator = Page.Locator("h1 + div + div span").First;
            var availabilityProductItemText = await availabilityProductItemLocator.InnerTextAsync();
            return availabilityProductItemText.Trim();
        }
        #endregion

        public async Task VerifyProductDetailDisplayedCorrectly(string expectedProductName, decimal expectedProductPrice, string expectedAvailability, string[] expectedColors, string expectedDescription, Dictionary<string, string> expectedProperties)
        {
            var expectedSlug = expectedProductName.ToLower().Replace(" ", "-");
            await Expect(Page).ToHaveURLAsync($"{ConfigReader.BaseUrl}/products/{expectedSlug}");

            //Verify product name, price, and availability
            var productName = await Page.GetByRole(AriaRole.Heading, new() { Name = $"{expectedProductName}" }).First.InnerTextAsync();
            Assert.That(productName, Is.EqualTo(expectedProductName), $"Expected product name: {expectedProductName}, but got: {productName}");

            var productPrice = await GetPriceOnDetailProductItemAsync();    
            Assert.That(productPrice, Is.EqualTo(expectedProductPrice), $"Expected price: {expectedProductPrice}, but got: {productPrice}");

            var actualAvailability = await GetAvailabilityOnDetailProductItemAsync();
            Assert.That(actualAvailability, Is.EqualTo(expectedAvailability), $"Expected availability: {expectedAvailability}, but got: {actualAvailability}");

            var colorOnDetailProduct = await GetAllColorOnDetailProductItemAsync();
            Assert.That(colorOnDetailProduct, Is.EquivalentTo(expectedColors), $"Expected colors: {string.Join(", ", expectedColors)}, but got: {string.Join(", ", colorOnDetailProduct)}");

            var descriptionLocator = Page.Locator("h2 + div").First;
            var actualDescription = await descriptionLocator.InnerTextAsync();
            Assert.That(actualDescription, Does.Contain(expectedDescription), $"Expected description: {expectedDescription}, but got: {actualDescription}");

            foreach (var prop in expectedProperties)
            {
                var propKeyLocator = Page.Locator($"text={prop.Key}").First;
                var propValueLocator = Page.Locator($"text={prop.Value}").First;

                await propKeyLocator.ScrollIntoViewIfNeededAsync();
                await Expect(propKeyLocator).ToBeVisibleAsync();
                await Expect(propValueLocator).ToBeVisibleAsync();
            }
        }

        public async Task SelectColorOnProductDetailAsync(string color)
        {
            var colorProductItemLocator = Page.Locator($"button[title='{color}']");
            await ClickAsync(colorProductItemLocator);
        }
    }
}