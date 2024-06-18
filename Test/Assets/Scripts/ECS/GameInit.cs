using Leopotam.EcsLite;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    private EcsWorld _world;
    private IEcsSystems _systems;

    void Start()
    {
        Application.targetFrameRate = 60;

        _world = new EcsWorld();
        _systems = new EcsSystems(_world);

        _systems
            .Add(new InputSystem())
            .Add(new MovementSystem())
            .Add(new PlayerInitSystem())
            .Add(new UISystem())  
            .Add(new CameraFollowSystem())
            .Add(new ObjectGeneratorSystem())
            .Add(new CollectSystem())
            .Add(new DropSystem()) 
            .Init();
    }

    void Update()
    {
        _systems.Run();
    }

    void FixedUpdate()
    {
        foreach (var system in _systems.GetAllSystems())
        {
            if (system is IEcsFixedRunSystem fixedRunSystem)
            {
                fixedRunSystem.FixedRun(_systems);
            }
        }
    }

    void OnDestroy()
    {
        _systems.Destroy();
        _world.Destroy();
    }
}
