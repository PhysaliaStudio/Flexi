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
    }
}
