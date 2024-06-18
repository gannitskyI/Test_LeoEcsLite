using Leopotam.EcsLite;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsWorld _world;
    private EcsFilter _playerFilter;
    private EcsFilter _dropZoneFilter;
    private EcsPool<StackComponent> _stackPool;
    private EcsPool<PositionComponent> _positionPool;
    private EcsPool<DropZoneComponent> _dropZonePool;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();
        _playerFilter = _world.Filter<StackComponent>().Inc<InputComponent>().End();
        _dropZoneFilter = _world.Filter<DropZoneComponent>().End();
        _stackPool = _world.GetPool<StackComponent>();
        _positionPool = _world.GetPool<PositionComponent>();
        _dropZonePool = _world.GetPool<DropZoneComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var playerEntity in _playerFilter)
        {
            ref var stack = ref _stackPool.Get(playerEntity);
            ref var playerPosition = ref _positionPool.Get(playerEntity);

            foreach (var dropZoneEntity in _dropZoneFilter)
            {
                ref var dropZone = ref _dropZonePool.Get(dropZoneEntity);

                if (dropZone.Collider.bounds.Contains(playerPosition.Position))
                {
                     
                    if (IsDropButtonPressed())
                    {
                        DropItems(ref stack, ref dropZone);
                    }
                    break;
                }
            }
        }
    }

 
    private bool IsDropButtonPressed()
    {
       
        return Input.GetMouseButtonDown(0) && EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.name == "DropButton";
    }

    private void DropItems(ref StackComponent stack, ref DropZoneComponent dropZone)
    {
        stack.DroppedItems = new List<GameObject>();

        var droppedItems = _world.Filter<DroppedItemComponent>().End();

        float totalHeight = 0f;

        foreach (var droppedItemEntity in droppedItems)
        {
            ref var droppedItemComponent = ref _world.GetPool<DroppedItemComponent>().Get(droppedItemEntity);
            totalHeight += droppedItemComponent.GameObject.GetComponent<Collider>().bounds.size.y;
        }

        foreach (var item in stack.Stack)
        {
            item.SetActive(true);
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Collider>().isTrigger = true;

            Vector3 dropPosition = new Vector3(dropZone.Collider.bounds.center.x,
                                                dropZone.Collider.bounds.max.y + totalHeight,
                                                dropZone.Collider.bounds.center.z);
            item.transform.position = dropPosition;

            
            item.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

            stack.DroppedItems.Add(item);

            var entity = _world.NewEntity();
            ref var droppedItemComponent = ref _world.GetPool<DroppedItemComponent>().Add(entity);
            droppedItemComponent.GameObject = item;

            totalHeight += item.GetComponent<Collider>().bounds.size.y;
        }

        stack.Stack.Clear();
    }
}
