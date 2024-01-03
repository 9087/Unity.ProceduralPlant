using System;
using ProceduralPlant.Core;
using UnityEngine;
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

        public LindenmayerSystem lindenmayerSystem { get; private set; } = null;

        private void Awake()
        {
            Refresh();
        }

        private void Populate(TransformData transformData, Node node)
        {
            if (node == null)
            {
                return;
            }
            switch (node)
            {
                case Symbol symbol:
                    if (symbol.descriptor != null)
                    {
                        transformData = symbol.descriptor.Populate(this.lindenmayerSystem, transformData, symbol);
                    }
                    break;
                case Branch branch:
                    Populate(transformData, branch.content);
                    break;
                case Polygon polygon:
                    Debug.LogWarning("Node `Polygon` is not implemented!");
                    break;
                default:
                    throw new NotImplementedException(node.ToString());
            }
            Populate(transformData, node.next);
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
            Populate(new TransformData(this.transform), lindenmayerSystem.current);
        }
    }
}
