using System;
using System.Text;
using ProceduralPlant.Core;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace ProceduralPlant
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class Plant : MonoBehaviour
    {
        [SerializeField] protected PlantSpecies m_Species = null;
        public PlantSpecies species => m_Species;
        
        [SerializeField] protected int m_RandomSeed = 0;
        
        public enum QualityLevel
        {
            Fastest,
            Fast,
            Simple,
            Good,
            Beautiful,
            Fantastic,
        }
        
        [SerializeField] protected QualityLevel m_QualityLevel = QualityLevel.Beautiful;

        internal PlantSpecies plantAsset => m_Species;
        internal int sideCount => 3 * ((int)m_QualityLevel + 1);
        
        public LindenmayerSystem lindenmayerSystem { get; private set; } = null;

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
                            symbol.descriptor.Generate(this, context, symbol);
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
            sub.transform.SetParent(this.transform, false);
            MeshFilter meshFilter_ = sub.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer_ = sub.AddComponent<MeshRenderer>();
            if (flags.HasFlag(OrganFlags.Branch))
            {
                meshRenderer_.sharedMaterial = m_Species.branchMaterial;
            }
            else if (flags.HasFlag(OrganFlags.Leaf))
            {
                meshRenderer_.sharedMaterial = m_Species.leafMaterial;
            }
            var mesh = new Mesh();
            mesh.name = $"Procedural Plant Mesh {name}";
            mesh.vertices = meshInfo.vertices.ToArray();
            mesh.SetIndices(meshInfo.indices.ToArray(), MeshTopology.Triangles, 0);
            mesh.normals = meshInfo.normals.ToArray();
            meshFilter_.mesh = mesh;
        }
        
        public void Refresh(StringBuilder error = null)
        {
            if (m_Species == null)
            {
                error?.Append("\nSpecify a plant species first. (Plant species is an asset, which can be created via the right-click Create menu.)");
                return;
            }
            
            for (int i = this.transform.childCount - 1; i >= 0; --i)
            {
                var child = this.transform.GetChild(i) as Transform;
                Object.DestroyImmediate(child!.gameObject);
            }

            lindenmayerSystem = LindenmayerSystem.Compile(m_Species.description, error);
            if (lindenmayerSystem == null)
            {
                return;
            }
            for (int i = 0; i < m_Species.iterationCount; ++i)
            {
                lindenmayerSystem.Simulate(this.m_RandomSeed);
            }
            lindenmayerSystem.MarkOrganFlags();

            var context = new GenerationContext(this.m_Species);
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
