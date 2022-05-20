using Unity.Entities;
using Unity.Mathematics;

namespace DotsMan.Components
{
    [GenerateAuthoringComponent]
    public struct PathfindingComponent : IComponentData
    {
        public float3 lastDirection;
    }
}


