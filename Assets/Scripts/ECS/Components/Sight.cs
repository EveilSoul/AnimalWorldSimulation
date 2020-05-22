using Unity.Entities;

[GenerateAuthoringComponent]
public struct Sight : IComponentData
{
    public float radius;
    public float angle;
}