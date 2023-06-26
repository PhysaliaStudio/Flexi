using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    /// <summary>
    /// Preserves a list of graphs.
    /// </summary>
    [Serializable]
    public class AbilityGraphGroup
    {
        public List<string> graphs = new();

        internal AbilityGraphGroup Clone()
        {
            var clone = new AbilityGraphGroup();
            for (var i = 0; i < graphs.Count; i++)
            {
                clone.graphs.Add(graphs[i]);
            }

            return clone;
        }
    }

    /// <summary>
    /// An AbilityData preserves an ability data.
    /// </summary>
    [Serializable]
    public class AbilityData
    {
        public string name;
        public List<BlackboardVariable> blackboard = new();
        public List<AbilityGraphGroup> graphGroups = new();

        /// <remarks>
        /// This method is for overriding values from outside data like Excel.
        /// </remarks>
        public void SetBlackboard(string key, int value)
        {
            for (var i = 0; i < blackboard.Count; i++)
            {
                if (blackboard[i].key == key)
                {
                    blackboard[i].value = value;
                    return;
                }
            }

            blackboard.Add(new BlackboardVariable { key = key, value = value });
        }

        public AbilityDataSource CreateDataSource(int groupIndex)
        {
            if (groupIndex < 0 || groupIndex >= graphGroups.Count)
            {
                throw new IndexOutOfRangeException();
            }

            return new AbilityDataSource(this, groupIndex);
        }
    }
}
