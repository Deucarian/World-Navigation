using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Deucarian.WorldNavigation.Tests
{
    public sealed class WorldNavigationPlayModeTests
    {
        [UnityTest]
        public IEnumerator TransformAgent_MovesAcrossFrame()
        {
            GameObject actor = new GameObject("navigation-playmode-agent");
            var service = new WorldNavigationService();
            MovementAgentHandle handle = service.Register(new TransformMovementPoseAccessor(actor.transform), new ConstantMovementSpeedProvider(1));
            service.SetDestination(handle.Id, Vector3.right);
            service.Tick(1);
            yield return null;
            Assert.AreEqual(Vector3.right, actor.transform.position);
            Object.Destroy(actor);
        }
    }
}
