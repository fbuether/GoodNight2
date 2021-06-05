Feature: Model.Read.Scene

  Scenario: An empty scene can be played to yield an empty action
    When playing the scene
    Then the action has no text
    Then the action has no effects
    Then the action has no options
    Then the action has no return
    Then the action has no continue

  Scenario: A scene with text yields that text to the action
    Given the scene content text "something"
    When playing the scene
    Then the action has text "something"

  Scenario: A scene with two texts yields both texts to the action
    Given the scene content text "something"
    Given the scene content text "else"
    When playing the scene
    Then the action has text
      """
      something
      else
      """

  Scenario: A scene with an option requirement is not playable
    Given the option "abc" with requirement "aquality"
    When playing the scene
    Then the action has an option that is not available

  Scenario: A scene with an option requirement that the player has is playable
    Given the option "abc" with requirement "aquality"
    Given a player with "aquality"
    When playing the scene
    Then the action has an option that is available
