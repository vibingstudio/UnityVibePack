using Randomer = UnityEngine.Random;
using UnityEngine;

namespace VibePack.Math
{
    public static class Mathv
    {
        public static bool Random() => Randomer.Range(0f, 1f) > 0.5f;

        public static float Eerp(float a, float b, float t) => Mathf.Pow(a, 1 - t) * Mathf.Pow(b, t);

        public static Vector3 Eerp(Vector3 a, Vector3 b, float t) => new Vector3(Eerp(a.x, b.x, t), Eerp(a.y, b.y, t), Eerp(a.z, b.z, t));

        public static float Map(this float value, float min1, float max1, float min2, float max2) => min2 + (max2 - min2) * ((value - min1) / (max1 - min1));

        public static Vector3 GetSDF(Vector3 start, Vector3 end, Vector3 point)
        {
            Vector3 line = (end - start);
            float len = line.magnitude;
            line.Normalize();

            float d = Mathf.Clamp(Vector3.Dot(point - start, line), 0f, len);
            return start + line * d;
        }

        public static int IntersectionPointsBetweenCircleAndLine(Vector2 circleCenter, float circleRadius, Vector2 point1, Vector2 point2, out Vector2 intersection1, out Vector2 intersection2)
        {
            float dx = point2.x - point1.x;
            float dy = point2.y - point1.y;

            float a = dx * dx + dy * dy;
            float b = 2 * (dx * (point1.x - circleCenter.x) + dy * (point1.y - circleCenter.y));
            float c = (point1.x - circleCenter.x) * (point1.x - circleCenter.x) + (point1.y - circleCenter.y) * (point1.y - circleCenter.y) - circleRadius * circleRadius;
            float determinate = b * b - 4 * a * c;

            if (a <= 0.0000001 || determinate < -0.0000001)
            {
                intersection1 = intersection2 = Vector2.zero;
                return 0;
            }

            float t;

            if (determinate < 0.0000001 && determinate > -0.0000001)
            {
                t = -b / (2 * a);
                intersection1 = new Vector2(point1.x + t * dx, point1.y + t * dy);
                intersection2 = Vector2.zero;
                return 1;
            }

            t = (-b + Mathf.Sqrt(determinate)) / (2 * a);
            intersection1 = new Vector2(point1.x + t * dx, point1.y + t * dy);
            intersection2 = new Vector2(point1.x + t * dx, point1.y + t * dy);
            return 2;
        }
    }
}
