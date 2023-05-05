using UnityEngine;

namespace Physalia.Flexi.Samples
{
    public static class VectorExtensions
    {
        public static Vector2 Rotate(this Vector2 vector, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
            float vx = vector.x;
            float vy = vector.y;

            vector.x = cos * vx - sin * vy;
            vector.y = sin * vx + cos * vy;
            return vector;
        }
    }
}
