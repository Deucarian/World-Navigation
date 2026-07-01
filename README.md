# Deucarian World Navigation

`com.deucarian.world-navigation` is a small Unity-side navigation package for centrally ticking registered movement agents.

It supports destination movement, supplied waypoint paths, path-progress reporting, transform pose adapters, speed-provider adapters, stop/pause/resume, despawn-safe cleanup, deterministic update order, and diagnostics snapshots.

It does not implement A*, NavMesh, grid pathfinding, obstacle avoidance, steering/flocking, tower-defense path construction, placement logic, ECS/DOTS, Combat, Encounters, Progression, Persistence, or UI.

## Install

Stable:

```json
"com.deucarian.world-navigation": "https://github.com/Deucarian/World-Navigation.git#main"
```

Development:

```json
"com.deucarian.world-navigation": "https://github.com/Deucarian/World-Navigation.git#develop"
```

Use `#main` for stable package consumption and `#develop` when testing active package work.

## When To Use This

Use this package when you need Unity-side centralized navigation for registered movement agents, destination movement, supplied waypoint paths, speed adapters, pose adapters, and diagnostics snapshots.

Do not use this package to take ownership of capabilities outside its `AGENTS.md` boundary. Reusable behavior should stay with the package that owns that capability in the Package Registry governance docs.

## Quick Start

1. Install the package through Deucarian Package Installer or Unity Package Manager using the URL above.
2. Let Unity finish resolving packages and compiling assemblies.
3. Import the `World Navigation Sandbox` sample if you want a working reference scene or setup.
4. Start from the package README sections above and the public runtime/editor APIs in this repository.

## Integrations

Direct Deucarian package dependencies:

- `com.deucarian.gameplay-foundation`
- `com.deucarian.world-spawning`

Install optional companion packages only when their owned capability is needed by production code, samples, or tests.

## Validation

Run the shared package validator from this repository root:

```powershell
python C:/Repositories/Package-Registry/Tools/deucarian_package_validator.py --registry-root C:/Repositories/Package-Registry --repository-root . --config deucarian-package.json
```

Documentation-only updates should still pass:

```powershell
git diff --check
```

## Troubleshooting

- Package does not resolve: confirm the stable or development Git URL matches the Package Registry entry and that required Deucarian dependencies are installed.
- Unity compile errors after install: let Package Manager finish resolving dependencies, then check asmdef references against `package.json` dependencies.
- Behavior appears to belong in another package: consult `AGENTS.md` and the Package Registry governance docs before moving or duplicating code.

## License

MIT. See `LICENSE.md`.
