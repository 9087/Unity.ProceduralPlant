using System;
using UnityEngine;

namespace ProceduralPlant.Core
{
    [Serializable]
    public class ParametersInfo
    {
        [SerializeField] [Range(0, 180)] public float angle = 22.5f;
        [SerializeField] [Min(0)] public float length = 3;
        [SerializeField] [Range(3, 16)] public int sideCount = 12;
        [SerializeField] [Range(0, 1)] public float thinningRate = 0.3f;
        [SerializeField] [Min(0)] public float initialDiameter = 1.0f;
        [SerializeField] public int randomSeed = 0;
    }
}
