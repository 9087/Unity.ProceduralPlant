using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("!")]
    public class DiameterDecrease : Descriptor
    {
        public override TransformData Populate(LindenmayerSystem lindenmayerSystem, TransformData transformData, Symbol symbol)
        {
            Debug.LogWarning("Descriptor `DiameterDecrease(!)` is not implemented!");
            return transformData;
        }
    }
}
