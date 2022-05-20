using DotsMan.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace DotsMan.Systems
{
    public class PhysicalMoveSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent();
            
            Entities.ForEach((Entity entity, int entityInQueryIndex, ref PhysicalMoveComponent moveToDurationComponent, 
                ref Translation translation, ref PhysicsVelocity physicsVelocity) =>
            {
                if (math.distance(translation.Value, moveToDurationComponent.toPoint) < 0.5f)
                {
                    ecb.RemoveComponent<PhysicalMoveComponent>(entityInQueryIndex, entity);
                }
                else
                {
                    var direction = math.normalize(moveToDurationComponent.toPoint - moveToDurationComponent.fromPoint);
                    physicsVelocity.Linear = direction * moveToDurationComponent.speed;
                }
            }).ScheduleParallel();
        }
    }
}