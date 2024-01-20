using System;
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

        public Dictionary<OrganFlags, List<MeshInfo>> meshInfoData { get; private set; }
        
        public Point current { get; private set; } = Point.origin;
        
        public Line last { get; private set; } = null;

        public GenerationContext() : this(null)
        {
        }
        
        protected GenerationContext(Dictionary<OrganFlags, List<MeshInfo>> meshInfoData)
        {
            this.meshInfoData = meshInfoData ?? new();
        }

        public GenerationContext CreateBranch()
        {
            var context = new GenerationContext(this.meshInfoData);
            context.current = current;
            context.last = last;
            context._onPointArrived = _onPointArrived;
            return context;
        }

        public void MoveForwardWithoutLine(float length)
        {
            current = this.current.MoveForward(length);
            last = null;
            _onPointArrived?.Invoke(current);
        }

        public void MoveForwardWithLine(float length)
        {
            var oldLast = this.last;
            var oldCurrent = this.current;
            var newCurrent = this.current.MoveForward(length);
            this.current = newCurrent;
            this.last = new Line(oldLast != null ? oldLast.end : oldCurrent, newCurrent);
            this._onPointArrived?.Invoke(this.current);
        }

        public void Rotate(Quaternion delta)
        {
            current = this.current.Rotate(delta);
        }

        public void Thin(float thinningRate)
        {
            current = this.current.Thin(thinningRate);
        }

        MeshInfo GetMeshInfo(OrganFlags flags)
        {
            if (!meshInfoData.TryGetValue(flags, out var meshInfos))
            {
                meshInfoData[flags] = new();
                meshInfos = meshInfoData[flags];
                meshInfos.Add(new());
            }
            return meshInfos.Last();
        }
        
        public void AppendVertex(OrganFlags flags, Vector3 position, Vector3 normal)
        {
            var meshInfo = GetMeshInfo(flags);
            meshInfo.vertices.Add(position);
            meshInfo.normals.Add(normal);
        }

        public void AppendIndex(OrganFlags flags, int index)
        {
            var meshInfo = GetMeshInfo(flags);
            meshInfo.indices.Add(index);
        }

        public int GetCurrentIndex(OrganFlags flags)
        {
            return GetMeshInfo(flags).vertices.Count;
        }

        public Vector3 GetVertexPosition(OrganFlags flags, int index)
        {
            return GetMeshInfo(flags).vertices[index];
        }

        public void Prepare(OrganFlags flags, int vertexCount)
        {
            Debug.Assert(vertexCount < 65536);
            if (GetCurrentIndex(flags) + vertexCount > 65536)
            {
                meshInfoData[flags].Add(new());
            }
        }
    }
}
