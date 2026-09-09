using Microsoft.Playwright;
using PlaywrightTests.Core.Base;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests.Pages.Authentication
{
    public class SetNewPasswordPage : BasePage
    {
        public SetNewPasswordPage(IPage page) : base(page)
        {
        }

        private ILocator _newPasswordInput = Page.Locator("#password");
        private ILocator _confirmPasswordInput = Page.Locator("#passwordConfirmation");
        private ILocator _resetPasswordBtn = Page.GetByRole(AriaRole.Button, new() { Name = "Reset password" });
        private ILocator _successBanner => Page.Locator("//div[@data-slot='card-title']");

        public async Task NavigateToSetNewPasswordPageAsync(string resetPasswordUrl)
        {
            await NavigateToAsync(resetPasswordUrl);
        }
        public async Task SetNewPasswordAsync(string newPassword, string confirmPassword)
        {
            await FillAsync(_newPasswordInput, newPassword);
            await FillAsync(_confirmPasswordInput, confirmPassword);
            await ClickAsync(_resetPasswordBtn);
        }

        public async Task VerifyPasswordResetSuccessMessageAsync(string expectedMessage)
        {
            await Expect(_successBanner).ToBeVisibleAsync();
            await Expect(_successBanner).ToHaveTextAsync(expectedMessage);
        }
    }
}
