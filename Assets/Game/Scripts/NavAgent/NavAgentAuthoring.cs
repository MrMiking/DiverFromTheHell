using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
public class NavAgentAuthoring : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private class AuthoringBaker : Baker<NavAgentAuthoring>
    {
        public override void Bake(NavAgentAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new NavAgentComponent
            {
                moveSpeed = authoring.moveSpeed
            });

            AddBuffer<WaypointBuffer>(entity);
        }
    }
}