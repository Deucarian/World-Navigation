# World Spawning Integration Guide

World Spawning creates or reuses a GameObject. Game code then registers the spawned object's transform with `WorldNavigationService`.

`RegisterSpawnedObject` accepts a `SpawnResult` and speed provider. On despawn, game code calls `CleanupDespawned` with the movement agent id.

World Spawning does not know movement rules.
