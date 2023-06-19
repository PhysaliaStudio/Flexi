using UnityEditor.Experimental.GraphView;

namespace Physalia.Flexi.GraphViewEditor
{
    public class EdgeViewDragHelper<TEdgeView> : EdgeDragHelper<EdgeView> where TEdgeView : EdgeView, new()
    {
        public EdgeViewDragHelper(IEdgeConnectorListener listener) : base(listener)
        {

        }
    }
}
