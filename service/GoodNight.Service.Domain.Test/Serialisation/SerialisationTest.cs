using System;
using System.Collections;
using System.Collections.Generic;
using Gherkin.Ast;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using Xunit;
using System.Text.Json;
using System.Text;
using System.IO;
using GoodNight.Service.Domain.Serialisation.Expressions;
using GoodNight.Service.Domain.Serialisation.Read;
using System.Text.Json.Serialization;
using GoodNight.Service.Storage;
using GoodNight.Service.Storage.Serialisation;
using System.Collections.Immutable;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Test.Serialisation
{
  [FeatureFile("Serialisation/SerialisationTest.feature")]
  public class SerialisationTest : Xunit.Gherkin.Quick.Feature
  {
    private ITestOutputHelper output;

    public SerialisationTest(ITestOutputHelper output)
    {
      this.output = output;

      options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
      options.Converters.Add(new ExpressionConverterFactory());
      options.Converters.Add(new ExpressionValueConverter());
      options.Converters.Add(new LogChoiceConverter());
      options.Converters.Add(new QualityConverter());
      options.Converters.Add(new SceneContentConverter());
      options.Converters.Add(new PlayerConverter());

      var store = new Store(new JsonConverter[] {}, new MemoryStream());
      options.Converters.Add(new ReferenceConverterFactory(store));
    }

    private JsonSerializerOptions options = new JsonSerializerOptions();

    private string input = "";
    private System.Type type = typeof(string);
    private string serialised = "";


    [Given(@"the serialisation (.+)")]
    public void GivenTheSerialisationString(string serialised)
    {
      input = serialised;
    }

    [Given(@"the type ""(.+)""")]
    public void GivenTheTypeString(string typeName)
    {
      var newType = System.Type.GetType(typeName);
      Assert.NotNull(newType);
      type = newType!;
    }

    [When(@"deserialising and serialising")]
    public void WhenDeserialisingAndSerialising()
    {
//       {
//       var journal = new MemoryStream();
//       var newStore = new Store(new JsonConverter[] {}, journal);
//       var store = newStore;

//       var objP = new Domain.Model.Read.Player("playerayer",
//         ImmutableList.Create(
//             (
//               (store as IStore).Create<Domain.Model.Read.Quality>().GetReference("qualrefef"),
//               new Domain.Model.Expressions.Value.Int(10) as Domain.Model.Expressions.Value
//             )
// ));

//       var typee = objP.GetType();

//       Console.WriteLine($"------4----------- rendering {objP}/{typee}");

//       var stream2 = new MemoryStream();
//       var writer2 = new Utf8JsonWriter(stream2);
//       JsonSerializer.Serialize(writer2, objP, typee, options);
//       writer2.Flush();
//       stream2.Flush();
//       stream2.Seek(0, SeekOrigin.Begin);
//       var reader22 = new StreamReader(stream2, Encoding.UTF8);
//       Console.WriteLine($"------yields: {reader22.ReadToEnd()}");
//       }


      var bytes = Encoding.UTF8.GetBytes(input);
      var reader = new Utf8JsonReader(bytes);
      var obj = JsonSerializer.Deserialize(ref reader, type, options);

      output.WriteLine($"intermediate object: {obj}");

      var stream = new MemoryStream();
      var writer = new Utf8JsonWriter(stream);
      JsonSerializer.Serialize(writer, obj, type, options);
      writer.Flush();
      stream.Flush();
      stream.Seek(0, SeekOrigin.Begin);
      var reader2 = new StreamReader(stream, Encoding.UTF8);
      serialised = reader2.ReadToEnd();
    }

    [Then(@"the new serialisation equals given input")]
    public void ThenTheNewSerialisationEqualsGivenInput()
    {
      Assert.Equal(input, serialised);
    }
  }
}

