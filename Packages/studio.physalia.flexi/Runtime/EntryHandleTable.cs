using System.Collections.Generic;

namespace Physalia.Flexi
{
    internal class EntryHandleTable
    {
        private readonly Dictionary<AbilityDataSource, List<EntryHandle>> entryHandles = new(16);

        public bool TryGetHandles(AbilityDataSource abilityDataSource, out List<EntryHandle> handles)
        {
            return entryHandles.TryGetValue(abilityDataSource, out handles);
        }

        public void Add(AbilityDataSource abilityDataSource, int flowIndex, int entryIndex, int order)
        {
            if (!entryHandles.TryGetValue(abilityDataSource, out var handles))
            {
                handles = new List<EntryHandle>(4);
                entryHandles.Add(abilityDataSource, handles);
            }

            var newHandle = new EntryHandle
            {
                abilityDataSource = abilityDataSource,
                flowIndex = flowIndex,
                entryIndex = entryIndex,
                order = order,
            };

            if (!handles.Contains(newHandle))
            {
                handles.Add(newHandle);
            }
        }

        public void Remove(AbilityDataSource abilityDataSource)
        {
            _ = entryHandles.Remove(abilityDataSource);
        }
    }
}
