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
    }
}
