using System.Collections.Generic;

namespace Physalia.Flexi
{
    public class DefaultModifierAlgorithm : IModifierAlgorithm
    {
        private readonly List<IModifierHandler> handlers = new()
        {
            new AddendModifierHandler(),
            new MultiplierModifierHandler(),
        };

        public void RefreshStats(StatOwner owner)
        {
            for (var i = 0; i < handlers.Count; i++)
            {
                handlers[i].RefreshStats(owner);
            }
        }
    }
}
