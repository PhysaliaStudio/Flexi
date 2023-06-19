using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphViewEditor
{
    public class PortView : UnityEditor.Experimental.GraphView.Port
    {
        public PortView(Orientation orientation, Direction direction, Capacity capacity, Type type)
            : base(orientation, direction, capacity, type)
        {
            m_EdgeConnector = new EdgeViewConnector<EdgeView>(new EdgeViewConnectorListener());
            this.AddManipulator(m_EdgeConnector);
        }
    }
}
