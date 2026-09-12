Feature: Product Detail Page
	Testing the Product Detail feature of application

	Background: 
			Given I navigate to the Product page

	@productDetail
		Scenario: Verify Product Detail Page Is Displayed Correctly
		When I verify details for all products defined in the JSON data file
		Then the product information should match with the JSON data file