using PlaywrightTests.Core.Config;
using Microsoft.Playwright;

namespace PlaywrightTests.Core.Driver
{
	public static class PlaywrightFactory
	{
		public static async Task<IBrowser> CreateBrowserAsync(IPlaywright playwright)
		{
			var launchOptions = new BrowserTypeLaunchOptions
			{
				Headless = ConfigReader.Headless,
				SlowMo = ConfigReader.SlowMo,
                Args = new[]
				{
					"--disable-blink-features=AutomationControlled",
					"--start-maximized",
					"--no-sandbox",
					"--disable-setuid-sandbox"
				}
            };

			var browserType = ConfigReader.Browser.ToLower();

			return browserType switch
			{
				"firefox" => await playwright.Firefox.LaunchAsync(launchOptions),
				"webkit" => await playwright.Webkit.LaunchAsync(launchOptions),
				"edge" => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
				{
					Headless = launchOptions.Headless,
					SlowMo = launchOptions.SlowMo,
					Channel = "msedge",
					Args = launchOptions.Args
				}),

                "chrome" => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
				{
					Headless = launchOptions.Headless,
					SlowMo = launchOptions.SlowMo,
					Channel = "chrome",
                    Args = launchOptions.Args
                }),
				_ => await playwright.Chromium.LaunchAsync(launchOptions) // Default: Chromium
			};
		}

		public static async Task<IBrowserContext> CreateBrowserContextAsync(IBrowser browser)
		{
			var context = await browser.NewContextAsync(new BrowserNewContextOptions
			{
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36",
                ViewportSize = ViewportSize.NoViewport,
                IgnoreHTTPSErrors = true,
				AcceptDownloads = true,
				RecordVideoDir = "TestResults/videos/",
				RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
			});
			return context;
		}
	}
}