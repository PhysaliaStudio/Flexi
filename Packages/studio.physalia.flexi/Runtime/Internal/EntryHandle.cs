using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    internal struct EntryHandle : IEquatable<EntryHandle>, IEqualityComparer<EntryHandle>
    {
        public AbilityHandle abilityHandle;
        public int flowIndex;
        public int entryIndex;
        public int order;

        public override readonly bool Equals(object obj)
        {
            return obj is EntryHandle other && Equals(this, other);
        }

        public readonly bool Equals(EntryHandle other)
        {
            return Equals(this, other);
        }

        public readonly bool Equals(EntryHandle x, EntryHandle y)
        {
            return x.abilityHandle == y.abilityHandle &&
                   x.flowIndex == y.flowIndex &&
                   x.entryIndex == y.entryIndex &&
                   x.order == y.order;
        }

        public override readonly int GetHashCode()
        {
            return GetHashCode(this);
        }

        public readonly int GetHashCode(EntryHandle obj)
        {
            return HashCode.Combine(obj.abilityHandle, obj.flowIndex, obj.entryIndex, obj.order);
        }

        public static bool operator ==(EntryHandle left, EntryHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntryHandle left, EntryHandle right)
        {
            return !left.Equals(right);
        }
    }
}
