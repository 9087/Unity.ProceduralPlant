using System;

namespace ProceduralPlant.Core
{
    public abstract class Node
    {
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
