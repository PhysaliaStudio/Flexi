using System;

namespace Physalia.AbilityFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class NodeCategory : Attribute
    {
        private readonly string name;

        public string Name => name;

        public NodeCategory(string name)
        {
            this.name = name.Trim('/');
        }
    }
}
