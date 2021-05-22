
Feature: Read.SceneFactory

  Scenario: Parsing a scene with a story name
    Given the story "whatsitsname"
    Given a name "named"
    When building a scene
    Then building succeeds
    Then the scene has story name "whatsitsname"

  Scenario: Parsing a scene without name fails
    When building a scene
    Then building fails

  Scenario: A set of expression can be built
    Given a name "something"
    Given the story "storystory"
    Given a set of "qualityName" to 2
    When building a scene
    Then building succeeds
    Then content 1 is an effect of "qualityName" set to 2
