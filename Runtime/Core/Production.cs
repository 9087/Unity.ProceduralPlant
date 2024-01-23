namespace ProceduralPlant.Core
{
    public class Production
    {
        public Node structure { get; private set; }
        
        public float probability { get; private set; }

        public Production(Node structure, float probability)
        {
            this.structure = structure;
            this.probability = probability;
        }
    }
}