namespace Physalia.AbilitySystem
{
    public class AbilityInstance
    {
        private readonly Ability ability;

        private FlowNode current;

        public FlowNode Current => current;

        internal AbilityInstance(Ability ability)
        {
            this.ability = ability;
        }

        public void Reset(int indexOfEntryNode)
        {
            if (indexOfEntryNode < 0 || indexOfEntryNode >= ability.EntryNodes.Count)
            {
                return;
            }

            current = ability.EntryNodes[indexOfEntryNode];
        }

        public bool MoveNext()
        {
            if (current != null)
            {
                current = current.Next;
            }

            return current != null;
        }
    }
}
