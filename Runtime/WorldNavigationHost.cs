using System;
using Deucarian.Diagnostics;
using UnityEngine;
namespace Deucarian.WorldNavigation
{
    /// <summary>Owns and ticks one movement scope. Agents retain their own registrations.</summary>
    [DefaultExecutionOrder(-1000), DisallowMultipleComponent]
    public sealed class WorldNavigationHost : MonoBehaviour, IDiagnosticProvider
    {
        private readonly WorldNavigationService service = new WorldNavigationService();
        private DiagnosticProviderRegistration diagnosticRegistration;
        public WorldNavigationService Service => !service.IsDisposed ? service : throw new ObjectDisposedException(nameof(WorldNavigationHost));
        private void Awake() => diagnosticRegistration = DiagnosticProviderRegistry.Register(this);
        private void Update() => service.Tick(Time.deltaTime);
        private void OnDestroy() { service.Dispose(); diagnosticRegistration?.Dispose(); diagnosticRegistration = null; }
        string IDiagnosticProvider.ProviderId => "world-navigation.scope." + GetInstanceID();
        string IDiagnosticProvider.DisplayName => "WorldNavigationHost";
        void IDiagnosticProvider.Collect(DiagnosticReportBuilder builder) => builder.AddSection(((IDiagnosticProvider)this).ProviderId, "World navigation")
            .AddItem("agents", "Registered agents", service.AgentCount.ToString())
            .AddItem("enabled", "Clock running", isActiveAndEnabled.ToString());
    }
}
