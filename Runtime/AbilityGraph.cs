namespace Physalia.AbilitySystem
{
    public class AbilityGraph : Graph
    {
        private FlowNode currentNode;

        public FlowNode Current => currentNode;

        public void Reset(int indexOfEntryNode)
        {
            if (indexOfEntryNode < 0 || indexOfEntryNode >= EntryNodes.Count)
            {
                currentNode = null;
                return;
            }

            currentNode = EntryNodes[indexOfEntryNode];
        }

        public bool MoveNext()
        {
            if (currentNode == null)
            {
                return false;
            }

            currentNode = currentNode.Next;
            return currentNode != null;
        }
    }
}
