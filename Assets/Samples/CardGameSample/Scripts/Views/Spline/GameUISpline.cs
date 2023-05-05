using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class GameUISpline : MonoBehaviour
    {
        public enum TargetType
        {
            None,
            Highlighted,
            Valid,
            Invalid,
        }

        private static readonly int CURVE_RESOLUTION = 9;

        [SerializeField]
        private Transform midPointContainer;

        [Space]
        [SerializeField]
        private GameUISplinePoint imagePrefabMidPointDefault;
        [SerializeField]
        private GameUISplinePoint imagePrefabMidPointValid;
        [SerializeField]
        private GameUISplinePoint imagePrefabMidPointInvalid;

        [Space]
        [SerializeField]
        private GameUISplinePoint imageEndPointDefault;
        [SerializeField]
        private GameUISplinePoint imageEndPointValid;
        [SerializeField]
        private GameUISplinePoint imageEndPointInvalid;

        [Space]
        [SerializeField]
        private bool applyTargetingColor;
        [SerializeField]
        private bool allowStraightLines;
        [SerializeField]
        private float splinePulseScaleIncrease = 0.1f;

        private bool isEnabled;
        private int maxPointCount = 32;
        private readonly List<Vector2> controlPoints = new();
        private readonly CatmullRomSpline spline = new();
        private SelectionStyle splineSelectionStyle;
        private Dictionary<TargetType, GameObjectPool<GameUISplinePoint>> midPointPoolTable;
        private Dictionary<TargetType, GameUISplinePoint> endPointTable;
        private Vector2 translatedPoint;
        private Quaternion translatedRot;

        private Canvas parentCanvas;
        private RectTransform rectTransform;

        private Vector2 posMid;
        private Vector2 posMidCurved;
        private Vector2 posMidInline;
        private Vector2 posDelta = new(1f, 1f);
        private Vector2 direction = new(0f, 1f);

        private TargetType CurrentTargetType
        {
            get
            {
                if (splineSelectionStyle == null)
                {
                    return TargetType.Highlighted;
                }

                return splineSelectionStyle.TargetType;
            }
        }

        private Color SplineColor
        {
            get
            {
                if (splineSelectionStyle == null)
                {
                    return Color.white;
                }

                return splineSelectionStyle.OutlineColor;
            }
        }

        private int NumMidPointsToSkipAtEnd
        {
            get
            {
                if (splineSelectionStyle == null)
                {
                    return 0;
                }

                return splineSelectionStyle.NumMidPointsToSkipAtEnd;
            }
        }

        private void Awake()
        {
            midPointPoolTable = new Dictionary<TargetType, GameObjectPool<GameUISplinePoint>>
            {
                { TargetType.Highlighted, new GameObjectPool<GameUISplinePoint>(imagePrefabMidPointDefault, midPointContainer, "DefaultPoints", 10, 40) },
                { TargetType.Valid, new GameObjectPool<GameUISplinePoint>(imagePrefabMidPointValid, midPointContainer, "ValidPoints", 10, 40) },
                { TargetType.Invalid, new GameObjectPool<GameUISplinePoint>(imagePrefabMidPointInvalid, midPointContainer, "InvalidPoints", 10, 40) }
            };

            endPointTable = new Dictionary<TargetType, GameUISplinePoint>
            {
                { TargetType.Highlighted, imageEndPointDefault },
                { TargetType.Valid, imageEndPointValid },
                { TargetType.Invalid, imageEndPointInvalid }
            };

            parentCanvas = GetComponentInParent<Canvas>();
            rectTransform = transform as RectTransform;
            SetControlPointCountMax(3);
        }

        private void Start()
        {
            SetEnabled(false);
        }

        public void SetEnabled(bool isEnabled)
        {
            this.isEnabled = isEnabled;
            DisableAllImageEndPoints();
            ReleaseAllImageMidPoints();
        }

        public bool IsEnabled()
        {
            return isEnabled;
        }

        public void SetControlPointCountMax(int maxCount)
        {
            maxPointCount = maxCount;
            spline.SetProperties(maxCount, CURVE_RESOLUTION, false);
        }

        public void SetSelectionStyle(SelectionStyle selectionStyle)
        {
            if (selectionStyle != splineSelectionStyle && selectionStyle.ShowPulseOnTransition)
            {
                endPointTable[selectionStyle.TargetType].PulsePoint(splinePulseScaleIncrease);
            }
            splineSelectionStyle = selectionStyle;
        }

        private void EnableImageEndPoint()
        {
            GameUISplinePoint gameUISplinePoint = endPointTable[CurrentTargetType];
            if (!gameUISplinePoint.gameObject.activeSelf)
            {
                DisableAllImageEndPoints();
                gameUISplinePoint.gameObject.SetActive(true);
            }
        }

        private void DisableAllImageEndPoints()
        {
            foreach (GameUISplinePoint gameUISplinePoint in endPointTable.Values)
            {
                gameUISplinePoint.gameObject.SetActive(false);
            }
        }

        private void ReleaseAllImageMidPoints()
        {
            foreach (GameObjectPool<GameUISplinePoint> objectPoolPrefabs in midPointPoolTable.Values)
            {
                objectPoolPrefabs.ReleaseAll();
            }
        }

        public void ClearControlPoints()
        {
            controlPoints.Clear();
        }

        public void AddWorldSpaceControlPoint(Vector3 posWorld)
        {
            if (controlPoints.Count == maxPointCount)
            {
                Debug.LogError("Exceeded max control point count!");
                return;
            }
            translatedPoint = parentCanvas.worldCamera.WorldToScreenPoint(posWorld);
            controlPoints.Add(translatedPoint);
        }

        public void AddScreenSpaceControlPoint(Vector3 posScreen)
        {
            if (controlPoints.Count == maxPointCount)
            {
                Debug.LogError("Exceeded max control point count!");
                return;
            }
            controlPoints.Add(posScreen);
        }

        public void GenerateCurve(Vector2 start, Vector2 end)
        {
            ClearControlPoints();
            posDelta = end - start;
            controlPoints.Add(start);
            posMidCurved.Set(start.x + posDelta.x * 0.125f, end.y + Mathf.Sign(posDelta.y) * Mathf.Min(26f, Mathf.Abs(posDelta.y)));
            posMid = posMidCurved;
            if (allowStraightLines)
            {
                direction = posDelta.normalized;
                float num = Mathf.Abs(Vector2.Dot(Vector2.up, direction));
                if (num > 0.707f)
                {
                    posMidInline = start + 0.45f * posDelta.magnitude * direction;
                    posMid = Vector2.Lerp(posMidCurved, posMidInline, num - (1f - num) / 0.29299998f);
                }
            }
            controlPoints.Add(posMid);
            controlPoints.Add(end);
            GenerateSegments();
        }

        private void GenerateSegments()
        {
            if (controlPoints.Count == 0)
            {
                return;
            }

            spline.GenerateCurves(controlPoints);
            Vector2[] curvePositions = spline.GetCurvePositions();

            if (endPointTable != null)
            {
                EnableImageEndPoint();
                GameUISplinePoint gameUISplinePoint = endPointTable[CurrentTargetType];
                if (applyTargetingColor)
                {
                    gameUISplinePoint.SetColor(SplineColor);
                }
                gameUISplinePoint.SetScreenPosition(rectTransform, curvePositions[curvePositions.Length - 1], parentCanvas.worldCamera);
                float curvePointRotation = spline.GetCurvePointRotation(curvePositions.Length - 1);
                translatedRot = Quaternion.Euler(0f, 0f, curvePointRotation);
                if (posDelta.magnitude > 20f)
                {
                    gameUISplinePoint.transform.rotation = translatedRot;
                }
            }

            if (midPointPoolTable != null)
            {
                ReleaseAllImageMidPoints();
                int num = 1;
                while (num < (curvePositions.Length - 1) - (long)((ulong)NumMidPointsToSkipAtEnd))
                {
                    GameUISplinePoint gameUISplinePoint2 = midPointPoolTable[CurrentTargetType].Get();
                    if (applyTargetingColor)
                    {
                        gameUISplinePoint2.SetColor(SplineColor);
                    }
                    gameUISplinePoint2.SetScreenPosition(rectTransform, curvePositions[num], parentCanvas.worldCamera);
                    float num2 = 1f - (float)num / curvePositions.Length;
                    gameUISplinePoint2.transform.localScale = Vector3.one * (0.75f + num2 * 1.25f);
                    gameUISplinePoint2.transform.SetAsLastSibling();
                    num++;
                }
            }
        }
    }
}
