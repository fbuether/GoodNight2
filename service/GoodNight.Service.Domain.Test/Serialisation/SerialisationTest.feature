Feature: Domain.Serialisation

  Scenario: Serialising a simple story
    Given the serialisation {"Name":"Eine neue Geschichte!","Scenes":[],"Qualities":[],"Urlname":"eine-neue-geschichte-","Key":"eine-neue-geschichte-"}
    Given the type "GoodNight.Service.Domain.Model.Write.Story, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
    When deserialising and serialising
    Then the new serialisation equals given input

  Scenario: Serialising a Read.Scene.Content.Text
    Given the serialisation {"type":"text","value":"okay."}
    Given the type "GoodNight.Service.Domain.Model.Read.Scene+Content, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
    When deserialising and serialising
    Then the new serialisation equals given input

  Scenario: Serialising a Read.Scene.Content.Effect
    Given the serialisation {"type":"effect","quality":{"typeName":"GoodNight.Service.Domain.Model.Read.Quality, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","key":"stooory/qualityname"},"expression":{"type":"number","Number":3}}
    Given the type "GoodNight.Service.Domain.Model.Read.Scene+Content, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
    When deserialising and serialising
    Then the new serialisation equals given input

  Scenario: Serialising a Model.Read.Scene
    Given the serialisation {"Name":"Start","Story":"eine-neue-geschichte-","IsStart":true,"ShowAlways":false,"ForceShow":false,"Contents":[{"type":"text","value":"This is where the story starts."},{"type":"option","urlname":"pfad1/4","description":"hier ist option fuer Pfad 1.\n\n\nmal sehen, ob der Abs\u00E4tze kann.","icon":"","requirements":[],"effects":[{"Item1":{"typeName":"GoodNight.Service.Domain.Model.Read.Quality, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","key":"eine-neue-geschichte-/woah"},"Item2":{"type":"number","Number":174}}],"scene":{"typeName":"GoodNight.Service.Domain.Model.Read.Scene, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","key":"eine-neue-geschichte-/pfad1"}},{"type":"option","urlname":"boarzba/5","description":"Oder sonst auch Barboza. Was auch immer.","icon":"","requirements":[],"effects":[],"scene":{"typeName":"GoodNight.Service.Domain.Model.Read.Scene, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","key":"eine-neue-geschichte-/boarzba"}}],"Urlname":"start","Key":"eine-neue-geschichte-/start"}
    Given the type "GoodNight.Service.Domain.Model.Read.Scene, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
    When deserialising and serialising
    Then the new serialisation equals given input

  Scenario: Serialising a shorter Model.Read.Scene
    Given the serialisation {"Name":"baa","Story":"baa","IsStart":false,"ShowAlways":false,"ForceShow":false,"Contents":[{"type":"effect","quality":{"typeName":"GoodNight.Service.Domain.Model.Read.Quality, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","key":"refreferf"},"expression":{"type":"number","Number":174}}],"Urlname":"baa","Key":"baa/baa"}
    Given the type "GoodNight.Service.Domain.Model.Read.Scene, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
    When deserialising and serialising
    Then the new serialisation equals given input

  Scenario: Serialising a Read.Scene.Content.Option
    Given the serialisation {"type":"option","urlname":"urlname","description":"description","icon":"icon","requirements":[],"effects":[{"Item1":{"typeName":"GoodNight.Service.Domain.Model.Read.Quality, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","key":"refreferf"},"Item2":{"type":"number","Number":174}}],"scene":{"typeName":"GoodNight.Service.Domain.Model.Read.Scene, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","key":"sceenenene"}}
    Given the type "GoodNight.Service.Domain.Model.Read.Scene+Content, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
    When deserialising and serialising
    Then the new serialisation equals given input

  Scenario: Serialising a Model.Expressions.Expression.Number
    Given the serialisation {"type":"number","Number":174}
    Given the type "GoodNight.Service.Domain.Model.Expressions.Expression`1[[GoodNight.Service.Storage.Interface.IReference`1[[GoodNight.Service.Domain.Model.Read.Quality, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], GoodNight.Service.Storage.Interface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
    When deserialising and serialising
    Then the new serialisation equals given input

  Scenario: Serialising a Model.Read.Choice.Action
    Given the serialisation {"User":"00000000-0000-0000-0000-000000000000","Number":2,"Scene":{"typeName":"GoodNight.Service.Domain.Model.Read.Scene, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","key":"eine-neue-geschichte-/start"},"Text":"This is where the story starts.","Effects":[{"Quality":{"typeName":"GoodNight.Service.Domain.Model.Read.Quality, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","key":"eine-neue-geschichte-/woah"},"Value":{"type":"int","iValue":61}}],"Chosen":{"Kind":"Action","Urlname":"pfad1/4","Text":"hier ist option fuer Pfad 1.\n\n\nmal sehen,ob der Absaetze kann.","Icon":"","Effects":[{"Quality":{"typeName":"GoodNight.Service.Domain.Model.Read.Quality, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","key":"eine-neue-geschichte-/woah"},"Value":{"type":"int","iValue":16}}]},"Key":"00000000-0000-0000-0000-000000000000/eine-neue-geschichte--start/2"}
    Given the type "GoodNight.Service.Domain.Model.Read.Log, GoodNight.Service.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
    When deserialising and serialising
    Then the new serialisation equals given input
