using Unity.Entities;
using UnityEngine;
public class NavAgentAuthoring : MonoBehaviour
{
    [SerializeField] private Transform targetTransfrom;
    [SerializeField] private float moveSpeed;

    private class AuthoringBaker : Baker<NavAgentAuthoring>
    {
        public override void Bake(NavAgentAuthoring authoring)
        {
            Entity authoringEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(authoringEntity, new NavAgentComponent
            {
                targetEnemy = GetEntity(authoring.targetTransfrom, TransformUsageFlags.Dynamic),
                moveSpeed = authoring.moveSpeed
            });

            AddBuffer<WaypointBuffer>(authoringEntity);
        }
    }
}