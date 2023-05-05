using System;

namespace Physalia.Flexi
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NodeCategory : Attribute
    {
        private readonly string menu;
        private readonly string name;
        private readonly int order;

        public string Menu => menu;
        public string Name => name;
        public int Order => order;

        public NodeCategory(string menu) : this(menu, null, 0)
        {

        }

        public NodeCategory(string menu, string name) : this(menu, name, 0)
        {

        }

        public NodeCategory(string menu, int order) : this(menu, null, order)
        {

        }

        public NodeCategory(string menu, string name, int order)
        {
            this.menu = menu.Trim('/');
            this.name = name;
            this.order = order;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class HideFromSearchWindow : Attribute
    {

    }
}
