using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class ApplyStatusNode : DefaultProcessNode
    {
        public Inport<IReadOnlyList<Unit>> targetsPort;
        public Inport<int> stackPort;
        public Variable<int> statusId;

        protected override FlowState OnExecute()
        {
            IReadOnlyList<Unit> targets = targetsPort.GetValue();
            int stack = stackPort.GetValue();

            for (var i = 0; i < targets.Count; i++)
            {
                Container.Game.ApplyStatus(targets[i], statusId.Value, stack);
            }

            return FlowState.Success;
        }
    }
}
