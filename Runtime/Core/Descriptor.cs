using UnityEngine;

namespace ProceduralPlant.Core
{
    public abstract class Descriptor
    {
        public string name { get; internal set; }

        public abstract void Generate(Plant plant, GenerationContext context, Symbol symbol);
    }
}
