using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public class CameraFollowAuthoring : MonoBehaviour
{
    public GameObject target;
    public float distance;
    public float smoothSpeed;

    public class CameraFollowBaker : Baker<CameraFollowAuthoring>
    {
        public override void Bake(CameraFollowAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new Target { Value = GetEntity(authoring.target, TransformUsageFlags.Dynamic) });
            AddComponent(entity, new CameraDistance { Value = authoring.distance });
            AddComponent(entity, new CameraSmoothSpeed { Value = authoring.smoothSpeed });
        }
    }
}

public struct CameraDistance : IComponentData { public float Value; }
public struct CameraSmoothSpeed : IComponentData { public float Value; }