using System.Collections.Generic;

namespace Physalia.Stats.Tests
{
    internal static class StatTestHelper
    {
        internal static readonly List<StatDefinition> ValidList = new()
        {
            new StatDefinition
            {
                Id = 1,
                Name = "Health"
            },
            new StatDefinition
            {
                Id = 2,
                Name = "MaxHealth"
            },
            new StatDefinition
            {
                Id = 11,
                Name = "Attack"
            },
        };

        internal static readonly List<StatDefinition> IdConflictList = new()
        {
            new StatDefinition
            {
                Id = 1,
                Name = "Health"
            },
            new StatDefinition
            {
                Id = 2,
                Name = "MaxHealth"
            },
            new StatDefinition
            {
                Id = 2,
                Name = "Attack"
            },
        };
    }
}
