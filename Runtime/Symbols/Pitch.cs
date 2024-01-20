using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("&")]
    public class PitchIncrease : Descriptor
    {
        public override void Generate(GenerationContext context, LindenmayerSystem lindenmayerSystem, Symbol symbol)
        {
            context.Rotate(Quaternion.Euler(lindenmayerSystem.parametersInfo.angle, 0, 0));
        }
    }
    
    [Symbol("^")]
    public class PitchDecrease : Descriptor
    {
        public override void Generate(GenerationContext context, LindenmayerSystem lindenmayerSystem, Symbol symbol)
        {
            context.Rotate(Quaternion.Euler(-lindenmayerSystem.parametersInfo.angle, 0, 0));
        }
    }
}
