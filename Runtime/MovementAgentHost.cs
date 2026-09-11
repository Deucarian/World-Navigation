using Deucarian.Diagnostics;
using System;
using UnityEngine;

namespace Deucarian.WorldNavigation
{
    /// <summary>Owns one movement registration. The shared navigation service retains all motion state and ticking.</summary>
    [DisallowMultipleComponent]
    public sealed class MovementAgentHost : MonoBehaviour, IDiagnosticProvider
    {
        private WorldNavigationService service;
        private IMovementSpeedProvider speed;
        private MovementAgentHandle handle;
        private bool registered;
        private bool destroyed;

        public void Configure(WorldNavigationService navigation, IMovementSpeedProvider speedProvider)
        {
            if (destroyed) throw new ObjectDisposedException(nameof(MovementAgentHost));
            if (service != null) throw new InvalidOperationException("MovementAgentHost '" + name + "' is already configured.");
            if (navigation == null) throw new ArgumentNullException(nameof(navigation));
            if (speedProvider == null) throw new ArgumentNullException(nameof(speedProvider));
            service = navigation;
            speed = speedProvider;
            try { if (isActiveAndEnabled) Register(); }
            catch { service = null; speed = null; throw; }
        }

        public MovementResult MoveTo(Vector3 destination) => Service.SetDestination(handle.Id, destination);
        public MovementResult Follow(MovementPath path) => Service.FollowPath(handle.Id, path);
        public MovementResult Stop() => Service.Stop(handle.Id);
        public MovementResult Pause() => Service.Pause(handle.Id);
        public MovementResult Resume() => Service.Resume(handle.Id);

        private WorldNavigationService Service
        {
            get
            {
                if (destroyed) throw new ObjectDisposedException(nameof(MovementAgentHost));
                if (service == null) throw new InvalidOperationException("MovementAgentHost '" + name + "' is not configured. Call Configure with the shared WorldNavigationService and a speed provider during startup.");
                if (!registered) throw new InvalidOperationException("MovementAgentHost '" + name + "' is disabled. Enable it before sending movement commands.");
                return service;
            }
        }
        private void Register() { handle = service.Register(new TransformMovementPoseAccessor(transform), speed); registered = true; }
        private void OnEnable() { if (service != null && !registered) Register(); }
        private void OnDisable() { if (registered) service.Unregister(handle.Id); registered = false; handle = default; }
        private void OnDestroy() { diagnosticRegistration?.Dispose(); diagnosticRegistration = null;  OnDisable(); destroyed = true; service = null; speed = null; }
        private DiagnosticProviderRegistration diagnosticRegistration;
        private void Awake() => diagnosticRegistration = DiagnosticProviderRegistry.Register(this);
        string IDiagnosticProvider.ProviderId => "world-navigation.host." + GetInstanceID();
        string IDiagnosticProvider.DisplayName => "MovementAgentHost";
        void IDiagnosticProvider.Collect(DiagnosticReportBuilder builder)
        {
            bool configured = service != null;
            builder.AddSection(((IDiagnosticProvider)this).ProviderId, "MovementAgentHost")
                .AddItem("configured", "Configured", configured ? "Ready" : "Call Configure during startup",
                    configured ? DiagnosticSeverity.Info : DiagnosticSeverity.Warning)
                .AddItem("enabled", "Enabled", isActiveAndEnabled.ToString());
        }
    }
}
