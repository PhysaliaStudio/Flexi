using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class SpawnEnemyGroup : ProcessNode
    {
        public Inport<Game> gamePort;

        protected override AbilityState DoLogic()
        {
            Game game = gamePort.GetValue();
            IReadOnlyList<Unit> units = game.RandomGenerateEnemyGroup();
            EnqueueEvent(new UnitSpawnedEvent { units = units });
            return AbilityState.RUNNING;
        }
    }
}
