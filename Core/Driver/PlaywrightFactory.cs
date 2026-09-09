using PlaywrightTests.Core.Config;
using Microsoft.Playwright;

namespace PlaywrightTests.Core.Driver
{
	public static class PlaywrightFactory
	{
		// 1. Tạo ra đối tượng IBrowser (Chrome, Firefox, WebKit, Edge)
		public static async Task<IBrowser> CreateBrowserAsync(IPlaywright playwright)
		{
			var launchOptions = new BrowserTypeLaunchOptions
			{
				Headless = ConfigReader.Headless,
				SlowMo = ConfigReader.SlowMo,
                Args = new[] { "--start-maximized" }
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
                    Args = new[] { "--start-maximized" }
                }),
				"chrome" => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
				{
					Headless = launchOptions.Headless,
					SlowMo = launchOptions.SlowMo,
					Channel = "chrome",
                    Args = new[] { "--start-maximized" }
                }),
				_ => await playwright.Chromium.LaunchAsync(launchOptions) // Default: Chromium
			};
		}

		// 2. Tạo ra IBrowserContext (Cửa sổ ẩn danh riêng cho từng bài Test)
		public static async Task<IBrowserContext> CreateBrowserContextAsync(IBrowser browser)
		{
			var context = await browser.NewContextAsync(new BrowserNewContextOptions
			{
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