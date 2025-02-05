using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    [HideFromSearchWindow]
    public class FakeFlowNode : BranchNode<DefaultAbilityContainer, EmptyResumeContext>
    {
        private int pauseCount = 0;

        public override FlowNode Next => throw new System.NotImplementedException();

        public void SetPauseCount(int pauseCount)
        {
            this.pauseCount = pauseCount;
        }

        protected override bool CanResume(EmptyResumeContext context)
        {
            return true;
        }

        protected override FlowState OnExecute()
        {
            if (pauseCount > 0)
            {
                pauseCount--;
                return FlowState.Pause;
            }

            return FlowState.Success;
        }

        protected override FlowState OnResume(EmptyResumeContext context)
        {
            if (pauseCount > 0)
            {
                pauseCount--;
                return FlowState.Pause;
            }

            return FlowState.Success;
        }
    }

    public class FakeFlow : IAbilityFlow
    {
        private readonly int countOfNode;
        private readonly List<FakeFlowNode> nodes = new();

        private int currentIndex = -1;

        public FlowNode this[int index] => nodes[index];
        public FlowNode Current => currentIndex >= 0 && currentIndex < countOfNode ? nodes[currentIndex] : null;

        public FakeFlow(int countOfNode = 0)
        {
            this.countOfNode = countOfNode;

            if (countOfNode > 0)
            {
                for (var i = 0; i < countOfNode; i++)
                {
                    nodes.Add(new FakeFlowNode());
                }
            }
        }

        public void SetPauseCount(int index, int pauseCount)
        {
            nodes[index].SetPauseCount(pauseCount);
        }

        public bool HasNext()
        {
            return currentIndex + 1 < countOfNode;
        }

        public bool MoveNext()
        {
            if (currentIndex >= countOfNode)
            {
                return false;
            }

            currentIndex++;
            return currentIndex < countOfNode;
        }

        public void Reset(int entryIndex)
        {
            currentIndex = -1;
        }
    }
}
