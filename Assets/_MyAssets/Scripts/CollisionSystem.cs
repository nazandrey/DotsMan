using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace DotsMan
{
    [UpdateAfter(typeof(StepPhysicsWorld))]
    public class CollisionSystem : SystemBase
    {
        private BuildPhysicsWorld _buildPhysicsWorldSystem;
        private StepPhysicsWorld _stepPhysicsWorldSystem;
        private EndSimulationEntityCommandBufferSystem _commandBufferSystem;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            _buildPhysicsWorldSystem =  World.GetExistingSystem<BuildPhysicsWorld>();
            _stepPhysicsWorldSystem = World.GetExistingSystem<StepPhysicsWorld>();
            _commandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            Dependency = new CollisionEventJob
                {
                    PlayersGroup = GetComponentDataFromEntity<PlayerComponent>(),
                    EnemiesGroup = GetComponentDataFromEntity<EnemyComponent>(),
                }
                .Schedule
                (
                    _stepPhysicsWorldSystem.Simulation,
                    ref _buildPhysicsWorldSystem.PhysicsWorld,
                    Dependency
                );
            _commandBufferSystem.AddJobHandleForProducer(Dependency);    
        }

        [BurstCompile]
        private struct CollisionEventJob : ICollisionEventsJob
        {
            [ReadOnly] public ComponentDataFromEntity<EnemyComponent> EnemiesGroup;
            public ComponentDataFromEntity<PlayerComponent> PlayersGroup;

            public void Execute(CollisionEvent collisionEvent)
            {
                var entityA = collisionEvent.Entities.EntityA;
                var entityB = collisionEvent.Entities.EntityB;
                
                var isPlayerA = PlayersGroup.HasComponent(entityA);
                var isPlayerB = PlayersGroup.HasComponent(entityB);
                
                var isEnemyA = EnemiesGroup.HasComponent(entityA);
                var isEnemyB = EnemiesGroup.HasComponent(entityB);

                if (isEnemyA && isPlayerB)
                {
                    var playerComponent = PlayersGroup[entityB];
                    var enemyComponent = EnemiesGroup[entityA];
                    
                    playerComponent.health -= enemyComponent.damage;
                    PlayersGroup[entityB] = playerComponent;
                } 
                else if (isEnemyB && isPlayerA)
                {
                    var playerComponent = PlayersGroup[entityA];
                    var enemyComponent = EnemiesGroup[entityB];
                    
                    playerComponent.health -= enemyComponent.damage;
                    PlayersGroup[entityA] = playerComponent;
                }
            }
        }
    }
}