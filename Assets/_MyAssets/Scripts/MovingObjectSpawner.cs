using Unity.Entities;
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
            var movingObjectByInput = entityManager.Instantiate(movingObjectEntityPrefab);
            entityManager.AddComponentData(movingObjectByInput, new MoveByInputComponent
            {
                speed = 10
            });
            entityManager.AddComponentData(movingObjectByInput, new PlayerComponent
            {
                health = 10
            });
        }
    }
}