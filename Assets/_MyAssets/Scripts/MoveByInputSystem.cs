using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DotsMan
{
    public class MoveByInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var horizontalInput = Input.GetAxis("Horizontal");
            var verticalInput = Input.GetAxis("Vertical");
            
            Entities.ForEach((ref MoveByInputComponent moveByInputComponent, ref Translation translation) =>
            {
                translation.Value += new float3(horizontalInput, 0, verticalInput) * moveByInputComponent.speed * deltaTime;
            }).ScheduleParallel();
        }
    }
}