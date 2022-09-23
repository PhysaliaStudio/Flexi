namespace Physalia.AbilitySystem
{
    public abstract class NodeLogic
    {
        private Node node;

        public Node Node => node;

        internal virtual void SetNode(Node node)
        {
            this.node = node;
        }

        public abstract void Do();
    }

    public abstract class NodeLogic<T> : NodeLogic where T : Node
    {
        private T node;

        public new T Node => node;

        internal override void SetNode(Node node)
        {
            if (node is T genericNode)
            {
                base.SetNode(node);
                this.node = genericNode;
            }
        }
    }
}
