using System;

namespace Physalia.AbilitySystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeLogicAttribute : Attribute
    {
        private readonly Type nodeLogicType;

        public Type NodeLogicType => nodeLogicType;

        public NodeLogicAttribute(Type nodeLogicType)
        {
            this.nodeLogicType = nodeLogicType ?? throw new ArgumentNullException(nameof(nodeLogicType));
        }
    }
}
