# Simple usage

Copy the reference example into your project, add SimpleUsageExample and assign its scoped host references. Its serialized definition fields use the same typed keys as code.

Add MovementAgentHost to the moving object. Configure it once with the world's existing WorldNavigationService and the object's IMovementSpeedProvider. The service owner keeps ticking it. The host registers while enabled and unregisters on disable/destroy; it never disposes the shared service. Destination coordinates are data, so they do not need a key registry.

Definitions are authored once in SampleDefinitions.cs where applicable; the caller never invents an ID. Replace the sample set with your project's central definitions. A selected key proves its identity and payload type; startup still needs to bind that definition in the correct scope. Missing configuration reports how to fix it. Dynamic targets and choices are issued by their owner instead of selected from a definition dropdown.
```csharp
using UnityEngine;

namespace Deucarian.WorldNavigation.Samples.SimpleUsage
{
    public sealed class SimpleUsageExample : MonoBehaviour
    {
        [SerializeField] private MovementAgentHost movement;
        public MovementResult GoTo(Vector3 destination) => movement.MoveTo(destination);
        public MovementResult Stop() => movement.Stop();
    }
}
```
