using Unity.Entities;

[GenerateAuthoringComponent]
public struct Diet : IComponentData
{
    public DietType type;
}

// None mean that it's a plant
public enum DietType
{
    Carnivorous,
    Herbivores,
    None
}