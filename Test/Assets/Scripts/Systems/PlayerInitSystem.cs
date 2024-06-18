using Leopotam.EcsLite;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInitSystem : IEcsInitSystem
{
    private const string PrefabPath = "Player";
    private const string CameraPath = "PlayerCamera";

    public void Init(IEcsSystems systems)
    {
        var world = systems.GetWorld();
        var playerPrefab = Resources.Load<GameObject>(PrefabPath);

        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not found in Resources folder.");
            return;
        }

        var entity = world.NewEntity();
        InitializePlayerComponents(world, entity, playerPrefab);
        InitializeCameraComponents(world, entity);
        InitializeUIComponents(world, entity);
        InitializeDropZone(world);
    }

    private void InitializePlayerComponents(EcsWorld world, int entity, GameObject playerPrefab)
    {
        var playerInstance = GameObject.Instantiate(playerPrefab);
        var rb = playerInstance.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        ref var playerPrefabComponent = ref world.GetPool<PlayerPrefabComponent>().Add(entity);
        playerPrefabComponent.Prefab = playerInstance;

        ref var positionComponent = ref world.GetPool<PositionComponent>().Add(entity);
        positionComponent.Position = playerInstance.transform.position;
        positionComponent.GameObject = playerInstance;

        world.GetPool<InputComponent>().Add(entity);

        ref var playerMovementComponent = ref world.GetPool<PlayerMovementComponent>().Add(entity);
        playerMovementComponent.RotationSpeed = 20f;
        playerMovementComponent.MovementSpeed = 1.5f;

        ref var animationComponent = ref world.GetPool<AnimationComponent>().Add(entity);
        animationComponent.Animator = playerInstance.GetComponent<Animator>();
        animationComponent.IsMoving = false;
        animationComponent.RotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        ref var stackComponent = ref world.GetPool<StackComponent>().Add(entity);
        stackComponent.Stack = new List<GameObject>();
    }

    private void InitializeCameraComponents(EcsWorld world, int entity)
    {
        var cameraPrefab = Resources.Load<GameObject>(CameraPath);
        if (cameraPrefab == null)
        {
            Debug.LogError("Camera prefab not found in Resources folder.");
            return;
        }

        var cameraInstance = GameObject.Instantiate(cameraPrefab);
        ref var cameraComponent = ref world.GetPool<CameraComponent>().Add(entity);
        cameraComponent.VirtualCamera = cameraInstance.GetComponent<Cinemachine.CinemachineVirtualCamera>();
    }

    private void InitializeUIComponents(EcsWorld world, int entity)
    {
        ref var uiComponent = ref world.GetPool<UIComponent>().Add(entity);
        uiComponent.StackInfoText = GetTextMeshProComponent("StackCountText");
        uiComponent.DropStackInfoText = GetTextMeshProComponent("DropStackText");
    }

    private TextMeshProUGUI GetTextMeshProComponent(string name)
    {
        var textComponent = GameObject.Find(name)?.GetComponent<TextMeshProUGUI>();
        if (textComponent == null)
        {
            Debug.LogWarning($"TextMeshPro component not found on {name} object.");
        }
        return textComponent;
    }

    private void InitializeDropZone(EcsWorld world)
    {
        var dropZonePrefab = Resources.Load<GameObject>("DropZone");
        if (dropZonePrefab == null)
        {
            Debug.LogError("DropZone prefab not found in Resources folder.");
            return;
        }

        var dropZoneInstance = GameObject.Instantiate(dropZonePrefab);
        CreateDropZoneEntity(world, dropZoneInstance);
    }

    private void CreateDropZoneEntity(EcsWorld world, GameObject dropZoneInstance)
    {
        var dropZoneEntity = world.NewEntity();
        ref var dropZoneComponent = ref world.GetPool<DropZoneComponent>().Add(dropZoneEntity);
        dropZoneComponent.Collider = dropZoneInstance.GetComponent<Collider>();

        var droppedItemsZoneEntity = world.NewEntity();
        ref var droppedItemsZoneComponent = ref world.GetPool<DroppedItemsZoneComponent>().Add(droppedItemsZoneEntity);
        droppedItemsZoneComponent.Collider = dropZoneInstance.GetComponent<Collider>();
    }
}
