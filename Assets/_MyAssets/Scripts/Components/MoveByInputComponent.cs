using System;
using Unity.Entities;

namespace DotsMan.Components
{
    [Serializable]
    public struct MoveByInputComponent : IComponentData
    {
        public float speed;
    }
}


