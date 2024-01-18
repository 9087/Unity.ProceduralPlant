using System.Linq;
using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("f")]
    public class MoveForwardWithoutLine : Descriptor
    {
        public override GenerationContext Generate(LindenmayerSystem lindenmayerSystem, GenerationContext context, Symbol symbol)
        {
            return context.MoveForwardWithoutLine(lindenmayerSystem.parametersInfo.length);
        }
    }
    
    [Symbol("F")]
    public class MoveForwardWithLine : MoveForwardWithoutLine
    {
        private static void GeneratePipe(GenerationContext context, int sideCount, GenerationContext.Line line)
        {
            context.Prepare(GenerationContext.MeshTag.Branch, sideCount * 2);
            
            var sAxis = line.start.rotation * Vector3.forward;
            var eAxis = line.end.rotation * Vector3.forward;
            
            var sFirstSide = line.start.rotation * Vector3.up;
            var eFirstSide = line.end.rotation * Vector3.up;
            
            var sPosition = line.start.position;
            var sRadius = line.start.diameter * 0.5f;
            
            var ePosition = line.end.position;
            var eRadius = line.end.diameter * 0.5f;

            var stepAngle = 360.0f / sideCount;

            var head = context.GetCurrentIndex(GenerationContext.MeshTag.Branch);
            int eIndex = 0;
            float minSqrDistance = float.MaxValue;
            for (int i = 0; i < sideCount; i++)
            {
                var sVertex = (Quaternion.AngleAxis(i * stepAngle, sAxis) * sFirstSide).normalized * sRadius + sPosition;
                var sNormal = (Quaternion.AngleAxis((i - 0.5f) * stepAngle, sAxis) * sFirstSide).normalized;
                context.AppendVertex(GenerationContext.MeshTag.Branch, sVertex, sNormal);
                
                var eVertex = (Quaternion.AngleAxis(i * stepAngle, eAxis) * eFirstSide).normalized * eRadius + ePosition;
                var eNormal = (Quaternion.AngleAxis((i - 0.5f) * stepAngle, eAxis) * eFirstSide).normalized;
                context.AppendVertex(GenerationContext.MeshTag.Branch, eVertex, eNormal);

                float sqrDistance = (context.GetVertexPosition(GenerationContext.MeshTag.Branch, head) - eVertex).sqrMagnitude;
                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    eIndex = i;
                }
            }

            for (int i = 0; i < sideCount; i++)
            {
                context.AppendIndex(GenerationContext.MeshTag.Branch, head + (2 * i + 0) % (2 * sideCount));
                context.AppendIndex(GenerationContext.MeshTag.Branch, head + (2 * i + 2) % (2 * sideCount));
                context.AppendIndex(GenerationContext.MeshTag.Branch, head + (2 * (i + eIndex) + 1) % (2 * sideCount));
                
                context.AppendIndex(GenerationContext.MeshTag.Branch, head + (2 * (i + eIndex) + 1) % (2 * sideCount));
                context.AppendIndex(GenerationContext.MeshTag.Branch, head + (2 * i + 2) % (2 * sideCount));
                context.AppendIndex(GenerationContext.MeshTag.Branch, head + (2 * (i + eIndex) + 3) % (2 * sideCount));
            }
        }
        
        public override GenerationContext Generate(LindenmayerSystem lindenmayerSystem, GenerationContext context, Symbol symbol)
        {
            var old = context;
            context = context.MoveForwardWithLine(lindenmayerSystem.parametersInfo.length);
            if (old.last == null)
            {
                GeneratePipe(context, lindenmayerSystem.parametersInfo.sideCount, context.last);
            }
            else
            {
                float curveSize = 0.1f;
                var line = context.last.Range(curveSize, symbol.organFlags.HasFlag(Node.OrganFlags.Tip) ? 1 : (1 - curveSize));
                GeneratePipe(context, lindenmayerSystem.parametersInfo.sideCount, line);
                var from = old.last.Sample(1 - curveSize);
                var to = line.start;
                var curve = new GenerationContext.Line(from, to);
                int segmentCount = 1;
                float segment = 1.0f / segmentCount;
                for (int i = 0; i < segmentCount; i++)
                {
                    var s = curve.Range(i * segment, (i + 1) * segment);
                    GeneratePipe(context, lindenmayerSystem.parametersInfo.sideCount, s);
                }
            }
            return context;
        }
    }
}
