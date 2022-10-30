namespace Physalia.AbilityFramework
{
    public class AbilityGraph : Graph
    {
        private FlowNode currentNode;
        private int indexOfEntryNode;
        private bool isRunning;

        public FlowNode Current => currentNode;

        public void Reset(int indexOfEntryNode)
        {
            currentNode = null;
            this.indexOfEntryNode = indexOfEntryNode;
            isRunning = false;
        }

        public bool MoveNext()
        {
            if (currentNode == null)
            {
                if (isRunning)
                {
                    return false;
                }

                if (indexOfEntryNode < 0 || indexOfEntryNode >= EntryNodes.Count)
                {
                    return false;
                }

                isRunning = true;
                currentNode = EntryNodes[indexOfEntryNode];
                return true;
            }

            currentNode = currentNode.Next;
            return currentNode != null;
        }
    }
}
