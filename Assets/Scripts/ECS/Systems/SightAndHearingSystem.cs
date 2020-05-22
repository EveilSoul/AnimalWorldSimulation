using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;

// Система реализующая зрение и слух
[UpdateAfter(typeof(QuadrantSystem))]
public class SightAndHearingJobSystem : JobComponentSystem {

    private struct EntityWithPosition {
        public Entity entity;
        public float3 position;
    }

    private struct EntityDescription
    {
        public Diet diet;
        public AnimalSize size;
        public Sight sight;
        public LocalToWorld location;
        public QuadrantEntity quadrantEntity;
        public LifeStatus lifeStatus;
    }

    // Добавление компонента HasTarget для дальнейшей обработки
    [RequireComponentTag(typeof(Tag_Animal))]
    private struct AddComponentJob : IJobForEachWithEntity<Translation, PathFollow> {

        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<EntityWithPosition> closestDangerTargetArray;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<EntityWithPosition> closestFoodTargetArray;
        public EntityCommandBuffer.Concurrent entityCommandBuffer;

        public void Execute(Entity entity, int index, ref Translation translation, ref PathFollow pathFollow)
        {
            if (closestDangerTargetArray[index].entity != Entity.Null)
            {
                entityCommandBuffer.RemoveComponent(index, entity, typeof(HasTarget));
                entityCommandBuffer.AddComponent(index, entity, new HasTarget
                {
                    targetEntity = closestDangerTargetArray[index].entity,
                    targetPosition = closestDangerTargetArray[index].position,
                    targetStatus = TargetStatus.Danger
                });
            }
            else
            {
                if (closestFoodTargetArray[index].entity != Entity.Null)
                {
                    entityCommandBuffer.RemoveComponent(index, entity, typeof(HasTarget));
                    entityCommandBuffer.AddComponent(index, entity, new HasTarget
                    {
                        targetEntity = closestFoodTargetArray[index].entity,
                        targetPosition = closestFoodTargetArray[index].position,
                        targetStatus = TargetStatus.Food
                    });
                }
                else
                {
                    entityCommandBuffer.RemoveComponent(index, entity, typeof(HasTarget));
                    entityCommandBuffer.AddComponent(index, entity, new HasTarget
                    {
                        targetEntity = Entity.Null,
                        targetPosition = float3.zero,
                        targetStatus = TargetStatus.None
                    });
                }
            }
        }

    }

    
    [RequireComponentTag(typeof(Tag_Animal))]
    [BurstCompile]
    private struct FindEnemyQuadrantSystemJob : IJobForEachWithEntity<Sight, LocalToWorld, QuadrantEntity, Diet, AnimalSize, LifeStatus> {

        [ReadOnly] public NativeMultiHashMap<int, QuadrantData> quadrantMultiHashMap;
        public NativeArray<EntityWithPosition> closestDangerTargetArray;
        public NativeArray<EntityWithPosition> closestFoodTargetArray;

        public void Execute(Entity entity, int index, [ReadOnly] ref Sight sight, 
            [ReadOnly] ref LocalToWorld location, [ReadOnly] ref QuadrantEntity quadrantEntity, [ReadOnly] ref Diet diet, 
            [ReadOnly] ref AnimalSize size, [ReadOnly] ref LifeStatus lifeStatus) {

            EntityWithPosition closestDangerTarget = new EntityWithPosition { entity = Entity.Null };
            float closestDangerTargetDistance = float.MaxValue;
            EntityWithPosition closestFoodTarget = new EntityWithPosition { entity = Entity.Null };
            float closestFoodTargetDistance = float.MaxValue;
            int hashMapKey = QuadrantSystem.GetPositionHashMapKey(location.Position);

            var entityDesc = new EntityDescription() {
                sight=sight, 
                diet=diet, 
                location=location, 
                quadrantEntity=quadrantEntity, 
                size=size,
                lifeStatus=lifeStatus
            };


            FindTarget(index, hashMapKey, entityDesc, ref closestDangerTarget, ref closestDangerTargetDistance, ref closestFoodTarget, ref closestFoodTargetDistance);
            FindTarget(index, hashMapKey + 1, entityDesc, ref closestDangerTarget, ref closestDangerTargetDistance, ref closestFoodTarget, ref closestFoodTargetDistance);
            FindTarget(index, hashMapKey - 1, entityDesc, ref closestDangerTarget, ref closestDangerTargetDistance, ref closestFoodTarget, ref closestFoodTargetDistance);
            FindTarget(index, hashMapKey + QuadrantSystem.quadrantYMultiplier, entityDesc, ref closestDangerTarget, ref closestDangerTargetDistance, ref closestFoodTarget, ref closestFoodTargetDistance);
            FindTarget(index, hashMapKey - QuadrantSystem.quadrantYMultiplier, entityDesc, ref closestDangerTarget, ref closestDangerTargetDistance, ref closestFoodTarget, ref closestFoodTargetDistance);
            FindTarget(index, hashMapKey + 1 + QuadrantSystem.quadrantYMultiplier, entityDesc, ref closestDangerTarget, ref closestDangerTargetDistance, ref closestFoodTarget, ref closestFoodTargetDistance);
            FindTarget(index, hashMapKey - 1 + QuadrantSystem.quadrantYMultiplier, entityDesc, ref closestDangerTarget, ref closestDangerTargetDistance, ref closestFoodTarget, ref closestFoodTargetDistance);
            FindTarget(index, hashMapKey + 1 - QuadrantSystem.quadrantYMultiplier, entityDesc, ref closestDangerTarget, ref closestDangerTargetDistance, ref closestFoodTarget, ref closestFoodTargetDistance);
            FindTarget(index, hashMapKey - 1 - QuadrantSystem.quadrantYMultiplier, entityDesc, ref closestDangerTarget, ref closestDangerTargetDistance, ref closestFoodTarget, ref closestFoodTargetDistance);

            closestDangerTargetArray[index] = closestDangerTarget;
            closestFoodTargetArray[index] = closestFoodTarget;
        }

        private void FindTarget(int entityIndex, int hashMapKey, EntityDescription entityDesc, ref EntityWithPosition closestDangerTarget, ref float closestDangerTargetDistance,
            ref EntityWithPosition closestFoodTarget, ref float closestFoodTargetDistance) {

            QuadrantData quadrantData;
            NativeMultiHashMapIterator<int> nativeMultiHashMapIterator;

            if (quadrantMultiHashMap.TryGetFirstValue(hashMapKey, out quadrantData, out nativeMultiHashMapIterator)) {
                do {
                    var canHear = Helper.CanHear(quadrantData.position, entityDesc.location.Position, quadrantData.soundRadius);
                    var canSee = Helper.CanSee(quadrantData.position, entityDesc.location, entityDesc.sight.angle);
                    var canSeeWithDoubleAngle = Helper.CanSee(quadrantData.position, entityDesc.location, entityDesc.sight.angle * 2);

                    // Check that it's different entity
                    if (quadrantData.entity.Index != entityIndex)
                    {
                        // Если видит врага или слышит его и при оглядывании видит или видит при оглядывании спасаясь от опасности
                        if (canSee || (canHear && canSeeWithDoubleAngle) || (entityDesc.lifeStatus.status==LivingStatus.LookAround && canSeeWithDoubleAngle))
                        {
                            var closestTargetStatus = TargetStatus.Neutral;

                            if (Helper.CanEat(entityDesc.size, entityDesc.diet, quadrantData.size, quadrantData.diet))
                                closestTargetStatus = TargetStatus.Food;
                            if (Helper.CanEat(quadrantData.size, quadrantData.diet, entityDesc.size, entityDesc.diet))
                                closestTargetStatus = TargetStatus.Danger;

                            if(closestTargetStatus == TargetStatus.Danger)
                            {
                                ChangeClosestTarget(ref closestDangerTarget, ref closestDangerTargetDistance, quadrantData, entityDesc);
                            }
                            if (closestTargetStatus == TargetStatus.Food)
                            {
                                ChangeClosestTarget(ref closestFoodTarget, ref closestFoodTargetDistance, quadrantData, entityDesc);
                            }

                        }
                    }
                    
                } while (quadrantMultiHashMap.TryGetNextValue(out quadrantData, ref nativeMultiHashMapIterator));
            }
        }
    }

    private static void ChangeClosestTarget(ref EntityWithPosition closestTarget, ref float closestTargetDistance, QuadrantData quadrantData, EntityDescription entityDesc)
    {
        if (closestTarget.entity == Entity.Null)
        {
            // No target
            closestTarget = new EntityWithPosition { entity = quadrantData.entity, position = quadrantData.position };
            closestTargetDistance = math.distancesq(entityDesc.location.Position, quadrantData.position);
        }
        else
        {
            if (math.distancesq(entityDesc.location.Position, quadrantData.position) < closestTargetDistance)
            {
                // This target is closer
                closestTarget = new EntityWithPosition { entity = quadrantData.entity, position = quadrantData.position };
                closestTargetDistance = math.distancesq(entityDesc.location.Position, quadrantData.position);
            }
        }
    }

    private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

    protected override void OnCreate() {
        endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityQuery animalQuery = GetEntityQuery(typeof(Tag_Animal), ComponentType.ReadOnly<LocalToWorld>());

        NativeArray<EntityWithPosition> closestDangerTargetArray = new NativeArray<EntityWithPosition>(animalQuery.CalculateEntityCount(), Allocator.TempJob);
        NativeArray<EntityWithPosition> closestFoodTargetArray = new NativeArray<EntityWithPosition>(animalQuery.CalculateEntityCount(), Allocator.TempJob);

        FindEnemyQuadrantSystemJob findTargetQuadrantSystemJob = new FindEnemyQuadrantSystemJob
        {
            quadrantMultiHashMap = QuadrantSystem.quadrantMultiHashMap,
            closestDangerTargetArray = closestDangerTargetArray,
            closestFoodTargetArray = closestFoodTargetArray
        };
        JobHandle jobHandle = findTargetQuadrantSystemJob.Schedule(this, inputDeps);

        // Add HasTarget Component to Entities that have a Closest Target
        AddComponentJob addComponentJob = new AddComponentJob
        {
            closestDangerTargetArray = closestDangerTargetArray,
            closestFoodTargetArray = closestFoodTargetArray,
            entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
        };
        jobHandle = addComponentJob.Schedule(this, jobHandle);

        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }

}

