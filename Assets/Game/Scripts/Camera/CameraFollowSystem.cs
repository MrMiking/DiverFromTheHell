using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct CameraFollowSystem : ISystem
{
    private EntityManager entityManager;

    private Entity playerEntity;
    private Entity cameraEntity;

    private CameraComponent cameraComponent;

    private void OnUpdate(ref SystemState state)
    {
        entityManager = state.EntityManager;

        playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        cameraEntity = SystemAPI.GetSingletonEntity<CameraComponent>();

        cameraComponent = entityManager.GetComponentData<CameraComponent>(cameraEntity);

        MoveCamera(ref state);
    }

    private void MoveCamera(ref SystemState state)
    {
        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
        LocalTransform cameraTransform = entityManager.GetComponentData<LocalTransform>(cameraEntity);
        
        cameraTransform.Position = playerTransform.Position + new float3(0, cameraComponent.range, 0);
        Debug.Log(cameraTransform.Position);
        entityManager.SetComponentData(cameraEntity, cameraTransform);
    }
}