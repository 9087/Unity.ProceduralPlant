namespace ProceduralPlant.Core
{
    internal class Branch : Node
    {
        public Node content { get; set; } = null;
        
        public Branch(Node structure)
        {
            this.content = structure;
        }

        public override Node Clone()
        {
            var branch = new Branch(this.content.Clone());
            branch.next = this.next?.Clone();
            return branch;
        }

        public override string ToString()
        {
            return $"[{content}]" + next;
        }
    }
}
