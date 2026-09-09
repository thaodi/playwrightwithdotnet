using PlaywrightTests.Core.Base;
using PlaywrightTests.Core.Config;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests.Pages.Authentication
{
    public class SignupPage : BasePage
    {
        public SignupPage(IPage page) : base(page)
        {
        }

        private ILocator _firstNameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "First name" });
        private ILocator _lastNameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Last name" });
        private ILocator _emailInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" });
        private ILocator _passwordInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).First;
        private ILocator _confirmPasswordInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm password" });
        private ILocator _agreeBtn => Page.GetByRole(AriaRole.Checkbox, new() { Name = "I agree to the Privacy Policy and Terms of Service" });
        private ILocator _createAccountBtn => Page.GetByRole(AriaRole.Button, new() { Name = "Create Account" });
        private ILocator _successIndicator => Page.GetByRole(AriaRole.Heading, new() { Name = "Account Overview" });
        private ILocator _signOutBtn => Page.GetByRole(AriaRole.Button, new() { Name = "Sign Out" });
        private ILocator _errorMessage => Page.Locator("//div[@role='alert']");

        public async Task NavigateToSignupPageAsync()
        {
            await NavigateToAsync("/account/register");
        }

        public async Task SignupAsync(string firstName, string lastName, string email, string password, string confirmPassword)
        {
            await FillAsync(_firstNameInput, firstName);
            await FillAsync(_lastNameInput, lastName);
            await FillAsync(_emailInput, email);
            await FillAsync(_passwordInput, password);
            await FillAsync(_confirmPasswordInput, confirmPassword);
            await CheckAsync(_agreeBtn);
            await ClickAsync(_createAccountBtn);
        }

        public async Task ClickSubmitAsync()
        {
            await _createAccountBtn.ClickAsync();
        }

        public async Task VerifySignupSuccessAsync(string url, string email)
        {
            var _expectedEmail = Page.GetByText(email);
            await Expect(Page).ToHaveURLAsync($"{ConfigReader.BaseUrl}/{url}");
            await Expect(_successIndicator).ToBeVisibleAsync();
            await Expect(_expectedEmail).ToBeVisibleAsync();
            await Expect(_signOutBtn).ToBeVisibleAsync();
        }

        public async Task VerifyAllEmptyFieldsShowValidationMessageAsync() 
        {
            ILocator[] inputFields = new[]
            {
                _firstNameInput,
                _lastNameInput,
                _emailInput,
                _passwordInput,
                _confirmPasswordInput,
            };

            foreach (var input in inputFields) 
            {
                await input.FillAsync("");
                var actualErrorMessage = await input.EvaluateAsync<string>("el => el.validationMessage");
                Assert.That(actualErrorMessage, Is.EqualTo("Please fill out this field."), $"Error message mismatch for {input}. The actual message is: '{actualErrorMessage}'");
            }
        }

        public async Task VerifyErrorMessageDisplayedAsync(string expectedMessage)
        {
            await Expect(_errorMessage).ToBeVisibleAsync();
            await Expect(_errorMessage).ToHaveTextAsync(expectedMessage);
        }
    }
}
