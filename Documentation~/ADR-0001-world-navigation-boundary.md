# ADR-0001: World Navigation Boundary

## Status

Accepted for 0.1.0.

## Decision

`com.deucarian.world-navigation` is a Unity-side navigation package whose first implementation owns centralized ticking for registered movement agents, destination movement, path-following over supplied waypoints, path-progress reporting, speed providers, transform pose adapters, pause/resume/stop, despawn-safe cleanup, deterministic update order, and diagnostics snapshots.

It depends on `com.deucarian.gameplay-foundation` and `com.deucarian.world-spawning`. It does not depend on Encounters, Combat, Progression, Persistence, UI packages, Core State, or Unity.Entities.

## Separation From World Spawning

World Spawning creates and despawns pooled objects. World Navigation moves objects only after game code registers them as agents. Spawn channels, prefab providers, and pooling remain outside this package.

## Separation From Encounters

Encounters emits spawn requests and owns wave/schedule state. Navigation does not consume encounter definitions or spawn requests.

## Separation From Combat

Combat may produce slow, stun, or root outcomes, but game code maps those outcomes to speed providers or pause/stop commands. World Navigation has no runtime Combat dependency.

## Centralized Ticking

The package uses `WorldNavigationService.Tick(deltaSeconds)` instead of per-agent `MonoBehaviour.Update`. Consumers own when ticking happens and can pause the whole service or individual agents deterministically.

## Agent Identity And Lifecycle

Agents receive stable `MovementAgentId` values. Registration rejects duplicates, invalid accessors, and invalid speed providers. Unregister and despawn cleanup remove agents from future ticks.

## Destination Model

A destination command moves an agent toward a target pose at the speed reported by its provider. Overshoot is clamped and arrival is reported explicitly.

## Path Model

A path command follows caller-supplied waypoints in order. The package does not build paths, query obstacles, use NavMesh, run A*, or interpret lanes.

## Path Progress

Progress reports waypoint index, completed waypoint count, total waypoint count, normalized progress, and completion state. Ordering is deterministic by agent id.

## Adapter Strategy

`IMovementPoseAccessor` abstracts pose reads/writes. `TransformMovementPoseAccessor` adapts Unity transforms. `IMovementSpeedProvider` abstracts speed and allows game-owned Combat/status adapters.

## Pause, Stop, And Despawn

Paused agents do not move. Stop clears the command. Despawn cleanup unregisters by agent id or by GameObject integration adapter.

## Determinism And Allocation

Tick order is by sorted `MovementAgentId`, not dictionary enumeration. Hot ticking over already registered agents avoids steady-state allocations where practical. Diagnostics snapshots allocate by design.

## Future ECS Boundary

Future packages may be:

- `com.deucarian.world-navigation.navmesh-integration`
- `com.deucarian.world-navigation.entities-integration`

Those packages should adapt pathfinding or ECS commands to this package's concepts without moving NavMesh, A*, grid pathfinding, obstacle avoidance, steering, or DOTS into `0.1.0`.
