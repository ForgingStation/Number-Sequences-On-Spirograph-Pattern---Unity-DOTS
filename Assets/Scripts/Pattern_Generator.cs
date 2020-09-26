using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using System.Collections.Generic;

public class Pattern_Generator : MonoBehaviour
{
    public int maxUnitsToSpawn;
    public int initialRatioQn;
    public int initialRatioQd;
    public int largeRadius;
    public int smallRadius;
    public float unit_MinDistanceReached;
    public int unit_MinSpeed;
    public int unit_MaxSpeed;
    public Mesh mesh;
    public Material material;
    public float changeEvery;
    public static Pattern_Generator instance { get; private set; }

    private EntityManager entitymamager;
    private EntityArchetype ea;
    private Vector3 destinationToSet;
    private Vector3 currentPosition;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        entitymamager = World.DefaultGameObjectInjectionWorld.EntityManager;
        ea = entitymamager.CreateArchetype(
                    typeof(Translation),
                    typeof(Rotation),
                    typeof(LocalToWorld),
                    typeof(RenderMesh),
                    typeof(RenderBounds),
                    typeof(AtomicUnit)
                    );
        currentPosition = transform.position;
        for (int i=1;i<=maxUnitsToSpawn;i++)
        {
            Entity e = entitymamager.CreateEntity(ea);
            entitymamager.AddComponentData(e, new Translation
            {
                Value = currentPosition
            });
            entitymamager.AddComponentData(e, new AtomicUnit
            {
                entityId = i,
                minDistanceReached = unit_MinDistanceReached,
                Speed = UnityEngine.Random.Range(unit_MinSpeed, unit_MaxSpeed),
            });
            entitymamager.AddSharedComponentData(e, new RenderMesh
            {
                mesh = mesh,
                material = material,
                castShadows = UnityEngine.Rendering.ShadowCastingMode.On
            });
        }
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 400, 40), "Fibonacci Series");
        GUI.skin.box.fontSize = 25;
    }
}

public struct AtomicUnit : IComponentData
{
    public int entityId;
    public float3 toLocation;
    public bool reached;
    public float3 waypointDirection;
    public float Speed;
    public float minDistanceReached;
    public float ratioQnDebug;
}






