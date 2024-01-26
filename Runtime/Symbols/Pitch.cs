using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("&")]
    public class PitchIncrease : Descriptor
    {
        public override void Generate(Plant plant, GenerationContext context, Symbol symbol)
        {
            context.Rotate(Quaternion.Euler(plant.plantAsset.angle, 0, 0));
        }
    }
    
    [Symbol("^")]
    public class PitchDecrease : Descriptor
    {
        public override void Generate(Plant plant, GenerationContext context, Symbol symbol)
        {
            context.Rotate(Quaternion.Euler(-plant.plantAsset.angle, 0, 0));
        }
    }
}
