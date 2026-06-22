using System;
using System.Collections.Generic;
using Deucarian.WorldSpawning;
using UnityEngine;

namespace Deucarian.WorldNavigation
{
    public readonly struct MovementAgentId : IEquatable<MovementAgentId>, IComparable<MovementAgentId>
    {
        public MovementAgentId(long value) { if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value)); Value = value; }
        public long Value { get; }
        public bool Equals(MovementAgentId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is MovementAgentId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public int CompareTo(MovementAgentId other) => Value.CompareTo(other.Value);
        public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public readonly struct MovementPose
    {
        public MovementPose(Vector3 position, Quaternion rotation) { Position = position; Rotation = rotation; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
    }

    public interface IMovementPoseAccessor
    {
        bool IsValid { get; }
        MovementPose GetPose();
        void SetPose(MovementPose pose);
    }

    public sealed class TransformMovementPoseAccessor : IMovementPoseAccessor
    {
        private readonly Transform _transform;
        public TransformMovementPoseAccessor(Transform transform) { _transform = transform; }
        public bool IsValid => _transform != null;
        public MovementPose GetPose() => new MovementPose(_transform.position, _transform.rotation);
        public void SetPose(MovementPose pose) { _transform.SetPositionAndRotation(pose.Position, pose.Rotation); }
    }

    public interface IMovementSpeedProvider
    {
        float GetSpeed(MovementAgentId agentId);
    }

    public sealed class ConstantMovementSpeedProvider : IMovementSpeedProvider
    {
        private readonly float _speed;
        public ConstantMovementSpeedProvider(float speed) { if (speed < 0f || float.IsNaN(speed) || float.IsInfinity(speed)) throw new ArgumentOutOfRangeException(nameof(speed)); _speed = speed; }
        public float GetSpeed(MovementAgentId agentId) => _speed;
    }

    public interface IWorldMovementAgent { MovementAgentId AgentId { get; } }
    public interface IWorldMovementResettable { void ResetWorldMovement(); }

    public readonly struct MovementWaypoint
    {
        public MovementWaypoint(Vector3 position) { Position = position; }
        public Vector3 Position { get; }
    }

    public sealed class MovementPath
    {
        private readonly MovementWaypoint[] _waypoints;
        public MovementPath(IReadOnlyList<MovementWaypoint> waypoints)
        {
            if (waypoints == null || waypoints.Count == 0) throw new ArgumentException("Path needs at least one waypoint.", nameof(waypoints));
            _waypoints = new MovementWaypoint[waypoints.Count];
            for (int i = 0; i < waypoints.Count; i++) _waypoints[i] = waypoints[i];
        }
        public IReadOnlyList<MovementWaypoint> Waypoints => _waypoints;
    }

    public enum MovementCommandKind { None = 0, Destination = 1, Path = 2 }
    public enum MovementStatus { Success = 0, InvalidInput = 1, DuplicateAgent = 2, UnknownAgent = 3, InvalidSpeed = 4, InvalidPath = 5, InvalidPoseAccessor = 6 }

    public sealed class MovementAgentDefinition
    {
        public MovementAgentDefinition(MovementAgentId id, IMovementPoseAccessor poseAccessor, IMovementSpeedProvider speedProvider)
        {
            if (id.Value <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            Id = id; PoseAccessor = poseAccessor ?? throw new ArgumentNullException(nameof(poseAccessor)); SpeedProvider = speedProvider ?? throw new ArgumentNullException(nameof(speedProvider));
        }
        public MovementAgentId Id { get; }
        public IMovementPoseAccessor PoseAccessor { get; }
        public IMovementSpeedProvider SpeedProvider { get; }
    }

    public readonly struct MovementAgentHandle
    {
        public MovementAgentHandle(MovementAgentId id) { Id = id; }
        public MovementAgentId Id { get; }
    }

    public readonly struct MovementProgress
    {
        public MovementProgress(MovementCommandKind kind, int waypointIndex, int completedWaypoints, int totalWaypoints, bool reachedDestination)
        {
            Kind = kind; WaypointIndex = waypointIndex; CompletedWaypoints = completedWaypoints; TotalWaypoints = totalWaypoints; ReachedDestination = reachedDestination;
        }
        public MovementCommandKind Kind { get; }
        public int WaypointIndex { get; }
        public int CompletedWaypoints { get; }
        public int TotalWaypoints { get; }
        public bool ReachedDestination { get; }
        public float NormalizedProgress => TotalWaypoints <= 0 ? (ReachedDestination ? 1f : 0f) : Mathf.Clamp01((float)CompletedWaypoints / TotalWaypoints);
    }

    public readonly struct MovementCommand
    {
        private MovementCommand(MovementCommandKind kind, Vector3 destination, MovementPath path, float reachDistance)
        {
            Kind = kind; Destination = destination; Path = path; ReachDistance = Mathf.Max(0f, reachDistance);
        }
        public MovementCommandKind Kind { get; }
        public Vector3 Destination { get; }
        public MovementPath Path { get; }
        public float ReachDistance { get; }
        public static MovementCommand None() => new MovementCommand(MovementCommandKind.None, default, null, 0f);
        public static MovementCommand DestinationTo(Vector3 destination, float reachDistance = 0.001f) => new MovementCommand(MovementCommandKind.Destination, destination, null, reachDistance);
        public static MovementCommand FollowPath(MovementPath path, float reachDistance = 0.001f) => new MovementCommand(MovementCommandKind.Path, default, path ?? throw new ArgumentNullException(nameof(path)), reachDistance);
    }

    public readonly struct MovementResult
    {
        public MovementResult(MovementStatus status, MovementAgentId agentId) { Status = status; AgentId = agentId; }
        public MovementStatus Status { get; }
        public MovementAgentId AgentId { get; }
        public bool Succeeded => Status == MovementStatus.Success;
    }

    public readonly struct MovementTickResult
    {
        public MovementTickResult(int agentsVisited, int agentsMoved, int agentsReached) { AgentsVisited = agentsVisited; AgentsMoved = agentsMoved; AgentsReached = agentsReached; }
        public int AgentsVisited { get; }
        public int AgentsMoved { get; }
        public int AgentsReached { get; }
    }

    public readonly struct MovementAgentSnapshot
    {
        public MovementAgentSnapshot(MovementAgentId id, Vector3 position, MovementCommandKind commandKind, bool paused, MovementProgress progress)
        {
            Id = id; Position = position; CommandKind = commandKind; Paused = paused; Progress = progress;
        }
        public MovementAgentId Id { get; }
        public Vector3 Position { get; }
        public MovementCommandKind CommandKind { get; }
        public bool Paused { get; }
        public MovementProgress Progress { get; }
    }

    public sealed class MovementSnapshot
    {
        public MovementSnapshot(IReadOnlyList<MovementAgentSnapshot> agents) { Agents = Copy(agents); }
        public IReadOnlyList<MovementAgentSnapshot> Agents { get; }
        private static T[] Copy<T>(IReadOnlyList<T> source) { if (source == null) return Array.Empty<T>(); var copy = new T[source.Count]; for (int i = 0; i < source.Count; i++) copy[i] = source[i]; return copy; }
    }

    public sealed class WorldNavigationService
    {
        private readonly Dictionary<MovementAgentId, AgentState> _agents = new Dictionary<MovementAgentId, AgentState>();
        private readonly List<MovementAgentId> _ordered = new List<MovementAgentId>();
        private long _nextAgentId;
        public bool Paused { get; private set; }
        public int AgentCount => _agents.Count;

        public MovementResult Register(MovementAgentDefinition definition)
        {
            if (definition == null || definition.PoseAccessor == null || definition.SpeedProvider == null || !definition.PoseAccessor.IsValid) return new MovementResult(MovementStatus.InvalidPoseAccessor, default);
            float speed = definition.SpeedProvider.GetSpeed(definition.Id);
            if (speed < 0f || float.IsNaN(speed) || float.IsInfinity(speed)) return new MovementResult(MovementStatus.InvalidSpeed, definition.Id);
            if (_agents.ContainsKey(definition.Id)) return new MovementResult(MovementStatus.DuplicateAgent, definition.Id);
            _agents.Add(definition.Id, new AgentState(definition));
            _ordered.Add(definition.Id);
            _ordered.Sort();
            _nextAgentId = Math.Max(_nextAgentId, definition.Id.Value);
            return new MovementResult(MovementStatus.Success, definition.Id);
        }

        public MovementAgentHandle Register(IMovementPoseAccessor accessor, IMovementSpeedProvider speedProvider)
        {
            MovementAgentId id = new MovementAgentId(++_nextAgentId);
            MovementResult result = Register(new MovementAgentDefinition(id, accessor, speedProvider));
            if (!result.Succeeded) throw new InvalidOperationException(result.Status.ToString());
            return new MovementAgentHandle(id);
        }

        public MovementResult RegisterSpawnedObject(SpawnResult spawnResult, IMovementSpeedProvider speedProvider, out MovementAgentHandle handle)
        {
            handle = default;
            if (!spawnResult.Succeeded || spawnResult.Instance == null) return new MovementResult(MovementStatus.InvalidInput, default);
            handle = Register(new TransformMovementPoseAccessor(spawnResult.Instance.transform), speedProvider);
            return new MovementResult(MovementStatus.Success, handle.Id);
        }

        public MovementResult Unregister(MovementAgentId id)
        {
            if (!_agents.Remove(id)) return new MovementResult(MovementStatus.UnknownAgent, id);
            _ordered.Remove(id);
            return new MovementResult(MovementStatus.Success, id);
        }

        public MovementResult CleanupDespawned(MovementAgentId id) => Unregister(id);

        public MovementResult SetDestination(MovementAgentId id, Vector3 destination, float reachDistance = 0.001f) => SetCommand(id, MovementCommand.DestinationTo(destination, reachDistance));
        public MovementResult FollowPath(MovementAgentId id, MovementPath path, float reachDistance = 0.001f) => path == null ? new MovementResult(MovementStatus.InvalidPath, id) : SetCommand(id, MovementCommand.FollowPath(path, reachDistance));
        public MovementResult Stop(MovementAgentId id) => SetCommand(id, MovementCommand.None());
        public MovementResult Pause(MovementAgentId id) { if (!_agents.TryGetValue(id, out AgentState state)) return new MovementResult(MovementStatus.UnknownAgent, id); state.Paused = true; return new MovementResult(MovementStatus.Success, id); }
        public MovementResult Resume(MovementAgentId id) { if (!_agents.TryGetValue(id, out AgentState state)) return new MovementResult(MovementStatus.UnknownAgent, id); state.Paused = false; return new MovementResult(MovementStatus.Success, id); }
        public void PauseAll() => Paused = true;
        public void ResumeAll() => Paused = false;
        public MovementResult Teleport(MovementAgentId id, MovementPose pose) { if (!_agents.TryGetValue(id, out AgentState state)) return new MovementResult(MovementStatus.UnknownAgent, id); state.Definition.PoseAccessor.SetPose(pose); return new MovementResult(MovementStatus.Success, id); }
        public bool TryGetPose(MovementAgentId id, out MovementPose pose) { if (_agents.TryGetValue(id, out AgentState state)) { pose = state.Definition.PoseAccessor.GetPose(); return true; } pose = default; return false; }
        public bool TryGetProgress(MovementAgentId id, out MovementProgress progress) { if (_agents.TryGetValue(id, out AgentState state)) { progress = state.Progress; return true; } progress = default; return false; }

        public MovementTickResult Tick(float deltaSeconds)
        {
            if (deltaSeconds < 0f || float.IsNaN(deltaSeconds) || float.IsInfinity(deltaSeconds)) throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
            if (Paused || deltaSeconds <= 0f) return new MovementTickResult(_ordered.Count, 0, 0);
            int moved = 0; int reached = 0;
            for (int i = 0; i < _ordered.Count; i++)
            {
                AgentState state = _agents[_ordered[i]];
                if (state.Paused || state.Command.Kind == MovementCommandKind.None) continue;
                float speed = state.Definition.SpeedProvider.GetSpeed(state.Definition.Id);
                if (speed < 0f || float.IsNaN(speed) || float.IsInfinity(speed)) throw new InvalidOperationException("Speed provider returned invalid speed.");
                if (speed <= 0f) continue;
                if (TickAgent(state, speed * deltaSeconds, out bool didMove, out bool didReach))
                {
                    if (didMove) moved++;
                    if (didReach) reached++;
                }
            }
            return new MovementTickResult(_ordered.Count, moved, reached);
        }

        public MovementSnapshot CreateSnapshot()
        {
            var snapshots = new MovementAgentSnapshot[_ordered.Count];
            for (int i = 0; i < _ordered.Count; i++)
            {
                AgentState state = _agents[_ordered[i]];
                snapshots[i] = new MovementAgentSnapshot(state.Definition.Id, state.Definition.PoseAccessor.GetPose().Position, state.Command.Kind, state.Paused, state.Progress);
            }
            return new MovementSnapshot(snapshots);
        }

        private MovementResult SetCommand(MovementAgentId id, MovementCommand command)
        {
            if (!_agents.TryGetValue(id, out AgentState state)) return new MovementResult(MovementStatus.UnknownAgent, id);
            if (command.Kind == MovementCommandKind.Path && (command.Path == null || command.Path.Waypoints.Count == 0)) return new MovementResult(MovementStatus.InvalidPath, id);
            state.Command = command;
            state.PathIndex = 0;
            state.Progress = BuildProgress(command, false, 0);
            return new MovementResult(MovementStatus.Success, id);
        }

        private static bool TickAgent(AgentState state, float distanceBudget, out bool moved, out bool reached)
        {
            moved = false; reached = false;
            while (distanceBudget > 0f && state.Command.Kind != MovementCommandKind.None)
            {
                Vector3 target = ResolveTarget(state);
                MovementPose pose = state.Definition.PoseAccessor.GetPose();
                Vector3 delta = target - pose.Position;
                float distance = delta.magnitude;
                if (distance <= state.Command.ReachDistance)
                {
                    if (AdvanceTarget(state)) { reached = true; }
                    continue;
                }
                float step = Mathf.Min(distanceBudget, distance);
                Vector3 next = pose.Position + (delta / distance * step);
                state.Definition.PoseAccessor.SetPose(new MovementPose(next, pose.Rotation));
                distanceBudget -= step;
                moved = true;
                if (step >= distance - state.Command.ReachDistance)
                {
                    if (AdvanceTarget(state)) { reached = true; }
                }
                else
                {
                    break;
                }
            }
            return true;
        }

        private static Vector3 ResolveTarget(AgentState state)
        {
            return state.Command.Kind == MovementCommandKind.Path ? state.Command.Path.Waypoints[state.PathIndex].Position : state.Command.Destination;
        }

        private static bool AdvanceTarget(AgentState state)
        {
            if (state.Command.Kind == MovementCommandKind.Destination)
            {
                state.Progress = new MovementProgress(MovementCommandKind.Destination, 0, 1, 1, true);
                state.Command = MovementCommand.None();
                return true;
            }
            state.PathIndex++;
            bool complete = state.PathIndex >= state.Command.Path.Waypoints.Count;
            state.Progress = BuildProgress(state.Command, complete, state.PathIndex);
            if (complete) state.Command = MovementCommand.None();
            return complete;
        }

        private static MovementProgress BuildProgress(MovementCommand command, bool reached, int completed)
        {
            if (command.Kind == MovementCommandKind.Path)
            {
                int total = command.Path == null ? 0 : command.Path.Waypoints.Count;
                return new MovementProgress(MovementCommandKind.Path, Math.Min(completed, Math.Max(0, total - 1)), Math.Min(completed, total), total, reached);
            }
            if (command.Kind == MovementCommandKind.Destination) return new MovementProgress(MovementCommandKind.Destination, 0, reached ? 1 : 0, 1, reached);
            return new MovementProgress(MovementCommandKind.None, 0, 0, 0, false);
        }

        private sealed class AgentState
        {
            public AgentState(MovementAgentDefinition definition) { Definition = definition; Progress = new MovementProgress(MovementCommandKind.None, 0, 0, 0, false); }
            public MovementAgentDefinition Definition { get; }
            public MovementCommand Command;
            public int PathIndex;
            public bool Paused;
            public MovementProgress Progress;
        }
    }
}
