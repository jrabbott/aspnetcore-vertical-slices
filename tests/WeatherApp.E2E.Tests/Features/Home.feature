Feature: Home navigation
  Visitors should land on search from the site root.

  Scenario: Root redirects to search
    Given I open the weather app
    Then I should be on the search page
