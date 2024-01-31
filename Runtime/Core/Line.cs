

using System;
using UnityEngine;

namespace ProceduralPlant.Core
{
    public readonly struct Line
    {
        public readonly Point start;
        public readonly Point end;

        private readonly bool _none;

        public static Line none { get; } = new Line(true);

        private Line(bool none)
        {
            this.start = Point.empty;
            this.end = Point.empty;
            this._none = none;
        }

        public Line(Point start, Point end)
        {
            this.start = start;
            this.end = end;
            this._none = false;
        }

        public Line(Point start, Point end, DiameterRange diameterRange) : this(start, end)
        {
            this.start.diameter = diameterRange.start;
            this.end.diameter = diameterRange.end;
        }

        public static bool operator ==(Line a, Line b)
        {
            if (a._none != b._none)
            {
                return false;
            }
            if (a._none)
            {
                return true;
            }
            return
                a.start == b.start &&
                a.end == b.end;
        }

        public static bool operator !=(Line a, Line b)
        {
            return !(a == b);
        }
        
        public bool Equals(Line other)
        {
            return start.Equals(other.start) && end.Equals(other.end) && _none == other._none;
        }

        public override bool Equals(object obj)
        {
            return obj is Line other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(start, end, _none);
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
            var p1 = (start.rotation * Vector3.forward) + p0;
            var p2 = (end.rotation * Vector3.back) + p3;
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
