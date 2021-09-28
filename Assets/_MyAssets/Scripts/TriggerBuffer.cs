using Unity.Entities;

namespace DotsMan
{
    public struct TriggerBuffer : IBufferElementData
    {
        public Entity entity;
    }
}