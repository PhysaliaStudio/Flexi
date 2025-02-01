using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    internal readonly struct AbilityHandle : IEquatable<AbilityHandle>, IEqualityComparer<AbilityHandle>
    {
        private readonly AbilityData abilityData;
        private readonly int groupIndex;

        public readonly AbilityData Data => abilityData;
        public readonly int GroupIndex => groupIndex;
        public bool IsValid => abilityData != null && groupIndex >= 0 && groupIndex < abilityData.graphGroups.Count;
        public AbilityGraphGroup GraphGroup => IsValid ? abilityData.graphGroups[groupIndex] : null;

        public AbilityHandle(AbilityData abilityData, int groupIndex)
        {
            this.abilityData = abilityData;
            this.groupIndex = groupIndex;
        }

        public override string ToString()
        {
            if (!IsValid)
            {
                return $"[{abilityData?.name}][{groupIndex}](Invalid)";
            }

            return $"{abilityData.name}[{groupIndex}]";
        }

        public override bool Equals(object obj)
        {
            return obj is AbilityHandle other && Equals(this, other);
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public bool Equals(AbilityHandle other)
        {
            return Data == other.Data && GroupIndex == other.GroupIndex;
        }

        public bool Equals(AbilityHandle x, AbilityHandle y)
        {
            return x.Data == y.Data && x.GroupIndex == y.GroupIndex;
        }

        public int GetHashCode(AbilityHandle obj)
        {
            return obj.Data.GetHashCode() ^ obj.GroupIndex.GetHashCode();
        }

        public static bool operator ==(AbilityHandle left, AbilityHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AbilityHandle left, AbilityHandle right)
        {
            return !left.Equals(right);
        }
    }
}
