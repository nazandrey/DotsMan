using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace DotsMan.Systems
{
    public class TriggerSystem : SystemBase
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
                .ForEach((DynamicBuffer<TriggerBuffer> triggers) =>
                {
                    triggers.Clear();
                })
                .Run();
            
            var jobHandle = new CollisionEventJob
                {
                    Triggers = GetBufferFromEntity<TriggerBuffer>()
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
        private struct CollisionEventJob : ITriggerEventsJob
        {
            public BufferFromEntity<TriggerBuffer> Triggers;

            public void Execute(TriggerEvent triggerEvent)
            {
                var entityA = triggerEvent.Entities.EntityA;
                var entityB = triggerEvent.Entities.EntityB;
                
                if (Triggers.Exists(entityA))
                    Triggers[entityA].Add(new TriggerBuffer {entity = entityB});
                else if (Triggers.Exists(entityB))
                    Triggers[entityB].Add(new TriggerBuffer {entity = entityA});
            }
        }
    }
}