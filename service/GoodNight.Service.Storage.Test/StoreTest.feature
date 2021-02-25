
Feature: Store

  Scenario: Getting a non-existing item
    Given a store
    Then fetching storable with key "foo" returns null

  Scenario: Adding an item can fetch it later
    Given a store
    When adding storable with key "foo"
    Then fetching storable with key "foo" returns storable

  Scenario: Writing an Add entry
    Given a store
    When adding storable with key "foo"
    # When adding storable with key "bar2"
    Then the journal is not empty

  # Scenario: Reading an Add entry
  #   Given a store with journal "{}"
  #   Then fetching storable with key "foo" returns storable

