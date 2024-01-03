using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("\\")]
    public class RollIncrease : Descriptor
    {
        public override TransformData Populate(LindenmayerSystem lindenmayerSystem, TransformData transformData, Symbol symbol)
        {
            return new TransformData(
                transformData.transform,
                transformData.position,
                transformData.rotation * Quaternion.Euler(0, 0, lindenmayerSystem.parameterInfo.angle),
                transformData.scale
                );
        }
    }
    
    [Symbol("/")]
    public class RollDecrease : Descriptor
    {
        public override TransformData Populate(LindenmayerSystem lindenmayerSystem, TransformData transformData, Symbol symbol)
        {
            return new TransformData(
                transformData.transform,
                transformData.position,
                transformData.rotation * Quaternion.Euler(0, 0, -lindenmayerSystem.parameterInfo.angle),
                transformData.scale
                );
        }
    }
}
