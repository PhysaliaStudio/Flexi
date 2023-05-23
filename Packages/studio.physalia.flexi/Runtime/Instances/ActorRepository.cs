using System.Collections.Generic;

namespace Physalia.Flexi
{
    public class ActorRepository
    {
        private readonly IModifierAlgorithm modifierAlgorithm;

        private readonly Dictionary<int, Actor> actorTable = new();
        private readonly List<Actor> actorList = new();

        public IReadOnlyList<Actor> Actors => actorList;

        public ActorRepository(IModifierAlgorithm modifierAlgorithm)
        {
            this.modifierAlgorithm = modifierAlgorithm;
        }

        internal Actor GetActor(int id)
        {
            if (actorTable.TryGetValue(id, out Actor actor))
            {
                return actor;
            }

            Logger.Warn($"Cannot find Actor with <Id:{id}>");
            return null;
        }

        internal void AddActor(int id, Actor actor)
        {
            actorTable.Add(id, actor);
            actorList.Add(actor);
        }

        internal void RemoveActor(int id)
        {
            bool success = actorTable.Remove(id, out Actor actor);
            if (success)
            {
                actorList.Remove(actor);
            }
        }

        internal void RefreshStats(Actor actor)
        {
            actor.ResetAllStats();
            modifierAlgorithm.RefreshStats(actor);
        }
    }
}
