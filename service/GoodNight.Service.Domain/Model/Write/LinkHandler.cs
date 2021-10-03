using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Write
{
  public static class LinkHandler
  {
    /// <summary>
    /// Identifies all scenes that may change from updates to the outLinks of
    /// a given scene, updates and then returns them.
    /// <summary>
    public static IEnumerable<Scene> UpdateLinkedScenes(
      IRepository<Scene> scenes, Scene updated)
    {
      // find all scenes that are not linked anymore.
      var prevLinked = scenes
        // filter by story to have less to sift through.
        .Where(s => s.Key.StartsWith(updated.Story))
        .Where(s => s.InLinks.Any(updated.Key.Equals) &&
          !updated.OutLinks.Any(s.Key.Equals))
        // update them to remove the link.
        .Select(s => s with { InLinks = s.InLinks.Remove(updated) });

      // next, update all newly created links.
      var newLinked = updated.OutLinks
        .Select(s => s.Get()).OfType<Scene>()
        .Where(s => !s.InLinks.Any(updated.Key.Equals))
        // and add the link.
        .Select(s => s with { InLinks = s.InLinks.Add(updated) });

      return prevLinked.Concat(newLinked);
    }

    /// <summary>
    /// Identifies all qualities that may change from updates to the outLinks of
    /// a given scene, updates and then returns them.
    /// <summary>
    public static IEnumerable<Quality> UpdateLinkedQualities(
      IRepository<Quality> qualities, Scene updated)
    {
      // find all scenes that are not linked anymore.
      var prevLinked = qualities
        // filter by story to have less to sift through.
        .Where(s => s.Key.StartsWith(updated.Story))
        .Where(s => s.InLinks.Any(updated.Key.Equals) &&
          !updated.OutLinks.Any(s.Key.Equals))
        // update them to remove the link.
        .Select(s => s with { InLinks = s.InLinks.Remove(updated) });

      // next, update all newly created links.
      var newLinked = updated.Qualities
        .Select(s => s.Get()).OfType<Quality>()
        .Where(s => !s.InLinks.Any(updated.Key.Equals))
        // and add the link.
        .Select(s => s with { InLinks = s.InLinks.Add(updated) });

      return prevLinked.Concat(newLinked);
    }
  }
}
