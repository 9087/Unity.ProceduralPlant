using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("'")]
    public class ColorIncrease : Descriptor
    {
        public override GenerationContext Generate(LindenmayerSystem lindenmayerSystem, GenerationContext context, Symbol symbol)
        {
            Debug.LogWarning("Descriptor `ColorIncrease(')` is not implemented!");
            return context;
        }
    }
}
