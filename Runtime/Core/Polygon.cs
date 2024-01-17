using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPlant.Core
{
    internal class Polygon : Node
    {
        public Node content { get; set; } = null;
        
        public Polygon(Node structure)
        {
            this.content = structure;
        }

        public override Node Clone()
        {
            var polygon = new Polygon(this.content.Clone());
            polygon.next = this.next?.Clone();
            return polygon;
        }

        public override string ToString()
        {
            return $"{{{content}}}" + next;
        }

        public static void Generate(LindenmayerSystem lindenmayerSystem, GenerationContext context, List<GenerationContext.Point> points)
        {
            points.RemoveAt(points.Count - 1);
            Vector3 positionAverage = Vector3.zero;
            Vector3 normalAverage = Vector3.zero;
            foreach (var point in points)
            {
                positionAverage += point.position;
            }
            positionAverage /= points.Count;
            var meshInfo = context.meshInfos[1];
            var head = meshInfo.vertices.Count;
            for (int i = 0; i < points.Count; ++i)
            {
                GenerationContext.Point last = points[(i - 1 + points.Count) % points.Count];
                GenerationContext.Point next = points[(i + 1) % points.Count];
                GenerationContext.Point current = points[i];
                var normal = Vector3.Cross(last.position - current.position, next.position - current.position);
                meshInfo.vertices.Add(current.position);
                meshInfo.normals.Add(normal);
                normalAverage += normal;
            }
            normalAverage /= points.Count;
            meshInfo.vertices.Add(positionAverage);
            meshInfo.normals.Add(normalAverage);
            for (int i = 0; i < points.Count; ++i)
            {
                int last = head + (i - 1 + points.Count) % points.Count;
                int current = head + (i) % points.Count;
                int center = head + points.Count;
                meshInfo.indices.Add(last);
                meshInfo.indices.Add(center);
                meshInfo.indices.Add(current);
                meshInfo.indices.Add(current);
                meshInfo.indices.Add(center);
                meshInfo.indices.Add(last);
            }
        }
    }
}
