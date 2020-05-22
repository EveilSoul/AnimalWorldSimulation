using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public enum TargetStatus
{
    Danger = 1,
    Neutral = 0,
    Food = 2,
    None=4
}

public struct Eating : IComponentData
{
    public float deathTime;
}

public struct LifeStatus : IComponentData
{
    public LivingStatus status;
}

public enum LivingStatus
{
    Nothing = 0,
    Feeding = 1,
    Danger = 2,
    Death=3,
    Walking=4,
    LookAround=5
}

public struct OnTarget : IComponentData
{
    public Entity fromEntity;
}

public struct Sound : IComponentData
{
    public float radius;
}

public struct HasEnemyNear : IComponentData
{
    public float3 enemyPosition;
}

public struct HasTarget : IComponentData
{
    public Entity targetEntity;
    public float3 targetPosition;
    public TargetStatus targetStatus;
}

public struct Hunting : IComponentData
{
    public Entity targetEntity;
    public float3 targetPosition;
}

public struct Damage : IComponentData
{
    public float value;
    public Entity from;
}

public struct MoveTo : IComponentData
{
    public float3 position;
    public float moveSpeed;
}

public struct Death : IComponentData
{
    public float value;
}