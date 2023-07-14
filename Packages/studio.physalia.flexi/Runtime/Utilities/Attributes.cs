using System;
using UnityEngine;

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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class NodeColor : Attribute
    {
        private readonly bool isValid;
        private readonly Color color;

        public bool IsValid => isValid;
        public Color Color => color;

        /// <param name="hexString">6 digit rgb string begin with '#'</param>
        /// <param name="a">Additional alpha</param>
        public NodeColor(string hexString, float a = 0.8f)
        {
            bool success = ColorUtility.TryParseHtmlString(hexString, out Color color);
            if (success)
            {
                isValid = true;
                this.color = color;
                color.a = a;
                return;
            }

            isValid = false;
            this.color = Color.clear;
        }

        public NodeColor(byte r, byte g, byte b, float a = 0.8f)
        {
            isValid = true;
            color = new Color(r / 255f, g / 255f, b / 255f, a);
        }
    }
}
