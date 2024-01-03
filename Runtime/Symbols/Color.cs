using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("'")]
    public class ColorIncrease : Descriptor
    {
        public override TransformData Populate(LindenmayerSystem lindenmayerSystem, TransformData transformData, Symbol symbol)
        {
            Debug.LogWarning("Descriptor `ColorIncrease(')` is not implemented!");
            return transformData;
        }
    }
}
