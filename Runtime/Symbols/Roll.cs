using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("\\")]
    public class RollIncrease : Descriptor
    {
        public override GenerationContext Generate(LindenmayerSystem lindenmayerSystem, GenerationContext context, Symbol symbol)
        {
            return context.Rotate(Quaternion.Euler(0, 0, lindenmayerSystem.parametersInfo.angle));
        }
    }
    
    [Symbol("/")]
    public class RollDecrease : Descriptor
    {
        public override GenerationContext Generate(LindenmayerSystem lindenmayerSystem, GenerationContext context, Symbol symbol)
        {
            return context.Rotate(Quaternion.Euler(0, 0, -lindenmayerSystem.parametersInfo.angle));
        }
    }
}
