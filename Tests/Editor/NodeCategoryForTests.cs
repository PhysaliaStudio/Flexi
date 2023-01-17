namespace Physalia.Flexi.Tests
{
    internal class NodeCategoryForTests : NodeCategory
    {
        private const string Category = BuiltInCategory.Root + "/[Test Custom]";

        internal NodeCategoryForTests(string name = null) : base(Category, name)
        {

        }
    }
}
