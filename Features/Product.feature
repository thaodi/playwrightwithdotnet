Feature: Product Page
    Testing the Product feature of application

	Background: 
		Given I navigate to the Product page

	@productPage
	Scenario: Verify Product Page Is Displayed Correctly
		Then The product item should be displayed correctly with complete details

	@searchProduct
	Scenario Outline: Search Product By Keyword
		When I enter the product name "<keyword>" in the search field
		Then the system should display <numberOfProductsExpected> suggestion items for keyword "<keyword>"
		Then the product suggestion item should display with correct details keyword "<keyword>"
		When I click on the view all results button based on keyword "<keyword>"
		Then the product search result should be verified based on keyword "<keyword>" and count <numberOfProductsExpected>
	Examples: 
		| keyword   | numberOfProductsExpected |
		| Automatic |                        2 |
		| Air       |                        9 |
		| blender   |                        2 |

	@searchProduct
	Scenario Outline: Search Non-Existing Product Shows No Products Found Message
		When I enter the product name "<keyword>" in the search field
		Then the system should display <numberOfProductsExpected> suggestion items for keyword "<keyword>"
		And the no products found suggestion message should be visible with text "<suggestionMessage>"
		When I press enter on the search field
		Then the product search result URL should be verified based on keyword "<keyword>"
		And the search results heading for "<keyword>" should be visible
		And the product search result count should show <numberOfProductsExpected> products
		And the no products found label should be visible
	Examples: 
		| keyword | numberOfProductsExpected | suggestionMessage  |
		| abc     |                        0 | No products found  |

	@sortProduct
	Scenario Outline: Sort Product
		Given I navigate to the Product page
		When I open the sort dropdown and select option "<sortOption>"
		Then the total number of sort options should be <numberOfSortOptions>
		And the product list should be sorted correctly based on "<sortOption>"

		Examples:
		  | sortOption       | numberOfSortOptions |
		  | Price (high-low) |                   8 |
		  | Name (A-Z)       |                   8 |
		  | Name (Z-A)       |                   8 |

	@filterProduct
	Scenario Outline: Filter Products By Color
		Given I navigate to the Product page
		When I click the filter color button
		Then the total number of available color options should be <expectedNumberOption>
		When I select the color options "<colors>"
		And I close the color filter menu
		Then the selected color count on the filter button should be <expectedSelectedColorCount>
		And the selected color tags should be displayed correctly as "<colors>"
		And at least one product should be displayed after color filtering
		And each checked product detail should contain at least one of the selected colors "<colors>"

		Examples:
		  | colors                                  | expectedSelectedColorCount | expectedNumberOption |
		  | Rose Gold, Silver, Mint                 |             3				 |                   19 |
		  | Blue and White, Black and Red, Graphite |             3              |                   19 |

	@filterProduct
	Scenario Outline: Filter Product By Price
		When I open the price filter menu
		Then the system should display <priceOptionCount> price options
		When I select the price range option "<priceRange>"
		When I close the price filter menu
		Then the selected price count should be <expectedPriceCount>
		And the selected price tag should be displayed correctly as "Price: <priceRange>"
		And at least one product should be displayed after price filtering
		And each checked product detail should fall within the price range greater than "<priceRange>"

		Examples:
		| priceRange | priceOptionCount | expectedPriceCount |
		| $200+      |                4 |                  1 | 

	@filterProduct
		Scenario Outline: Filter Product By Availability
			When I open the availability filter menu
			Then the system should display "<expectedOptionCount>" availability options
			When I select the availability option "<selectedAvailability>"
			When I close the availability filter menu
			Then the product filtering result by availability should be verified based on expectation "<hasProductsExpected>" and count "<numberOfProductsExpected>"
			And the selected availability tags should be displayed correctly as "<selectedAvailability>"

		Examples:
			| expectedOptionCount | selectedAvailability | hasProductsExpected | numberOfProductsExpected |
			| 3                   | In Stock             | true                |                       36 |
			| 3                   | Out of Stock         | false               |                        0 |