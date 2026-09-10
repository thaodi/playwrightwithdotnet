using PlaywrightTests.Core.Base;
using PlaywrightTests.Pages.Authentication;

namespace PlaywrightTests.Tests.E2E
{
    [TestFixture]
    public class ForgotPasswordTest : BaseTest
    {
        //[TestCase("thaodi218@gmail.com")]
        //public async Task TC_User_Can_Reset_Password_Successfully(string email)
        //{
        //    var forgotPasswordPage = new ForgotPasswordPage(Page);
        //    var setNewPasswordPage = new SetNewPasswordPage(Page);

        //    await forgotPasswordPage.NavigateToForgotPasswordPageAsync();
        //    await forgotPasswordPage.RequestPasswordResetAsync(email);
        //    await forgotPasswordPage.VerifySendEmailResetPasswordSuccessAsync("Check your email", email);

        //    var fakeResetUrl = "https://demo.spreecommerce.org/gb/en/account/reset-password?token=eyJfcmFpbHMiOnsiZGF0YSI6WzExNywiR0JaY2pGWS94VyJdLCJleHAiOiIyMDI2LTA4LTAxVDA4OjM5OjMwLjIxMFoiLCJwdXIiOiJTcHJlZTo6VXNlclxucGFzc3dvcmRfcmVzZXRcbjkwMCJ9fQ%3D%3D--3f8446c332167e50eaab5b367b8625cfe05e3f2c";
        //    await Page.RouteAsync("https://demo.spreecommerce.org/gb/en/account/forgot-password", async route =>
        //    {
        //        await route.FulfillAsync(new()
        //        {
        //            Status = 200,
        //            ContentType = "application/json",
        //            Json = new Dictionary<string, object>
        //            {
        //                { "success", true },
        //                { "reset_url", fakeResetUrl }
        //            }
        //        });
        //    });

        //    await setNewPasswordPage.NavigateToSetNewPasswordPageAsync(fakeResetUrl);
        //    await setNewPasswordPage.SetNewPasswordAsync("thaoyi", "thaoyi");
        //    await setNewPasswordPage.VerifyPasswordResetSuccessMessageAsync("Your password has been reset successfully.");
        //}

        [Test]
        public async Task TC02_Forgot_Password_With_Invalid_Email_Should_Show_Error()
        {
            var forgotPasswordPage = new ForgotPasswordPage(Page);
            await forgotPasswordPage.NavigateToForgotPasswordPageAsync();
            await forgotPasswordPage.RequestPasswordResetAsync("thaodi123@gmail.com");
        }
    }
}