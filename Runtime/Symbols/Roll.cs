using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("\\")]
    public class RollIncrease : Descriptor
    {
        public override void Generate(GenerationContext context, LindenmayerSystem lindenmayerSystem, Symbol symbol)
        {
            context.Rotate(Quaternion.Euler(0, 0, lindenmayerSystem.parametersInfo.angle));
        }
    }
    
    [Symbol("/")]
    public class RollDecrease : Descriptor
    {
        public override void Generate(GenerationContext context, LindenmayerSystem lindenmayerSystem, Symbol symbol)
        {
            context.Rotate(Quaternion.Euler(0, 0, -lindenmayerSystem.parametersInfo.angle));
        }
    }
}
