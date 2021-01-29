
Feature: SceneParser

  Scenario: Extract name from scene
    Given a scene with body
      """
$ name: it 'as a name, though!
this is a first scene
it's just a string.
      """
    Then the scene has a name of "it 'as a name, though!"
