using Leopotam.EcsLite;
using UnityEngine;

public class CameraFollowSystem : IEcsInitSystem
{
    private EcsWorld _world;
    private EcsFilter _filter;
    private EcsPool<PlayerPrefabComponent> _playerPool;
    private EcsPool<CameraComponent> _cameraPool;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();
        _filter = _world.Filter<PlayerPrefabComponent>().Inc<CameraComponent>().End();
        _playerPool = _world.GetPool<PlayerPrefabComponent>();
        _cameraPool = _world.GetPool<CameraComponent>();

        foreach (var entity in _filter)
        {
            ref var playerPrefabComponent = ref _playerPool.Get(entity);
            ref var cameraComponent = ref _cameraPool.Get(entity);

            if (cameraComponent.VirtualCamera != null && playerPrefabComponent.Prefab != null)
            {
                cameraComponent.VirtualCamera.Follow = playerPrefabComponent.Prefab.transform;
            }
        }
    }
}
