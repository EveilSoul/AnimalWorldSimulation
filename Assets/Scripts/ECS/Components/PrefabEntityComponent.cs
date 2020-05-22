using Unity.Entities;

[GenerateAuthoringComponent]
public struct PrefabEntityComponent : IComponentData {

    public Entity prefabEntity;
    public int count;
}
