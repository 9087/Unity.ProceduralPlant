using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("\\")]
    public class RollIncrease : Descriptor
    {
        public override void Generate(Plant plant, GenerationContext context, Symbol symbol)
        {
            context.Rotate(Quaternion.Euler(0, 0, plant.plantAsset.angle));
        }
    }
    
    [Symbol("/")]
    public class RollDecrease : Descriptor
    {
        public override void Generate(Plant plant, GenerationContext context, Symbol symbol)
        {
            context.Rotate(Quaternion.Euler(0, 0, -plant.plantAsset.angle));
        }
    }
}
