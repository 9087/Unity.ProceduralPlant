using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("!")]
    public class DiameterDecrease : Descriptor
    {
        public override void Generate(Plant plant, GenerationContext context, Symbol symbol)
        {
            // Diameter generated in `LindenmayerSystem.PregenerateDiameter`
        }
    }
}
