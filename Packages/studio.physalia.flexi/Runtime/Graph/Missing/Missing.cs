using System;

namespace Physalia.Flexi
{
    internal interface IIsMissing { }

    /// <summary>
    /// Representing the value type of any missing ports
    /// </summary>
    internal sealed class Missing
    {
        public static readonly Type TYPE = typeof(Missing);
    }
}
