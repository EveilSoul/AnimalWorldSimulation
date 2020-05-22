using Unity.Entities;

// Система отвечающая за удаление сущностей 
[UpdateAfter(typeof(DamageSystem))]
public class DeathSystem : ComponentSystem
{
    public float deathTime = 5f;

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Death death) =>
        {
            // Раз в 10 кадров проверяем прошло ли достаточно времени для удаления сущности
            if (UnityEngine.Time.frameCount % 100 == 0)
            {
                if (UnityEngine.Time.time - death.value > deathTime)
                    PostUpdateCommands.DestroyEntity(entity);
            }
        });
    }
}