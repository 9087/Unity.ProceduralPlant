using System.Linq;
using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("f")]
    public class MoveForwardWithoutLine : Descriptor
    {
        public override void Generate(Plant plant, GenerationContext context, Symbol symbol)
        {
            context.MoveForwardWithoutLine(plant.plantAsset.length);
        }
    }
    
    [Symbol("F")]
    public class MoveForwardWithLine : MoveForwardWithoutLine
    {
        private static void GeneratePipe(GenerationContext context, int sideCount, Line line, Symbol symbol)
        {
            var buffer = context.buffer;
            var meshInfo = buffer.Prepare(symbol.organFlags, sideCount * 2);

            line = line.Elongate(symbol.length + 0.01f);
            
            var sAxis = line.start.rotation * Vector3.forward;
            var eAxis = line.end.rotation * Vector3.forward;
            
            var sPosition = line.start.position;
            var sRadius = line.start.diameter * 0.5f;
            
            var sFirstSide = (line.start.rotation * Vector3.up).normalized;
            var eFirstSide = (line.end.rotation * Vector3.up).normalized;
            
            var ePosition = line.end.position;
            var eRadius = line.end.diameter * 0.5f;

            var stepAngle = 360.0f / sideCount;

            var head = meshInfo.GetVertexCount();
            Vector3 headVertexPosition = Vector3.zero;
            int eIndex = 0;
            float minSqrDistance = float.MaxValue;
            for (int i = 0; i < sideCount; i++)
            {
                var sVertex = Quaternion.AngleAxis(i * stepAngle, sAxis) * sFirstSide * sRadius + sPosition;
                var sNormal = Quaternion.AngleAxis((i - 0.5f) * stepAngle, sAxis) * sFirstSide;
                meshInfo.AppendVertex(sVertex, sNormal);
                if (i == 0)
                {
                    headVertexPosition = sVertex;
                }
                var eVertex = Quaternion.AngleAxis(i * stepAngle, eAxis) * eFirstSide * eRadius + ePosition;
                var eNormal = Quaternion.AngleAxis((i - 0.5f) * stepAngle, eAxis) * eFirstSide;
                meshInfo.AppendVertex(eVertex, eNormal);

                float sqrDistance = (headVertexPosition - eVertex).sqrMagnitude;
                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    eIndex = i;
                }
            }

            for (int i = 0; i < sideCount; i++)
            {
                meshInfo.AppendTriangle(
                    head + (2 * i + 0) % (2 * sideCount),
                    head + (2 * i + 2) % (2 * sideCount),
                    head + (2 * (i + eIndex) + 1) % (2 * sideCount));

                meshInfo.AppendTriangle(
                    head + (2 * (i + eIndex) + 1) % (2 * sideCount),
                    head + (2 * i + 2) % (2 * sideCount),
                    head + (2 * (i + eIndex) + 3) % (2 * sideCount));
            }
        }
        
        public override void Generate(Plant plant, GenerationContext context, Symbol symbol)
        {
            using var old = context.Clone();
            context.MoveForwardWithLine(plant.plantAsset.length, symbol);
            if (symbol.merged)
                return;
            GeneratePipe(context, plant.sideCount, context.last, symbol);
        }
    }
}
