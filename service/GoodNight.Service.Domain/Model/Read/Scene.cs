using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Model.Read.Error;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Read
{
  using Expression = Expression<IReference<Quality>>;

  /// <summary>
  /// A Scene is one unit of a Story. It represents one step in the Adventure
  /// that a player may take. It is not materialised and contains the
  /// information to adopt itself to a specific Player.
  /// </summary>
  public record Scene(
    string Name,
    string Story, // the urlname of the story.
    bool IsStart,
    bool ShowAlways,
    bool ForceShow,
    IImmutableList<Scene.Content> Contents)
    : IStorable<Scene>
  {
    public interface Content
    {
      public record Text(
        string Value)
        : Content
      {
        public Action AddTo(Player player, Random rnd, Action action) {
          var replaced = Content.ReplacePlaceholders(player, Value);
          return action with { Text = action.Text == ""
              ? replaced
              : action.Text + "\n" + replaced };
        }
      }

      public record Effect(
        IReference<Quality> Quality,
        Expression Expression)
        : Content
      {
        public Action AddTo(Player player, Random rnd, Action action) =>
          action with {
          Effects = action.Effects.Add(new Property(Quality,
              Expression.Evaluate(player.GetValueOf, rnd)))
        };

        public override string ToString()
        {
          return $"Effect {{Quality:{Quality}, Expression:{Expression}}}";
        }
      }

      /// <summary>
      /// An Option the Player may take here. May only occur in regular Scenes.
      ///
      /// The Urlname may be either the regular Scene name pointed towards.
      /// For inline-options, this will create a virtual Scene that is named
      /// here.
      /// The linked Scene will be used to fill this Option.
      /// </summary>
      public record Option(
        string Urlname,
        IImmutableList<Content> Scene)
        : Content
      {
        public Action AddTo(Player player, Random rnd, Action action)
        {
          var requirements = ImmutableList.CreateRange(
            Requirements.Select(expression => {
              var value = expression.Evaluate(player.GetValueOf, rnd);
              if (value is Value.Bool bValue)
              {
                return new Requirement(expression, bValue.Value);
              }
              else
              {
                throw new TypeError(
                  $"Scene Requirement \"{expression}\" does not evaluate to "
                  + $"bool. Result: \"{value}\", Option: {this}");
              }
            }));

          var effects = Effects.Select(qe => {
            var (quality, expression) = qe;
            return new Property(quality,
              expression.Evaluate(player.GetValueOf, rnd));
          });

          return action with {
            Options = action.Options.Add(new Read.Option(Urlname,
                ReplacePlaceholders(player, Description), Icon,
                requirements.All(r => r.Passed), requirements,
                ImmutableList.CreateRange(effects), Scene))
              };
        }

        public override string ToString()
        {
          string reqs = string.Join(", ", Requirements.Select(r => r.ToString()));
          var effs = string.Join(", ", Effects.Select(r => r.ToString()));

          return $"Option {{Urlname:{Urlname}, Description:{Description}, "
            + $"Icon:{Icon}, "
            + "Requirements:[" + reqs + "], "
            + "Effects:[" + effs + "], "
            + $"Scene:{Scene}}}";
        }
      }


      /// <summary>
      /// Returns to a previous Scene. This may only occur once on each
      /// manifestation of a Scene.
      /// </summary>
      public record Return(
        IReference<Scene> Scene)
        : Content
      {
        public Action AddTo(Player player, Random rnd, Action action)
        {
          if (action.Return is not null)
            throw new InvalidSceneException(
              $"Scene \"{Scene.Key}\" contains multiple returns.");

          return action with { Return = Scene };
        }
      }


      /// <summary>
      /// Continues to a next Scene. This may only occur once on each
      /// manifestation of a Scene.
      /// </summary>
      public record Continue(
        IReference<Scene> Scene)
        : Content
      {
        public Action AddTo(Player player, Random rnd, Action action)
        {
          if (action.Continue is not null)
            throw new InvalidSceneException(
              $"Scene \"{Scene.Key}\" contains multiple continues.");

          return action with { Continue = Scene };
        }
      }


      /// <summary>
      /// Adds Content if a specific condition holds or does not hold.
      /// One of the branches may be an empty list if no Content exists.
      /// </summary>
      public record Condition(
        Expression If,
        IImmutableList<Content> Then,
        IImmutableList<Content> Else)
        : Content
      {
        public Action AddTo(Player player, Random rnd, Action action)
        {
          var cond = If.Evaluate(player.GetValueOf, rnd);
          if (cond is Value.Bool(var b))
          {
            var additonalContent = b ? Then : Else;
            return ((Content)this).OnAction(player, rnd, action,
              additonalContent);
          }
          else
          {
            throw new TypeError($"Invalid condition {cond}");
          }
        }
      }


      /// <summary>
      /// Includes the content of another Scene.
      /// This inserts the Content of the other Scene at exactly this position,
      /// considering e.g. a Condition.
      /// It does *not* transitively include Includes in the other Scene's
      /// Content.
      /// </summary>
      public record Include(
        IReference<Scene> Scene)
        : Content
      {
        public Action AddTo(Player player, Random rnd, Action action)
        {
          var scene = Scene.Get();
          if (scene is null)
            throw new InvalidSceneException(
              $"Scene {Scene.Key} does not exist.");

          return ((Content)this).OnAction(player, rnd, action, scene.Contents);
        }
      }


      private static string ReplacePlaceholders(Player player, string text)
      {
        var placeholder = @"\${(\w+)(,(\w+))?}";
        return Regex.Replace(text, placeholder, (Match match) => {
          string expr = match.Groups[1].Value;
          string op = match.Groups[3].Value;

          string replaced = expr switch {
            "name" => player.Name,
            _ => expr
          };

          return op switch {
            "upper" or "uppercase" => replaced.ToUpper(),
            "lower" or "lowercase" => replaced.ToLower(),
            _ => replaced
          };
        });
      }


      protected Action OnAction(Player player, Random rnd, Action action,
        IImmutableList<Content> content) =>
        content.Aggregate(action,
          (action, content) => content.AddTo(player, rnd, action));

      /// <summary>
      /// Apply this Content to a Player and a partial Action, yielding a
      /// more complete Action representing the current Scene.
      /// </summary>
      public abstract Action AddTo(Player player, Random rnd, Action action);
    }


    public string Urlname => NameConverter.OfString(Name);

    public string Key => NameConverter.Concat(Story, Urlname);

    /// <summary>
    /// Play this scene onto a given Player.
    /// This computes the effects that this Scene has onto a specific player,
    /// resulting in the Action that the Player has taken.
    /// </summary>
    public Action Play(Player player, int rndSeed)
    {
      var rnd = new Random(rndSeed);

      var action = new Action(this, "",
        ImmutableList<Property>.Empty,
        ImmutableList<Option>.Empty, null, null);
      return Contents.Aggregate(action,
        (action, content) => content.AddTo(player, rnd, action));
    }

    public override string ToString()
    {
      var contents = Contents != null
        ? "[" + string.Join(", ", Contents.Select(c => c.ToString())) + "]"
        : "<null>";

      return $"Scene {{ Name:{Name}, Story:{Story}, IS:{IsStart}, "
        + $"SA:{ShowAlways}, FS:{ForceShow}, Contents: {contents}}}";
    }
  }
}
