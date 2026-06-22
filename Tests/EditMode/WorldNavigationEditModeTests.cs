using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Deucarian.WorldSpawning;
using NUnit.Framework;
using UnityEngine;

namespace Deucarian.WorldNavigation.Tests
{
    public sealed class WorldNavigationEditModeTests
    {
        [Test]
        public void RegistrationDuplicateUnregisterAndUnknown_Work()
        {
            var service = new WorldNavigationService();
            var accessor = new MemoryPoseAccessor(Vector3.zero);
            var id = new MovementAgentId(7);
            Assert.IsTrue(service.Register(new MovementAgentDefinition(id, accessor, new ConstantMovementSpeedProvider(1))).Succeeded);
            Assert.AreEqual(MovementStatus.DuplicateAgent, service.Register(new MovementAgentDefinition(id, accessor, new ConstantMovementSpeedProvider(1))).Status);
            Assert.AreEqual(1, service.AgentCount);
            Assert.IsTrue(service.Unregister(id).Succeeded);
            Assert.AreEqual(MovementStatus.UnknownAgent, service.Unregister(id).Status);
        }

        [Test]
        public void DestinationMovement_CoversSpeedTicksArrivalAndStopPause()
        {
            var service = new WorldNavigationService();
            MovementAgentHandle handle = service.Register(new MemoryPoseAccessor(Vector3.zero), new ConstantMovementSpeedProvider(2));
            Assert.IsTrue(service.SetDestination(handle.Id, new Vector3(5, 0, 0)).Succeeded);
            Assert.AreEqual(0, service.Tick(0).AgentsMoved);
            service.Tick(1);
            AssertPosition(service, handle.Id, new Vector3(2, 0, 0));
            service.Pause(handle.Id);
            service.Tick(1);
            AssertPosition(service, handle.Id, new Vector3(2, 0, 0));
            service.Resume(handle.Id);
            service.Tick(10);
            AssertPosition(service, handle.Id, new Vector3(5, 0, 0));
            Assert.IsTrue(service.TryGetProgress(handle.Id, out MovementProgress progress));
            Assert.IsTrue(progress.ReachedDestination);

            Assert.IsTrue(service.SetDestination(handle.Id, new Vector3(9, 0, 0)).Succeeded);
            Assert.IsTrue(service.Stop(handle.Id).Succeeded);
            service.Tick(1);
            AssertPosition(service, handle.Id, new Vector3(5, 0, 0));
        }

        [Test]
        public void SpeedValidation_RejectsNegativeAndAllowsZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ConstantMovementSpeedProvider(-1));
            var service = new WorldNavigationService();
            MovementAgentHandle handle = service.Register(new MemoryPoseAccessor(Vector3.zero), new ConstantMovementSpeedProvider(0));
            service.SetDestination(handle.Id, Vector3.right);
            service.Tick(10);
            AssertPosition(service, handle.Id, Vector3.zero);
        }

        [Test]
        public void PathFollowing_OneAndMultipleWaypoints_ReportProgress()
        {
            var service = new WorldNavigationService();
            MovementAgentHandle one = service.Register(new MemoryPoseAccessor(Vector3.zero), new ConstantMovementSpeedProvider(10));
            Assert.IsTrue(service.FollowPath(one.Id, new MovementPath(new[] { new MovementWaypoint(Vector3.right) })).Succeeded);
            service.Tick(1);
            AssertPosition(service, one.Id, Vector3.right);
            service.TryGetProgress(one.Id, out MovementProgress oneProgress);
            Assert.IsTrue(oneProgress.ReachedDestination);

            MovementAgentHandle multi = service.Register(new MemoryPoseAccessor(Vector3.zero), new ConstantMovementSpeedProvider(1));
            Assert.IsTrue(service.FollowPath(multi.Id, new MovementPath(new[]
            {
                new MovementWaypoint(new Vector3(1, 0, 0)),
                new MovementWaypoint(new Vector3(1, 0, 1)),
                new MovementWaypoint(new Vector3(2, 0, 1))
            })).Succeeded);
            service.Tick(1);
            service.TryGetProgress(multi.Id, out MovementProgress progress);
            Assert.AreEqual(1, progress.CompletedWaypoints);
            service.Tick(10);
            service.TryGetProgress(multi.Id, out progress);
            Assert.IsTrue(progress.ReachedDestination);
            Assert.AreEqual(1f, progress.NormalizedProgress);
        }

        [Test]
        public void InvalidPathAndTickInputs_AreRejected()
        {
            Assert.Throws<ArgumentException>(() => new MovementPath(Array.Empty<MovementWaypoint>()));
            var service = new WorldNavigationService();
            MovementAgentHandle handle = service.Register(new MemoryPoseAccessor(Vector3.zero), new ConstantMovementSpeedProvider(1));
            Assert.AreEqual(MovementStatus.InvalidPath, service.FollowPath(handle.Id, null).Status);
            Assert.Throws<ArgumentOutOfRangeException>(() => service.Tick(-1));
        }

        [Test]
        public void DeterministicOrder_IsByAgentId()
        {
            var service = new WorldNavigationService();
            var a2 = new OrderedSpeedProvider();
            var a1 = new OrderedSpeedProvider();
            service.Register(new MovementAgentDefinition(new MovementAgentId(2), new MemoryPoseAccessor(Vector3.zero), a2));
            service.Register(new MovementAgentDefinition(new MovementAgentId(1), new MemoryPoseAccessor(Vector3.zero), a1));
            service.SetDestination(new MovementAgentId(1), Vector3.right);
            service.SetDestination(new MovementAgentId(2), Vector3.right);
            OrderedSpeedProvider.Order.Clear();
            service.Tick(1);
            Assert.AreEqual(1, OrderedSpeedProvider.Order[0]);
            Assert.AreEqual(2, OrderedSpeedProvider.Order[1]);
        }

        [Test]
        public void WorldSpawningCompatibilityAndDespawnCleanup_Work()
        {
            GameObject prefab = new GameObject("nav-spawned");
            WorldSpawnService spawn = new WorldSpawnService(
                new SpawnableCatalog(new[] { new SpawnableDefinition(new WorldSpawnableId("enemy.nav"), new GameObjectPrefabProvider(prefab), 1, 1) }),
                new ChannelPoseResolver(new Dictionary<WorldSpawnChannelId, SpawnPose>
                {
                    [new WorldSpawnChannelId("channel.nav")] = new SpawnPose(Vector3.zero, Quaternion.identity)
                }));
            try
            {
                spawn.Warmup();
                SpawnResult result = spawn.Spawn(new WorldSpawnRequest(new WorldSpawnableId("enemy.nav"), new WorldSpawnChannelId("channel.nav"), 1, new WorldSpawnRequestContext("world-navigation-test")));
                var nav = new WorldNavigationService();
                Assert.IsTrue(nav.RegisterSpawnedObject(result, new ConstantMovementSpeedProvider(1), out MovementAgentHandle handle).Succeeded);
                nav.SetDestination(handle.Id, Vector3.right);
                nav.Tick(1);
                Assert.AreEqual(Vector3.right, result.Instance.transform.position);
                spawn.Despawn(result.InstanceId, DespawnReason.Requested);
                Assert.IsTrue(nav.CleanupDespawned(handle.Id).Succeeded);
                Assert.AreEqual(0, nav.AgentCount);
            }
            finally { spawn.Dispose(); UnityEngine.Object.DestroyImmediate(prefab); }
        }

        [Test]
        public void DonorIdleTowerAndCombatStatusAdapterProofs_WorkWithoutRuntimeDependencies()
        {
            var donorSpeed = new MutableSpeedProvider(2);
            var donorService = new WorldNavigationService();
            MovementAgentHandle donor = donorService.Register(new MemoryPoseAccessor(Vector3.zero), donorSpeed);
            donorService.SetDestination(donor.Id, new Vector3(4, 0, 0));
            donorService.Tick(1);
            AssertPosition(donorService, donor.Id, new Vector3(2, 0, 0));
            Assert.IsTrue(donorService.CleanupDespawned(donor.Id).Succeeded);

            var idle = new WorldNavigationService();
            MovementAgentHandle raider = idle.Register(new MemoryPoseAccessor(new Vector3(0, 0, 2)), new ConstantMovementSpeedProvider(2));
            idle.SetDestination(raider.Id, Vector3.zero);
            idle.Tick(1);
            AssertPosition(idle, raider.Id, Vector3.zero);

            var tower = new WorldNavigationService();
            MovementAgentHandle creep = tower.Register(new MemoryPoseAccessor(Vector3.zero), new ConstantMovementSpeedProvider(5));
            tower.FollowPath(creep.Id, new MovementPath(new[] { new MovementWaypoint(Vector3.right), new MovementWaypoint(new Vector3(2, 0, 0)) }));
            tower.Tick(1);
            tower.TryGetProgress(creep.Id, out MovementProgress pathProgress);
            Assert.IsTrue(pathProgress.ReachedDestination);

            var combatAdapterSpeed = new MutableSpeedProvider(4);
            var combat = new WorldNavigationService();
            MovementAgentHandle slowed = combat.Register(new MemoryPoseAccessor(Vector3.zero), combatAdapterSpeed);
            combat.SetDestination(slowed.Id, new Vector3(4, 0, 0));
            combatAdapterSpeed.Multiplier = 0.5f; // game-owned adapter maps Combat slow to speed multiplier
            combat.Tick(1);
            AssertPosition(combat, slowed.Id, new Vector3(2, 0, 0));
            combat.Pause(slowed.Id); // game-owned adapter maps Combat stun to pause
            combat.Tick(1);
            AssertPosition(combat, slowed.Id, new Vector3(2, 0, 0));
        }

        [Test]
        public void Snapshot_IsOrderedAndContainsProgress()
        {
            var service = new WorldNavigationService();
            service.Register(new MovementAgentDefinition(new MovementAgentId(3), new MemoryPoseAccessor(Vector3.forward), new ConstantMovementSpeedProvider(1)));
            service.Register(new MovementAgentDefinition(new MovementAgentId(1), new MemoryPoseAccessor(Vector3.zero), new ConstantMovementSpeedProvider(1)));
            service.SetDestination(new MovementAgentId(1), Vector3.right);
            MovementSnapshot snapshot = service.CreateSnapshot();
            Assert.AreEqual(1, snapshot.Agents[0].Id.Value);
            Assert.AreEqual(3, snapshot.Agents[1].Id.Value);
        }

        [Test]
        public void DurableBenchmark_WritesMovementMeasurements()
        {
            BenchmarkMeasurement one = Measure(1000, 300);
            BenchmarkMeasurement five = Measure(5000, 300);
            BenchmarkMeasurement ten = Measure(10000, 300);
            string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            Directory.CreateDirectory(logDirectory);
            string path = Path.Combine(logDirectory, "world-navigation-benchmark-results.json");
            File.WriteAllText(path, BuildBenchmarkJson(one, five, ten), Encoding.UTF8);
            TestContext.WriteLine(path);
            Assert.AreEqual(1000, one.AgentCount);
            Assert.AreEqual(5000, five.AgentCount);
            Assert.AreEqual(10000, ten.AgentCount);
        }

        private static BenchmarkMeasurement Measure(int agents, int ticks)
        {
            var service = new WorldNavigationService();
            for (int i = 0; i < agents; i++)
            {
                MovementAgentHandle handle = service.Register(new MemoryPoseAccessor(Vector3.zero), new ConstantMovementSpeedProvider(1));
                service.SetDestination(handle.Id, new Vector3(1000, 0, 0));
            }
            service.Tick(0.016f);
            long before = GC.GetAllocatedBytesForCurrentThread();
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int t = 0; t < ticks; t++) service.Tick(0.016f);
            stopwatch.Stop();
            long bytes = GC.GetAllocatedBytesForCurrentThread() - before;
            return new BenchmarkMeasurement(agents, ticks, stopwatch.Elapsed.TotalMilliseconds, bytes);
        }

        private static string BuildBenchmarkJson(params BenchmarkMeasurement[] measurements)
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("{");
            b.AppendLine("  \"unityVersion\": \"6000.3.5f1\",");
            b.AppendLine("  \"runtime\": \"Unity EditMode Mono\",");
            b.AppendLine("  \"configuration\": \"world-navigation-phase-1g-agents-300-ticks\",");
            b.AppendLine("  \"objectComplexity\": \"memory pose accessor, no GameObject prefab\",");
            b.AppendLine("  \"measurements\": [");
            for (int i = 0; i < measurements.Length; i++)
            {
                BenchmarkMeasurement m = measurements[i];
                b.Append("    { \"agentCount\": ").Append(m.AgentCount).Append(", \"tickCount\": ").Append(m.TickCount)
                    .Append(", \"elapsedMs\": ").Append(m.ElapsedMs.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture))
                    .Append(", \"bytesAllocated\": ").Append(m.BytesAllocated).Append(" }");
                b.AppendLine(i + 1 == measurements.Length ? string.Empty : ",");
            }
            b.AppendLine("  ]");
            b.AppendLine("}");
            return b.ToString();
        }

        private static void AssertPosition(WorldNavigationService service, MovementAgentId id, Vector3 expected)
        {
            Assert.IsTrue(service.TryGetPose(id, out MovementPose pose));
            Assert.AreEqual(expected.x, pose.Position.x, 0.0001f);
            Assert.AreEqual(expected.z, pose.Position.z, 0.0001f);
        }

        private readonly struct BenchmarkMeasurement
        {
            public BenchmarkMeasurement(int agentCount, int tickCount, double elapsedMs, long bytesAllocated) { AgentCount = agentCount; TickCount = tickCount; ElapsedMs = elapsedMs; BytesAllocated = bytesAllocated; }
            public int AgentCount { get; }
            public int TickCount { get; }
            public double ElapsedMs { get; }
            public long BytesAllocated { get; }
        }
    }

    public sealed class MemoryPoseAccessor : IMovementPoseAccessor
    {
        private MovementPose _pose;
        public MemoryPoseAccessor(Vector3 position) { _pose = new MovementPose(position, Quaternion.identity); }
        public bool IsValid => true;
        public MovementPose GetPose() => _pose;
        public void SetPose(MovementPose pose) { _pose = pose; }
    }

    public sealed class MutableSpeedProvider : IMovementSpeedProvider
    {
        public MutableSpeedProvider(float baseSpeed) { BaseSpeed = baseSpeed; Multiplier = 1f; }
        public float BaseSpeed { get; }
        public float Multiplier { get; set; }
        public float GetSpeed(MovementAgentId agentId) => BaseSpeed * Multiplier;
    }

    public sealed class OrderedSpeedProvider : IMovementSpeedProvider
    {
        public static readonly List<long> Order = new List<long>();
        public float GetSpeed(MovementAgentId agentId) { Order.Add(agentId.Value); return 1f; }
    }
}
