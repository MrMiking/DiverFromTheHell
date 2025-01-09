using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

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
        LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);

        playerTransform.Position += new float3(
            _inputComponent.Movement.x,0,_inputComponent.Movement.y) * _playerComponent.moveSpeed * SystemAPI.Time.DeltaTime;

        Vector2 direction = (Vector2)_inputComponent.MousePosition - (Vector2)Camera.main.WorldToScreenPoint(new Vector2(
            playerTransform.Position.x, 
            playerTransform.Position.z));
        float angle = math.degrees(math.atan2(direction.x, direction.y));
        playerTransform.Rotation = Quaternion.AngleAxis(angle, Vector3.up);

        _entityManager.SetComponentData(_playerEntity, playerTransform);
    }

    private void Shoot(ref SystemState state)
    {
        if (_inputComponent.Shoot)
        {
            for (int i = 0; i < _playerComponent.numOfBulletsToSpawn; i++)
            {
                EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);

                Entity bulletEntity = _entityManager.Instantiate(_playerComponent.bulletPrefab);

                ECB.AddComponent(bulletEntity, new BulletComponent
                {
                    speed = 25f
                });

                ECB.AddComponent(bulletEntity, new BulletLifeTimeComponent
                {
                    remainingLifeTime = 1.5f
                });

                LocalTransform bulletTransform = _entityManager.GetComponentData<LocalTransform>(bulletEntity);
                LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);

                bulletTransform.Rotation = playerTransform.Rotation;

                float randomOffset = UnityEngine.Random.Range(-_playerComponent.bulletSpread, _playerComponent.bulletSpread);
                bulletTransform.Position = playerTransform.Position + playerTransform.Forward() * 1.65f + (bulletTransform.Right() * randomOffset);

                ECB.SetComponent(bulletEntity, bulletTransform);
                ECB.Playback(_entityManager);

                ECB.Dispose();
            }
        }
    }
}