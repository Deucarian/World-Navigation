# API

Namespace: `Deucarian.WorldNavigation`

- `WorldNavigationService`
- `MovementAgentId`, `MovementAgentHandle`
- `MovementAgentDefinition`
- `MovementPose`
- `MovementPath`, `MovementWaypoint`
- `MovementProgress`
- `MovementCommand`
- `MovementResult`, `MovementTickResult`
- `IMovementPoseAccessor`, `TransformMovementPoseAccessor`
- `IMovementSpeedProvider`, `ConstantMovementSpeedProvider`
- `IWorldMovementAgent`, `IWorldMovementResettable`
- `MovementSnapshot`, `MovementAgentSnapshot`

Runtime dependencies are `com.deucarian.gameplay-foundation` and `com.deucarian.world-spawning`.

The runtime has no dependency on Encounters, Combat, Progression, Persistence, UI packages, Core State, or Unity.Entities.
