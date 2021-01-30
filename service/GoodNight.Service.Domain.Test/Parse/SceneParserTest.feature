
Feature: SceneParser

  Scenario: A scene with just a simple name works
    Given the scene input
      """
      $name:the name
      """
    When the parser parses the scene
    Then the parsed scene has a name of "the name"

  Scenario: A name setting without semicolon is not accepted
    Given the scene input
      """
      $name the name
      """
    When the parser parses the scene
    Then parsing fails


  Scenario: A name setting without $ counts as text
    Given the scene input
      """
      name: the name
      """
    When the parser parses the scene
    Then the parsed scene has only text content


  Scenario: Name declarations can have spacing with space and tab
    Given the scene input
      """
      $  	  name 		 :  thisisthename  	
      """
    When the parser parses the scene
    Then the parsed scene has a name of "thisisthename"


  Scenario: A scene with just text works
    Given the scene input
      """
      This is just content text.
      """
    When the parser parses the scene
    Then the parsed scene has only text content


  Scenario: Extract name from scene
    Given the scene input
      """
      $ name: it 'as a name, though!
      this is a first scene
      it's just a string.
      """
    When the parser parses the scene
    Then the parsed scene has a name of "it 'as a name, though!"


  Scenario: Empty lines count as text
    Given the scene input
      """
      Line 1

      Line 3
      """
    When the parser parses the scene
    Then parsing succeeds
    Then the parsed scene has only text content


  Scenario: An empty body parses successfully
    Given the scene input
      """
      """
    When the parser parses the scene
    Then parsing succeeds
