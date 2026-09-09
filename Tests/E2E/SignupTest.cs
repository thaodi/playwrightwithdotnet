using PlaywrightTests.Core.Base;
using PlaywrightTests.Pages.Authentication;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Tests.E2E
{
    public class SignupTest : BaseTest
    {
        [Test, TestCaseSource(nameof(GetSignupData))]
        public async Task TC_Signup_DataDriven_Scenarios(SignupTestData signupData)
        {
            var signupPage = new SignupPage(Page);
            await signupPage.NavigateToSignupPageAsync();

            await signupPage.SignupAsync(
                signupData.FirstName,
                signupData.LastName,
                signupData.Email,
                signupData.Password,
                signupData.ConfirmPassword
            );

            switch (signupData.ExpectedResult)
            {
                case "Success":
                    await signupPage.VerifySignupSuccessAsync("account", signupData.Email);
                    break;

                case "PasswordConfirmationError":
                    await signupPage.VerifyErrorMessageDisplayedAsync("Passwords do not match");
                    break;

                case "ValidationErrorEmptyFields":
                    await signupPage.VerifyAllEmptyFieldsShowValidationMessageAsync();
                    break;

                case "ValidationErrorEmailExists":
                    await signupPage.VerifyErrorMessageDisplayedAsync("Email has already been taken");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(signupData.ExpectedResult), $": {signupData.ExpectedResult}");
            }
        }

        private static IEnumerable<SignupTestData> GetSignupData()
        {
            return JsonDataReader.GetData<SignupTestData>("TestData", "Authentication", "SignupData.json");
        }
    }
}
