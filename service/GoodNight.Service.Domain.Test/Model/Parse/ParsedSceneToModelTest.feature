
Feature: Parse.Scene to Model.Scene

  Scenario: A scene with nothing can be converted
    Given the raw content
      """
      """
    When converting to a Write.Scene
    Then the Write.Scene has Name ""
    Then the Write.Scene is not start
    Then the Write.Scene is not always shown
    Then the Write.Scene is not forced to show
    Then the Write.Scene has no tags
    Then the Write.Scene has category ""
    Then the Write.Scene has no sets
    Then the Write.Scene has no return
    Then the Write.Scene has no continue
    Then the Write.Scene has no content

  Scenario: A scene with just text can be converted
    Given the text content "something happens"
    Given the text content "it is suprising!"
    When converting to a Write.Scene
    Then the Write.Scene has 1 content node
    Then content 0 of Write.Scene is text
      """
      something happens
      it is suprising!
      """
