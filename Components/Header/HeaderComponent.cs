using Microsoft.Playwright;
using PlaywrightTests.Core.Base;
namespace PlaywrightTests.Components.Header
{
	public class HeaderComponent : BasePage
	{
		public HeaderComponent(IPage page) : base(page) { }

		private ILocator _btnHamburger => Page.GetByRole(AriaRole.Button, new() { Name = "Open menu" });
		private ILocator _logoImage => Page.GetByRole(AriaRole.Img, new() { Name = "Spree" });
		private ILocator _wholeSaleLink => Page.GetByRole(AriaRole.Link, new() { Name = "Wholesale" });
		private ILocator _btnLanguage => Page.GetByRole(AriaRole.Button, new() { Name = "Region and language" });
		private ILocator _btnSearch => Page.GetByRole(AriaRole.Button, new() { Name = "Open search" });
		private ILocator _accountLink => Page.GetByRole(AriaRole.Link, new() { Name = "Account" });
		private ILocator _btnCart => Page.GetByRole(AriaRole.Button, new() { Name = "Open cart" });

        //Search
        private ILocator _searchInput => Page.GetByRole(AriaRole.Combobox, new () { Name = "Search..." });
		private ILocator _noProductsFoundMessage => Page.GetByText("No products found");
		private ILocator _noProductsFoundLbl => Page.GetByRole(AriaRole.Heading, new() { Name = "No products found" });

        //Region
        private ILocator _regionDropdown => Page.GetByRole(AriaRole.Combobox, new() { Name = "Region" });
		private ILocator _languageDropdown => Page.GetByRole(AriaRole.Combobox, new() { Name = "Language" });
		private ILocator _updatePreferencesButton => Page.GetByRole(AriaRole.Button, new() { Name = "Update preferences" });
		private ILocator _closeDialogLanguageButton => Page.GetByRole(AriaRole.Button, new() { Name = "Close" });
		private ILocator _dialogRegionAndLanguage => Page.GetByRole(AriaRole.Dialog, new() { Name = "Region and language" });

		public async Task OpenMenuAsync()
		{
			await ClickAsync(_btnHamburger);
		}

		public async Task SelectMenuItemAsync(string itemName) { 
			await OpenMenuAsync();
			await Page.GetByRole(AriaRole.Link, new() { Name = itemName }).ClickAsync();
		}

		public async Task SelectSubMenuItemAsync(string mainItemName, string subItemName)
		{
			await OpenMenuAsync();
			await Page.GetByRole(AriaRole.Link, new() { Name = mainItemName }).ClickAsync();
			await Page.GetByRole(AriaRole.Link, new() { Name = subItemName }).ClickAsync();
		}

		public async Task ClickLogoAsync()
		{
			await ClickAsync(_logoImage);
		}

		public async Task ClickWholeSaleLinkAsync()
		{
			await ClickAsync(_wholeSaleLink);
		}

		public async Task OpenRegionAndLanguageDialogAsync()
		{
			await ClickAsync(_btnLanguage);
		}

		public async Task<bool> IsDialogRegionAndLanguageVisible()
		{
			return await IsVisibleAsync(_dialogRegionAndLanguage);
		}

		public async Task<string> ChangeRegionAndGetAutoLanguageAsync(string regionName)
		{
			await OpenRegionAndLanguageDialogAsync();
			await _regionDropdown.SelectOptionAsync(regionName);
			var autoSelectedLanguage = await _languageDropdown.InputValueAsync();

			await ClickAsync(_updatePreferencesButton);
			return autoSelectedLanguage;
		}

		public async Task SearchProductAsync(string keyword)
		{
            await ClickAsync(_btnSearch);
            await FillAsync(_searchInput, keyword);
        }

		public async Task<int> GetSuggestionCountAsync(string keyword)
		{
			var suggestionItems = Page.GetByRole(AriaRole.Option).Filter(new() { HasText = keyword });
            return await suggestionItems.CountAsync();
		}

		public async Task<string> GetSuggestionItemTextAsync(int index, string keyword)
		{
            var suggestionItems = Page.GetByRole(AriaRole.Option).Filter(new() { HasText = keyword });
			return await GetTextAsync(suggestionItems.Nth(index));
        }

        public async Task ClickViewAllResultsOfProductSearchAsync(string keyword)
        {
            var viewAllResultBtn = Page.GetByRole(AriaRole.Button, new() { Name = $"View all results for \"{keyword}\"" });
            await viewAllResultBtn.ClickAsync();
        }

		public async Task<ILocator> GetNoProductsFoundMessageAsync()
		{
            await _noProductsFoundMessage.WaitForAsync();
			return _noProductsFoundMessage;
        }

        public ILocator GetSearchResultsHeading(string keyword)
		{
			return Page.GetByRole(AriaRole.Heading, new() { Name = $"Search results for \"{keyword}\"" });
        }

		public ILocator GetNoProductsFoundLabelAsync()
        {
            return _noProductsFoundLbl;
        }
        public async Task PressEnterButtonAsync()
		{
			await PressAsync(_searchInput, "Enter");
		}

        public async Task ClickAccountLinkAsync()
		{
			await ClickAsync(_accountLink);
		}

		public async Task ClickCartButtonAsync()
		{
			await ClickAsync(_btnCart);
		}
	}
}