using System;
using System.Collections.Generic;
using System.Linq;
using ProceduralPlant.Core;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace ProceduralPlant
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
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

        [SerializeField] private Material m_BranchMaterial;
        [SerializeField] private Material m_LeafMaterial;

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
                            symbol.descriptor.Generate(context, this.lindenmayerSystem, symbol);
                        }
                        break;
                    case Branch branch:
                        using (var branchContext = context.Clone())
                        {
                            Generate(branchContext, branch.content);
                        }
                        break;
                    case Polygon polygon:
                        var points = ListPool<Point>.Get();
                        void OnPointArrived(Point point)
                        {
                            points.Add(point);
                        }
                        context.onPointArrived += OnPointArrived;
                        Generate(context, polygon.content);
                        context.onPointArrived -= OnPointArrived;
                        Polygon.Generate(context, this.lindenmayerSystem, polygon, points);
                        ListPool<Point>.Release(points);
                        break;
                    default:
                        throw new NotImplementedException(node.ToString());
                }
                node = node.next;
            }
        }

        private void CreatePlant(string name, OrganFlags flags, MeshInfo meshInfo)
        {
            GameObject sub = new GameObject($"Procedural Plant Mesh {name}");
            sub.hideFlags = HideFlags.HideAndDontSave;
            sub.transform.parent = this.transform;
            MeshFilter meshFilter_ = sub.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer_ = sub.AddComponent<MeshRenderer>();
            if (flags.HasFlag(OrganFlags.Branch))
            {
                meshRenderer_.sharedMaterial = m_BranchMaterial;
            }
            else if (flags.HasFlag(OrganFlags.Leaf))
            {
                meshRenderer_.sharedMaterial = m_LeafMaterial;
            }
            var mesh = new Mesh();
            mesh.name = $"Procedural Plant Mesh {name}";
            mesh.vertices = meshInfo.vertices.ToArray();
            mesh.SetIndices(meshInfo.indices.ToArray(), MeshTopology.Triangles, 0);
            mesh.normals = meshInfo.normals.ToArray();
            meshFilter_.mesh = mesh;
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

            var context = new GenerationContext();
            Generate(context, lindenmayerSystem.current);

            foreach (Transform childTransform in this.transform)
            {
                Object.DestroyImmediate(childTransform.gameObject);
            }

            foreach (var (flags, list) in context.buffer.data)
            {
                int index = 0;
                foreach (var meshInfo in list)
                {
                    CreatePlant($"Plant {flags} {index}", flags, meshInfo);
                    index++;
                }
            }
        }
    }
}
