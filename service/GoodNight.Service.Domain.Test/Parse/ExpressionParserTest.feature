Feature: ExpressionParser

  Scenario: Parse non-spaced quality names
    Given the input qualityname
    When the parser parses the input
    Then parsing succeeds
    Then the result is Quality "qualityname"

  Scenario: Parse spaced quality names
    Given the input quality name
    When the parser parses the input
    Then parsing succeeds
    Then the result is Quality "quality name"

  Scenario: Parse several spaced quality names
    Given the input quality name with several parts
    When the parser parses the input
    Then parsing succeeds
    Then the result is Quality "quality name with several parts"

  Scenario: Parse quoted quality names
    Given the input "qualityname"
    When the parser parses the input
    Then parsing succeeds
    Then the result is Quality "qualityname"

  Scenario: Parse quoted, spaced quality names
    Given the input "quality name"
    When the parser parses the input
    Then parsing succeeds
    Then the result is Quality "quality name"

  Scenario: Parse quoted, reserved quality names
    Given the input "not"
    When the parser parses the input
    Then parsing succeeds
    Then the result is Quality "not"

  Scenario: Names are okay if starting with reserved keyword not
    Given the input notable
    When the parser parses the input
    Then parsing succeeds
    Then the result is Quality "notable"

  Scenario: Names are okay if starting with reserved keyword or
    Given the input orwellian
    When the parser parses the input
    Then parsing succeeds
    Then the result is Quality "orwellian"


  Scenario: Parse boolean constant true
    Given the input true
    When the parser parses the input
    Then parsing succeeds
    Then the result is true

  Scenario: Parse boolean constant false
    Given the input false
    When the parser parses the input
    Then parsing succeeds
    Then the result is false


  Scenario: Parse constant short numbers
    Given the input 7
    When the parser parses the input
    Then parsing succeeds
    Then the result is 7

  Scenario: Parse constant long numbers
    Given the input 77516
    When the parser parses the input
    Then parsing succeeds
    Then the result is 77516

  Scenario: Parse constant negative numbers
    Given the input -14
    When the parser parses the input
    Then parsing succeeds
    Then the result is -14


  Scenario: Parse ranges
    Given the input [4,13]
    When the parser parses the input
    Then parsing succeeds
    Then the result is [4,13]

  Scenario: Parse ranges with negative numbers
    Given the input [-4,-13]
    When the parser parses the input
    Then parsing succeeds
    Then the result is [-4,-13]

  Scenario: Parse ranges with sub expressions
    Given the input [(0),9+7]
    When the parser parses the input
    Then parsing succeeds
    Then the result is [0,9+7]


  Scenario: Parse application of unary not operator
    Given the input not true
    When the parser parses the input
    Then parsing succeeds
    Then the result is not true

  Scenario: Parse application of unary ! operator
    Given the input !true
    When the parser parses the input
    Then parsing succeeds
    Then the result is not true

  Scenario: Parse recursive application of unary operator
    Given the input not not true
    When the parser parses the input
    Then parsing succeeds
    Then the result is not not true


  Scenario: Parse application of binary operator to constants
    Given the input true and true
    When the parser parses the input
    Then parsing succeeds
    Then the result is true and true

  Scenario: Parse application of binary operator to quality names
    Given the input this and that
    When the parser parses the input
    Then parsing succeeds
    Then the result is Quality "this" and Quality "that"

  Scenario: Parse application of binary operator to constants and quality names
    Given the input false and that
    When the parser parses the input
    Then parsing succeeds
    Then the result is false and Quality "that"


  Scenario: Parse application of binary operator Add
    Given the input 7 + 9
    When the parser parses the input
    Then parsing succeeds
    Then the result is 7 + 9

  Scenario: Parse application of binary operator Sub
    Given the input 7 - 9
    When the parser parses the input
    Then parsing succeeds
    Then the result is 7 - 9

  Scenario: Parse application of binary operator Mult
    Given the input 7 * 9
    When the parser parses the input
    Then parsing succeeds
    Then the result is 7 * 9

  Scenario: Parse application of binary operator Div
    Given the input 7 / 9
    When the parser parses the input
    Then parsing succeeds
    Then the result is 7 / 9

  Scenario: Parse application of binary operator And
    Given the input true and false
    When the parser parses the input
    Then parsing succeeds
    Then the result is true and false

  Scenario: Parse application of binary operator Or
    Given the input true or false
    When the parser parses the input
    Then parsing succeeds
    Then the result is true or false

  Scenario: Parse application of binary operator Greater
    Given the input 7 > 9
    When the parser parses the input
    Then parsing succeeds
    Then the result is 7 > 9

  Scenario: Parse application of binary operator GreaterOrEqual
    Given the input 7 >= 9
    When the parser parses the input
    Then parsing succeeds
    Then the result is 7 >= 9

  Scenario: Parse application of binary operator Less
    Given the input 7 < 9
    When the parser parses the input
    Then parsing succeeds
    Then the result is 7 < 9

  Scenario: Parse application of binary operator LessOrEqual
    Given the input 7 <= 9
    When the parser parses the input
    Then parsing succeeds
    Then the result is 7 <= 9

  Scenario: Parse application of binary operator Equal
    Given the input 7 = 9
    When the parser parses the input
    Then parsing succeeds
    Then the result is 7 = 9

  Scenario: Parse application of binary operator NotEqual with !=
    Given the input 7 != 9
    When the parser parses the input
    Then parsing succeeds
    Then the result is 7 != 9

  Scenario: Parse application of binary operator NotEqual with <>
    Given the input 7 <> 9
    When the parser parses the input
    Then parsing succeeds
    Then the result is 7 != 9


  Scenario: Parse braces around a constant
    Given the input (true)
    When the parser parses the input
    Then parsing succeeds
    Then the result is true

  Scenario: Parse multiple braces around a constant
    Given the input (((true)))
    When the parser parses the input
    Then parsing succeeds
    Then the result is true

  Scenario: Parse braces around quality name
    Given the input (quality name)
    When the parser parses the input
    Then parsing succeeds
    Then the result is Quality "quality name"

  Scenario: Parse braces around unary application
    Given the input (not true)
    When the parser parses the input
    Then parsing succeeds
    Then the result is not true

  Scenario: Parse braces within unary application
    Given the input not (true)
    When the parser parses the input
    Then parsing succeeds
    Then the result is not true

  Scenario: Parse braces around and within unary application
    Given the input (not (true))
    When the parser parses the input
    Then parsing succeeds
    Then the result is not true

  Scenario: Parse braces around and within nested unary application
    Given the input (not ((not (true))))
    When the parser parses the input
    Then parsing succeeds
    Then the result is not not true


  Scenario: Parsing binary operators happens left-associated
    Given the input true and true and true
    When the parser parses the input
    Then parsing succeeds
    Then the result is (true and true) and true

  Scenario: Parsing nested and and or binds and stronger if left
    Given the input true and true or true
    When the parser parses the input
    Then parsing succeeds
    Then the result is (true and true) or true

  Scenario: Parsing nested and and or binds and stronger if right
    Given the input true or true and true
    When the parser parses the input
    Then parsing succeeds
    Then the result is true or (true and true)

  Scenario: Parsing constants with whitespace around
    Given the input  	   	  true  		  
    When the parser parses the input
    Then parsing succeeds

  Scenario: Parsing unary ops with whitespace around
    Given the input  	  not 	!  true  		  
    When the parser parses the input
    Then parsing succeeds

  Scenario: Parsing binary ops with whitespace around
    Given the input  	  true 	  	or 	   true  		  
    When the parser parses the input
    Then parsing succeeds

  Scenario: Parsing just single braces
    Given the input (true)
    When the parser parses the input
    Then parsing succeeds
    Then the result is true

  Scenario: Parsing single braces with just a tiny bit of whitespace around
    Given the input  (true)
    When the parser parses the input
    Then parsing succeeds
    Then the result is true

  Scenario: Parsing single braces with whitespace around
    Given the input  		(    true		 )	  	
    When the parser parses the input
    Then parsing succeeds
    Then the result is true

  Scenario: Parsing multiple nested braces with whitespace around
    Given the input  (		((    true		 )	 )) 	
    When the parser parses the input
    Then parsing succeeds
    Then the result is true

  Scenario: Parsing multiple branching braces with whitespace around
    Given the input  	(((  true )	 ) 	or 	 ((  not (  true  ))		  )  	)	  
    When the parser parses the input
    Then parsing succeeds
    Then the result is true or not true

  Scenario: Parsing succeeds on qualities with operators following
    Given the input here and not there
    When the parser parses the input
    Then parsing succeeds

  Scenario: Parsing succeeds on qualities with spaces and operators inbetween
    Given the input here or maybe there
    When the parser parses the input
    Then parsing succeeds



  Scenario: Parsing fails on qualities with keyword infix not
    Given the input this not that
    When the parser parses the input
    Then parsing fails

  Scenario: Parsing fails on qualities with prefix keyword or
    Given the input or that
    When the parser parses the input
    Then parsing fails

  Scenario: Parsing fails on just reserved keyword not
    Given the input not
    When the parser parses the input
    Then parsing fails

  Scenario: Parsing fails on just reserved keyword and
    Given the input and
    When the parser parses the input
    Then parsing fails


  Scenario: Parsing fails on unmatched opening brace
    Given the input (true
    When the parser parses the input
    Then parsing fails

  Scenario: Parsing fails on unmatched closing brace
    Given the input true)
    When the parser parses the input
    Then parsing fails

  Scenario: Parsing fails on nested, unmatched opening brace
    Given the input (not (true)
    When the parser parses the input
    Then parsing fails

  Scenario: Parsing fails on nested, unmatched closing brace
    Given the input (not true))
    When the parser parses the input
    Then parsing fails

  # at the very bottom, because it (rightly) confuses syntax highlighting
  Scenario: Un-even number of quotes
    Given the input not "something
    When the parser parses the input
    Then parsing fails

