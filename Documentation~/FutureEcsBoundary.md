# Future ECS Boundary

Future integration packages may be:

- `com.deucarian.world-navigation.navmesh-integration`
- `com.deucarian.world-navigation.entities-integration`

They should remain separate from `com.deucarian.world-navigation` so the base package stays small and does not absorb NavMesh, A*, grid pathfinding, obstacle avoidance, steering, flocking, or DOTS concerns.
