using ProceduralPlant.Core;
using UnityEngine;

namespace ProceduralPlant.Symbols
{
    [Symbol("f")]
    public class MoveForwardWithoutLine : Descriptor
    {
        public override TransformData Populate(LindenmayerSystem lindenmayerSystem, TransformData transformData, Symbol symbol)
        {
            return new TransformData(
                transformData.transform,
                transformData.rotation * Vector3.forward * lindenmayerSystem.parameterInfo.length + transformData.position,
                transformData.rotation,
                transformData.scale
                );
        }
    }
    
    [Symbol("F")]
    public class MoveForwardWithLine : MoveForwardWithoutLine
    {
        public override TransformData Populate(LindenmayerSystem lindenmayerSystem, TransformData transformData, Symbol symbol)
        {
            var old = transformData;
            transformData = base.Populate(lindenmayerSystem, transformData, symbol);
            var model = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            model.hideFlags = HideFlags.HideAndDontSave;
            model.name = "Model";
            var cylinder = new GameObject("Cylinder");
            cylinder.hideFlags = HideFlags.HideAndDontSave;
            model.transform.localRotation = Quaternion.Euler(90, 0, 0);
            model.transform.localPosition = new Vector3(0, 0, lindenmayerSystem.parameterInfo.length * 0.5f);
            model.transform.localScale = new Vector3(1, lindenmayerSystem.parameterInfo.length * 0.5f, 1);
            model.transform.SetParent(cylinder.transform);
            var transform = cylinder.transform;
            transform.SetParent(transformData.transform);
            var delta = transformData.position - old.position;
            transform.localRotation = Quaternion.FromToRotation(Vector3.forward, delta);
            transform.localPosition = old.position;
            transform.localScale = old.scale;
            return transformData;
        }
    }
}
