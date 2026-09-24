@a11y
Feature: Accessibility
  Key pages meet axe WCAG 2.1 A/AA checks.

  Scenario: Search page has no accessibility violations
    Given I am on the search page
    Then the page should have no accessibility violations

  Scenario: Search results have no accessibility violations
    Given I am on the search page
    When I search for "London"
    Then I should see current weather for "London"
    And the page should have no accessibility violations

  Scenario: Forecast page has no accessibility violations
    Given I am on the forecast page
    When I request a 3-day forecast for "London"
    Then I should see a forecast for "London"
    And the page should have no accessibility violations

  Scenario: Favourites page has no accessibility violations
    Given I am on the favourites page
    Then the page should have no accessibility violations
