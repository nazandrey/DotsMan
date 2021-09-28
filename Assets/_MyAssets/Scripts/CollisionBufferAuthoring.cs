using Unity.Entities;
using UnityEngine;

namespace DotsMan
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class CollisionBufferAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddBuffer<CollisionBuffer>(entity);
        }
    }
}