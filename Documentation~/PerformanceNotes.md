# Performance Notes

Hot ticking sorts agents at registration time and ticks in deterministic id order. Snapshots allocate by design.

The benchmark uses memory pose accessors to measure service logic independently from Transform overhead. It is an EditMode Mono measurement and does not claim mobile, IL2CPP, Burst, ECS, or NavMesh performance.
