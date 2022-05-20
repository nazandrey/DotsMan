using System;
using Unity.Entities;
using Unity.Mathematics;

namespace DotsMan.Components
{
    [Serializable]
    public struct MoveByDirectionComponent : IComponentData
    {
        public float speed;
        public float3 direction;
    }
}


