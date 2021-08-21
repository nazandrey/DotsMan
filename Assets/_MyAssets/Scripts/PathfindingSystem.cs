using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace DotsMan
{
    //should work after wall conversion for proper initial pathfinding
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class PathfindingSystem : SystemBase
    {
        private Random _random = new Random(1234);
        
        private static readonly float3[] Directions = new[]
        {
            (float3) Vector3.forward,
            (float3) Vector3.back,
            (float3) Vector3.left,
            (float3) Vector3.right,
        };

        private EntityCommandBufferSystem ecbSystem;
    
        protected override void OnCreate()
        {
            base.OnCreate();
            ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            _random.NextInt();
            var random = _random;
            
            var ecb = ecbSystem.CreateCommandBuffer().ToConcurrent();

            var physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
            var raycaster = new Raycaster(collisionWorld);
            
            var directionsNative = new NativeArray<float3>(Directions, Allocator.TempJob);

            Entities
                .WithAll<PhysicsCollider>()
                .WithNone<PhysicalMoveComponent>()
                .ForEach((Entity entity, int entityInQueryIndex, ref PathfindingComponent pathfindingComponent, in Translation translation) =>
                {
                    var possibleDirections = new NativeList<float3>(Allocator.Temp);
                    foreach (var direction in directionsNative)
                    {
                        //not going back
                        if (direction.Equals(-pathfindingComponent.lastDirection))
                            continue;
                        var hitWall = raycaster.WallCheck(translation.Value, translation.Value + direction);
                        if (!hitWall)
                            possibleDirections.Add(direction);
                    }

                    if (possibleDirections.Length > 0)
                    {
                        var resultDirectionIndex = random.NextInt(possibleDirections.Length);
                        var resultDirection = possibleDirections[resultDirectionIndex];

                        var translationXZ = new float3(translation.Value.x, 0, translation.Value.z);
                        ecb.AddComponent(entityInQueryIndex, entity, new PhysicalMoveComponent
                        {
                            speed = 3,
                            fromPoint = translationXZ,
                            toPoint = translationXZ + resultDirection
                        });
                        pathfindingComponent.lastDirection = resultDirection;
                    }

                    possibleDirections.Dispose();
                })
                .ScheduleParallel();

            CompleteDependency();
            directionsNative.Dispose();
        }
    }
}