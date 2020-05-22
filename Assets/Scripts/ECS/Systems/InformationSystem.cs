using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

// Система выведения информации о сущности
public class SelectionSystem : ComponentSystem
{
    private float3 pointPosition;
    private float maxDistance = 1f;

    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                pointPosition = hit.point;
            }

            Entities.WithAll(typeof(Tag_Animal)).ForEach((Entity entity, ref Translation translation) =>
            {
                if (math.distancesq(translation.Value, pointPosition) <= maxDistance)
                {
                    Debug.Log(entity.Index);
                }
            });
        }
    }
}