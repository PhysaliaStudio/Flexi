using System;
using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class CatmullRomSpline
    {
        private int controlPointCountMax;
        private int resolution = 2;
        private bool isClosedLoop;

        private Vector2[] splinePositions;
        private Vector2[] splineNormals;
        private Vector2[] splineTangents;
        private int splinePointCount;

        public CatmullRomSpline()
        {

        }

        public void SetProperties(int controlPointCountMax, int resolution, bool isClosedLoop)
        {
            if (resolution < 2)
            {
                throw new ArgumentException($"[{nameof(CatmullRomSpline)}] Resolution must be >= 2");
            }

            this.controlPointCountMax = controlPointCountMax;
            this.resolution = resolution;
            this.isClosedLoop = isClosedLoop;
            splinePointCount = controlPointCountMax * resolution;

            if (isClosedLoop)
            {
                splinePointCount += resolution;
            }
            else
            {
                splinePointCount -= resolution;
            }

            splinePositions = new Vector2[splinePointCount];
            splineNormals = new Vector2[splinePointCount];
            splineTangents = new Vector2[splinePointCount];
        }

        public Vector2[] GetCurvePositions()
        {
            if (splinePositions == null)
            {
                throw new NullReferenceException($"[{nameof(CatmullRomSpline)}] Spline not Initialized!");
            }
            return splinePositions;
        }

        public float GetCurvePointRotation(int posIndex)
        {
            if (splinePointCount > 0 && posIndex < splinePositions.Length)
            {
                float num = Vector3.Dot(splineTangents[posIndex], Vector2.up);
                float num2 = Vector3.Dot(splineNormals[posIndex], Vector2.up);
                float num3 = 90f - num * 90f;
                if (num2 < 0f)
                {
                    num3 = -num3;
                }
                return num3;
            }
            return 0f;
        }

        public float GetCurvePointDeltaDistance(int posIndex)
        {
            if (splinePointCount > 0 && posIndex > 0 && posIndex < splinePositions.Length)
            {
                return Vector2.Distance(splinePositions[posIndex], splinePositions[posIndex - 1]);
            }
            return 0f;
        }

        public float GetCurvePointDistanceFromEnd(int posIndex)
        {
            if (splinePointCount > 0 && posIndex > 0 && posIndex < splinePositions.Length)
            {
                return Vector2.Distance(splinePositions[posIndex], splinePositions[splinePositions.Length - 1]);
            }
            return 0f;
        }

        public void DebugDrawSpline(Color color)
        {
            if (ValidatePoints())
            {
                for (var i = 0; i < splinePositions.Length; i++)
                {
                    if (i == splinePositions.Length - 1 && isClosedLoop)
                    {
                        Debug.DrawLine(splinePositions[i], splinePositions[0], color);
                    }
                    else if (i < splinePositions.Length - 1)
                    {
                        Debug.DrawLine(splinePositions[i], splinePositions[i + 1], color);
                    }
                }
            }
        }

        public void DebugDrawNormals(float extrusion, Color color)
        {
            if (ValidatePoints())
            {
                for (var i = 0; i < splinePositions.Length; i++)
                {
                    Debug.DrawLine(splinePositions[i], splinePositions[i] + splineNormals[i] * extrusion, color);
                }
            }
        }

        public void DebugDrawTangents(float extrusion, Color color)
        {
            if (ValidatePoints())
            {
                for (var i = 0; i < splinePositions.Length; i++)
                {
                    Debug.DrawLine(splinePositions[i], splinePositions[i] + splineTangents[i] * extrusion, color);
                }
            }
        }

        private bool ValidatePoints()
        {
            if (splinePositions.Length == 0)
            {
                throw new NullReferenceException($"[{nameof(CatmullRomSpline)}] Spline not initialized!");
            }
            return splinePositions.Length != 0;
        }

        public void GenerateCurves(List<Vector2> controlPoints)
        {
            if (controlPoints == null || controlPoints.Count <= 0)
            {
                throw new ArgumentException($"[{nameof(CatmullRomSpline)}] Invalid control point collection");
            }
            CalculateSpinePoints(controlPoints);
        }

        private void CalculateSpinePoints(List<Vector2> controlPoints)
        {
            int num = isClosedLoop ? 0 : 1;
            for (var i = 0; i < controlPoints.Count - num; i++)
            {
                bool flag = isClosedLoop && i == controlPoints.Count - 1;
                Vector2 vector = controlPoints[i];

                Vector2 vector2;
                if (flag)
                {
                    vector2 = controlPoints[0];
                }
                else
                {
                    vector2 = controlPoints[i + 1];
                }

                Vector2 vector3;
                if (i == 0)
                {
                    if (isClosedLoop)
                    {
                        vector3 = vector2 - controlPoints[controlPoints.Count - 1];
                    }
                    else
                    {
                        vector3 = vector2 - vector;
                    }
                }
                else
                {
                    vector3 = vector2 - controlPoints[i - 1];
                }

                Vector2 vector4;
                if (isClosedLoop)
                {
                    if (i == controlPoints.Count - 1)
                    {
                        vector4 = controlPoints[(i + 2) % controlPoints.Count] - vector;
                    }
                    else if (i == 0)
                    {
                        vector4 = controlPoints[i + 2] - vector;
                    }
                    else
                    {
                        vector4 = controlPoints[(i + 2) % controlPoints.Count] - vector;
                    }
                }
                else if (i < controlPoints.Count - 2)
                {
                    vector4 = controlPoints[(i + 2) % controlPoints.Count] - vector;
                }
                else
                {
                    vector4 = vector2 - vector;
                }

                vector3 *= 0.5f;
                vector4 *= 0.5f;

                float num2 = 1f / resolution;
                if ((i == controlPoints.Count - 2 && !isClosedLoop) || flag)
                {
                    num2 = 1f / (resolution - 1);
                }

                var num3 = 0;
                while (num3 < resolution)
                {
                    float t = num3 * num2;
                    int num4 = i * resolution + num3;
                    splinePositions[num4] = CalculatePosition(vector, vector2, vector3, vector4, t);
                    splineTangents[num4] = CalculateTangent(vector, vector2, vector3, vector4, t);
                    splineNormals[num4] = NormalFromTangent(splineTangents[num4]);
                    num3++;
                }
            }
        }

        private Vector2 CalculatePosition(Vector2 start, Vector2 end, Vector2 controlPoint1, Vector2 controlPoint2, float t)
        {
            float num = t * t;
            float num2 = num * t;
            return (2f * num2 - 3f * num + 1f) * start + (num2 - 2f * num + t) * controlPoint1 + (-2f * num2 + 3f * num) * end + (num2 - num) * controlPoint2;
        }

        private Vector2 CalculateTangent(Vector2 start, Vector2 end, Vector2 controlPoint1, Vector2 controlPoint2, float t)
        {
            float num = t * t;
            return ((6f * num - 6f * t) * start + (3f * num - 4f * t + 1f) * controlPoint1 + (-6f * num + 6f * t) * end + (3f * num - 2f * t) * controlPoint2).normalized;
        }

        private Vector2 NormalFromTangent(Vector2 tangent)
        {
            return Vector3.Cross(tangent, Vector3.forward).normalized;
        }
    }
}
