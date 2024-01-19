using System;

namespace ProceduralPlant.Core
{
    [Flags]
    public enum OrganFlags
    {
        None = 0,
        Branch = 1 << 0,
        Leaf = 1 << 1,
        Tip = 1 << 10,
    }
        
    public abstract class Node
    {
        public OrganFlags organFlags { get; set; } = OrganFlags.None;
        
        public Node next { get; set; } = null;

        public Node last
        {
            get
            {
                var node = this;
                while (node.next != null)
                {
                    node = node.next;
                }
                return node;
            }
        }

        public abstract Node Clone();

        public T Clone<T>() where T : Node => Clone() as T;

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
