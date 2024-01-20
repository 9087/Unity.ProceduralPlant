using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPlant.Core
{
    public class MeshInfo
    {
        public readonly List<Vector3> vertices = new();
        public readonly List<Vector3> normals = new();
        public readonly List<int> indices = new();

        public void AppendVertex(Vector3 position, Vector3 normal)
        {
            this.vertices.Add(position);
            this.normals.Add(normal);
        }

        public Vector3 GetVertexPosition(int index)
        {
            return this.vertices[index];
        }

        public Vector3 GetVertexNormal(int index)
        {
            return this.normals[index];
        }

        public int GetVertexCount()
        {
            return this.vertices.Count;
        }

        public void AppendTriangle(int index0, int index1, int index2)
        {
            this.indices.Add(index0);
            this.indices.Add(index1);
            this.indices.Add(index2);
        }
    }
}