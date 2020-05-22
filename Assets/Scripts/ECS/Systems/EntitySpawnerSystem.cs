using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


// Система спавна сущностей
public class EntitySpawnerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref PrefabEntityComponent prefab) =>
        {
            for (int i = 0; i < prefab.count; i++)
            {
                Entity spawnedEntity = EntityManager.Instantiate(prefab.prefabEntity);

                EntityManager.AddComponent(spawnedEntity, typeof(Sound));
                EntityManager.AddComponent(spawnedEntity, typeof(QuadrantEntity));
                EntityManager.AddComponent(spawnedEntity, typeof(LifeStatus));

                EntityManager.SetComponentData(spawnedEntity, new Sound
                {
                    radius = Helper.GetSoundRadius(spawnedEntity)
                });
                EntityManager.SetComponentData(spawnedEntity,
                      new Translation { Value = new float3(UnityEngine.Random.Range(-100f, 100f), 0, UnityEngine.Random.Range(-100, 100)) }
                  );
                EntityManager.SetComponentData(spawnedEntity,
                      new LifeStatus { status = LivingStatus.Nothing }
                  );
            }

            PostUpdateCommands.RemoveComponent(entity, typeof(PrefabEntityComponent));
            PostUpdateCommands.AddComponent(entity, typeof(Tag_Prefab));
        });
    }
}
