using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    internal static class AbilityTestHelper
    {
        internal static AbilityHandle CreateValidHandle()
        {
            var abilityData = new AbilityData();
            abilityData.graphGroups.Add(new AbilityGraphGroup());
            return abilityData.CreateHandle(0);
        }

        internal static AbilityData CreateSingleGraphData(string json)
        {
            return new AbilityData()
            {
                graphGroups = new List<AbilityGraphGroup>
                {
                    new AbilityGraphGroup()
                    {
                        graphs = new List<string>{ json }
                    }
                }
            };
        }

        internal static void AppendGraphToSource(AbilityHandle abilityHandle, string json)
        {
            AbilityData abilityData = abilityHandle.Data;
            int groupIndex = abilityHandle.GroupIndex;
            abilityData.graphGroups[groupIndex].graphs.Add(json);
        }
    }
}
