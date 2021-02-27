
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

  Scenario: Writing an Add entry
    Given a repository for Demo
    When adding Demo with key "journaled" and value 2
    # When adding storable with key "bar2"
    Then the journal is not empty

  # Scenario: Reading an Add entry
  #   Given a store with journal "{}"
  #   Then fetching storable with key "foo" returns storable

