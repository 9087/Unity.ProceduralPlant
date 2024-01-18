using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProceduralPlant.Core
{
    public class GenerationContext
    {
        public class MeshInfo
        {
            public readonly List<Vector3> vertices = new();
            public readonly List<Vector3> normals = new();
            public readonly List<int> indices = new();
        }
        
        public readonly Transform transform;

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
        
        public Point current { get; private set; } = Point.origin;
        
        public Line last { get; private set; } = null;

        private System.Action<Point> _onPointArrived;
        public event System.Action<Point> onPointArrived
        {
            add
            {
                _onPointArrived += value;
                value?.Invoke(this.current);
            }
            remove => _onPointArrived -= value;
        }

        public GenerationContext(Transform transform, Dictionary<MeshTag, List<MeshInfo>> meshInfoData)
        {
            this.transform = transform;
            this.meshInfoData = meshInfoData;
        }

        public GenerationContext MoveForwardWithoutLine(float length)
        {
            var context = new GenerationContext(this.transform, this.meshInfoData)
            {
                _onPointArrived = _onPointArrived,
                current = this.current.MoveForward(length),
                last = null,
            };
            _onPointArrived?.Invoke(context.current);
            return context;
        }

        public GenerationContext MoveForwardWithLine(float length)
        {
            var context = new GenerationContext(this.transform, this.meshInfoData)
            {
                _onPointArrived = _onPointArrived,
                current = this.current.MoveForward(length),
            };
            context.last = new Line(this.last != null ? this.last.end : this.current, context.current);
            _onPointArrived?.Invoke(context.current);
            return context;
        }

        public GenerationContext Rotate(Quaternion delta)
        {
            return new GenerationContext(this.transform, this.meshInfoData)
            {
                _onPointArrived = _onPointArrived,
                current = this.current.Rotate(delta),
                last = this.last,
            };
        }

        public GenerationContext Thin(float thinningRate)
        {
            return new GenerationContext(this.transform, this.meshInfoData)
            {
                _onPointArrived = _onPointArrived,
                current = this.current.Thin(thinningRate),
                last = this.last,
            };
        }

        public enum MeshTag
        {
            Branch,
            Leaf,
        }

        private readonly Dictionary<MeshTag, List<MeshInfo>> meshInfoData;

        MeshInfo GetMeshInfo(MeshTag tag)
        {
            if (!meshInfoData.TryGetValue(tag, out var meshInfos))
            {
                meshInfoData[tag] = new();
                meshInfos = meshInfoData[tag];
                meshInfos.Add(new());
            }
            return meshInfos.Last();
        }
        
        public void AppendVertex(MeshTag tag, Vector3 position, Vector3 normal)
        {
            var meshInfo = GetMeshInfo(tag);
            meshInfo.vertices.Add(position);
            meshInfo.normals.Add(normal);
        }

        public void AppendIndex(MeshTag tag, int index)
        {
            var meshInfo = GetMeshInfo(tag);
            meshInfo.indices.Add(index);
        }

        public int GetCurrentIndex(MeshTag tag)
        {
            return GetMeshInfo(tag).vertices.Count;
        }

        public Vector3 GetVertexPosition(MeshTag tag, int index)
        {
            return GetMeshInfo(tag).vertices[index];
        }

        public void Prepare(MeshTag tag, int vertexCount)
        {
            Debug.Assert(vertexCount < 65536);
            if (GetCurrentIndex(tag) + vertexCount > 65536)
            {
                meshInfoData[tag].Add(new());
            }
        }
    }
}
