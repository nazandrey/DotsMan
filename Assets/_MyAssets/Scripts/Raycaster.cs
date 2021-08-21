using Unity.Collections;
using Unity.Mathematics;
using Unity.Physics;

namespace DotsMan
{
    public struct Raycaster
    {
        [ReadOnly] private CollisionWorld _collisionWorld;

        public Raycaster(CollisionWorld collisionWorld)
        {
            _collisionWorld = collisionWorld;
        }

        // https://docs.unity3d.com/Packages/com.unity.physics@0.6/manual/collision_queries.html
        public bool WallCheck(float3 rayFrom, float3 rayTo)
        {
            var input = new RaycastInput()
            {
                Start = rayFrom,
                End = rayTo,
                Filter = new CollisionFilter()
                {
                    BelongsTo = (uint) PhysicsLayer.Enemy,
                    CollidesWith = (uint) PhysicsLayer.Wall,
                    GroupIndex = 0
                }
            };

            return _collisionWorld.CastRay(input);
        }
    }
}