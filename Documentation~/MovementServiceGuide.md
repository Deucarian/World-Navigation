# Movement Service Guide

`WorldNavigationService` is explicitly owned and ticked by the consumer. There is no hidden global service and no per-agent `Update`.

Register an agent with a pose accessor and speed provider, assign a destination or path command, and call `Tick(deltaSeconds)` from the owning game loop.
