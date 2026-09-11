using System;

using UnityEngine;

namespace Deucarian.WorldNavigation.Unity
{
    /// <summary>Reusable defaults for code calls and serialized typed keys; runtime state remains in the core service.</summary>
    public sealed class MovementPresetDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private float speed = 3f;
        public string Id => id;
        public string DisplayName => displayName;
        public MovementPresetKey Key => new AssetKey(id);
        public float Speed => speed;
        public void Validate() { if (float.IsNaN(speed) || float.IsInfinity(speed) || speed < 0) throw new InvalidOperationException("Set a finite nonnegative speed in the movement preset."); }
        private sealed class AssetKey : MovementPresetKey { public AssetKey(string value) : base(value) { } }
    }
}
