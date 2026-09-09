using PlaywrightTests.Core.Base;
using PlaywrightTests.Core.Config;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests.Pages.Authentication
{
    public class LoginPage : BasePage
    {
        public LoginPage(IPage page) : base(page)
        {
        }

        private ILocator _emailInput => Page.Locator("#email");
        private ILocator _passwordInput => Page.Locator("#password");
        private ILocator _loginBtn => Page.GetByRole(AriaRole.Button, new() { Name = "Sign In" });
        private ILocator _errorMessage => Page.GetByText("Invalid email or password", new() { Exact = true });
        private ILocator _successIndicator => Page.GetByRole(AriaRole.Heading, new() { Name = "Account Overview" });
        private ILocator _signOutBtn => Page.GetByRole(AriaRole.Button, new() { Name = "Sign Out" });
        private ILocator _forgotPasswordLink => Page.GetByRole(AriaRole.Link, new() { Name = "Forgot password?" });

        public async Task NavigateToLoginPageAsync()
        {
            await NavigateToAsync("/account");
        }

        public async Task LoginAsync(string email, string password)
        {
            await FillAsync(_emailInput, email);
            await FillAsync(_passwordInput, password);
            await ClickAsync(_loginBtn);
        }

        public async Task VerifyLoginSuccessAsync(string url, string email)
        {
            var _expectedEmail = Page.GetByText(email);
            await Expect(Page).ToHaveURLAsync($"{ConfigReader.BaseUrl}/{url}");
            await Expect(_successIndicator).ToBeVisibleAsync();
            await Expect(_expectedEmail).ToBeVisibleAsync();
            await Expect(_signOutBtn).ToBeVisibleAsync();
        }

        public async Task VerifyErrorMessageDisplayedAsync(string expectedMessage)
        {
            await Expect(_errorMessage).ToBeVisibleAsync();
            await Expect(_errorMessage).ToHaveTextAsync(expectedMessage);
        }

        public async Task ClickForgotPasswordAsync()
        {
            await ClickAsync(_forgotPasswordLink);
        }
    }
}