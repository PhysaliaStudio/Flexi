using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    public readonly struct AbilityDataSource : IEquatable<AbilityDataSource>, IEqualityComparer<AbilityDataSource>
    {
        private readonly AbilityData abilityData;
        private readonly int groupIndex;

        public readonly AbilityData AbilityData => abilityData;
        public readonly int GroupIndex => groupIndex;
        public bool IsValid => abilityData != null && groupIndex >= 0 && groupIndex < abilityData.graphGroups.Count;
        public AbilityGraphGroup GraphGroup => IsValid ? abilityData.graphGroups[groupIndex] : null;

        public AbilityDataSource(AbilityData abilityData, int groupIndex)
        {
            this.abilityData = abilityData;
            this.groupIndex = groupIndex;
        }

        public override string ToString()
        {
            if (!IsValid)
            {
                return "Invalid Source";
            }

            return $"Source({abilityData.name}-{groupIndex})";
        }

        public override bool Equals(object obj)
        {
            return obj is AbilityDataSource other && Equals(this, other);
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public bool Equals(AbilityDataSource other)
        {
            return AbilityData == other.AbilityData && GroupIndex == other.GroupIndex;
        }

        public bool Equals(AbilityDataSource x, AbilityDataSource y)
        {
            return x.AbilityData == y.AbilityData && x.GroupIndex == y.GroupIndex;
        }

        public int GetHashCode(AbilityDataSource obj)
        {
            return obj.AbilityData.GetHashCode() ^ obj.GroupIndex.GetHashCode();
        }

        public static bool operator ==(AbilityDataSource left, AbilityDataSource right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AbilityDataSource left, AbilityDataSource right)
        {
            return !left.Equals(right);
        }
    }
}
