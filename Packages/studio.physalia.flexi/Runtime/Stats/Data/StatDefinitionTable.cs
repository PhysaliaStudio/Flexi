using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi
{
    internal class StatDefinitionTable
    {
        internal class Factory
        {
            internal StatDefinitionTable Create(List<StatDefinition> definitions)
            {
                var table = new StatDefinitionTable();
                for (var i = 0; i < definitions.Count; i++)
                {
                    table.AddStatDefinition(definitions[i]);
                }

                // If the counts are different, it means there are id conflictions and skipped
                if (table.Count != definitions.Count)
                {
                    Logger.Error($"Create table failed! Detected id conflictions, see upon messages for details");
                    return null;
                }

                return table;
            }
        }

        private readonly Dictionary<int, StatDefinition> table = new();

        private int Count => table.Count;

        private void AddStatDefinition(StatDefinition definition)
        {
            if (table.ContainsKey(definition.Id))
            {
                Logger.Error($"Add stat definition failed! Already contains the same <Id:{definition.Id}>");
                return;
            }

            table.Add(definition.Id, definition);
            Logger.Info($"Added stat definition {definition}");
        }

        internal StatDefinition GetStatDefinition(int id)
        {
            if (!table.ContainsKey(id))
            {
                Logger.Error($"Get stat definition failed! The stat with <Id:{id}> does not exist");
                return null;
            }

            return table[id];
        }
    }
}
