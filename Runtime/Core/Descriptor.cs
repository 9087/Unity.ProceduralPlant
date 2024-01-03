using UnityEngine;

namespace ProceduralPlant.Core
{
    public abstract class Descriptor
    {
        public string name { get; internal set; }

        public abstract TransformData Populate(LindenmayerSystem lindenmayerSystem, TransformData transformData, Symbol symbol);
    }
}
