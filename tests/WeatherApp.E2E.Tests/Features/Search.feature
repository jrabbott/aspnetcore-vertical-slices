Feature: Search weather
  Visitors can look up current conditions for a known city.

  Scenario: Search shows stubbed current weather
    Given I am on the search page
    When I search for "London"
    Then I should see current weather for "London"

  Scenario: Search result links to forecast
    Given I am on the search page
    When I search for "London"
    And I open the forecast from the search result
    Then I should see a forecast for "London"
