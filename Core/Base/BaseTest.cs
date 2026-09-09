using Allure.Net.Commons;
using Allure.NUnit;
using Microsoft.Playwright;
using NUnit.Framework.Interfaces;
using PlaywrightTests.Core.Config;
using PlaywrightTests.Core.Driver;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Core.Base
{
    [TestFixture]
    [AllureNUnit]
    public abstract class BaseTest
    {
        protected IPlaywright PlaywrightInstance { get; private set; } = null!;
        protected IBrowser Browser { get; private set; } = null!;
        protected IBrowserContext Context { get; private set; } = null!;
        protected IPage Page { get; private set; } = null!;

        [OneTimeSetUp]
        public void SetupEnvironmentProperties()
        {
            var allureResultsDir = Path.Combine(Directory.GetCurrentDirectory(), "allure-results");
            Directory.CreateDirectory(allureResultsDir);

            var envPropertiesPath = Path.Combine(allureResultsDir, "environment.properties");
            var lines = new[]
            {
                "Browser = Chromium",
                "OS = Windows 11",
                "Environment = Staging",
                "BaseUrl = https://demo.spreecommerce.org",
                $"ExecutionTime = {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
            };

            File.WriteAllLines(envPropertiesPath, lines);
        }

        [SetUp]
        public async Task GlobalSetup()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            LoggerHelper.Info($"=== STARTING TEST: {testName} ===");

            PlaywrightInstance = await Playwright.CreateAsync();
            Browser = await PlaywrightFactory.CreateBrowserAsync(PlaywrightInstance);
            Context = await PlaywrightFactory.CreateBrowserContextAsync(Browser);

            await Context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

            Page = await Context.NewPageAsync();
            Page.SetDefaultTimeout(ConfigReader.DefaultTimeout);
        }

        [TearDown]
        public async Task GlobalTeardown()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
            var errorMessage = TestContext.CurrentContext.Result.Message;

            if (testStatus == TestStatus.Failed)
            {
                LoggerHelper.Error($"TEST FAILED/ERROR: {testName}. Reason: {errorMessage}");

                if (Page != null)
                {
                    try
                    {
                        var screenshotBytes = await Page.ScreenshotAsync(new PageScreenshotOptions {});
                        AllureApi.AddAttachment("Failure Screenshot", "image/png", screenshotBytes);

                        var tracePath = Path.Combine("TestResults", $"trace_{testName}.zip");
                        await Context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
                        AllureApi.AddAttachment("Playwright Trace", "application/zip", tracePath);

                        LoggerHelper.Info("Screenshot successfully captured and attached to Allure.");
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Error($"Failed to capture screenshot: {ex.Message}");
                    }
                }
                else
                {
                    LoggerHelper.Error("Page object is null, cannot take screenshot.");
                }
            }
            else
            {
                LoggerHelper.Info($"TEST PASSED: {testName}");
                if (Context != null)
                {
                    await Context.Tracing.StopAsync(new TracingStopOptions());
                }
            }

            if (Context != null) await Context.CloseAsync();
            if (Browser != null) await Browser.CloseAsync();
            PlaywrightInstance?.Dispose();
        }
    }
}