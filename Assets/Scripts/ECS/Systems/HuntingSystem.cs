using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

// Система охоты
[UpdateAfter(typeof(DetermineBehaviourSystem))]
public class HuntingSystem : ComponentSystem
{
    private float damageDistance = 9f;

    protected override void OnUpdate()
    {
        Entities.WithAll(typeof(LifeStatus)).ForEach((Entity entity, ref Hunting huntInfo, ref Translation translation) =>
        {
            // Если дистанция меньше заданной 
            if (math.distancesq(translation.Value, huntInfo.targetPosition) <= damageDistance)
            {
                // Проверяем жива ли цель
                bool isTargetAlive = GameStateData.ActiveEntityManager.Exists(huntInfo.targetEntity) &&
                GameStateData.ActiveEntityManager.GetComponentData<LifeStatus>(huntInfo.targetEntity).status != LivingStatus.Death;
                // Если цель уже убита
                if (!isTargetAlive)
                    return;

                // Получаем урон, который будет нанесен
                var damage = Helper.GetDamage(entity, huntInfo.targetEntity);
                PostUpdateCommands.AddComponent(huntInfo.targetEntity, new Damage { value = damage, from = entity });
            }
        });
    }
}