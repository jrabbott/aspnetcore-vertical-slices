Feature: Forecast
  Visitors can request a multi-day forecast for a known city.

  Scenario: Forecast shows stubbed daily weather
    Given I am on the forecast page
    When I request a 3-day forecast for "London"
    Then I should see a forecast for "London"
