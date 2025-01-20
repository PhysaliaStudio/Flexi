using System.Collections.Generic;

namespace Physalia.Flexi
{
    internal class EntryHandleTable
    {
        private readonly Dictionary<AbilityHandle, List<EntryHandle>> entryHandles = new(16);

        public bool TryGetHandles(AbilityHandle abilityHandle, out List<EntryHandle> handles)
        {
            return entryHandles.TryGetValue(abilityHandle, out handles);
        }

        public void Add(AbilityHandle abilityHandle, int flowIndex, int entryIndex, int order)
        {
            if (!entryHandles.TryGetValue(abilityHandle, out var handles))
            {
                handles = new List<EntryHandle>(4);
                entryHandles.Add(abilityHandle, handles);
            }

            var newHandle = new EntryHandle
            {
                abilityHandle = abilityHandle,
                flowIndex = flowIndex,
                entryIndex = entryIndex,
                order = order,
            };

            if (!handles.Contains(newHandle))
            {
                handles.Add(newHandle);
            }
        }

        public void Remove(AbilityHandle abilityHandle)
        {
            _ = entryHandles.Remove(abilityHandle);
        }
    }
}
