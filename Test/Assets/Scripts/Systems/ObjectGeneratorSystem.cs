using Leopotam.EcsLite;
using UnityEngine;

public class ObjectGeneratorSystem : IEcsInitSystem, IEcsRunSystem
{
    private const int MaxCoins = 10;  
    private const float GenerationInterval = 3f;
    private const string PrefabPath = "Coin_gem";
    private const float DefaultCollectDistance = 0.7f;

    private float _timeToNextGeneration = GenerationInterval;
    private GameObject _prefab;
    private EcsWorld _world;
    private EcsPool<CollectibleComponent> _collectiblePool;
    private EcsPool<PositionComponent> _positionPool;
    private int _generatedCoins = 0; 

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();
        _prefab = Resources.Load<GameObject>(PrefabPath);
        _collectiblePool = _world.GetPool<CollectibleComponent>();
        _positionPool = _world.GetPool<PositionComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        _timeToNextGeneration -= Time.deltaTime;

        if (_timeToNextGeneration <= 0 && _generatedCoins < MaxCoins)
        {
            GenerateObject();
            _timeToNextGeneration = GenerationInterval;
        }
    }

    private void GenerateObject()
    {
        Vector2 randomPos = Random.insideUnitCircle * 5f;
        GameObject newObject = GameObject.Instantiate(_prefab, new Vector3(randomPos.x, 0.5f, randomPos.y), Quaternion.identity);
        newObject.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        var entity = _world.NewEntity();
        ref var collectibleComponent = ref _collectiblePool.Add(entity);
        collectibleComponent.IsCollected = false;
        collectibleComponent.CollectDistance = DefaultCollectDistance;

        ref var positionComponent = ref _positionPool.Add(entity);
        positionComponent.Position = newObject.transform.position;
        positionComponent.GameObject = newObject;

        _generatedCoins++; 
    }
}
