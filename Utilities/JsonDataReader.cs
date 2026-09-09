using System.Text.Json;

namespace PlaywrightTests.Utilities
{
	public static class JsonDataReader
	{
		public static List<T> GetData<T>(string folderName, string subfolderName, string fileName)
		{
            var projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
            var filePath = Path.Combine(projectDirectory, folderName, subfolderName, fileName);
			var jsonString = File.ReadAllText(filePath);
			return JsonSerializer.Deserialize<List<T>>(jsonString) ?? new List<T>();
		}
	}

	public class ProductTestData
	{
		public string ProductName { get; set; }
		public decimal ProductPrice { get; set; }
		public string Availability { get; set; }
		public List<string> Colors { get; set; }
		public string DescriptionKeyword { get; set; }
		public Dictionary<string, string> Properties { get; set; }
		public List<ProductDetails> Details { get; set; }
	}

	public class ProductDetails
	{
		public string Color { get; set; }
		public string ExpectedSku { get; set; }
		public string ExpectedOptionText { get; set; }
	}

    public class SignupTestData
    {
        public string TestName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string ExpectedResult { get; set; }

        public override string ToString() => TestName;
    }
}