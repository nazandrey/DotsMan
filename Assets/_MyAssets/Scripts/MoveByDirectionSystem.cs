using Unity.Entities;
using Unity.Transforms;

namespace DotsMan
{
    public class MoveByDirectionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((ref MoveByDirectionComponent moveByDirectionComponent, ref Translation translation) =>
            {
                translation.Value += moveByDirectionComponent.direction * moveByDirectionComponent.speed * deltaTime;
            }).ScheduleParallel();
        }
    }
}