using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

// Система питания
[UpdateAfter(typeof(DamageSystem))]
public class EatinfSystem : ComponentSystem
{
    private float delay = 3f;

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Eating eating, ref LifeStatus inProcess) =>
        {
            if (UnityEngine.Time.time - eating.deathTime >= delay)
            {
                PostUpdateCommands.RemoveComponent(entity, typeof(Eating));
                inProcess.status = LivingStatus.Nothing;
            }
        });
    }
}