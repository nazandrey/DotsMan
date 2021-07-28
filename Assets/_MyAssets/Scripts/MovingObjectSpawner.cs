using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DotsMan
{
    public class MovingObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject movingObjectPrefab;
        
        private void Start()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            var entityManager = world.EntityManager;
            var conversionSystem = world.GetExistingSystem<ConvertToEntitySystem>();
            var blobStore = conversionSystem.BlobAssetStore;
            
            var settings = GameObjectConversionSettings.FromWorld(world, blobStore);
            var movingObjectEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(movingObjectPrefab, settings);
            var movingObject = entityManager.Instantiate(movingObjectEntityPrefab);
            entityManager.AddComponentData(movingObject, new MoveByDirectionComponent
            {
                direction = math.forward(quaternion.identity),
                speed = 1
            });
        }
    }
}