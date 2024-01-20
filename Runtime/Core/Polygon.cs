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

        public static void Generate(GenerationContext context, LindenmayerSystem lindenmayerSystem, Polygon polygon, List<Point> points)
        {
            var buffer = context.buffer;
            points.RemoveAt(points.Count - 1);
            var meshInfo = buffer.Prepare(polygon.organFlags, points.Count + 1);
            Vector3 positionAverage = Vector3.zero;
            Vector3 normalAverage = Vector3.zero;
            foreach (var point in points)
            {
                positionAverage += point.position;
            }
            positionAverage /= points.Count;
            var head = meshInfo.GetVertexCount();
            for (int i = 0; i < points.Count; ++i)
            {
                Point last = points[(i - 1 + points.Count) % points.Count];
                Point next = points[(i + 1) % points.Count];
                Point current = points[i];
                var normal = Vector3.Cross(last.position - current.position, next.position - current.position);
                meshInfo.AppendVertex(current.position, normal);
                normalAverage += normal;
            }
            normalAverage /= points.Count;
            meshInfo.AppendVertex(positionAverage, normalAverage);
            for (int i = 0; i < points.Count; ++i)
            {
                int last = head + (i - 1 + points.Count) % points.Count;
                int current = head + (i) % points.Count;
                int center = head + points.Count;
                meshInfo.AppendTriangle(last, center, current);
                meshInfo.AppendTriangle(current, center, last);
            }
        }
    }
}
