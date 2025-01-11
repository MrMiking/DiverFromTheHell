using Unity.Entities;
using Unity.Mathematics;
public struct NavAgentComponent : IComponentData
{
    public float3 targetPosition;
    public bool pathCalculated;
    public int currentWaypoint;
    public float moveSpeed;
    public float nextPathCalculateTime;
    public bool querySet;
}

public struct WaypointBuffer : IBufferElementData
{
    public float3 wayPoint;
}