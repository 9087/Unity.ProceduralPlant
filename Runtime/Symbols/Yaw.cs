using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("+")]
    public class YawIncrease : Descriptor
    {
        public override void Generate(Plant plant, GenerationContext context, Symbol symbol)
        {
            context.Rotate(Quaternion.Euler(0, plant.plantAsset.angle, 0));
        }
    }
    
    [Symbol("-")]
    public class YawDecrease : Descriptor
    {
        public override void Generate(Plant plant, GenerationContext context, Symbol symbol)
        {
            context.Rotate(Quaternion.Euler(0, -plant.plantAsset.angle, 0));
        }
    }
    
    [Symbol("|")]
    public class YawTurnaround : Descriptor
    {
        public override void Generate(Plant plant, GenerationContext context, Symbol symbol)
        {
            context.Rotate(Quaternion.Euler(0, 180, 0));
        }
    }
}
