using Leopotam.EcsLite;
using TMPro;
using UnityEngine;

public class UISystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsWorld _world;
    private EcsFilter _playerFilter;
    private EcsPool<UIComponent> _uiPool;
    private EcsPool<StackComponent> _stackPool;
    public TextMeshProUGUI StackInfoText { get; private set; }
    public TextMeshProUGUI DropStackInfoText { get; private set; }

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();
        _playerFilter = _world.Filter<StackComponent>().End();
        _uiPool = _world.GetPool<UIComponent>();
        _stackPool = _world.GetPool<StackComponent>();

    }

    public void Run(IEcsSystems systems)
    {
        foreach (var playerEntity in _playerFilter)
        {
            ref var stack = ref _stackPool.Get(playerEntity);
            ref var uiComponent = ref _uiPool.Get(playerEntity);

            // Проверяем наличие компонентов TextMeshPro в UIComponent
            if (uiComponent.StackInfoText != null)
            {
                uiComponent.StackInfoText.text = $"{stack.Stack.Count}/5";
            }
            else
            {
                Debug.LogWarning("TextMeshPro component not found in UIComponent. Cannot update stack count.");
            }
 
        }
    }
}
