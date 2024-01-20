

using UnityEngine;

namespace ProceduralPlant.Core
{
    public class Line
    {
        public readonly Point start;
        public readonly Point end;

        public Line(Point start, Point end)
        {
            this.start = start;
            this.end = end;
        }

        private static Vector3 BezierPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }

        private static Quaternion BezierRotation(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float uu = u * u;
            float tt = t * t;

            Vector3 v = 3 * uu * (p1 - p0) + 6 * u * t * (p2 - p1) + 3 * tt * (p3 - p2);
            return Quaternion.FromToRotation(Vector3.forward, v);
        }

        public Point Sample(float progress)
        {
            var scale = start.scale + progress * (end.scale - start.scale);
            var diameter = start.diameter + progress * (end.diameter - start.diameter);
            var p0 = start.position;
            var p3 = end.position;
            var p1 = (start.rotation * Vector3.forward).normalized + p0;
            var p2 = (end.rotation * Vector3.back).normalized + p3;
            var position = BezierPosition(progress, p0, p1, p2, p3);
            var rotation = Quaternion.Lerp(start.rotation, end.rotation, progress);
            return new()
            {
                position = position,
                rotation = rotation,
                scale = scale,
                diameter = diameter,
            };
        }

        public Line Range(float from, float to)
        {
            return new Line(this.Sample(from), this.Sample(to));
        }
    }
}
