using Unity.Entities;
using UnityEngine;

public class CameraAuthoring : MonoBehaviour
{
    public float range;

    private class CameraBaker : Baker<CameraAuthoring>
    {
        public override void Bake(CameraAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new CameraComponent
            {
                range = authoring.range,
            });
        }
    }
}