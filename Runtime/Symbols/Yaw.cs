using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("+")]
    public class YawIncrease : Descriptor
    {
        public override TransformData Populate(LindenmayerSystem lindenmayerSystem, TransformData transformData, Symbol symbol)
        {
            return new TransformData(
                transformData.transform,
                transformData.position,
                transformData.rotation * Quaternion.Euler(0, lindenmayerSystem.parameterInfo.angle, 0),
                transformData.scale
                );
        }
    }
    
    [Symbol("-")]
    public class YawDecrease : Descriptor
    {
        public override TransformData Populate(LindenmayerSystem lindenmayerSystem, TransformData transformData, Symbol symbol)
        {
            return new TransformData(
                transformData.transform,
                transformData.position,
                transformData.rotation * Quaternion.Euler(0, -lindenmayerSystem.parameterInfo.angle, 0),
                transformData.scale
                );
        }
    }
    
    [Symbol("|")]
    public class YawTurnaround : Descriptor
    {
        public override TransformData Populate(LindenmayerSystem lindenmayerSystem, TransformData transformData, Symbol symbol)
        {
            return new TransformData(
                transformData.transform,
                transformData.position,
                transformData.rotation * Quaternion.Euler(0, 180, 0),
                transformData.scale
                );
        }
    }
}
