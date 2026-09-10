# Deucarian World Navigation Agent Notes

Package ID: `com.deucarian.world-navigation`
Repository: `Deucarian/World-Navigation`

Follow the canonical Deucarian governance docs in [Package Registry](https://github.com/Deucarian/Package-Registry/blob/main/ARCHITECTURE.md), especially capability ownership and dependency rules.

## Ownership

This package owns:

- Unity-facing registered movement agents, destination/path movement commands, supplied waypoint path traversal, speed and pose adapters, pause/resume/stop behavior, despawn-safe cleanup, deterministic tick/update order, and navigation diagnostics snapshots.

Registered capabilities:
- None.

This package must not own:

- A*, NavMesh, grid pathfinding, obstacle avoidance, steering/flocking, encounter scheduling, world spawning implementation, combat, attacks, weapons, tower placement, progression, persistence, UI, ECS/DOTS, or product-specific movement rules.

## Dependencies

Allowed dependency shape:

- May depend on Gameplay Foundation for gameplay IDs and shared primitives.
- May depend on World Spawning only for spawn instance identifiers and despawn-aware adapters.

Required dependencies and why:

- `com.deucarian.gameplay-foundation`: shared gameplay IDs and deterministic primitives used by navigation commands and snapshots.
- `com.deucarian.world-spawning`: spawn instance identity and spawned-object lifecycle context for navigation targets.

Optional/version-defined dependencies:

- None.

Architecture exceptions:

- None.

## Policies

- Keep this package focused on supplied-path movement and agent coordination.
- Do not add hard dependencies on Encounters, Combat, Attacks, Projectiles, Weapon Systems, Defense Games, Auto Defense, Progression, Persistence, UI, or template packages.
- Higher-level path construction and genre-specific movement behavior belong in the caller, framework, or template package.
- Logging: Do not introduce direct Unity Debug calls.
- Unity object lifetime: Use Common only if production code directly owns transient Unity object cleanup.
- Testing: Test fixture teardown may use Unity `DestroyImmediate` directly.

## Validation

Run the shared validator before committing:

```powershell
python C:/Repositories/Package-Registry/Tools/deucarian_package_validator.py --registry-root C:/Repositories/Package-Registry --repository-root . --config deucarian-package.json
```

Also run existing repository tests when changing code or asmdefs. Documentation-only updates should still run `git diff --check`.

## Codex Guidance

- Inspect current files before changing anything.
- Work on `develop`; do not edit or merge `main` unless the task is promotion-only.
- Do not edit `Library/PackageCache`.
- Do not guess package versions or dependency versions.
- Do not add package dependencies casually; update asmdefs, `package.json`, `deucarian-package.json`, Package Registry, Package Installer fallback, and Bootstrap fallback together when a dependency is truly required.
- Do not create local copies of shared helpers.
- Keep commits focused and report exactly what changed and what was validated.
