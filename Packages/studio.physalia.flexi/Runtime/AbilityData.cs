using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    /// <summary>
    /// An AbilityData preserves an ability data.
    /// </summary>
    [Serializable]
    public class AbilityData
    {
        public string name;
        public List<BlackboardVariable> blackboard = new();
        public List<string> graphJsons = new();

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
    }
}
