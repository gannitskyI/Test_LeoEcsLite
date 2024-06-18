using Leopotam.EcsLite;
using UnityEngine;

public class MovementSystem : IEcsInitSystem, IEcsRunSystem, IEcsFixedRunSystem
{
    private EcsWorld _world;
    private EcsFilter _filter;
    private EcsPool<PositionComponent> _positionPool;
    private EcsPool<InputComponent> _inputPool;
    private EcsPool<PlayerMovementComponent> _movementPool;
    private EcsPool<AnimationComponent> _animationPool;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();
        _filter = _world.Filter<PlayerPrefabComponent>()
                        .Inc<PositionComponent>()
                        .Inc<InputComponent>()
                        .Inc<PlayerMovementComponent>()
                        .Inc<AnimationComponent>()
                        .End();
        _positionPool = _world.GetPool<PositionComponent>();
        _inputPool = _world.GetPool<InputComponent>();
        _movementPool = _world.GetPool<PlayerMovementComponent>();
        _animationPool = _world.GetPool<AnimationComponent>();
    }

    public void Run(IEcsSystems systems)
    {
       
    }

    public void FixedRun(IEcsSystems systems)
    {
        foreach (var entity in _filter)
        {
            ref var positionComponent = ref _positionPool.Get(entity);
            ref var inputComponent = ref _inputPool.Get(entity);
            ref var movementComponent = ref _movementPool.Get(entity);
            ref var animationComponent = ref _animationPool.Get(entity);

            var playerPrefabComponent = _world.GetPool<PlayerPrefabComponent>().Get(entity);
            var playerObject = playerPrefabComponent.Prefab;

            if (playerObject == null)
            {
                Debug.LogError("Player object not found.");
                continue;
            }

            var rb = playerObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody not found on player object.");
                continue;
            }

            Vector3 movement = new Vector3(inputComponent.JoystickInput.x, 0, inputComponent.JoystickInput.y);
            if (movement.magnitude > 0)
            {
                animationComponent.IsMoving = true;

                Quaternion targetRotation = Quaternion.LookRotation(movement);
                playerObject.transform.rotation = Quaternion.Slerp(playerObject.transform.rotation, targetRotation, movementComponent.RotationSpeed * Time.fixedDeltaTime);
            }
            else
            {
                animationComponent.IsMoving = false;
            }

            positionComponent.Position = rb.position;

            if (animationComponent.Animator != null)
            {
                animationComponent.Animator.SetBool("IsMoving", animationComponent.IsMoving);
            }

            Vector3 newPosition = rb.position + movement.normalized * movementComponent.MovementSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
    }
}
