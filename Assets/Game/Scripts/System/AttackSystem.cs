using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct AttackSystem : ISystem
{
    private EntityQuery targetQuery;

    public void OnCreate(ref SystemState state)
    {
        targetQuery = state.GetEntityQuery(ComponentType.ReadOnly<Health>(), ComponentType.ReadOnly<LocalTransform>());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float elapsedTime = (float)SystemAPI.Time.ElapsedTime;

        var targetEntities = targetQuery.ToEntityArray(Allocator.Temp);
        var targetHealths = targetQuery.ToComponentDataArray<Health>(Allocator.Temp);
        var targetTransforms = targetQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

        foreach (var (attack, transform) in SystemAPI.Query<RefRW<Attack>, RefRO<LocalTransform>>())
        {
            if (attack.ValueRO.isAttacking && elapsedTime - attack.ValueRO.lastAttackTime < attack.ValueRO.cooldown)
                continue;

            attack.ValueRW.isAttacking = false;

            for (int i = 0; i < targetEntities.Length; i++)
            {
                if(!IsValidTarget(ref state, targetEntities[i], attack.ValueRO.targetType)) return;

                if (math.distance(transform.ValueRO.Position, targetTransforms[i].Position) <= attack.ValueRO.attackRange)
                {
                    var targetHealth = targetHealths[i];
                    targetHealth.currentHealth -= attack.ValueRO.damage;
                    state.EntityManager.SetComponentData(targetEntities[i], targetHealth);

                    attack.ValueRW.isAttacking = true;
                    attack.ValueRW.lastAttackTime = elapsedTime;
                    break;
                }
            }
        }

        targetEntities.Dispose();
        targetHealths.Dispose();
        targetTransforms.Dispose();
    }

    private bool IsValidTarget(ref SystemState state, Entity targetEntity, TargetType targetPreference)
    {
        bool isPlayer = state.EntityManager.HasComponent<PlayerTag>(targetEntity);
        bool isEnemy = state.EntityManager.HasComponent<EnemyTag>(targetEntity);

        return targetPreference switch
        {
            TargetType.Player => isPlayer,
            TargetType.Enemy => isEnemy,
            TargetType.Everyone => true,
            _ => false
        };
    }
}