
Feature: Parsing of Qualities

  Scenario: A full quality
    Given the quality input
      """
      $ name: "Hilda's Hammer"
      Hilda works the smithy wielding the mightiest hammer that ever existed.

      And now you carry this hammer.

      $ type: bool
      """
    When the parser parses the input
    Then parsing succeeds


  Scenario: A quality of only text can be parsed.
    Given the quality input
      """
      This is the text.
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has description "This is the text."
    Then the quality has name ""


  Scenario: A quality may have a plain name
    Given the quality input
      """
      $name: Cunning
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has name "Cunning"

  Scenario: A quality may have a name with spaces
    Given the quality input
      """
      $name: Water under the bridge
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has name "Water under the bridge"

  Scenario: A quality may have a quoted name
    Given the quality input
      """
      $name: "Blueberry Pie"
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has name "Blueberry Pie"

  Scenario: A quality may have a quoted name with special characters
    Given the quality input
      """
      $name: "_- !\"%^&*()=+[]{}`;:'@#~|<>.?	\/,"
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has name "_- !\"%^&*()=+[]{}`;:'@#~|<>.?	\/,"


  Scenario: A quality of default type
    Given the quality input
      """
      this is just a quality
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has type Bool

  Scenario: A quality of type bool
    Given the quality input
      """
      $type: bool
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has type Bool

  Scenario: A quality of type Bool
    Given the quality input
      """
      $type: Bool
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has type Bool

  Scenario: A quality of type boolean
    Given the quality input
      """
      $type: boolean
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has type Bool

  Scenario: A quality of type int
    Given the quality input
      """
      $type: int
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has type Int

  Scenario: A quality of type integer
    Given the quality input
      """
      $type: integer
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has type Int

  Scenario: A quality of type Integer
    Given the quality input
      """
      $type: Integer
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has type Int

  Scenario: A quality of type enum
    Given the quality input
      """
      $type: enum
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has type Enum

  Scenario: A quality that is default hidden
    Given the quality input
      """
      Oh my, am i there?
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality is not hidden

  Scenario: A quality that is explicitly hidden
    Given the quality input
      """
      Oh my, am i there?
      $hidden: true
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality is hidden

  Scenario: A quality that is explicitly not hidden
    Given the quality input
      """
      Oh my, am i there?
      $hidden: false
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality is not hidden


  Scenario: A quality that has no scene name
    Given the quality input
      """
      Well, I am pretty to look at.
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has no scene

  Scenario: A quality that has a plain scene name
    Given the quality input
      """
      Well, I am pretty to look at:
      $ scene: somewhere nice
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has scene somewhere nice


  Scenario: An empty scene
    Given the quality input
      """
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has description ""
    Then the quality has name ""
    Then the quality has no scene
    Then the quality has type Bool
    Then the quality is not hidden

  Scenario: An enum quality with a level description
    Given the quality input
      """
      $type: enum
      $level 1: Have you thought about it?
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has type Enum
    Then the quality has level 1 with text Have you thought about it?

  Scenario: An enum quality with several level descriptions
    Given the quality input
      """
      $type: enum
      $level 1: Empty
      $level 2: Yawning
      $level 3: Imposing
      $level 4: In use
      $level 5: Stuffy
      """
    When the parser parses the input
    Then parsing succeeds
    Then the quality has type Enum
    Then the quality has level 1 with text Empty
    Then the quality has level 2 with text Yawning
    Then the quality has level 3 with text Imposing
    Then the quality has level 4 with text In use
    Then the quality has level 5 with text Stuffy

  Scenario: An int quality with a range
    Given the quality input
      """
      $type: int
      $min: 5
      $max: 9
      """
    When the parser parsers the input
    Then parsing succeeds
    Then the quality has type Int
    Then the quality has minimum 5
    Then the quality has maximum 9



 
