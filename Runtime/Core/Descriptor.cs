using UnityEngine;

namespace ProceduralPlant.Core
{
    public abstract class Descriptor
    {
        public string name { get; internal set; }

        public abstract void Generate(GenerationContext context, LindenmayerSystem lindenmayerSystem, Symbol symbol);
    }
}
