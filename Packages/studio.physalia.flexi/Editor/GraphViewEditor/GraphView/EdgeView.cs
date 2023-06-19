using UnityEngine;
using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphViewEditor
{
    public class EdgeView : UnityEditor.Experimental.GraphView.Edge
    {
        public bool isConnected = false;

        public EdgeView() : base()
        {
            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            if (e.clickCount == 2)
            {
                // Empirical offset:
                //var position = e.mousePosition;
                //position += new Vector2(-10f, -28);
                //Vector2 mousePos = owner.ChangeCoordinatesTo(owner.contentViewContainer, position);

                //owner.AddRelayNode(input as PortView, output as PortView, mousePos);
            }
        }
    }
}
