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

        #region Locators
        private ILocator _allProductLbl => Page.GetByRole(AriaRole.Heading, new() { Name = "All Products" });

        private ILocator _colorBtn => Page.GetByRole(AriaRole.Button, new() { Name = "Color" });
        private ILocator _priceBtn => Page.GetByRole(AriaRole.Button, new() { Name = "Price" });
        private ILocator _availabilityBtn => Page.GetByRole(AriaRole.Button, new() { Name = "Availability" });
        private ILocator _sortBtn => Page.GetByRole(AriaRole.Button, new() { Name = "Sort" });
        private ILocator _productItems => Page.Locator("//div[contains(@class, 'grid')]//div[contains(@class, 'group relative')]");
        #endregion

        public async Task NavigateToProductPageAsync()
        {
            await NavigateToAsync("/products");
        }

        public async Task VerifyAllProductIsDisplayedAsync(string url, int numberOfProducts)
        {
            await Expect(Page).ToHaveURLAsync($"{ConfigReader.BaseUrl}/{url}");
            await Expect(_allProductLbl).ToBeVisibleAsync();
            await Expect(_colorBtn).ToBeVisibleAsync();
            await Expect(_priceBtn).ToBeVisibleAsync();
            await Expect(_availabilityBtn).ToBeVisibleAsync();
            await Expect(_sortBtn).ToBeVisibleAsync();
            await Expect(Page.GetByText($"{numberOfProducts} products")).ToBeVisibleAsync();
            await Expect(_productItems.First).ToBeVisibleAsync();
        }

        public async Task<bool> IsProductImageVisibleByIndexAsync(int index)
        {
            return await _productItems.Locator("//img").Nth(index).IsVisibleAsync();
        }

        public async Task<string> GetProductNameByIndexAsync(int index)
        {
            return await _productItems.Locator("//h3").Nth(index).InnerTextAsync();
        }

        public async Task<string> GetProductPriceByIndexAsync(int index)
        {
            return await _productItems.Locator("//span").Nth(index).InnerTextAsync();
        }

        #region Sort Filter
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
            await ClickSortButtonAsync();

            var count = await GetSortOptionCountAsync();
            await ClickSortOptionAsync(sortOption);

            return count;
        }
        #endregion
        #region Color Filter

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
        #endregion

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

        #region Price Filter
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
        #endregion

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

        #endregion
    }
}