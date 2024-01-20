using UnityEngine;

namespace ProceduralPlant.Core
{
    public class Point
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public float diameter;

        public static Point origin
            =>
            new()
            {
                position = Vector3.zero,
                rotation = Quaternion.Euler(-90, 0, 0),
                scale = Vector3.one,
                diameter = 1.0f,
            };
        
        public static Point empty
            =>
            new()
            {
                position = Vector3.zero,
                rotation = new Quaternion(0, 0, 0, 0),
                scale = Vector3.zero,
                diameter = 0,
            };

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

        public Point Thin(float thinningRate)
        {
            return new()
            {
                position = this.position,
                rotation = this.rotation,
                scale = this.scale,
                diameter = this.diameter * (1 - thinningRate),
            };
        }
    }
}
