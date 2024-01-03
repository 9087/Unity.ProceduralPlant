using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("&")]
    public class PitchIncrease : Descriptor
    {
        public override GenerationContext Generate(LindenmayerSystem lindenmayerSystem, GenerationContext context, Symbol symbol)
        {
            return context.Rotate(Quaternion.Euler(lindenmayerSystem.parametersInfo.angle, 0, 0));
        }
    }
    
    [Symbol("^")]
    public class PitchDecrease : Descriptor
    {
        public override GenerationContext Generate(LindenmayerSystem lindenmayerSystem, GenerationContext context, Symbol symbol)
        {
            return context.Rotate(Quaternion.Euler(-lindenmayerSystem.parametersInfo.angle, 0, 0));
        }
    }
}
