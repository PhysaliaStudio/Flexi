using System;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class SelectionBehaviour : MonoBehaviour
    {
        public event Action<Unit> TargetSelected;
        public event Action Canceled;

        [SerializeField]
        private Canvas parentCanvas;
        [SerializeField]
        private GameUISpline targetingSpline;

        private CardUI selectedCardUI;
        private SelectionData selectionData;
        private Vector3 previousMousePosition;

        public void SetupTargetingCursor(CardUI selectedCardUI, SelectionData selectionData)
        {
            this.selectedCardUI = selectedCardUI;
            this.selectionData = selectionData;

            if (!selectionData.isTargetless)
            {
                targetingSpline.SetEnabled(true);
            }
            else
            {
                selectedCardUI.SetInteractable(false);
            }
        }

        private void Update()
        {
            if (selectedCardUI != null)
            {
                Vector3 currentMousePosition = Input.mousePosition;
                if (currentMousePosition != previousMousePosition)
                {
                    UpdateTargetingCursor(selectedCardUI, currentMousePosition);
                }

                previousMousePosition = currentMousePosition;

                if (Input.GetMouseButtonDown(0))
                {
                    CleanUp();

                    Ray ray = Camera.main.ScreenPointToRay(currentMousePosition);
                    bool success = Physics.Raycast(ray, out RaycastHit hitInfo);
                    if (success)
                    {
                        var unitAvatar = hitInfo.transform.parent.GetComponent<UnitAvatar>();
                        TargetSelected?.Invoke(unitAvatar.Unit);
                    }
                    else
                    {
                        Canceled?.Invoke();
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    CleanUp();
                    Canceled?.Invoke();
                }
            }
        }

        public void CleanUp()
        {
            selectedCardUI = null;
            selectionData = null;
            targetingSpline.SetEnabled(false);
        }

        private void UpdateTargetingCursor(CardUI selectedCardUI, Vector2 posDrag)
        {
            if (!selectionData.isTargetless)
            {
                Vector2 start = TransformWorldPointToScreenPoint(selectedCardUI.GetTargetingSplineStartWorldPosition());
                targetingSpline.GenerateCurve(start, posDrag);
            }
            else
            {
                // TODO
            }
        }

        private Vector2 TransformWorldPointToScreenPoint(Vector3 worldPosition)
        {
            return parentCanvas.worldCamera.WorldToScreenPoint(worldPosition);
        }
    }
}
