
using System.Collections.Immutable;
using System.Linq;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Read
{
  using StoredQuality = IStorableReference<Quality, string>;
  using Expression = Expression<IStorableReference<Quality, string>>;

  public interface Content
  {
    public record Text(
      string Value)
      : Content
    {
      public Action AddTo(Player player, Action action) =>
        action with { Text = action.Text + Value };
    }

    public record Effect(
      StoredQuality Quality,
      Expression Expression)
      : Content
    {
      public Action AddTo(Player player, Action action) =>
        action with {
          Effects = action.Effects.Add(new Property(Quality,
            Expression.Evaluate(player.GetValueOf)))
        };
    }


    /// <summary>
    /// Adds an Option the Player may take here. It has a specific Urlname to
    /// make this Option uniquely chooseable, a descriptive text and optionally
    // an icon. A set of requirement Expressions may guard this Option. If
    /// chosen, it may have a set of Effects and a Scene to continue to.
    /// </summary>
    public record Option(
      string Urlname,
      string Description,
      string? Icon,
      IImmutableList<Expression> Requirements,
      IImmutableList<(StoredQuality, Expression)> Effects,
      IStorableReference<Scene, string> Scene)
      : Content
    {
      public Action AddTo(Player player, Action action)
      {
        var isAvailable = false;
        var requirements = Requirements.Select(expression => {
          var value = expression.Evaluate(player.GetValueOf);
          if (value is Value.Bool bValue) {
            return new Requirement(expression, bValue.Value);
          }
          else {
            throw new TypeError("Scene Requirement does not evaluate to bool.");
          }
        });

        var effects = Effects.Select(qe => {
          var (quality, expression) = qe;
          return new Property(quality, expression.Evaluate(player.GetValueOf));
        });

        return action with {
          Options = action.Options.Add(new Read.Option(Urlname,
            Description, Icon, isAvailable,
              ImmutableList.CreateRange(requirements),
              ImmutableList.CreateRange(effects), Scene))
        };
      }
    }


    /// <summary>
    /// Returns to a previous Scene. This may only occur once on each
    /// manifestation of a Scene.
    /// </summary>
    public record Return(
      IStorableReference<Scene, string> Scene)
      : Content
    {
      public Action AddTo(Player player, Action action)
      {
        if (action.Return is not null)
          throw new InvalidSceneError("Scene contains multiple returns.");

        return action with { Return = Scene };
      }
    }


    /// <summary>
    /// Continues to a next Scene. This may only occur once on each
    /// manifestation of a Scene.
    /// </summary>
    public record Continue(
      IStorableReference<Scene, string> Scene)
      : Content
    {
      public Action AddTo(Player player, Action action)
      {
        if (action.Continue is not null)
          throw new InvalidSceneError("Scene contains multiple continues.");

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
      public Action AddTo(Player player, Action action)
      {
        var cond = If.Evaluate(player.GetValueOf);
        if (cond is Value.Bool(var b))
        {
          var additonalContent = b ? Then : Else;
          return ((Content)this).OnAction(player, action, additonalContent);
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
      IStorableReference<Scene, string> Scene)
      : Content
    {
      public Action AddTo(Player player, Action action)
      {
        var scene = Scene.Get();
        if (scene is null)
          throw new InvalidSceneError($"Scene {Scene.Key} does not exist.");

        return ((Content)this).OnAction(player, action, scene.Content);
      }
    }


    protected Action OnAction(Player player, Action action,
      IImmutableList<Content> content) =>
      content.Aggregate(action,
        (action, content) => content.AddTo(player, action));

    public abstract Action AddTo(Player player, Action action);
  }

  /// <summary>
  /// A Scene is one unit of a Story. It represents one step in the Adventure
  /// that a player may take. It is not materialised and contains the
  /// information to adopt itself to a specific Player.
  /// </summary>
  public record Scene(
    string Name,
    IImmutableList<Content> Content)
    : IStorable<string>
  {
    public string Urlname
    {
      get
      {
        return NameConverter.OfString(Name);
      }
    }

    public string GetKey()
    {
      return Urlname;
    }

    public Action Play(Player player)
    {
      var action = new Action(this, "", ImmutableList<Property>.Empty,
        ImmutableList<Option>.Empty, null, null);
      return Content.Aggregate(action,
        (action, content) => content.AddTo(player, action));
    }
  }
}

