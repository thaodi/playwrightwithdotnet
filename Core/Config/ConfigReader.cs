namespace PlaywrightTests.Core.Config
{
	public static class ConfigReader
	{
		// 1. Khai báo một biến lưu trữ object TestSettings (chưa có giá trị)
		private static TestSettings? _settings;

		// 2. Property công khai để các nơi khác (BaseTest, BasePage) lấy dữ liệu dùng
		public static TestSettings Settings
		{
			get
			{
				// Kỹ thuật Singleton / Lazy Loading: 
				// Nếu _settings chưa có dữ liệu, thì gọi hàm LoadSettings() để tải. 
				// Nếu đã tải rồi thì trả về luôn, không cần đọc lại từ đĩa nữa cho nhanh!
				_settings ??= LoadSettings();
				return _settings;
			}
		}

		// 3. Hàm cốt lõi: Đọc dữ liệu từ .runsettings đổ vào TestSettings
		private static TestSettings LoadSettings()
		{
			// Tạo ra chiếc giỏ với các giá trị mặc định có sẵn trong TestSettings
			var settings = new TestSettings();

			// Kiểm tra xem NUnit có đọc được file .runsettings nào không
			if (TestContext.Parameters.Count > 0) {
				
				settings.BaseUrl =GetParams("BaseUrl", settings.BaseUrl);

				settings.Browser = GetParams("Browser", settings.Browser);

				settings.Headless = bool.TryParse(GetParams("Headless", settings.Headless.ToString()), out var headless) ? headless : settings.Headless;

				settings.SlowMo = float.TryParse(GetParams("SlowMo", settings.SlowMo.ToString()), out var slowMo) ? slowMo : settings.SlowMo;

				settings.DefaultTimeout = float.TryParse(GetParams("DefaultTimeout", settings.DefaultTimeout.ToString()), out var defaultTimeout) ? defaultTimeout : settings.DefaultTimeout;
			
				settings.Environment = GetParams("Environment", settings.Environment);
			}

			return settings;
		}

		// 4. Hàm phụ trợ trợ giúp đọc tham số từ NUnit cho an toàn
		private static string GetParams(string key, string defaultValue) { 
			return TestContext.Parameters[key] ?? defaultValue;
		}

		// 5. Mấy đường dẫn tắt (Shorthands) giúp gọi code ngắn gọn hơn ở bên ngoài
		public static string BaseUrl => Settings.BaseUrl;
		public static string Browser => Settings.Browser;
		public static bool Headless => Settings.Headless;
		public static float SlowMo => Settings.SlowMo;
        public static float DefaultTimeout => Settings.DefaultTimeout;
		public static string Environment => Settings.Environment;
	}
}