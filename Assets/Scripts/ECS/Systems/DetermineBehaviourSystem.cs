using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

// Система определяющая реакцию на посткпившие данные об окружении
[UpdateAfter(typeof(SightAndHearingJobSystem))]
public class DetermineBehaviourSystem : ComponentSystem
{
    private float checkDangerDelay = 1f;
    private float lastCheckDanger = 0;

    protected override void OnUpdate()
    {
        Entities.WithNone(typeof(Tag_Plant)).ForEach((Entity entity, ref HasTarget hasTarget, ref LifeStatus lifeStatus, ref Translation translation, ref Rotation rotation) =>
        {
            if (hasTarget.targetStatus == TargetStatus.Danger)
            {
                if (lifeStatus.status == LivingStatus.Feeding)
                {
                    PostUpdateCommands.RemoveComponent(entity, typeof(Hunting));
                }
                lifeStatus.status = LivingStatus.Danger;
                // Если еще не было проверки окружения на опасность или прошло достаточно времени для следующей проверки
                if (UnityEngine.Time.time - lastCheckDanger > checkDangerDelay)
                {
                    PostUpdateCommands.RemoveComponent(entity, typeof(MoveTo));

                    // Задаем направление движения противоположное врагу
                    float lenght = 10f;
                    float3 target = translation.Value + math.normalize(translation.Value - hasTarget.targetPosition) * lenght;
                    PostUpdateCommands.AddComponent(entity, new MoveTo()
                    {
                        moveSpeed = 7,
                        position = target
                    });

                    lastCheckDanger = UnityEngine.Time.time;
                }
                return;
            }

            // Если была опасность, но она уже достаточно далеко ставис статус для того чтобы оглянуться
            if (lifeStatus.status == LivingStatus.Danger && (hasTarget.targetStatus == TargetStatus.Food || hasTarget.targetStatus == TargetStatus.None))
            {
                lifeStatus.status = LivingStatus.LookAround;
            }

            // Если после оглядывания не было замечано опасности 
            if (lifeStatus.status == LivingStatus.LookAround && (hasTarget.targetStatus == TargetStatus.Food || hasTarget.targetStatus == TargetStatus.None))
            {
                lifeStatus.status = LivingStatus.Nothing;
            }

            if (hasTarget.targetStatus == TargetStatus.Food)
            {
                //Debug.Log(entity.Index + " has target food");
                // Если сущность ничем не занята и на цель никто не нацелен
                if ((lifeStatus.status == LivingStatus.Nothing || lifeStatus.status == LivingStatus.Walking) 
                && !GameStateData.ActiveEntityManager.HasComponent<OnTarget>(hasTarget.targetEntity))
                {
                    // Ставим статус поиска еды
                    lifeStatus.status = LivingStatus.Feeding;
                    PostUpdateCommands.RemoveComponent(entity, typeof(MoveTo));
                    PostUpdateCommands.RemoveComponent(entity, typeof(Hunting));

                    PostUpdateCommands.AddComponent(entity, new MoveTo()
                    {
                        moveSpeed = 4,
                        position = hasTarget.targetPosition
                    });
                    PostUpdateCommands.AddComponent(entity, new Hunting
                    {
                        targetEntity = hasTarget.targetEntity,
                        targetPosition = hasTarget.targetPosition
                    });
                    // Делаем добавление быстрым, чтобы не возникло ситуации, когда на одного entity нацелено сразу несколько
                    GameStateData.ActiveEntityManager.AddComponent<OnTarget>(hasTarget.targetEntity);
                    GameStateData.ActiveEntityManager.SetComponentData<OnTarget>(hasTarget.targetEntity, new OnTarget
                    {
                        fromEntity = entity
                    });
                    return;
                }
            }

            // Если не было найдено цели
            if (hasTarget.targetStatus == TargetStatus.None && lifeStatus.status!=LivingStatus.Feeding)
            {
                lifeStatus.status = LivingStatus.Walking;
            }
        });
    }
}