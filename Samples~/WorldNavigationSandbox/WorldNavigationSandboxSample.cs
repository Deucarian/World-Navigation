using Deucarian.WorldSpawning;
using UnityEngine;

namespace Deucarian.WorldNavigation.Samples
{
    public static class WorldNavigationSandboxSample
    {
        public static MovementAgentHandle RegisterSpawnedObject(WorldNavigationService navigation, SpawnResult spawnResult, float speed)
        {
            navigation.RegisterSpawnedObject(spawnResult, new ConstantMovementSpeedProvider(speed), out MovementAgentHandle handle);
            navigation.SetDestination(handle.Id, Vector3.zero);
            return handle;
        }
    }
}
