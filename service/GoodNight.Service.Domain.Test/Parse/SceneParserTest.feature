
Feature: SceneParser

  Scenario: A full scenario can be parsed.
    Given the scene input
      """
      $name: Unten vor der Kellertreppe
      $require: "Licht im Keller"

      Du stehst vor der Treppe, die in das eigentliche Gebäude hinaufführt.
      $if: "hat Schlüssel"
      Vorsichtig wiegst du den schweren, eisenen Schlüssel in deiner Hand.
      $end

      Du weisst, dass du oben dein Ziel finden wirst.

      $return: Im Keller
      $category: quest/Salars Keller

      $option: Gehe die Treppe hoch

      $if: "Gespür für Leben" > 5 and Atem > 4
      $option: Spüre oben nach Leben
      $end
      """
    When the parser parses the input
    Then parsing succeeds


  Scenario: A simple conditional works for text.
    Given the scene input
      """
      $if: true
      some expression text.
      $end
      """
    When the parser parses the input
    Then parsing succeeds

  Scenario: A scene with just a simple name works
    Given the scene input
      """
      $name:the name
      """
    When the parser parses the input
    Then the parsed scene has a name of "the name"

  Scenario: A name setting without semicolon is not accepted
    Given the scene input
      """
      $name the name
      """
    When the parser parses the input
    Then parsing fails


  Scenario: A name setting without $ counts as text
    Given the scene input
      """
      name: the name
      """
    When the parser parses the input
    Then the parsed scene has only text content


  Scenario: Name declarations can have spacing with space and tab
    Given the scene input
      """
      $  	  name 		 :  thisisthename  	
      """
    When the parser parses the input
    Then the parsed scene has a name of "thisisthename"


  Scenario: A scene with just text works
    Given the scene input
      """
      This is just content text.
      """
    When the parser parses the input
    Then the parsed scene has only text content


  Scenario: Extract name from scene
    Given the scene input
      """
      $ name: it 'as a name, though!
      this is a first scene
      it's just a string.
      """
    When the parser parses the input
    Then the parsed scene has a name of "it 'as a name, though!"


  Scenario: Empty lines count as text
    Given the scene input
      """
      Line 1

      Line 3
      """
    When the parser parses the input
    Then parsing succeeds
    Then the parsed scene has only text content


  Scenario: An empty body parses successfully
    Given the scene input
      """
      """
    When the parser parses the input
    Then parsing succeeds

