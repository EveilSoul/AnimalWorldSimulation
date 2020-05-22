using Unity.Collections;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;


public struct QuadrantEntity : IComponentData {
    public int quadro; 
}

public struct QuadrantData {
    public Entity entity;
    public float3 position;
    public float soundRadius;
    public QuadrantEntity quadrantEntity;
    public AnimalSize size;
    public Diet diet;
}

// Система разделяющая квадранты для облегчения поиска ближайших сущностей
public class QuadrantSystem : ComponentSystem {

    // Каждому квадранту (по его индексу) соответствует набор данных о сужностях которые находятся в нем
    public static NativeMultiHashMap<int, QuadrantData> quadrantMultiHashMap;

    // Размеры карты
    public const int quadrantYMultiplier = 1000;
    private const int quadrantCellSize = 50;

    // Выдает индекс квадранта по мировой позиции
    public static int GetPositionHashMapKey(float3 position) {
        return (int) (math.floor(position.x / quadrantCellSize) 
            + (quadrantYMultiplier * math.floor(position.z / quadrantCellSize)));
    }

    // Возвращает количество сущностей в заданном квадранте
    private static int GetEntityCountInHashMap(NativeMultiHashMap<int, QuadrantData> quadrantMultiHashMap, int hashMapKey) {
        QuadrantData quadrantData;
        NativeMultiHashMapIterator<int> nativeMultiHashMapIterator;
        int count = 0;
        if (quadrantMultiHashMap.TryGetFirstValue(hashMapKey, out quadrantData, out nativeMultiHashMapIterator)) {
            do {
                count++;
            } while (quadrantMultiHashMap.TryGetNextValue(out quadrantData, ref nativeMultiHashMapIterator));
        }
        return count;
    }

    [BurstCompile]
    private struct SetQuadrantDataHashMapJob : IJobForEachWithEntity<Translation, QuadrantEntity, Sound, AnimalSize, Diet> {

        public NativeMultiHashMap<int, QuadrantData>.ParallelWriter quadrantMultiHashMap;

        public void Execute(Entity entity, int index, ref Translation translation, ref QuadrantEntity quadrantEntity, ref Sound sound, ref AnimalSize size, ref Diet diet) {
            int hashMapKey = GetPositionHashMapKey(translation.Value);
            quadrantMultiHashMap.Add(hashMapKey, new QuadrantData {
                entity = entity,
                position = translation.Value,
                quadrantEntity = quadrantEntity,
                soundRadius=sound.radius,
                diet=diet,
                size=size
            });
        }
    }

    protected override void OnCreate() {
        quadrantMultiHashMap = new NativeMultiHashMap<int, QuadrantData>(0, Allocator.Persistent);
        base.OnCreate();
    }

    protected override void OnDestroy() {
        quadrantMultiHashMap.Dispose();
        base.OnDestroy();
    }

    // Обновление данных во всех квадрантах
    protected override void OnUpdate() {
        EntityQuery entityQuery = GetEntityQuery(typeof(Translation), typeof(QuadrantEntity), typeof(Sound), typeof(AnimalSize), typeof(Diet));

        Entities.ForEach((ref QuadrantEntity entity, ref Translation translation) =>
        {
            entity.quadro = GetPositionHashMapKey(translation.Value);
        });

        quadrantMultiHashMap.Clear();
        if (entityQuery.CalculateEntityCount() > quadrantMultiHashMap.Capacity) {
            quadrantMultiHashMap.Capacity = entityQuery.CalculateEntityCount();
        }

        SetQuadrantDataHashMapJob setQuadrantDataHashMapJob = new SetQuadrantDataHashMapJob {
            quadrantMultiHashMap = quadrantMultiHashMap.AsParallelWriter(),
        };
        JobHandle jobHandle = JobForEachExtensions.Schedule(setQuadrantDataHashMapJob, entityQuery);
        jobHandle.Complete();
    }
}
