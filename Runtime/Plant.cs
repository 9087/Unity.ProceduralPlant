using System;
using System.Collections.Generic;
using System.Linq;
using ProceduralPlant.Core;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace ProceduralPlant
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class Plant : MonoBehaviour
    {
        [SerializeField] private string m_LindenmayerSystemDescription = "F;F->FF-[-F+F+F]+[+F-F-F];";

        public string lindenmayerSystemDescription
        {
            get => m_LindenmayerSystemDescription;
            set
            {
                if (m_LindenmayerSystemDescription == value)
                {
                    return;
                }
                m_LindenmayerSystemDescription = value;
                this.Refresh();
            }
        }

        [SerializeField] private int m_IterationCount = 3;

        public int iterationCount
        {
            get => m_IterationCount;
            set
            {
                if (value < 0 || m_IterationCount == value)
                {
                    return;
                }
                m_IterationCount = 0;
                Refresh();
            }
        }

        [SerializeField] private ParametersInfo m_ParametersInfo = new();

        public ParametersInfo parametersInfo
        {
            get => m_ParametersInfo;
            set
            {
                m_ParametersInfo = value;
                this.Refresh();
            }
        }

        public LindenmayerSystem lindenmayerSystem { get; private set; } = null;

        public MeshRenderer meshRenderer => GetComponent<MeshRenderer>();
        
        public MeshFilter meshFilter => GetComponent<MeshFilter>();

        private void Awake()
        {
            Refresh();
        }

        private void Generate(GenerationContext context, Node node)
        {
            while (node != null)
            {
                switch (node)
                {
                    case Symbol symbol:
                        if (symbol.descriptor != null)
                        {
                            context = symbol.descriptor.Generate(this.lindenmayerSystem, context, symbol);
                        }
                        break;
                    case Branch branch:
                        Generate(context, branch.content);
                        break;
                    case Polygon polygon:
                        Debug.LogWarning("Node `Polygon` is not implemented!");
                        break;
                    default:
                        throw new NotImplementedException(node.ToString());
                }
                node = node.next;
            }
        }
        
        public void Refresh()
        {
            for (int i = this.transform.childCount - 1; i >= 0; --i)
            {
                var child = this.transform.GetChild(i) as Transform;
                Object.DestroyImmediate(child!.gameObject);
            }
            lindenmayerSystem = LindenmayerSystem.Compile(lindenmayerSystemDescription, this.m_ParametersInfo);
            if (lindenmayerSystem == null)
            {
                return;
            }
            for (int i = 0; i < iterationCount; ++i)
            {
                lindenmayerSystem.Simulate();
            }
            lindenmayerSystem.MarkOrganFlags();

            var meshInfo = new GenerationContext.MeshInfo()
            {
                vertices = ListPool<Vector3>.Get(),
                indices = ListPool<int>.Get(),
                normals = ListPool<Vector3>.Get(),
            };
            var context = new GenerationContext(this.transform, meshInfo);
            Generate(context, lindenmayerSystem.current);

            var mesh = new Mesh();
            mesh.name = "Procedural Plant";
            mesh.vertices = meshInfo.vertices.ToArray();
            mesh.SetIndices(meshInfo.indices.ToArray(), MeshTopology.Triangles, 0);
            mesh.normals = meshInfo.normals.ToArray();
            meshFilter.mesh = mesh;
            ListPool<Vector3>.Release(meshInfo.vertices);
            ListPool<Vector3>.Release(meshInfo.normals);
            ListPool<int>.Release(meshInfo.indices);
        }
    }
}
