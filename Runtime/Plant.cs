using System;
using System.Collections.Generic;
using System.Linq;
using ProceduralPlant.Core;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace ProceduralPlant
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(MeshRenderer))]
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

        public List<MeshFilter> meshFilters = new();

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
                        var points = ListPool<GenerationContext.Point>.Get();
                        void OnPointArrived(GenerationContext.Point point)
                        {
                            points.Add(point);
                        }
                        context.onPointArrived += OnPointArrived;
                        Generate(context, polygon.content);
                        context.onPointArrived -= OnPointArrived;
                        Polygon.Generate(this.lindenmayerSystem, context, points);
                        ListPool<GenerationContext.Point>.Release(points);
                        break;
                    default:
                        throw new NotImplementedException(node.ToString());
                }
                node = node.next;
            }
        }

        private void CreatePlant(int i, GenerationContext.MeshInfo meshInfo)
        {
            MeshFilter meshFilter_ = null;
            GameObject sub = new GameObject($"Procedural Plant Mesh {i}");
            sub.hideFlags = HideFlags.HideAndDontSave;
            sub.transform.parent = this.transform;
            sub.AddComponent<MeshRenderer>().sharedMaterials = this.meshRenderer.sharedMaterials;
            meshFilter_ = sub.AddComponent<MeshFilter>();
            var mesh = new Mesh();
            mesh.name = $"Procedural Plant Mesh {i}";
            mesh.vertices = meshInfo.vertices.ToArray();
            mesh.SetIndices(meshInfo.indices.ToArray(), MeshTopology.Triangles, 0);
            mesh.normals = meshInfo.normals.ToArray();
            meshFilter_.mesh = mesh;
            meshFilters.Add(meshFilter_);
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

            var meshInfoData = DictionaryPool<GenerationContext.MeshTag, List<GenerationContext.MeshInfo>>.Get(); 
            var context = new GenerationContext(this.transform, meshInfoData);
            Generate(context, lindenmayerSystem.current);

            foreach (Transform childTransform in this.transform)
            {
                Object.DestroyImmediate(childTransform.gameObject);
            }

            meshFilters.Clear();
            int index = 0;
            foreach (var (tag, list) in meshInfoData)
            {
                foreach (var meshInfo in list)
                {
                    CreatePlant(index, meshInfo);
                    index++;
                }
            }
            DictionaryPool<GenerationContext.MeshTag, List<GenerationContext.MeshInfo>>.Release(meshInfoData);
        }
    }
}
