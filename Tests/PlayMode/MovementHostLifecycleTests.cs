using System;
using NUnit.Framework;
using UnityEngine;
namespace Deucarian.WorldNavigation.Tests
{
    public sealed class MovementHostLifecycleTests
    {
        [Test]
        public void HostRegistrationFollowsEnableStateAndDoesNotOwnSharedService()
        {
            var go = new GameObject("mover");
            var service = new WorldNavigationService();
            try
            {
                var host = go.AddComponent<MovementAgentHost>();
                Assert.That(Assert.Throws<InvalidOperationException>(() => host.MoveTo(Vector3.one)).Message,
                    Does.Contain("Configure"));
                host.Configure(service, new ConstantMovementSpeedProvider(1));
                Assert.That(service.AgentCount, Is.EqualTo(1));
                Assert.That(host.MoveTo(Vector3.right).Succeeded, Is.True);
                service.Tick(1);
                Assert.That(go.transform.position, Is.EqualTo(Vector3.right));
                host.enabled = false;
                Assert.That(service.AgentCount, Is.Zero);
                host.enabled = true;
                Assert.That(service.AgentCount, Is.EqualTo(1));
                UnityEngine.Object.DestroyImmediate(go);
                Assert.That(service.AgentCount, Is.Zero);
                Assert.DoesNotThrow(() => service.Tick(0));
            }
            finally { if (go != null) UnityEngine.Object.DestroyImmediate(go); }
        }

    }
}
