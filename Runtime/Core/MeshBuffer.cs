using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProceduralPlant.Core
{
    public class MeshBuffer
    {
        internal readonly Dictionary<OrganFlags, List<MeshInfo>> data = new();
        
        private MeshInfo GetCurrentMeshInfo(OrganFlags flags)
        {
            if (!data.TryGetValue(flags, out var meshInfos))
            {
                data[flags] = new();
                meshInfos = data[flags];
                meshInfos.Add(new());
            }
            return meshInfos[^1];
        }

        public MeshInfo Prepare(OrganFlags flags, int requiredVertexCount)
        {
            Debug.Assert(requiredVertexCount < 65536);
            List<MeshInfo> meshInfos = null;
            if (GetCurrentMeshInfo(flags).GetVertexCount() + requiredVertexCount > 65536)
            {
                meshInfos = data[flags]; 
                meshInfos.Add(new());
            }
            else
            {
                meshInfos = data[flags];
            }
            return meshInfos[^1];
        }
    }
}

