using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

// Система блуждания
[UpdateAfter(typeof(DetermineBehaviourSystem))]
public class WalkingSystem : ComponentSystem
{
    static Unity.Mathematics.Random random = new Unity.Mathematics.Random(5);
    float lenght = 5f;

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Translation translation, ref Rotation rotation, ref LifeStatus lifeStatus, ref PathFollow pathFollow) =>
        {
            // Если текущая сущность уже в процессе перемещения
            if (lifeStatus.status != LivingStatus.Walking || pathFollow.pathIndex>=0)
            {
                return;
            }

            // Определяем направление с небольшим случайным отклонением
            var randomAngle = random.NextFloat(0, 1);
            var forwardVec = (math.forward(rotation.Value) + new float3(randomAngle, 0, (1 - randomAngle))) * lenght;
            forwardVec.y = 0;

            PostUpdateCommands.AddComponent(entity, new MoveTo()
            {
                moveSpeed = 3,
                position = translation.Value + forwardVec
            });
        });
    }
}