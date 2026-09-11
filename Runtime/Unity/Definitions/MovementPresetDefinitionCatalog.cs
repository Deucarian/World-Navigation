using System;
using System.Linq;
using UnityEngine;
namespace Deucarian.WorldNavigation.Unity
{
    public sealed class MovementPresetDefinitionCatalog : ScriptableObject
    {
        public const string ResourcePath = "Deucarian/Definitions/MovementPresetDefinitionCatalog";
        [SerializeField] private MovementPresetDefinitionAsset[] definitions = Array.Empty<MovementPresetDefinitionAsset>();
        public static MovementPresetDefinitionCatalog LoadProject() => Resources.Load<MovementPresetDefinitionCatalog>(ResourcePath) ?? throw new InvalidOperationException("Create a MovementPreset definition in Definitions before using its catalog.");
        public MovementPresetDefinitionAsset Get(MovementPresetKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key), "Select a MovementPreset key in the Inspector or pass a generated key.");
            var matches = definitions.Where(x => x != null && x.Id == key.Id).ToArray();
            if (matches.Length != 1) throw new InvalidOperationException("The MovementPreset catalog needs exactly one definition for '" + key.Id + "'. Synchronize Definitions before using it.");
            matches[0].Validate(); return matches[0];
        }
    }
}
