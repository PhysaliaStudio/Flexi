using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    internal static class CustomStats
    {
        internal static readonly int HEALTH = 1;
        internal static readonly int MAX_HEALTH = 2;
        internal static readonly int ATTACK = 11;

        internal static readonly IReadOnlyList<StatDefinition> List = new List<StatDefinition>
        {
            new StatDefinition
            {
                Id = HEALTH,
                Name = "Health"
            },
            new StatDefinition
            {
                Id = MAX_HEALTH,
                Name = "MaxHealth"
            },
            new StatDefinition
            {
                Id = ATTACK,
                Name = "Attack"
            },
        };
    }
}
