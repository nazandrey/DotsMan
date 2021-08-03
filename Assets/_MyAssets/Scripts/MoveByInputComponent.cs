using System;
using Unity.Entities;

namespace DotsMan
{
    [Serializable]
    public struct MoveByInputComponent : IComponentData
    {
        public float speed;
    }
}


