using DotsMan.Components;
using Unity.Entities;

namespace DotsMan.Systems
{
    public class PlayerDestroySystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent();
            
            Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex, ref PlayerComponent playerComponent) =>
            {
                if (playerComponent.health <= 0.01f)
                    ecb.DestroyEntity(entityInQueryIndex, entity);
            }).Schedule(Dependency);
            
            _ecbSystem.AddJobHandleForProducer(Dependency);   
        }
    }
}