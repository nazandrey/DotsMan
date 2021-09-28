using Unity.Entities;

namespace DotsMan
{
    public struct CollisionBuffer : IBufferElementData
    {
        public Entity entity;
    }
}