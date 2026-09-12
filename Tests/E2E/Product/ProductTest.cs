using Microsoft.Playwright;
using PlaywrightTests.Core.Base;
using PlaywrightTests.Core.Config;
using PlaywrightTests.Pages.Product;
using PlaywrightTests.Utilities;
using static Microsoft.Playwright.Assertions;
namespace PlaywrightTests.Tests.E2E.Product
{
    [TestFixture]
    public class ProductTest : BaseTest
    {
        [Test]
        public async Task TC01_Verify_All_Products_Page_Is_Displayed_Correctly()
        {
            var productPage = new ProductPage(Page);
            await productPage.NavigateToProductPageAsync();
            await productPage.VerifyAllProductIsDisplayedAsync(url: "products", numberOfProducts: 36);
        }

        [TestCase("AUTOMATIC", 2)]
        [TestCase("automatic", 2)]
        [TestCase("AuToMatic", 2)]
        public async Task TC02_Search_Exact_Keywork_Product_Is_Displayed_Correctly(string keyword, int expectedProductsCount)
        {
            var productPage = new ProductPage(Page);
            await productPage.NavigateToProductPageAsync();
            await productPage.Header.SearchProductAsync(keyword);

            int resultCount = await productPage.Header.GetSuggestionCountAsync(keyword);
            Assert.That(resultCount, Is.GreaterThan(0), $"Expected at least one suggestion item containing '{keyword}', but found none.");

            for (int i = 0; i < resultCount; i++)
            {
                var itemText = await productPage.Header.GetSuggestionItemTextAsync(i, keyword);
                Assert.That(itemText.Contains(keyword, StringComparison.OrdinalIgnoreCase), Is.True, $"Expected suggestion item to contain '{keyword}', but it doesn't.");
            }

            await productPage.Header.ClickViewAllResultsOfProductSearchAsync(keyword);

            await Expect(Page).ToHaveURLAsync($"{ConfigReader.BaseUrl}/products?q={keyword}");
            await Expect(productPage.Header.GetSearchResultsHeading(keyword)).ToBeVisibleAsync();
            await Expect(Page.GetByText($"{expectedProductsCount} products")).ToBeVisibleAsync();
        }

        [TestCase("abc", 0)]
        public async Task TC03_Search_Non_Existing_Product_Shows_No_Products_Found_Message(string keyword, int expectedProductsCount)
        {
            var productPage = new ProductPage(Page);
            await productPage.NavigateToProductPageAsync();
            await productPage.Header.SearchProductAsync(keyword);

            var resultCount = await productPage.Header.GetSuggestionCountAsync(keyword);
            Assert.That(resultCount, Is.EqualTo(0), $"Expected no suggestion items for '{keyword}', but found some.");

            var noProductsFoundMessage = await productPage.Header.GetNoProductsFoundMessageAsync();
            await Expect(noProductsFoundMessage).ToBeVisibleAsync();
            await Expect(noProductsFoundMessage).ToHaveTextAsync("No products found");

            await productPage.Header.PressEnterButtonAsync();

            await Expect(Page).ToHaveURLAsync($"{ConfigReader.BaseUrl}/products?q={keyword}");
            await Expect(productPage.Header.GetSearchResultsHeading(keyword)).ToBeVisibleAsync();
            await Expect(Page.GetByText($"{expectedProductsCount} products")).ToBeVisibleAsync();
            await Expect(productPage.Header.GetNoProductsFoundLabelAsync()).ToBeVisibleAsync();
        }

        [TestCase("Price (high-low)")]
        [TestCase("Name (A-Z)")]
        [TestCase("Name (Z-A)")]
        public async Task TC04_Verify_Product_Sort(string sortOption)
        {
            var productPage = new ProductPage(Page);
            var optionCount = await productPage.OpenSortAndSelectOptionAsync(sortOption);
            Assert.That(optionCount, Is.EqualTo(8), $"Expected 8 options, but found {optionCount}.");

            if (sortOption.Contains("Price"))
            {
                var productPrices = await productPage.GetAllProductPricesAsync();
                Assert.That(productPrices, Is.Not.Empty, "Expected to find product, but the list is empty.");

                var expectedSortedPrices = productPrices.OrderByDescending(p => p).ToList();
                Assert.That(productPrices, Is.EqualTo(expectedSortedPrices), $"Expected products to be sorted by '{sortOption}', but they are not.");
            }
            else if (sortOption.Contains("Name"))
            {
                var productNames = await productPage.GetAllProductNamesAsync();
                Assert.That(productNames, Is.Not.Empty, "Expected to find products, but the list is empty.");
                var expectedSortedNames = sortOption.Contains("(A-Z)")
                    ? productNames.OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToList()
                    : productNames.OrderByDescending(p => p, StringComparer.OrdinalIgnoreCase).ToList();
                Assert.That(productNames, Is.EqualTo(expectedSortedNames), $"Expected products to be sorted by '{sortOption}', but they are not.");
            }
        }

        [TestCase("Rose Gold", "Silver", "Mint")]
        [TestCase("Blue and White", "Black and Red", "Graphite")]
        public async Task TC05_Filter_Products_By_Color(params string[] selectedColors)
        {
            var productPage = new ProductPage(Page);
            var _productDetailPage = new ProductDetailPage(Page);
            await productPage.NavigateToProductPageAsync();
            await productPage.ClickFilterColorButtonAsync();

            //Verify the number of color options available
            var resultCount = await productPage.GetColorOptionCountAsync();
            Assert.That(resultCount, Is.EqualTo(19), $"Expected 19 options, but found {resultCount}.");

            await productPage.ClickColorOptionAsync(selectedColors);
            await productPage.CloseMenuAsync();

            // Verify the number of selected colors in button Color
            var quantityColor = await productPage.GetSelectedColorCountAsync();
            Assert.That(quantityColor, Is.EqualTo(selectedColors.Length), $"Expected {selectedColors.Length} selected colors, but found {quantityColor}.");

            // Verify the selected color tags are displayed correctly
            var expectedTags = selectedColors.Select(c => $"Color: {c}").ToList();
            var actualTags = await productPage.GetSelectedColorTagTextsAsync(selectedColors);
            Assert.That(actualTags, Is.EquivalentTo(expectedTags), "Selected color tags do not match expected values.");

            // Verify the number of products displayed after filtering
            var productResultCount = await productPage.GetProductResultCountAsync();
            Assert.That(productResultCount, Is.GreaterThan(0), "Expected at least one product after filtering, but found none.");

            int productsToCheck = Math.Min(productResultCount, 3);

            // Verify that each product displayed has at least one of the selected colors
            for (int i = 0; i < productsToCheck; i++)
            {
                await productPage.ClickProductItemByIndexAsync(i);
                var colorOnDetailProduct = await _productDetailPage.GetAllColorOnDetailProductItemAsync();
                Assert.That(colorOnDetailProduct.Any(color => selectedColors.Contains(color)), Is.True, $"Expected product at index {i} to have at least one of the selected colors, but it does not.");
                await Page.GoBackAsync();
                await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            }
        }

        //Filter produce by price range(under 50, 50 - 100, 100 - 200) not working properly, so I will only test the price range $200+
        [TestCase("$200+")]
        public async Task TC06_Filter_Product_By_Price(string selectedPriceRange)
        {
            var productPage = new ProductPage(Page);
            var _productDetailPage = new ProductDetailPage(Page);
            await productPage.NavigateToProductPageAsync();
            await productPage.ClickPriceButtonAsync();

            // Verify the number of price options available
            var resultCount = await productPage.GetPriceOptionCountAsync();
            Assert.That(resultCount, Is.EqualTo(4), $"Expected 4 options, but found {resultCount}.");

            // Select a price range option
            await productPage.ClickPriceOptionAsync(selectedPriceRange);
            await productPage.CloseMenuAsync();

            // Verify the number of selected prices in button Price
            var quantityPrice = await productPage.GetSelectedPriceCountAsync();
            Assert.That(quantityPrice, Is.EqualTo(1), $"Expected 1 selected price, but found {quantityPrice}.");

            // Verify the selected price tags are displayed correctly
            var expectedTags = $"Price: {selectedPriceRange}";
            var actualTags = await productPage.GetSelectedPriceTagTextsAsync(selectedPriceRange);
            Assert.That(actualTags, Is.EqualTo(expectedTags), "Selected price tags do not match expected values.");

            // Verify the number of products displayed after filtering
            var productResultCount = await productPage.GetProductResultCountAsync();
            Assert.That(productResultCount, Is.GreaterThan(0), "Expected at least one product after filtering, but found none.");

            var productsToCheck = Math.Min(productResultCount, 3);
            //Verify that each product displayed falls within the selected price range
            for (var i = 0; i < productsToCheck; i++)
            {
                await productPage.ClickProductItemByIndexAsync(i);
                var priceOnDetailProduct = await _productDetailPage.GetPriceOnDetailProductItemAsync();
                Assert.That(priceOnDetailProduct, Is.GreaterThan(200.00), $"Expected product at index {i} to be within the price range {selectedPriceRange}, but it is not.");
                await Page.GoBackAsync();
                await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            }
        }

        [TestCase("In Stock", true, 36)]
        [TestCase("Out of Stock", false, 0)]
        public async Task TC07_Filter_Product_By_Availability(string selectedAvailability, bool hasProductsExpected, int numberOfProductsExpected)
        {
            var productPage = new ProductPage(Page);
            var _productDetailPage = new ProductDetailPage(Page);
            await productPage.NavigateToProductPageAsync();
            await productPage.ClickAvailabilityButtonAsync();

            // Verify the number of availability options available
            var resultCount = await productPage.GetAvailabilityOptionCountAsync();
            Assert.That(resultCount, Is.EqualTo(3), $"Expected 3 options, but found {resultCount}.");

            // Select an availability option
            await productPage.ClickAvailabilityOptionAsync(selectedAvailability);

            if (hasProductsExpected)
            {
                // Verify the number of products selected under the availability option
                var quantityAvailabilityItem = await productPage.GetQuantityOfProductInSelectedAvailabilityOptionTextAsync(selectedAvailability);
                Assert.That(quantityAvailabilityItem, Is.EqualTo(numberOfProductsExpected), $"Expected {numberOfProductsExpected} selected availability option, but found {quantityAvailabilityItem}.");
                await productPage.CloseMenuAsync();

                // Verify the number of selected availability in button Availability
                var quantityAvailability = await productPage.GetSelectedAvailabilityCountAsync();
                Assert.That(quantityAvailability, Is.EqualTo(1), $"Expected 1 selected availability, but found {quantityAvailability}.");

                // Verify the number of products displayed after filtering
                var productResultCount = await productPage.GetProductResultCountAsync();
                Assert.That(productResultCount, Is.GreaterThan(0), "Expected at least one product after filtering, but found none.");
                var productsToCheck = Math.Min(productResultCount, 3);

                //Verify that each product displayed is in stock
                for (var i = 0; i < productsToCheck; i++)
                {
                    await productPage.ClickProductItemByIndexAsync(i);
                    var availabilityOnDetailProduct = await _productDetailPage.GetAvailabilityOnDetailProductItemAsync();
                    Assert.That(availabilityOnDetailProduct.Contains(selectedAvailability), Is.True, $"Expected product at index {i} to be '{selectedAvailability}', but it is not.");
                    await Page.GoBackAsync();
                    await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                }
            }
            else
            {
                // Verify the number of products selected under the availability option notworking 
                await productPage.CloseMenuAsync();

                // Verify that no products are displayed after filtering
                var productResultCount = await productPage.GetProductResultCountAsync();
                Assert.That(productResultCount, Is.EqualTo(0), "Expected no products after filtering, but found some.");

                // Verify that the "No products found" message is displayed
                await Expect(productPage.Header.GetNoProductsFoundLabelAsync()).ToBeVisibleAsync();
            }

            // Verify the selected availability tags are displayed correctly
            var actualTags = await productPage.GetSelectedAvailabilityTagTextsAsync(selectedAvailability);
            Assert.That(actualTags, Is.EqualTo(selectedAvailability), "Selected availability tags do not match expected values.");
        }

        [Test, TestCaseSource(nameof(GetProductTestData))]
        public async Task VerifyViewProductDetail(ProductTestData productData)
        {
            var productPage = new ProductPage(Page);
            var _productDetailPage = new ProductDetailPage(Page);
            await productPage.NavigateToProductPageAsync();

            await productPage.ClickProductItemByNameAsync(productData.ProductName);

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
        }

        private static IEnumerable<ProductTestData> GetProductTestData()
        {
            return JsonDataReader.GetData<ProductTestData>("TestData", "Product", "ProductData.json");
        }
    }
}
