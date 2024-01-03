using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("!")]
    public class DiameterDecrease : Descriptor
    {
        public override GenerationContext Generate(LindenmayerSystem lindenmayerSystem, GenerationContext context, Symbol symbol)
        {
            context = context.Thin(lindenmayerSystem.parametersInfo.thinningRate);
            return context;
        }
    }
}
