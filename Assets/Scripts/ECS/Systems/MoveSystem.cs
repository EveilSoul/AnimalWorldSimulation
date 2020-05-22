using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;


// Система перемещения
[UpdateAfter(typeof(QuadrantSystem))]
public class MoveSystem : ComponentSystem
{
    public float deltaTime;

    protected override void OnUpdate()
    {
        deltaTime = Time.DeltaTime;

        Entities.WithNone(typeof(Tag_Plant), typeof(Tag_Prefab))
            .ForEach((Entity entity, ref MoveTo moveTo, ref Translation translation, ref Rotation rotation) =>
        {
            if (moveTo.moveSpeed == 0)
                return;

            int startX, startY, targetX, targetY;
            PathfindingGridSetup.Instance.pathfindingGrid.GetXY(translation.Value, out startX, out startY);
            PathfindingGridSetup.Instance.pathfindingGrid.GetXY(moveTo.position, out targetX, out targetY);

            PostUpdateCommands.AddComponent(entity, new PathfindingParams
            {
                startPosition = new int2(startX, startY),
                endPosition = new int2(targetX, targetY)
            });
            PostUpdateCommands.RemoveComponent(entity, typeof(MoveTo));
        });
    }
}