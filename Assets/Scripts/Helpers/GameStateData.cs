using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


public static class GameStateData
{
    public static Mesh[] Meshes;
    public static Material[] Materials;

    public static Dictionary<string, Entity> Prefabs;

    private static EntityManager _activeEntityManager;

    public static EntityManager ActiveEntityManager
    {
        get
        {
            if (_activeEntityManager == null)
            {
                _activeEntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            }

            return _activeEntityManager;
        }
    }

    public static EntityArchetype AnimalArchetype = ActiveEntityManager.CreateArchetype(
            typeof(QuadrantEntity),
            typeof(Health),
            typeof(MoveTo),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(Translation),
            typeof(Rotation),
            typeof(Sight),
            typeof(Sound),
            typeof(Tag_Animal),
            typeof(Diet),
            typeof(AnimalSize)
        );

    public static EntityArchetype PlantArchetype = ActiveEntityManager.CreateArchetype(
            typeof(QuadrantEntity),
            typeof(Health),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(Translation),
            typeof(Rotation),
            typeof(Sight),
            typeof(Sound),
            typeof(Tag_Animal),
            typeof(Diet),
            typeof(AnimalSize),
            typeof(Tag_Plant)
        );

    public static RenderMesh GetRenderMesh(int index)
    {
        return new RenderMesh
        {
            mesh = Meshes[index],
            material = Materials[index]
        };
    }
}
