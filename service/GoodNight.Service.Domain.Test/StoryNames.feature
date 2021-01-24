
Feature: StoryNames
  As the client
  I can generate Url Names for Stories,
  so that I can use those in Urls.

  Scenario: Generate simple Url Name
    Given a scenario named "Helms Schlund"
    When I generate the urlname
    Then the urlname should be helms-schlund

