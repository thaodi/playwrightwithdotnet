using PlaywrightTests.Core.Base;
using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests.Tests.Smoke
{
    [TestFixture]
    public class CoreSanityTest : BaseTest
    {
        [Test]
        public async Task VerifyCoreFrameworkWorks()
        {
            await Page.GotoAsync("https://playwright.dev/dotnet/");
            var title = await Page.TitleAsync();

            Assert.That(title, Contains.Substring("Playwright"));
        }
    }
}