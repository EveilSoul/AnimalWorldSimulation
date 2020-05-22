using Unity.Entities;
using UnityEngine;


[GenerateAuthoringComponent]
public struct AnimalSize : IComponentData
{
    [Range(0, 10)]
    public float value;
}