using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial class NavAgentSystem : ISystem
{
    [BurstCompile]
    private void OnUpdate(ref SystemState state)
    {
        /*foreach(var (navAgent, transform, entity) in SystemAPI.Query<RefRW<NavAgentComponent>, RefRW<LocalTransform>>().WithEntityAccess())
        {

        }*/
    }
}