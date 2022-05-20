using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace DotsMan.Systems
{
    public class CollisionSystem : SystemBase
    {
        private BuildPhysicsWorld _buildPhysicsWorldSystem;
        private StepPhysicsWorld _stepPhysicsWorldSystem;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            _buildPhysicsWorldSystem =  World.GetExistingSystem<BuildPhysicsWorld>();
            _stepPhysicsWorldSystem = World.GetExistingSystem<StepPhysicsWorld>();
        }

        protected override void OnUpdate()
        {
            Entities
                .ForEach((DynamicBuffer<CollisionBuffer> collisions) =>
                {
                    collisions.Clear();
                })
                .Run();
            
            var jobHandle = new CollisionEventJob
                {
                    Collisions = GetBufferFromEntity<CollisionBuffer>()
                }
                .Schedule
                (
                    _stepPhysicsWorldSystem.Simulation,
                    ref _buildPhysicsWorldSystem.PhysicsWorld,
                    Dependency
                );
            jobHandle.Complete();
        }

        [BurstCompile]
        private struct CollisionEventJob : ICollisionEventsJob
        {
            public BufferFromEntity<CollisionBuffer> Collisions;

            public void Execute(CollisionEvent collisionEvent)
            {
                var entityA = collisionEvent.Entities.EntityA;
                var entityB = collisionEvent.Entities.EntityB;
                
                if (Collisions.Exists(entityA))
                    Collisions[entityA].Add(new CollisionBuffer {entity = entityB});
                else if (Collisions.Exists(entityB))
                    Collisions[entityB].Add(new CollisionBuffer {entity = entityA});
            }
        }
    }
}