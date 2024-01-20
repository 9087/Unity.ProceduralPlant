using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("'")]
    public class ColorIncrease : Descriptor
    {
        public static bool WarningRaised = false; 
        
        public override void Generate(GenerationContext context, LindenmayerSystem lindenmayerSystem, Symbol symbol)
        {
            if (!WarningRaised)
            {
                Debug.LogWarning("Descriptor `ColorIncrease(')` is not implemented!");
                WarningRaised = true;
            }
        }
    }
}
