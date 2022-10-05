using System.Collections.Generic;

namespace Physalia.AbilitySystem.Tests
{
    public class CustomPayload
    {
        public CustomUnit owner;
        public CustomUnit instigator;
        public List<CustomUnit> targets = new();
    }
}
