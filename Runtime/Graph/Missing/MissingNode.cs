namespace Physalia.Flexi
{
    [HideFromSearchWindow]
    internal sealed class MissingNode : Node, IIsMissing
    {
        private readonly string typeFullName;
        private readonly string typeName;

        public string TypeFullName => typeFullName;
        public string TypeName => typeName;

        public MissingNode(string typeFullName)
        {
            if (string.IsNullOrEmpty(typeFullName))
            {
                this.typeFullName = "";
                typeName = "";
            }
            else
            {
                this.typeFullName = typeFullName;
                string[] splits = typeFullName.Split('.', '+');
                typeName = splits[^1];
            }
        }
    }
}
