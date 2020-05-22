using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

// Система отвечающая за нанесение урона
[UpdateAfter(typeof(HuntingSystem))]
public class DamageSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Damage damage, ref Health health, ref LifeStatus lifeStatus) =>
        {
            if (health.value <= 0)
                return;

            // Убавляем из текущего здоровья нанесенный урон
            health.value -= damage.value;

            // Если здоровье стало меньше 0
            if (health.value <= 0)
            {
                // Меняем статус на "мертвый"
                lifeStatus.status = LivingStatus.Death;

                GameStateData.ActiveEntityManager.RemoveComponent(damage.from, typeof(Hunting));
                GameStateData.ActiveEntityManager.RemoveComponent(entity, typeof(QuadrantEntity));
                GameStateData.ActiveEntityManager.RemoveComponent(entity, typeof(PathFollow));
                GameStateData.ActiveEntityManager.RemoveComponent(entity, typeof(OnTarget));

                PostUpdateCommands.AddComponent(damage.from, new Eating
                {
                    deathTime = UnityEngine.Time.time
                });
                PostUpdateCommands.AddComponent(entity, new Death { value = UnityEngine.Time.time });

                Debug.Log(entity.Index + " is dead");
            }

            PostUpdateCommands.RemoveComponent(entity, typeof(Damage));
        });
    }
}