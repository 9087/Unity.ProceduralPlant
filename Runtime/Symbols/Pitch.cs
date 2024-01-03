using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("&")]
    public class PitchIncrease : Descriptor
    {
        public override TransformData Populate(LindenmayerSystem lindenmayerSystem, TransformData transformData, Symbol symbol)
        {
            return new TransformData(
                transformData.transform,
                transformData.position,
                transformData.rotation * Quaternion.Euler(lindenmayerSystem.parameterInfo.angle, 0, 0),
                transformData.scale
                );
        }
    }
    
    [Symbol("^")]
    public class PitchDecrease : Descriptor
    {
        public override TransformData Populate(LindenmayerSystem lindenmayerSystem, TransformData transformData, Symbol symbol)
        {
            return new TransformData(
                transformData.transform,
                transformData.position,
                transformData.rotation * Quaternion.Euler(-lindenmayerSystem.parameterInfo.angle, 0, 0),
                transformData.scale
                );
        }
    }
}
