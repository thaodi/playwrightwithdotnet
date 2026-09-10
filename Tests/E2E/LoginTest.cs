using PlaywrightTests.Core.Base;
using PlaywrightTests.Pages.Authentication;

namespace PlaywrightTests.Tests.E2E
{
    [TestFixture]
    public class LoginTest : BaseTest
    {
        [Test]
        public async Task TC01_Login_With_Valid_Credentials_Should_Succeed()
        {
            var loginPage = new LoginPage(Page);
            await loginPage.NavigateToLoginPageAsync();
            await loginPage.LoginAsync("thao.nguyen.test202626@yopmail.com", "SecurePassword@123");
            await loginPage.VerifyLoginSuccessAsync("account", "thao.nguyen.test202626@yopmail.com");
        }

        [Test]
        public async Task TC02_Login_With_Invalid_Credentials_Should_Show_Error()
        {
            var loginPage = new LoginPage(Page);
            await loginPage.NavigateToLoginPageAsync();
            await loginPage.LoginAsync("thaodi123@gmail.com", "123");
            await loginPage.VerifyErrorMessageDisplayedAsync("Invalid email or password");
        }
    }
}