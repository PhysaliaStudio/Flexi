using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class AllEnemiesNode : ValueNode
    {
        public Inport<Game> gamePort;
        public Outport<List<Unit>> resultPort;

        private readonly List<Unit> resultCache = new();

        protected override void EvaluateSelf()
        {
            resultCache.Clear();
            Game game = gamePort.GetValue();
            resultCache.AddRange(game.Enemies);
            resultPort.SetValue(resultCache);
        }
    }
}
