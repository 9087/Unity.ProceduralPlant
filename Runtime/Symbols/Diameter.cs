using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("!")]
    public class DiameterDecrease : Descriptor
    {
        public override void Generate(GenerationContext context, LindenmayerSystem lindenmayerSystem, Symbol symbol)
        {
            context.Thin(lindenmayerSystem.parametersInfo.thinningRate);
        }
    }
}
