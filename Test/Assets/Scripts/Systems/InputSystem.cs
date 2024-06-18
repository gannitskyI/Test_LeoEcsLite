using Leopotam.EcsLite;
using UnityEngine;

public class InputSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsWorld _world;
    private EcsFilter _filter;
    private EcsPool<InputComponent> _inputPool;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();
        _filter = _world.Filter<InputComponent>().End();
        _inputPool = _world.GetPool<InputComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entity in _filter)
        {
            ref var inputComponent = ref _inputPool.Get(entity);
            inputComponent.JoystickInput = new Vector2(SimpleInput.GetAxis("Horizontal"), SimpleInput.GetAxis("Vertical"));


        }
    }
}
