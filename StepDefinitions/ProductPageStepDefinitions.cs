using Microsoft.Playwright;
using PlaywrightTests.Core.Config;
using PlaywrightTests.Pages.Product;
using Reqnroll;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests.StepDefinitions
{
    [Binding]
    public class ProductPageStepDefinitions
    {
        private IPage Page => _scenarioContext.Get<IPage>("Page");
        private readonly ScenarioContext _scenarioContext;
        private ProductPage _productPage;
        private ProductDetailPage _productDetailPage;

        public ProductPageStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _productPage = new ProductPage(Page);
            _productDetailPage = new ProductDetailPage(Page);
        }

        [Given(@"I navigate to the Product page")]
        public async Task GivenINavigateToTheProductPage()
        {
            await _productPage.NavigateToProductPageAsync();
        }

        [Then(@"The product item should be displayed correctly with complete details")]
        public async Task ThenTheProductItemShouldBeDisplayedWithTheCompleteDetails()
        {
            var productCount = await _productPage.GetProductResultCountAsync();
            Assert.That(productCount, Is.GreaterThan(0), "Expected at least one product after filtering, but found none.");
            var productsToCheck = Math.Min(productCount, 3);

            for(int i = 0; i < productsToCheck; i++)
            {
                var isImageVisible = await _productPage.IsProductImageVisibleByIndexAsync(i);
                Assert.That(isImageVisible, Is.True, $"Expected product image at index {i} to be visible.");

                var productName = await _productPage.GetProductNameByIndexAsync(i);
                Assert.That(string.IsNullOrWhiteSpace(productName), Is.False, $"Product name at index {i} is missing or empty.");

                var productPrice = await _productPage.GetProductPriceByIndexAsync(i);
                Assert.That(productPrice, Does.Match(@"^\$\d+(\.\d{2})?$"), $"Product price format at index '{i}' is invalid: '{productPrice}'.");
            }
        }

        [When(@"I enter the product name {string} in the search field")]
        public async Task WhenIEnterTheProductNameInTheSearchField(string keyword)
        {
            await _productPage.Header.SearchProductAsync(keyword);
        }

        [When(@"I click on the view all results button based on keyword {string}")]
        public async Task WhenIClickOnTheViewAllResultsButton(string keyword)
        {
            await _productPage.Header.ClickViewAllResultsOfProductSearchAsync(keyword);
        }

        [Then(@"the system should display {int} suggestion items for keyword {string}")]
        public async Task ThenTheSystemShouldDispplaySuggestionItemsForKeyword(int numberOfProductsExpected, string keyword)
        {
            var resultCount = await _productPage.Header.GetSuggestionCountAsync(keyword);
            Assert.That(resultCount, Is.LessThanOrEqualTo(numberOfProductsExpected), $"Expected {numberOfProductsExpected} products for keyword '{keyword}', but found {resultCount}.");
            _scenarioContext.Set(resultCount, "resultCount");
        }

        [Then(@"the product suggestion item should display with correct details keyword {string}")]
        public async Task ThenTheProductResultShouldBeDisplayedCorrectly(string keyword)
        {
            var resultCount = _scenarioContext.Get<int>("resultCount");
            for (int i = 0; i < resultCount; i++)
            {
                var itemText = await _productPage.Header.GetSuggestionItemTextAsync(i, keyword);
                Assert.That(itemText.Contains(keyword, StringComparison.OrdinalIgnoreCase), Is.True, $"Expected suggestion item to contain '{keyword}', but it doesn't.");
            }
        }

        [Then(@"the product search result should be verified based on keyword {string} and count {int}")]
        public async Task ThenTheProductSearchResultShouldBeVerified(string keyword, int numberOfProductsExpected)
        {
            await Expect(Page).ToHaveURLAsync($"{ConfigReader.BaseUrl}/products?q={keyword}");
            await Expect(_productPage.Header.GetSearchResultsHeading(keyword)).ToBeVisibleAsync();
            await Expect(Page.GetByText($"{numberOfProductsExpected} products")).ToBeVisibleAsync();

            var resultCount = await _productPage.GetProductResultCountAsync();
            Assert.That(resultCount, Is.EqualTo(numberOfProductsExpected), $"Expected {numberOfProductsExpected} products for keyword '{keyword}', but found {resultCount}.");

            for (int i = 0; i < resultCount; i++)
            {
                var itemText = await _productPage.GetProductNameByIndexAsync(i);
                Assert.That(itemText.Contains(keyword, StringComparison.OrdinalIgnoreCase), Is.True, $"Expected product name at index {i} to contain '{keyword}', but it doesn't.");
            }
        }

        [Then(@"the no products found suggestion message should be visible with text {string}")]
        public async Task ThenTheNoProductsFoundSuggestionMessageShouldBeVisibleWithText(string expectedText)
        {
            var noProductsFoundMessage = await _productPage.Header.GetNoProductsFoundMessageAsync();
            await Expect(noProductsFoundMessage).ToBeVisibleAsync();
            await Expect(noProductsFoundMessage).ToHaveTextAsync(expectedText);
        }
        [When("I press enter on the search field")]
        public async Task WhenIPressEnterOnTheSearchField()
        {
            await _productPage.Header.PressEnterButtonAsync();
        }

        [Then("the product search result URL should be verified based on keyword {string}")]
        public async Task ThenTheProductSearchResultURLShouldBeVerifiedBasedOnKeyword(string keyword)
        {
            await Expect(Page).ToHaveURLAsync($"{ConfigReader.BaseUrl}/products?q={keyword}");
        }

        [Then("the search results heading for {string} should be visible")]
        public async Task ThenTheSearchResultsHeadingForKeywordShouldBeVisible(string keyword)
        {
            await Expect(_productPage.Header.GetSearchResultsHeading(keyword)).ToBeVisibleAsync();
        }

        [Then("the product search result count should show {int} products")]
        public async Task ThenTheProductSearchResultCountShouldShow(int expectedProductsCount)
        {
            await Expect(Page.GetByText($"{expectedProductsCount} products")).ToBeVisibleAsync();
        }

        [Then("the no products found label should be visible")]
        public async Task ThenTheNoProductsFoundLabelShouldBeVisible()
        {
            await Expect(_productPage.Header.GetNoProductsFoundLabelAsync()).ToBeVisibleAsync();
        }

        [When("I click the filter color button")]
        public async Task WhenIClickTheFilterColorButton()
        {
            await _productPage.ClickFilterColorButtonAsync();
        }

        [Then("the total number of available color options should be {int}")]
        public async Task ThenTheTotalNumberOfAvailableColorOptionsShouldBe(int expectedNumberOption)
        {
            var resultCount = await _productPage.GetColorOptionCountAsync();
            Assert.That(resultCount, Is.EqualTo(expectedNumberOption), $"Expected 19 options, but found {resultCount}.");
        }

        [When("I select the color options {string}")]
        public async Task WhenISelectTheColorOptions(string colorsParam)
        {
            string[] selectedColors = colorsParam.Split(',').Select(c => c.Trim()).ToArray();

            _scenarioContext.Set(selectedColors, "SelectedColors");
            await _productPage.ClickColorOptionAsync(selectedColors);
        }

        [When("I close the color filter menu")]
        public async Task WhenICloseTheColorFilterMenu()
        {
            await _productPage.CloseMenuAsync();
        }

        [Then("the selected color count on the filter button should be {int}")]
        public async Task ThenTheSelectedColorCountOnTheFilterButtonShouldBe(int expectedSelectedColorCount)
        {
            var quantityColor = await _productPage.GetSelectedColorCountAsync();
            Assert.That(quantityColor, Is.EqualTo(expectedSelectedColorCount), $"Expected {expectedSelectedColorCount} selected colors, but found {quantityColor}.");
        }

        [Then("the selected color tags should be displayed correctly as {string}")]
        public async Task ThenTheSelectedColorTagsShouldBeDisplayedCorrectlyAs(string colorsParam)
        {
            string[] selectedColors = colorsParam.Split(',').Select(c => c.Trim()).ToArray();

            var expectedTags = selectedColors.Select(c => $"Color: {c}").ToList();
            var actualTags = await _productPage.GetSelectedColorTagTextsAsync(selectedColors);
            Assert.That(actualTags, Is.EquivalentTo(expectedTags), "Selected color tags do not match expected values.");
        }

        [Then("at least one product should be displayed after color filtering")]
        public async Task ThenAtLeastOneProductShouldBeDisplayedAfterColorFiltering()
        {
            var productResultCount = await _productPage.GetProductResultCountAsync();

            _scenarioContext.Set(productResultCount, "ProductResultCount");

            Assert.That(productResultCount, Is.GreaterThan(0), "Expected at least one product after filtering, but found none.");
        }

        [Then("each checked product detail should contain at least one of the selected colors {string}")]
        public async Task ThenEachCheckedProductDetailShouldContainAtLeastOneOfTheSelectedColors(string colorsParam)
        {
            string[] selectedColors = colorsParam.Split(',').Select(c => c.Trim()).ToArray();

            int productResultCount = _scenarioContext.Get<int>("ProductResultCount");
            int productsToCheck = Math.Min(productResultCount, 3);

            for (int i = 0; i < productsToCheck; i++)
            {
                await _productPage.ClickProductItemByIndexAsync(i);
                var colorOnDetailProduct = await _productDetailPage.GetAllColorOnDetailProductItemAsync();
                Assert.That(colorOnDetailProduct.Any(color => selectedColors.Contains(color)), Is.True, $"Expected product at index {i} to have at least one of the selected colors, but it does not.");
                await Page.GoBackAsync();
                await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            }
        }

        [When("I open the sort dropdown and select option {string}")]
        public async Task WhenIOpenTheSortDropdownAndSelectOption(string sortOption)
        {
            var optionCount = await _productPage.OpenSortAndSelectOptionAsync(sortOption);
            _scenarioContext.Set(optionCount, "OptionCount");
        }

        [Then("the total number of sort options should be {int}")]
        public async Task ThenTheTotalNumberOfSortOptionsShouldBe(int expectedCount)
        {
            var optionCount = _scenarioContext.Get<int>("OptionCount");
            Assert.That(optionCount, Is.EqualTo(expectedCount), $"Expected {expectedCount} options, but found {optionCount}.");
        }

        [Then("the product list should be sorted correctly based on {string}")]
        public async Task ThenTheProductListShouldBeSortedCorrectlyBasedOn(string sortOption)
        {
            if (sortOption.Contains("Price"))
            {
                var productPrices = await _productPage.GetAllProductPricesAsync();
                Assert.That(productPrices, Is.Not.Empty, "Expected to find product, but the list is empty.");

                var expectedSortedPrices = productPrices.OrderByDescending(p => p).ToList();
                Assert.That(productPrices, Is.EqualTo(expectedSortedPrices), $"Expected products to be sorted by '{sortOption}', but they are not.");
            }
            else if (sortOption.Contains("Name"))
            {
                var productNames = await _productPage.GetAllProductNamesAsync();
                Assert.That(productNames, Is.Not.Empty, "Expected to find products, but the list is empty.");
                var expectedSortedNames = sortOption.Contains("(A-Z)")
                    ? productNames.OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToList()
                    : productNames.OrderByDescending(p => p, StringComparer.OrdinalIgnoreCase).ToList();
                Assert.That(productNames, Is.EqualTo(expectedSortedNames), $"Expected products to be sorted by '{sortOption}', but they are not.");
            }
        }

        [When(@"I open the availability filter menu")]
        public async Task WhenIOpenTheAvailabilityFilterMenu()
        {
            await _productPage.ClickAvailabilityButtonAsync();
        }

        [Then(@"the system should display ""(.*)"" availability options")]
        public async Task ThenTheSystemShouldDisplayAvailabilityOptions(int expectedOptionCount)
        {
            var resultCount = await _productPage.GetAvailabilityOptionCountAsync();
            Assert.That(resultCount, Is.EqualTo(expectedOptionCount), $"Expected {expectedOptionCount} options, but found {resultCount}.");
        }

        [When(@"I select the availability option ""(.*)""")]
        public async Task WhenISelectTheAvailabilityOption(string selectedAvailability)
        {
            await _productPage.ClickAvailabilityOptionAsync(selectedAvailability);
            _scenarioContext.Set(selectedAvailability, "SelectedAvailability");
        }

        [When(@"I close the availability filter menu")]
        public async Task WhenICloseTheAvailabilityFilterMenu()
        {
            await _productPage.CloseMenuAsync();
        }

        [Then(@"the product filtering result by availability should be verified based on expectation ""(.*)"" and count ""(.*)""")]
        public async Task ThenTheProductFilteringResultByAvailabilityShouldBeVerified(bool hasProductsExpected, int numberOfProductsExpected)
        {
            var selectedAvailability = _scenarioContext.Get<string>("SelectedAvailability");

            if (hasProductsExpected)
            {
                var quantityAvailabilityItem = await _productPage.GetQuantityOfProductInSelectedAvailabilityOptionTextAsync(selectedAvailability);
                Assert.That(quantityAvailabilityItem, Is.EqualTo(numberOfProductsExpected), $"Expected {numberOfProductsExpected} selected availability option, but found {quantityAvailabilityItem}.");
                await _productPage.CloseMenuAsync();

                var quantityAvailability = await _productPage.GetSelectedAvailabilityCountAsync();
                Assert.That(quantityAvailability, Is.EqualTo(1), $"Expected 1 selected availability, but found {quantityAvailability}.");

                var productResultCount = await _productPage.GetProductResultCountAsync();
                Assert.That(productResultCount, Is.GreaterThan(0), "Expected at least one product after filtering, but found none.");
                var productsToCheck = Math.Min(productResultCount, 3);

                for (var i = 0; i < productsToCheck; i++)
                {
                    await _productPage.ClickProductItemByIndexAsync(i);
                    var availabilityOnDetailProduct = await _productDetailPage.GetAvailabilityOnDetailProductItemAsync();
                    Assert.That(availabilityOnDetailProduct.Contains(selectedAvailability), Is.True, $"Expected product at index {i} to be '{selectedAvailability}', but it is not.");
                    await Page.GoBackAsync();
                    await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                }
            }
            else
            {
                await _productPage.CloseMenuAsync();
                var productResultCount = await _productPage.GetProductResultCountAsync();
                Assert.That(productResultCount, Is.EqualTo(0), "Expected no products after filtering, but found some.");
                await Expect(_productPage.Header.GetNoProductsFoundLabelAsync()).ToBeVisibleAsync();
            }
        }

        [Then(@"the selected availability tags should be displayed correctly as ""(.*)""")]
        public async Task ThenTheSelectedAvailabilityTagsShouldBeDisplayedCorrectlyAs(string expectedTag)
        {
            var actualTags = await _productPage.GetSelectedAvailabilityTagTextsAsync(expectedTag);
            Assert.That(actualTags, Is.EqualTo(expectedTag), "Selected availability tags do not match expected values.");
        }

        [When(@"I open the price filter menu")]
        public async Task WhenIOpenThePriceFilterMenu()
        {
            await _productPage.ClickPriceButtonAsync();
        }

        [Then(@"the system should display {int} price options")]
        public async Task ThenTheSystemShouldDisplayPriceOptions(int priceOptionCount)
        {
            var actualCount = await _productPage.GetPriceOptionCountAsync();
            Assert.That(actualCount, Is.EqualTo(priceOptionCount), "Price option count does not match expected value.");
        }

        [When(@"I select the price range option ""(.*)""")]
        public async Task WhenISelectThePriceRangeOption(string selectedPriceRange)
        {
            await _productPage.ClickPriceOptionAsync(selectedPriceRange);
            _scenarioContext.Set(selectedPriceRange, "SelectedPriceRange");
        }

        [When(@"I close the price filter menu")]
        public async Task WhenICloseThePriceFilterMenu()
        {
            await _productPage.CloseMenuAsync();
        }

        [Then(@"the selected price count should be {int}")]
        public async Task ThenTheSelectedPriceCountShouldBeOne(int expectedCount)
        {
            var quantityPrice = await _productPage.GetSelectedPriceCountAsync();
            Assert.That(quantityPrice, Is.EqualTo(expectedCount), $"Expected {expectedCount} selected price, but found {quantityPrice}.");
        }

        [Then(@"the selected price tag should be displayed correctly as ""Price: (.*)""")]
        public async Task ThenTheSelectedPriceTagShouldBeDisplayedCorrectlyAs(string selectedPriceRange)
        {
            var expectedTags = $"Price: {selectedPriceRange}";
            var actualTags = await _productPage.GetSelectedPriceTagTextsAsync(selectedPriceRange);
            Assert.That(actualTags, Is.EqualTo(expectedTags), "Selected price tags do not match expected values.");
        }

        [Then(@"at least one product should be displayed after price filtering")]
        public async Task ThenAtLeastOneProductShouldBeDisplayedAfterPriceFiltering()
        {
            var productResultCount = await _productPage.GetProductResultCountAsync();
            Assert.That(productResultCount, Is.GreaterThan(0), "Expected at least one product after filtering, but found none.");
            _scenarioContext.Set(productResultCount, "ProductResultCount");
        }

        [Then(@"each checked product detail should fall within the price range greater than ""(.*)""")]
        public async Task ThenEachCheckedProductDetailShouldFallWithinThePriceRangeGreaterThan(string selectedPrice)
        {
            var productResultCount = _scenarioContext.Get<int>("ProductResultCount");
            var productsToCheck = Math.Min(productResultCount, 3);

            for (var i = 0; i < productsToCheck; i++)
            {
                await _productPage.ClickProductItemByIndexAsync(i);
                var priceOnDetailProduct = await _productDetailPage.GetPriceOnDetailProductItemAsync();
                Assert.That(priceOnDetailProduct, Is.GreaterThan(200.00), $"Expected product at index {i} to be within the price range {selectedPrice}, but it is not.");
                await Page.GoBackAsync();
                await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            }
        }
    }
}