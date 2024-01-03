using UnityEngine;

namespace ProceduralPlant.Core
{
    public class TransformData
    {
        public readonly Transform transform;
        
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformData(Transform transform)
        {
            this.transform = transform;
            this.position = Vector3.zero;
            this.rotation = Quaternion.Euler(-90, 0, 0);
            this.scale = Vector3.one;
        }

        public TransformData(Transform transform, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.transform = transform;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
}
