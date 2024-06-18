using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CollectSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsWorld _world;
    private EcsFilter _playerFilter;
    private EcsFilter _collectibleFilter;
    private EcsPool<StackComponent> _stackPool;
    private EcsPool<CollectibleComponent> _collectiblePool;
    private EcsPool<PositionComponent> _positionPool;
    private Button _takeButton;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();
        _playerFilter = _world.Filter<StackComponent>().End();
        _collectibleFilter = _world.Filter<CollectibleComponent>().End();
        _stackPool = _world.GetPool<StackComponent>();
        _collectiblePool = _world.GetPool<CollectibleComponent>();
        _positionPool = _world.GetPool<PositionComponent>();

        var buttonManager = GameObject.FindObjectOfType<ButtonManager>();
        if (buttonManager != null)
        {
            _takeButton = buttonManager.TakeButton;
            if (_takeButton != null)
            {
                _takeButton.onClick.AddListener(CollectItems);
            }
            else
            {
                Debug.LogWarning("TakeButton component not found on the ButtonManager.");
            }
        }
        else
        {
            Debug.LogWarning("ButtonManager not found in the scene.");
        }
    }

    public void Run(IEcsSystems systems)
    {
        if (IsButtonPressed())
        {
            CollectItems();
        }
    }

    private void CollectItems()
    {
        foreach (var playerEntity in _playerFilter)
        {
            ref var stack = ref _stackPool.Get(playerEntity);
            if (stack.Stack.Count >= 5) continue;

            ref var playerPosition = ref _positionPool.Get(playerEntity);

            foreach (var collectibleEntity in _collectibleFilter)
            {
                ref var collectible = ref _collectiblePool.Get(collectibleEntity);
                if (collectible.IsCollected) continue;

                ref var collectiblePosition = ref _positionPool.Get(collectibleEntity);
                float distance = Vector3.Distance(playerPosition.Position, collectiblePosition.Position);

                if (distance < collectible.CollectDistance)
                {
                    collectible.IsCollected = true;
                    GameObject collectibleObject = collectiblePosition.GameObject;
                    stack.Stack.Add(collectibleObject);
                    collectibleObject.SetActive(false);

                    if (stack.Stack.Count >= 5) break;
                }
            }
        }
    }

    private bool IsButtonPressed()
    {
        return Input.GetMouseButtonDown(0) && EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null;
    }
}
