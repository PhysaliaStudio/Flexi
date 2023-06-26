using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    internal static class AbilityTestHelper
    {
        internal static AbilityDataSource CreateValidDataSource()
        {
            var abilityData = new AbilityData();
            abilityData.graphGroups.Add(new AbilityGraphGroup());
            return abilityData.CreateDataSource(0);
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

        internal static void AppendGraphToSource(AbilityDataSource abilityDataSource, string json)
        {
            AbilityData abilityData = abilityDataSource.AbilityData;
            int groupIndex = abilityDataSource.GroupIndex;
            abilityData.graphGroups[groupIndex].graphs.Add(json);
        }
    }
}
