using System;

namespace Physalia.AbilityFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class NodeCategory : Attribute
    {
        private readonly string menu;
        private readonly string name;

        public string Menu => menu;
        public string Name => name;

        public NodeCategory(string menu, string name = null)
        {
            this.menu = menu.Trim('/');
            this.name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class HideFromSearchWindow : Attribute
    {

    }
}
