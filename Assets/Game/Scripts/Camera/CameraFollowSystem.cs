using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
public partial struct CameraFollowSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        Camera camera = Camera.main;

        if (camera == null) Debug.LogError("No main camera Found");

        foreach (var (followTarget, distance, smoothSpeed) in SystemAPI.Query<Target, CameraDistance, CameraSmoothSpeed>())
        {
            float3 targetPosition = SystemAPI.GetComponent<LocalTransform>(followTarget.Value).Position;

            float3 desiredPosition = targetPosition + new float3(0, distance.Value, 0);

            camera.transform.position = math.lerp(camera.transform.position, desiredPosition, smoothSpeed.Value * SystemAPI.Time.DeltaTime);
        }
    }
}