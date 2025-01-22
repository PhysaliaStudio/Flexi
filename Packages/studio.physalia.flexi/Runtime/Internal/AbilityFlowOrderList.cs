using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    internal class AbilityFlowOrderList : IComparable, IComparable<AbilityFlowOrderList>
    {
        private readonly int order;
        private readonly List<AbilityFlow> list;

        public int Order => order;
        public AbilityFlow this[int index] => list[index];
        public int Count => list.Count;
        public int Capacity => list.Capacity;
        public IReadOnlyList<AbilityFlow> Elements => list;

        public AbilityFlowOrderList(int order)
        {
            this.order = order;
            list = new List<AbilityFlow>();
        }

        public AbilityFlowOrderList(int order, int capacity)
        {
            this.order = order;
            list = new List<AbilityFlow>(capacity);
        }

        public AbilityFlowOrderList(int order, IEnumerable<AbilityFlow> collection)
        {
            this.order = order;
            list = new List<AbilityFlow>(collection);
        }

        public int CompareTo(object obj)
        {
            if (obj is AbilityFlowOrderList other)
            {
                return CompareTo(other);
            }
            else
            {
                throw new ArgumentException("Object is not an AbilityFlowOrderList");
            }
        }

        public int CompareTo(AbilityFlowOrderList other)
        {
            return order.CompareTo(other.order);
        }

        public void Add(AbilityFlow item)
        {
            list.Add(item);
        }

        public void AddRange(IEnumerable<AbilityFlow> collection)
        {
            list.AddRange(collection);
        }

        public void Clear()
        {
            list.Clear();
        }
    }
}
