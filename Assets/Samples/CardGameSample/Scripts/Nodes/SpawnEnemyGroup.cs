using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class SpawnEnemyGroup : DefaultProcessNode
    {
        protected override FlowState OnExecute()
        {
            Game game = Container.Game;
            IReadOnlyList<Unit> units = game.RandomGenerateEnemyGroup();
            EnqueueEvent(new UnitSpawnedEvent { units = units });
            return FlowState.Success;
        }
    }
}
