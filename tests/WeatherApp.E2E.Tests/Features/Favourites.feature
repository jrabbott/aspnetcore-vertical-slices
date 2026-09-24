Feature: Favourites
  Visitors can keep a short list of cities in session storage.

  Scenario: Add and remove a favourite city
    Given I am on the favourites page
    When I add "London" as a favourite
    Then I should see "London" in my favourites
    When I remove the favourite "London"
    Then I should not see "London" in my favourites
