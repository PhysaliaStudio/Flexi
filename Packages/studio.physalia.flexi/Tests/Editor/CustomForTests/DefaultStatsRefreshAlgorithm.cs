using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    public class DefaultStatsRefreshAlgorithm : IStatsRefreshAlgorithm
    {
        private readonly List<IModifierHandler> handlers = new()
        {
            new AddendModifierHandler(),
            new MultiplierModifierHandler(),
        };

        public void OnBeforeCollectModifiers(Actor actor)
        {

        }

        public void ApplyModifiers(Actor actor)
        {
            for (var i = 0; i < handlers.Count; i++)
            {
                handlers[i].ApplyModifiers(actor.Owner);
            }
        }
    }
}
