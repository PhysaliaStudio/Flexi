using System;

namespace Physalia.AbilitySystem
{
    internal static class NodeLogicFactory
    {
        internal static NodeLogic Create(Node node)
        {
            Type nodeLogicType = NodeLogicCache.GetNodeLogicType(node);
            var nodeLogic = Activator.CreateInstance(nodeLogicType) as NodeLogic;
            nodeLogic.SetNode(node);
            return nodeLogic;
        }
    }
}
