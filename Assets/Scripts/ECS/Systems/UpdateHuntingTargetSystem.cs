using Unity.Entities;
using Unity.Transforms;

// Система отвечающая за обновление позиции цели
[UpdateAfter(typeof(DetermineBehaviourSystem))]
public class UpdateHuntingTargetSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone(typeof(Tag_Plant)).ForEach((Entity entity, ref Translation translation, ref OnTarget onTargetInfo) =>
        {
            if (UnityEngine.Time.frameCount % 40 == 0)
            {
                var hunt = GameStateData.ActiveEntityManager.GetComponentData<Hunting>(onTargetInfo.fromEntity);
                PostUpdateCommands.RemoveComponent<Hunting>(onTargetInfo.fromEntity);
                PostUpdateCommands.AddComponent(onTargetInfo.fromEntity, new Hunting
                {
                    targetEntity = entity,
                    targetPosition = translation.Value
                });
                PostUpdateCommands.RemoveComponent<MoveTo>(onTargetInfo.fromEntity);
                PostUpdateCommands.AddComponent(onTargetInfo.fromEntity, new MoveTo
                {
                    moveSpeed = 3,
                    position = translation.Value
                });
            }
        });
    }
}