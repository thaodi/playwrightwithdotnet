using PlaywrightTests.Core.Base;
using PlaywrightTests.Core.Config;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests.Pages.Home
{
    public class HomePage : BasePage
    {
        public HomePage(IPage page) : base(page)
        {
        }


        public async Task NavigateToHomePageAsync()
        {
            await NavigateToAsync("/");
        }
    }
}