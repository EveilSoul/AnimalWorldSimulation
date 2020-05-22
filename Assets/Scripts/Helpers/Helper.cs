using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;



public static class Helper
{
    public static float GetAngle(float3 x, LocalToWorld y)
    {
        var targetVector = x - y.Position;
        var v = (y.Forward.x * targetVector.x + y.Forward.z * targetVector.z) /
            math.sqrt((y.Forward.x * y.Forward.x + y.Forward.z * y.Forward.z) *
            (targetVector.x * targetVector.x + targetVector.z * targetVector.z));
        return math.acos(v) * 180 / math.PI;
    }

    public static bool CanSee(float3 enemyPos, LocalToWorld entityLocation, float entitySightAngle)
    {
        var angle = GetAngle(enemyPos, entityLocation);
        return angle <= entitySightAngle;
    }

    public static bool CanHear(float3 enemyPos, float3 entityPosition, float soundRadius)
    {
        return math.distance(enemyPos, entityPosition) <= soundRadius;
    }

    public static bool CanEat(AnimalSize size, Diet diet, AnimalSize foodSize, Diet foodDiet)
    {
        if (diet.type == DietType.Herbivores)
        {
            if (foodDiet.type == DietType.None)
                return true;
            else
                return false;
        }
        if (diet.type == DietType.Carnivorous)
        {
            if (foodDiet.type == DietType.Herbivores && foodSize.value <= (size.value - 1.5f))
                return true;
            if (foodDiet.type == DietType.Carnivorous && foodSize.value <= (size.value - 2))
                return true;
            else
                return false;
        }
        return false;
    }

    public static float GetDamage(Entity from, Entity to)
    {
        return 20f;
    }

    public static float GetSoundRadius(Entity entity)
    {
        return 2f;
    }
}

