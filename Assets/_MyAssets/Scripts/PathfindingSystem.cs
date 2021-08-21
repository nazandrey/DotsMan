using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;
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
        private NativeArray<Random> _randoms;

        [NativeSetThreadIndex]
        private int _threadId;
        
        private static readonly float3[] Directions = new[]
        {
            (float3) Vector3.forward,
            (float3) Vector3.back,
            (float3) Vector3.left,
            (float3) Vector3.right,
        };

        protected override void OnUpdate()
        {
            var directionsNative = new NativeArray<float3>(Directions, Allocator.TempJob);

            Entities
                .WithStructuralChanges()
                .WithoutBurst()
                .WithAll<PhysicsCollider>()
                .WithNone<PhysicalMoveComponent>()
                .ForEach((Entity entity, ref PathfindingComponent pathfindingComponent, in Translation translation) =>
                {
                    var possibleDirections = new NativeList<float3>(Allocator.TempJob);
                    foreach (var direction in directionsNative)
                    {
                        //not going back
                        if (direction.Equals(-pathfindingComponent.lastDirection))
                            continue;
                        var foundEntity = Raycast(translation.Value, translation.Value + direction);
                        var notWall = foundEntity == Entity.Null;
                        if (notWall)
                        {
                            possibleDirections.Add(direction);
                        }
                    }

                    if (possibleDirections.Length > 0)
                    {
                        var random = _randoms[_threadId];
                        var resultDirectionIndex = random.NextInt(0, possibleDirections.Length);
                        _randoms[_threadId] = random; //This is necessary to update the state of the element inside the array
                        var resultDirection = possibleDirections[resultDirectionIndex];

                        var translationXZ = new float3(translation.Value.x, 0, translation.Value.z);
                        EntityManager.AddComponentData(entity, new PhysicalMoveComponent
                        {
                            speed = 3,
                            fromPoint = translationXZ,
                            toPoint = translationXZ + resultDirection
                        });
                        pathfindingComponent.lastDirection = resultDirection;
                    }

                    possibleDirections.Dispose();
                })
                .Run();

            directionsNative.Dispose();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _randoms.Dispose();
        }

        // https://docs.unity3d.com/Packages/com.unity.physics@0.6/manual/collision_queries.html
        private Entity Raycast(float3 rayFrom, float3 rayTo)
        {
            var physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
            var input = new RaycastInput()
            {
                Start = rayFrom,
                End = rayTo,
                Filter = new CollisionFilter()
                {
                    BelongsTo = (uint) PhysicsLayer.Enemy,
                    CollidesWith = (uint) PhysicsLayer.Wall, 
                    GroupIndex = 0
                }
            };

            if (collisionWorld.CastRay(input, out var hit))
            {
                // see hit.Position
                // see hit.SurfaceNormal
                var entity = physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                return entity;
            }
            return Entity.Null;
        }
    }
}