using Allure.Net.Commons;
using Microsoft.Playwright;
using PlaywrightTests.Core.Config;
using PlaywrightTests.Core.Driver;
using PlaywrightTests.Utilities;
using Reqnroll;

//[assembly: LevelOfParallelism(2)]
//[assembly: Parallelizable(ParallelScope.All)]
namespace PlaywrightTests.Core.Base
{
    [Binding]
    public class Hooks
    {
        protected static IPlaywright PlaywrightInstance { get; private set; } = null!;
        protected static IBrowser Browser { get; private set; } = null!;
        protected IBrowserContext Context { get; private set; } = null!;
        protected IPage Page { get; private set; } = null!;
        private readonly ScenarioContext _scenarioContext;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static async Task BeforeTestRun()
        {
            var allureResultsDir = Path.Combine(Directory.GetCurrentDirectory(), "allure-results");

            if (Directory.Exists(allureResultsDir))
            {
                Directory.Delete(allureResultsDir, true);
            }
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

            var testName = TestContext.CurrentContext.Test.Name;
            LoggerHelper.Info($"=== STARTING TEST: {testName} ===");

            PlaywrightInstance = await Playwright.CreateAsync();
            Browser = await PlaywrightFactory.CreateBrowserAsync(PlaywrightInstance);

            // Do not create a shared Context/Page here. Create per-scenario contexts in BeforeScenario.
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            var scenarioName = _scenarioContext.ScenarioInfo.Title;
            LoggerHelper.Info($"=== STARTING SCENARIO: {scenarioName} ===");

            Context = await PlaywrightFactory.CreateBrowserContextAsync(Browser);

            await Context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

            Page = await Context.NewPageAsync();
            Page.SetDefaultTimeout(ConfigReader.DefaultTimeout);

            _scenarioContext.Set(Page, "Page");
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            var scenarioName = _scenarioContext.ScenarioInfo.Title;
            var errorMessage = _scenarioContext.TestError;

            if (errorMessage != null)
            {
                LoggerHelper.Error($"TEST FAILED/ERROR: {scenarioName}. Reason: {errorMessage}");

                if (Page != null)
                {
                    try
                    {
                        var screenshotBytes = await Page.ScreenshotAsync(new PageScreenshotOptions { });
                        AllureApi.AddAttachment("Failure Screenshot", "image/png", screenshotBytes);

                        var tracePath = Path.Combine("TestResults", $"trace_{scenarioName}.zip");
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
                LoggerHelper.Info($"TEST PASSED: {scenarioName}");
                if (Context != null)
                {
                    await Context.Tracing.StopAsync(new TracingStopOptions());
                }
            }

            if (Context != null) await Context.CloseAsync();
        }

        [AfterTestRun]
        public static async Task AfterTestRun()
        {
            if (Browser != null) await Browser.CloseAsync();
            PlaywrightInstance?.Dispose();
        }
    }
}