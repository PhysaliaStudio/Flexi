using System.Collections.Generic;

namespace Physalia.Flexi
{
    public class ActorRepository
    {
        private readonly IStatsRefreshAlgorithm statsRefreshAlgorithm;

        private readonly Dictionary<int, Actor> actorTable = new();
        private readonly List<Actor> actorList = new();

        public IReadOnlyList<Actor> Actors => actorList;

        public ActorRepository(IStatsRefreshAlgorithm statsRefreshAlgorithm)
        {
            this.statsRefreshAlgorithm = statsRefreshAlgorithm;
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

        internal void OnBeforeCollectModifiersForAll()
        {
            for (var i = 0; i < actorList.Count; i++)
            {
                statsRefreshAlgorithm.OnBeforeCollectModifiers(actorList[i]);
            }
        }

        /// <summary>
        /// This method just total all modifiers by algorithm, so there is no priority issue.
        /// </summary>
        internal void RefreshStatsForAll()
        {
            for (var i = 0; i < actorList.Count; i++)
            {
                RefreshStats(actorList[i]);
            }
        }

        /// <summary>
        /// This method just total all modifiers by algorithm, so there is no priority issue.
        /// </summary>
        internal void RefreshStats(Actor actor)
        {
            actor.ResetAllStats();
            statsRefreshAlgorithm.RefreshStats(actor);
        }
    }
}
