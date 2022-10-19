using System.Collections.Generic;

namespace Physalia.AbilityFramework.Tests
{
    public class CustomPayload
    {
        public CustomUnit owner;
        public CustomUnit instigator;
        public List<CustomUnit> targets = new();
    }
}
