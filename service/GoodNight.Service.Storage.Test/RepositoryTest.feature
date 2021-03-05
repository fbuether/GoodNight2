
Feature: Store

  Scenario: Getting a non-existing item
    Given a repository for Demo
    Then getting key "foo" returns null

  Scenario: Adding an item can fetch it later
    Given a repository for Demo
    When adding Demo with key "foo" and value 4
    Then getting key "foo" returns Demo with value 4

  Scenario: Adding multiple items and fetching a specific one
    Given a repository for Demo
    When adding Demo with key "foo" and value 4
    When adding Demo with key "temporary" and value 13
    Then getting key "foo" returns Demo with value 4
    Then getting key "temporary" returns Demo with value 13

  Scenario: Adding multiple items and fetching a specific one in inverse order
    Given a repository for Demo
    When adding Demo with key "foo" and value 4
    When adding Demo with key "temporary" and value 13
    Then getting key "temporary" returns Demo with value 13
    Then getting key "foo" returns Demo with value 4

  Scenario: Adding multiple items and fetching an invalid one
    Given a repository for Demo
    When adding Demo with key "foo" and value 4
    When adding Demo with key "temporary" and value 13
    Then getting key "oh noes" returns null

  Scenario: Adding an item and updating it
    Given a repository for Demo
    When adding Demo with key "mutating" and value 7
    When updating Demo with key "mutating" to value 31
    Then getting key "mutating" returns Demo with value 31

  Scenario: Adding an item and removing it
    Given a repository for Demo
    When adding Demo with key "mutating" and value 7
    When deleting Demo with key "mutating"
    Then getting key "mutating" returns null


  Scenario: Reading an Add entry
    Given a store with journal and repository for Demo
      """
      {"repos":"Demo","kind":"Add","value":{"Key":"replayed","Value":7}}
      """
    Then getting key "replayed" returns Demo with value 7

  Scenario: Reading several Add entries
    Given a store with journal and repository for Demo
      """
      {"repos":"Demo","kind":"Add","value":{"Key":"replayed","Value":7}}
      {"repos":"Demo","kind":"Add","value":{"Key":"else","Value":17}}
      {"repos":"Demo","kind":"Add","value":{"Key":"or not?","Value":42}}
      """
    Then getting key "replayed" returns Demo with value 7
    Then getting key "else" returns Demo with value 17
    Then getting key "or not?" returns Demo with value 42
    Then getting key "not-replayed" returns null

  Scenario: Reading an Add as well as an Update
    Given a store with journal and repository for Demo
      """
      {"repos":"Demo","kind":"Add","value":{"Key":"replayed","Value":7}}
      {"repos":"Demo","kind":"Update","key":"replayed","value":{"Key":"replayed","Value":144}}
      """
    Then getting key "replayed" returns Demo with value 144

  Scenario: Reading an Add and a Delete
    Given a store with journal and repository for Demo
      """
      {"repos":"Demo","kind":"Add","value":{"Key":"replayed","Value":7}}
      {"repos":"Demo","kind":"Delete","key":"replayed"}
      """
    Then getting key "replayed" returns null


  Scenario: Writing an Add entry
    Given a repository for Demo
    When adding Demo with key "dem key" and value 2
    Then the journal is not empty
    Then the journal is this
      """
      {"repos":"Demo","kind":"Add","value":{"Key":"dem key","Value":2}}

      """

  Scenario: Writing an Add entry with escaped key
    Given a repository for Demo
    When adding Demo with key "dem\" key" and value 2
    Then the journal is not empty
    Then the journal is this
      """
      {"repos":"Demo","kind":"Add","value":{"Key":"dem\\\u0022 key","Value":2}}

      """

  Scenario: Writing an item and updating it
    Given a repository for Demo
    When adding Demo with key "mutating" and value 7
    When updating Demo with key "mutating" to value 31
    Then the journal is not empty
    Then the journal is this
      """
      {"repos":"Demo","kind":"Add","value":{"Key":"mutating","Value":7}}
      {"repos":"Demo","kind":"Update","key":"mutating","value":{"Key":"mutating","Value":31}}

      """

  Scenario: Writing an item and removing it
    Given a repository for Demo
    When adding Demo with key "mutating" and value 7
    When deleting Demo with key "mutating"
    Then the journal is not empty
    Then the journal is this
      """
      {"repos":"Demo","kind":"Add","value":{"Key":"mutating","Value":7}}
      {"repos":"Demo","kind":"Delete","key":"mutating"}

      """

  Scenario: Writing adding several items
    Given a repository for Demo
    When adding Demo with key "mutating" and value 7
    When adding Demo with key "more item" and value 71
    When adding Demo with key "something else?" and value 771
    Then the journal is not empty
    Then the journal is this
      """
      {"repos":"Demo","kind":"Add","value":{"Key":"mutating","Value":7}}
      {"repos":"Demo","kind":"Add","value":{"Key":"more item","Value":71}}
      {"repos":"Demo","kind":"Add","value":{"Key":"something else?","Value":771}}

      """
