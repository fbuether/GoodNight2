
Feature: StoryNames
  As the client
  I can generate Url Names for Stories,
  so that I can use those in Urls.

  Scenario: Generate simple Url Name
    Given a scenario named "Helms Schlund"
    When I generate the urlname
    Then the urlname should be helms-schlund

  Scenario: Generate more complex Url Name
    Given a scenario named "Wait -- is pete around? and where?"
    When I generate the urlname
    Then the urlname should be wait----is-pete-around--and-where-

  Scenario: Tackle special chars in Url Names
    Given a scenario named " !%^!£%()//oHOMY"
    When I generate the urlname
    Then the urlname should be -----------ohomy

