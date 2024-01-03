using UnityEngine;

namespace ProceduralPlant.Core
{
    public abstract class Descriptor
    {
        public string name { get; internal set; }

        public abstract GenerationContext Generate(LindenmayerSystem lindenmayerSystem, GenerationContext context, Symbol symbol);
    }
}
