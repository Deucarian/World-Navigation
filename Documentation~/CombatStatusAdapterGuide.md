# Combat Status Adapter Guide

World Navigation has no Combat runtime dependency.

Game code can map Combat output to navigation behavior:

- slow status -> speed provider multiplier
- stun/root/freeze -> pause, stop, or zero-speed provider

The package tests include a game-owned mutable speed provider proof.
