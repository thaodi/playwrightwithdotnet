using Microsoft.Playwright;
using PlaywrightTests.Core.Base;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests.Pages.Authentication
{
    public class ForgotPasswordPage : BasePage
    {
        public ForgotPasswordPage(IPage page) : base(page)
        {
        }

        private ILocator _emailInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" });
        private ILocator _sendResetLinkBtn => Page.GetByRole(AriaRole.Button, new() { Name = "Send reset link" });

        private ILocator _titleHeading => Page.Locator("//div[@data-slot = 'card-title']");
        private ILocator _descriptionMessage => Page.Locator("//div[@data-slot='card-description']");
        private ILocator _tryDifferentEmailBtn => Page.GetByRole(AriaRole.Button, new() { Name = "Try a different email" });
        private ILocator _backToSignInLink => Page.GetByRole(AriaRole.Link, new() { Name = "Back to sign in" });


        public async Task NavigateToForgotPasswordPageAsync()
        {
            await NavigateToAsync("/account/forgot-password");
        }

        public async Task RequestPasswordResetAsync(string email)
        {
            await FillAsync(_emailInput, email);
            await ClickAsync(_sendResetLinkBtn);
        }

        public async Task VerifySendEmailResetPasswordSuccessAsync(string titleHeading, string expectedEmail)
        {
            await Expect(_titleHeading).ToBeVisibleAsync();
            await Expect(_titleHeading).ToHaveTextAsync(titleHeading);
            await Expect(_descriptionMessage).ToHaveTextAsync($"If an account exists for {expectedEmail}, we've sent password reset instructions.");
            await Expect(_tryDifferentEmailBtn).ToBeVisibleAsync();
            await Expect(_backToSignInLink).ToBeVisibleAsync();
        }


    }
}
