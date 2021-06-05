
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
      Klettere vorsichtig die steile Treppe hinauf.
      $end

      $option: Spüre oben nach Leben
      $require: "Gespür für Leben" > 5 and Atem > 4
      Fokussiere dich auf deinen Atem und spüre nach Leben über dir.
      $end
      """
    When the parser parses the input
    Then parsing succeeds


  Scenario: Another complex scene with nested ifs
    Given the scene input
      """
      $name: Einfaches Ereignis: Verteiler
      $category: Ereignisse/Einfach

      $set: Der Wald = rand 1 3

      $if: "Der Wald" = 1 and !"Spuren gefunden"
      $include: Spuren Start
      $else
      $if: "Der Wald" = 2 and !"Hasen getroffen"
      $include: Hase Start
      $else
      $include: Blutbuche
      $end
      $end
      """
    When the parser parses the input
    Then parsing succeeds


  Scenario: A scene with just text
    Given the scene input
      """
      this scene has just text.
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 1 node
    Then the result has only "Text" nodes
    Then the node 1 has text "this scene has just text."

  Scenario: A scene with several lines of text
    Given the scene input
      """
      this scene
      has several lines
      of text.
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 3 nodes
    Then the result has only "Text" nodes
    Then the node 1 has text "this scene"
    Then the node 2 has text "has several lines"
    Then the node 3 has text "of text."

  Scenario: A scene with one empty line
    Given the scene input
      """
      this scene has several

      lines of text.
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 3 nodes
    Then the result has only "Text" nodes
    Then the node 1 has text "this scene has several"
    Then the node 2 has text ""
    Then the node 3 has text "lines of text."

  Scenario: A scene with several empty lines
    Given the scene input
      """
      this scene has several



      lines of text.
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 5 nodes
    Then the result has only "Text" nodes
    Then the node 1 has text "this scene has several"
    Then the node 2 has text ""
    Then the node 3 has text ""
    Then the node 4 has text ""
    Then the node 5 has text "lines of text."

  Scenario: A scene without any text
    Given the scene input
      """
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 0 nodes

  Scenario: A scene with just a single empty line
    Given the scene input
      """


      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 1 nodes
    Then the result has only "Text" nodes
    Then the node 1 has text ""

  Scenario: A simple name
    Given the scene input
      """
      $name:Entry into the depth
      """
      When the parser parses the input
      Then parsing succeeds
      Then the result has 1 nodes
      Then the result has only "Name" nodes
      Then the scene has name "Entry into the depth"

  Scenario: Spacing in name settings is allowed and names are trimmed
    Given the scene input
      """
      $   	   name	 	:	 		Entry into 		  the depth	   	 
      """
      When the parser parses the input
      Then parsing succeeds
      Then the result has 1 nodes
      Then the result has only "Name" nodes
      Then the scene has name "Entry into 		  the depth"

  Scenario: Names allow special characters
    Given the scene input
      """
      $name: cmofrhqmpQI_- !\0"1%^&2*(3)=4+[5]{6};7:'8@#~\9\|/a<>b,c.dfeftanof  RVP?NRCNHENEC
      """
      When the parser parses the input
      Then parsing succeeds
      Then the result has 1 nodes
      Then the result has only "Name" nodes
      Then the scene has name "cmofrhqmpQI_- !\0"1%^&2*(3)=4+[5]{6};7:'8@#~\9\|/a<>b,c.dfeftanof  RVP?NRCNHENEC"


  Scenario: Name settings require a colon
    Given the scene input
      """
      $name the name
      """
    When the parser parses the input
    Then parsing fails

  Scenario: Names do not allow even more special characters
    Given the scene input
      """
      $name: 😁
      """
      When the parser parses the input
      Then parsing fails

  Scenario: Names may be empty
    Given the scene input
      """
      $name:
      """
      When the parser parses the input
      Then parsing succeeds


  Scenario: A scene with a start setting
    Given the scene input
      """
      On this scene there is a:
      $ start
      """
    When the parser parses the input
    Then parsing succeeds
    Then the node 2 is a "IsStart"

  Scenario: A scene with a show always setting
    Given the scene input
      """
      $ always show
      $ show always
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 2 nodes
    Then the result has only "ShowAlways" nodes

  Scenario: A scene with a force show setting
    Given the scene input
      """
      $ force show
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 1 nodes
    Then the result has only "ForceShow" nodes


  Scenario: A scene with a single tag
    Given the scene input
      """
      $ tags: humpel
      And there is text too.
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 2 nodes
    Then the node 1 is a "Tag"
    Then the result has the tag "humpel"

  Scenario: A scene with a tag with multiple entries
    Given the scene input
      """
      $ tags: From Russia With Love, Goldfinger, You Only Live 2*
      And there is text too.
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has the tag "From Russia With Love"
    Then the result has the tag "Goldfinger"
    Then the result has the tag "You Only Live 2*"

  Scenario: A scene with multiple tag nodes
    Given the scene input
      """
      $ tags: You Only Live 2*, From Russia With Love
      $ tags: Goldfinger
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has the tag "From Russia With Love"
    Then the result has the tag "Goldfinger"
    Then the result has the tag "You Only Live 2*"

  Scenario: Tag names are trimmed around them, but not inside.
    Given the scene input
      """
      $ tags: 		   	From 		Russia 	With     Love		  	  
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has the tag "From 		Russia 	With     Love"

  Scenario: A single category
    Given the scene input
      """
      $ category: areas
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has the category "areas"

  Scenario: A single category may use the short form cat:
    Given the scene input
      """
      $ cat: areas
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has the category "areas"

  Scenario: Several categories are split
    Given the scene input
      """
      $ category: quest/Hildas Hammer
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has the category "quest/Hildas Hammer"

  Scenario: Very many categories are still split
    Given the scene input
      """
      $ cat: a/b/c/d/e/f/g/h/i/j/k/l/m/n
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has the category "a/b/c/d/e/f/g/h/i/j/k/l/m/n"


  Scenario: A set setting
    Given the scene input
      """
      $ set: Hildas Hammer = true
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has only "Set" nodes
    Then "Set" node 1 sets quality "Hildas Hammer"

  Scenario: Several set settings
    Given the scene input
      """
      $ set: Hildas Hammer = true
      $ set: Life = Life - Enemy Level
      $ set: Enraged = 10 - (Enemy Level * 2)
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has only "Set" nodes

  Scenario: A requirement
    Given the scene input
      """
      $ require: (Enraged > 3) and Hildas Hammer
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has only "Require" nodes


  Scenario: A scene with a return option
    Given the scene input
      """
      $ return: At the top of the mountain
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has only "Return" nodes

  Scenario: A scene with a continue option
    Given the scene input
      """
      $ continue: At the top of the mountain
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has only "Continue" nodes

  Scenario: A scene with an include
    Given the scene input
      """
      $ include: At the top of the mountain
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has only "Include" nodes

  Scenario: A simple conditional works for text.
    Given the scene input
      """
      $if: true
      some expression text.
      $end
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 1 node
    Then the node 1 is a "Condition"

  Scenario: Consecutive conditions
    Given the scene input
      """
      $if: true
      some expression text.
      $end
      $if: false
      some other text.
      $end
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 2 nodes
    Then the result has only "Condition" nodes

  Scenario: A condition with an else case
    Given the scene input
      """
      $ if: true or false
      some expression text.
      $ else
      nothing to see here.
      $ end
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 1 node
    Then the result has only "Condition" nodes

  Scenario: A condition with empty branches
    Given the scene input
      """
      $ if: false
      $ else
      $ end
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 1 node
    Then the result has only "Condition" nodes

  Scenario: A condition with a complex condition
    Given the scene input
      """
      $ if:   	 (true or ((false))) and (7 > 9 + 16) or not (Kildurs Hammer)
      $ else
      $ end
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 1 node
    Then the result has only "Condition" nodes

  Scenario: Nested conditions
    Given the scene input
      """
      $ if: false
      $ if: true
      $ if: false
      $ end
      $ else
      $end
      $ else
      $ if: 9 > 67
      $ end
      $ end
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 1 node
    Then the result has only "Condition" nodes
    Then the result has 4 "Condition" nodes in branches

  Scenario: Other settings in branches
    Given the scene input
      """
      $ if: false
      $ set: starving = true
      $ else
      $ set: Provisions = Provisions - 1
      $ set: starving = not Provisions >= 1
      $ end
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 3 "Set" nodes in branches

  Scenario: No other content after else marker
    Given the scene input
      """
      $ if: false
      $ else --
      $ end
      """
    When the parser parses the input
    Then parsing fails

  Scenario: No other content after end marker
    Given the scene input
      """
      $ if: false
      $ end --
      """
    When the parser parses the input
    Then parsing fails

  Scenario: More content on the next line after end
    Given the scene input
      """
      $ if: false
      $ end
      --
      """
    When the parser parses the input
    Then parsing succeeds


  Scenario: A Simple option with text
    Given the scene input
      """
      $ option: scenename
      text content
      $ end
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 1 node
    Then the result has only "Option" nodes

  Scenario: A Simple option with other text around
    Given the scene input
      """
      Hello, here!
      $ option: scenename
      text content
      $ end
      this, as well.
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 3 nodes
    Then the node 2 is a "Option"

  Scenario: Several options consecutively
    Given the scene input
      """
      $ option: scenename
      text content
      $ end
      $ option: something else
      other content
      $ end
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 2 nodes
    Then the result has only "Option" nodes

  Scenario: An option with nested settings
    Given the scene input
      """
      $ option: scenename
      $ require: cookies
      $ always show
      $ end
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 1 node
    Then the result has only "Option" nodes

  Scenario: A conditional with a nested option
    Given the scene input
      """
      $ if: not cookies
      $ option: scenename
      $ require: cookies
      it's optional.
      $ end
      $ end
      """
    When the parser parses the input
    Then parsing succeeds
    Then the result has 1 node
    Then the node 1 is a "Condition"
    Then the result has 1 "Option" nodes in branches
