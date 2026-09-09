using PlaywrightTests.Utilities;
using PlaywrightTests.Core.Config;
using Microsoft.Playwright;

namespace PlaywrightTests.Core.Base
{
    public abstract class BasePage
    {
        protected static IPage Page;

        protected BasePage(IPage page)
        {
            Page = page;
        }

        public async Task NavigateToAsync(string path = "")
        {
            var url = path.StartsWith("http") ? path : $"{ConfigReader.BaseUrl}{path}";

            LoggerHelper.Info($"Navigating to: {url}");
            await Page.GotoAsync(url);
        }

        public async Task ForceClickAsync(ILocator locator)
        {
            LoggerHelper.Info($"Clicking on element: {locator}");
            await locator.ClickAsync(new() { Force = true });
        }

        public async Task ClickAsync(ILocator locator)
        {
            LoggerHelper.Info($"Clicking on element: {locator}");
            await locator.ClickAsync();
        }

        public async Task CheckAsync(ILocator locator)
        {
            LoggerHelper.Info($"Checking checkbox: {locator}");
            await locator.CheckAsync();
        }

        public async Task FillAsync(ILocator locator, string text)
        {
            LoggerHelper.Info($"Filling text into: {locator}");
            await locator.FillAsync(text);
        }

        public async Task PressAsync(ILocator locator, string key)
        {
            LoggerHelper.Info($"Pressing key '{key}' on: {locator}");
            await locator.PressAsync(key);
        }
        public async Task<string> GetTextAsync(ILocator locator)
        {
            LoggerHelper.Info($"Getting text from: {locator}");
            return await locator.InnerTextAsync();
        }

        public async Task<bool> IsVisibleAsync(ILocator locator)
        {
            LoggerHelper.Info($"Checking visibility of: {locator}");
            return await locator.IsVisibleAsync();
        }
    }
}