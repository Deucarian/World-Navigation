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
