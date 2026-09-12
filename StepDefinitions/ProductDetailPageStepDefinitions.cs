using Microsoft.Playwright;
using PlaywrightTests.Pages.Product;
using PlaywrightTests.Utilities;
using Reqnroll;

namespace PlaywrightTests.StepDefinitions
{
    [Binding]
    public class ProductDetailPageStepDefinitions
    {
        private IPage Page => _scenarioContext.Get<IPage>("Page");
        private readonly ScenarioContext _scenarioContext;
        private ProductDetailPage _productDetailPage;
        private ProductPage _productPage;

        public ProductDetailPageStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _productPage = new ProductPage(Page);
            _productDetailPage = new ProductDetailPage(Page);
        }


        [When(@"I verify details for all products defined in the JSON data file")]
        public async Task WhenIVerifyDetailsForAllProductsDefinedInTheJsonDataFile()
        {
            var productList = JsonDataReader.GetData<ProductTestData>("TestData", "Product", "ProductData.json");
            _scenarioContext.Set(productList, "ProductList");
        }

        [Then(@"the product information should match with the JSON data file")]
        public async Task ThenTheProductInformationShouldMatchWithTheJsonDataFile()
        {
            var productList = _scenarioContext.Get<List<ProductTestData>>("ProductList");

            foreach (var productData in productList)
            {
                _productPage.ClickProductItemByNameAsync(productData.ProductName);

                foreach (var detail in productData.Details)
                {
                    await _productDetailPage.SelectColorOnDetailProductItemAsync(detail.Color);

                    var properties = new Dictionary<string, string>
                    {
                        { "Warranty", productData.Properties["Warranty"] },
                        { "Wattage", productData.Properties["Wattage"] },
                        { "Voltage", productData.Properties["Voltage"] },
                        { "SKU", detail.ExpectedSku },
                        { "Options", detail.ExpectedOptionText }
                    };

                    await _productDetailPage.VerifyProductDetailDisplayedCorrectly(
                        expectedProductName: productData.ProductName,
                        expectedProductPrice: productData.ProductPrice,
                        expectedAvailability: productData.Availability,
                        expectedColors: productData.Colors.ToArray(),
                        expectedDescription: productData.DescriptionKeyword,
                        expectedProperties: properties
                    );
                }
                await Page.GoBackAsync();
                await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            }
                
        }
    }
}
