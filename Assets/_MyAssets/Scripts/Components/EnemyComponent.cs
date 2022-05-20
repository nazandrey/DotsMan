using Unity.Entities;

namespace DotsMan.Components
{
    [GenerateAuthoringComponent]
    public struct EnemyComponent : IComponentData
    {
        public int damage;
    }
}


