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
        playerTransform.Position += new float3(_inputComponent.Movement * _playerComponent.MoveSpeed * SystemAPI.Time.DeltaTime, 0f);

        Vector2 direction = (Vector2)_inputComponent.MousePosition - (Vector2)Camera.main.WorldToScreenPoint(playerTransform.Position);
        float angle = math.degrees(math.atan2(direction.y, direction.x));
        playerTransform.Rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        _entityManager.SetComponentData(_playerEntity, playerTransform);
    }

    private void Shoot(ref SystemState state)
    {

    }
}