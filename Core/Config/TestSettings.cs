namespace PlaywrightTests.Core.Config
{
    public class TestSettings
    {
        public string BaseUrl { get; set; } = "https://demo.spreecommerce.org/us/en";
        public string Browser { get; set; } = "chrome";
        public bool Headless { get; set; } = true;
        public float SlowMo { get; set; } = 2000;
        public float DefaultTimeout { get; set; } = 30000;
        public string Environment { get; set; } = "Staging";
    }
}