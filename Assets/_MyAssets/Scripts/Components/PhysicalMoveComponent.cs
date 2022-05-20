using Unity.Entities;
using Unity.Mathematics;

namespace DotsMan.Components
{
    public struct PhysicalMoveComponent : IComponentData
    {
        public float speed;
        public float3 fromPoint;
        public float3 toPoint;
    }
}


