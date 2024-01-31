using System;
using UnityEngine;

namespace ProceduralPlant.Core
{
    public struct Point
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public float diameter;

        public static Point origin { get; } =
            new()
            {
                position = Vector3.zero,
                rotation = Quaternion.Euler(-90, 0, 0),
                scale = Vector3.one,
                diameter = 1.0f,
            };
        
        public static Point empty { get; } =
            new()
            {
                position = Vector3.zero,
                rotation = new Quaternion(0, 0, 0, 0),
                scale = Vector3.zero,
                diameter = 0,
            };

        public static bool operator ==(Point a, Point b)
        {
            return
                a.position == b.position &&
                a.rotation == b.rotation &&
                a.scale == b.scale &&
                a.diameter == b.diameter;
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }
        
        public bool Equals(Point other)
        {
            return position.Equals(other.position) && rotation.Equals(other.rotation) && scale.Equals(other.scale) && diameter.Equals(other.diameter);
        }

        public override bool Equals(object obj)
        {
            return obj is Point other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(position, rotation, scale, diameter);
        }
        
        public Point MoveForward(float length)
        {
            return new()
            {
                position = this.position + this.rotation * Vector3.forward * length,
                rotation = this.rotation,
                scale = this.scale,
                diameter = this.diameter,
            };
        }

        public Point Rotate(Quaternion delta)
        {
            return new()
            {
                position = this.position,
                rotation = this.rotation * delta,
                scale = this.scale,
                diameter = this.diameter,
            };
        }
    }
}
