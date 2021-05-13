
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
    Then content 1 is a text with value "This is the text."

  Scenario: A quality may have a plain name
    Given the quality input
      """
      $name: Cunning
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 1 is a name with value "Cunning"

  Scenario: A quality may have a name with spaces
    Given the quality input
      """
      $name: Water under the bridge
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 1 is a name with value "Water under the bridge"

  Scenario: A quality name may not start with 'not' if it is not quoted.
    Given the quality input
      """
      huh.
      $name: not so cold thing
      $cat: toasty things
      $type: bool
      interesting!
      """
    When the parser parses the input
    Then parsing fails

  Scenario: A quality may have a quoted name
    Given the quality input
      """
      $name: "Blueberry Pie"
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 1 is a name with value "Blueberry Pie"

  Scenario: A quality may have a quoted name with special characters
    Given the quality input
      """
      $name: "_- !\%^&*()=+[]{}`;:'@#~|<>.?	\/,"
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 1 is a name with value "_- !\%^&*()=+[]{}`;:'@#~|<>.?	\/,"

  Scenario: A quality of type bool
    Given the quality input
      """
      $type: bool
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 1 is a type of bool

  Scenario: A quality of type Bool
    Given the quality input
      """
      $type: Bool
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 1 is a type of bool

  Scenario: A quality of type boolean
    Given the quality input
      """
      $type: boolean
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 1 is a type of bool

  Scenario: A quality of type int
    Given the quality input
      """
      $type: int
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 1 is a type of int

  Scenario: A quality of type integer
    Given the quality input
      """
      $type: integer
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 1 is a type of int

  Scenario: A quality of type Integer
    Given the quality input
      """
      $type: Integer
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 1 is a type of int

  Scenario: A quality of type enum
    Given the quality input
      """
      $type: enum
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 1 is a type of enum

  Scenario: A quality that is default hidden
    Given the quality input
      """
      Oh my, am i there?
      """
    When the parser parses the input
    Then parsing succeeds
    Then no content is hidden

  Scenario: A quality that is explicitly hidden
    Given the quality input
      """
      Oh my, am i there?
      $hidden
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 2 is hidden


  Scenario: A quality that has no scene name
    Given the quality input
      """
      Well, I am pretty to look at.
      """
    When the parser parses the input
    Then parsing succeeds
    Then no content is scene

  Scenario: A quality that has a plain scene name
    Given the quality input
      """
      Well, I am pretty to look at:
      $ scene: somewhere nice
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 2 is scene with name "somewhere nice"


  Scenario: An empty quality
    Given the quality input
      """
      """
    When the parser parses the input
    Then parsing succeeds
    Then there is 0 content

  Scenario: An enum quality with a level description
    Given the quality input
      """
      $type: enum
      $level 1: Have you thought about it?
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 1 is a type of enum
    Then content 2 is a level of number 1 and text "Have you thought about it?"

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
    Then content 1 is a type of enum
    Then content 2 is a level of number 1 and text "Empty"
    Then content 3 is a level of number 2 and text "Yawning"
    Then content 4 is a level of number 3 and text "Imposing"
    Then content 5 is a level of number 4 and text "In use"
    Then content 6 is a level of number 5 and text "Stuffy"

  Scenario: An int quality with a range
    Given the quality input
      """
      $type: int
      $min: 5
      $max: 9
      """
    When the parser parses the input
    Then parsing succeeds
    Then content 1 is a type of int
    Then content 2 is a minimum of 5
    Then content 3 is a maximum of 9
