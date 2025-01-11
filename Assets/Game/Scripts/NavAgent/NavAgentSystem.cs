using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Experimental.AI;
using Unity.Jobs;
using UnityEngine.UIElements;

[BurstCompile]
public partial struct NavAgentsystem : ISystem
{
    private EntityManager entityManager;
    private Entity playerEntity;

    private EntityQuery entityQuery;
    private NavMeshWorld navMeshWorld;
    private BufferLookup<WaypointBuffer> waypointBufferLookup;

    private NativeArray<Entity> entities;
    private NativeArray<EntityCommandBuffer> ecbs;
    private NativeList<NavMeshQuery> navMeshQueries;

    [BurstCompile]
    private void OnCreate(ref SystemState state)
    {
        entityQuery = new EntityQueryBuilder(Allocator.Persistent)
            .WithAll<NavAgentComponent>()
            .WithAll<LocalTransform>()
            .Build(ref state);

        navMeshWorld = NavMeshWorld.GetDefaultWorld();
        waypointBufferLookup = state.GetBufferLookup<WaypointBuffer>(true);

        navMeshQueries = new NativeList<NavMeshQuery>(Allocator.Persistent);
    }

    [BurstCompile]
    private void OnDestroy(ref SystemState state)
    {
        foreach(NavMeshQuery n in navMeshQueries)
        {
            n.Dispose();
        }

        navMeshQueries.Dispose();
    }

    [BurstCompile]
    private void OnUpdate(ref SystemState state)
    {
        entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();

        entities = entityQuery.ToEntityArray(Allocator.TempJob);
        ecbs = new NativeArray<EntityCommandBuffer>(entities.Length, Allocator.TempJob);

        for (int i = 0; i < entities.Length; i++)
        {
            ecbs[i] = new EntityCommandBuffer(Allocator.TempJob);
        }

        waypointBufferLookup.Update(ref state);

        NativeArray<JobHandle> handles = new NativeArray<JobHandle>(entities.Length, Allocator.TempJob);
        NativeArray<NavAgentComponent> agents = entityQuery.ToComponentDataArray<NavAgentComponent>(Allocator.TempJob);
        NativeArray<LocalTransform> transforms = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        double elapsedTime = SystemAPI.Time.ElapsedTime;
        for (int i = 0; i < entities.Length; i++)
        {
            if (agents[i].querySet == false)
            {
                NavAgentComponent agent = agents[i];
                navMeshQueries.Add(new NavMeshQuery(navMeshWorld, Allocator.Persistent, 1000));
                agent.querySet = true;
                agents[i] = agent;
            }

            if (agents[i].nextPathCalculateTime < elapsedTime)
            {
                CalculatePathJob calculatePathJob = new CalculatePathJob
                {
                    entity = entities[i],
                    agent = agents[i],
                    fromPosition = transforms[i].Position,
                    toPosition = entityManager.GetComponentData<LocalTransform>(playerEntity).Position,
                    ecb = ecbs[i],
                    query = navMeshQueries[i]
                };

                handles[i] = calculatePathJob.Schedule();
            }
            else if (agents[i].pathCalculated)
            {
                MoveJob moveJob = new MoveJob
                {
                    entity = entities[i],
                    agent = agents[i],
                    toPosition = entityManager.GetComponentData<LocalTransform>(playerEntity).Position,
                    transform = transforms[i],
                    ecb = ecbs[i],
                    deltaTime = SystemAPI.Time.DeltaTime,
                    waypoints = waypointBufferLookup
                };

                handles[i] = moveJob.Schedule();
            }
        }

        JobHandle.CompleteAll(handles);

        for (int i = 0; i < entities.Length; i++)
        {
            ecbs[i].Playback(state.EntityManager);
            ecbs[i].Dispose();
        }

        agents.Dispose();
        transforms.Dispose();
        handles.Dispose();
        entities.Dispose();
        ecbs.Dispose();
    }


    [BurstCompile]
    private struct MoveJob : IJob
    {
        public NavAgentComponent agent;
        public LocalTransform transform;
        public float3 toPosition;
        public Entity entity;
        public float deltaTime;
        public EntityCommandBuffer ecb;
        [ReadOnly] public BufferLookup<WaypointBuffer> waypoints;

        public void Execute()
        {
            if (math.distance(transform.Position, toPosition) <= 3)
            {
                // Si la distance est inférieure ou égale au stopDistance, arrêter le mouvement
                return;
            }

            if (waypoints.TryGetBuffer(entity, out DynamicBuffer<WaypointBuffer> waypointBuffer) &&
                math.distance(transform.Position, waypointBuffer[agent.currentWaypoint].wayPoint) < 0.4f)
            {
                if(agent.currentWaypoint + 1 < waypointBuffer.Length)
                {
                    agent.currentWaypoint += 1;
                }

                ecb.SetComponent(entity, agent);
            }

            float3 direction = waypointBuffer[agent.currentWaypoint].wayPoint - transform.Position;
            float angle = math.PI * 0.5f - math.atan2(direction.z, direction.x);

            transform.Rotation = math.slerp(
                        transform.Rotation,
                        quaternion.Euler(new float3(0, angle, 0)),
                        deltaTime * 10);

            transform.Position += new float3(math.normalize(direction).x, 0, math.normalize(direction).z) * deltaTime * agent.moveSpeed;
            ecb.SetComponent(entity, transform);
        }
    }

    [BurstCompile]
    private struct CalculatePathJob : IJob
    {
        public Entity entity;
        public NavAgentComponent agent;
        public EntityCommandBuffer ecb;
        public float3 fromPosition;
        public float3 toPosition;
        public NavMeshQuery query;

        public void Execute()
        {
            agent.nextPathCalculateTime += 1;
            agent.pathCalculated = false;
            ecb.SetComponent(entity, agent);

            float3 extents = new float3(1, 1, 1);

            NavMeshLocation fromLocation = query.MapLocation(fromPosition, extents, 0);
            NavMeshLocation toLocation = query.MapLocation(toPosition, extents, 0);

            PathQueryStatus status;
            PathQueryStatus returningStatus;
            int maxPathSize = 100;

            if (query.IsValid(fromLocation) && query.IsValid(toLocation))
            {
                status = query.BeginFindPath(fromLocation, toLocation);

                if (status == PathQueryStatus.InProgress || status == PathQueryStatus.Success)
                {
                    status = query.UpdateFindPath(100, out int iterationPerformed);

                    if (status == PathQueryStatus.Success)
                    {
                        status = query.EndFindPath(out int pathSize);

                        NativeArray<NavMeshLocation> result = new NativeArray<NavMeshLocation>(pathSize + 1, Allocator.Temp);
                        NativeArray<StraightPathFlags> straightPathFlags = new NativeArray<StraightPathFlags>(maxPathSize, Allocator.Temp);
                        NativeArray<float> vertexSide = new NativeArray<float>(maxPathSize, Allocator.Temp);
                        NativeArray<PolygonId> polygonIds = new NativeArray<PolygonId>(pathSize + 1, Allocator.Temp);
                        int straightPathCount = 0;

                        query.GetPathResult(polygonIds);

                        returningStatus = PathUtils.FindStraightPath
                            (
                            query,
                            fromPosition,
                            toPosition,
                            polygonIds,
                            pathSize,
                            ref result,
                            ref straightPathFlags,
                            ref vertexSide,
                            ref straightPathCount,
                            maxPathSize
                            );

                        if (returningStatus == PathQueryStatus.Success)
                        {
                            ecb.SetBuffer<WaypointBuffer>(entity);

                            foreach (NavMeshLocation location in result)
                            {
                                if (location.position != Vector3.zero)
                                {
                                    ecb.AppendToBuffer(entity, new WaypointBuffer { wayPoint = location.position });
                                }
                            }

                            agent.currentWaypoint = 0;
                            agent.pathCalculated = true;
                            ecb.SetComponent(entity, agent);
                        }
                        straightPathFlags.Dispose();
                        polygonIds.Dispose();
                        vertexSide.Dispose();
                    }
                }
            }
        }
    }
}