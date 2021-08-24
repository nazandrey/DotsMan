using Unity.Entities;

namespace DotsMan
{
    [GenerateAuthoringComponent]
    public struct EnemyComponent : IComponentData
    {
        public int damage;
    }
}


