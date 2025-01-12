using Unity.Collections;
using Unity.Entities;

public partial struct DeathSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        foreach (var (health, entity) in SystemAPI.Query<Health>().WithEntityAccess())
        {
            if (health.currentHealth <= 0)
            {
                ecb.DestroyEntity(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
