using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class AllEnemiesNode : DefaultValueNode
    {
        public Outport<List<Unit>> resultPort;

        private readonly List<Unit> resultCache = new();

        protected override void EvaluateSelf()
        {
            resultCache.Clear();
            Game game = Container.Game;
            resultCache.AddRange(game.Enemies);
            resultPort.SetValue(resultCache);
        }
    }
}
