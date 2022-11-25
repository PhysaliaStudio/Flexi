namespace Physalia.AbilityFramework
{
    internal sealed class MissingNode : Node
    {
        private readonly string typeName;

        public string TypeName => typeName;

        public MissingNode(string typeName)
        {
            if (typeName == null)
            {
                this.typeName = "";
            }
            else
            {
                this.typeName = typeName;
            }
        }
    }
}
