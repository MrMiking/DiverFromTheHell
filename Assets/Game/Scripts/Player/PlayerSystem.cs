using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEditor;

public partial struct PlayerSystem : ISystem
{
    private EntityManager _entityManager;

    private Entity _playerEntity;
    private Entity _inputEntity;

    private PlayerComponent _playerComponent;
    private InputComponent _inputComponent;

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        _playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        _inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();

        _playerComponent = _entityManager.GetComponentData<PlayerComponent>(_playerEntity);
        _inputComponent = _entityManager.GetComponentData<InputComponent>(_inputEntity);

        Move(ref state);
        Shoot(ref state);
    }

    private void Move(ref SystemState state)
    {
        float rotationSpeed = 10;

        LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);

        playerTransform.Position += new float3(_inputComponent.Movement.x, 0, _inputComponent.Movement.y) 
            * _playerComponent.moveSpeed * SystemAPI.Time.DeltaTime;

        Plane playerPlane = new Plane(Vector3.up, playerTransform.Position);

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(_inputComponent.MousePosition.x, _inputComponent.MousePosition.y, 0));

        if(playerPlane.Raycast(ray, out float hitDistance))
        {
            Vector3 targetPoint = ray.GetPoint(hitDistance);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - new Vector3(playerTransform.Position.x, playerTransform.Position.y, playerTransform.Position.z));

            playerTransform.Rotation = Quaternion.Slerp(playerTransform.Rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        _entityManager.SetComponentData(_playerEntity, playerTransform);
    }

    private void Shoot(ref SystemState state)
    {
        if (_inputComponent.Shoot)
        {
            EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);

            Entity bulletEntity = _entityManager.Instantiate(_playerComponent.bulletPrefab);

            ECB.AddComponent(bulletEntity, new BulletComponent
            {
                speed = 25f,
                size = 0.25f,
                damage = 10f
            });

            ECB.AddComponent(bulletEntity, new BulletLifeTimeComponent
            {
                remainingLifeTime = 1.5f
            });

            LocalTransform bulletTransform = _entityManager.GetComponentData<LocalTransform>(bulletEntity);
            LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);

            bulletTransform.Rotation = playerTransform.Rotation;

            bulletTransform.Position = playerTransform.Position + playerTransform.Forward();

            ECB.SetComponent(bulletEntity, bulletTransform);
            ECB.Playback(_entityManager);

            ECB.Dispose();
        }
    }
}