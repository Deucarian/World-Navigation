# Donor Integration Findings

Primary donor:

`C:/Repositories/JorisHoef/Codex-Attempted-Vampire-Project/Codex-Attempted-Vampire-Project`

The donor centralizes enemy ticking through `EnemyRegistry.SimulateAll`, but `EnemyActor.Simulate` still owns movement, combat contact damage, status ticking, despawn distance checks, health UI, and status presentation together.

Clean mappings:

- `EnemyActor` transform -> `TransformMovementPoseAccessor`
- donor move speed and status multiplier -> `IMovementSpeedProvider`
- player position chase target -> destination command
- donor despawn/death release -> `CleanupDespawned`

Discarded assumptions:

- player chase as a generic rule
- status/combat/UI logic inside navigation
- despawn radius policy inside navigation
- per-agent `Update`
