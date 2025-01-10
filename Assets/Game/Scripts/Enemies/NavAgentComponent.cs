using Unity.Entities;
using Unity.Mathematics;
public struct NavAgentComponent : IComponentData
{
    public Entity targetEnemy;
    public bool pathCalculated;
    public int currentWaypoint;
    public float moveSpeed;
    public float nextPathCalculateTime;
}

public struct WaypointBuffer : IBufferElementData
{
    public float3 wayPoint;
}