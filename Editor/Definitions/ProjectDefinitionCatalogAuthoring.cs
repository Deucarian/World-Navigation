using System;
using System.Linq;
using Deucarian.Editor.Definitions;
using Deucarian.WorldNavigation.Unity;
using UnityEditor;
namespace Deucarian.WorldNavigation.Editor.Definitions
{
    internal static class ProjectDefinitionCatalogAuthoring
    {
        internal static void Refresh(bool validateOnly = false)
        {
            var definitions = AssetDatabase.FindAssets("t:MovementPresetDefinitionAsset", new[] { "Assets" }).Select(x => AssetDatabase.LoadAssetAtPath<MovementPresetDefinitionAsset>(AssetDatabase.GUIDToAssetPath(x))).Where(x => x != null).OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
            if (definitions.GroupBy(x => x.Id).Any(x => x.Count() > 1)) throw new InvalidOperationException("MovementPreset definition IDs must be unique.");
            DeucarianDefinitionCatalog.Update<MovementPresetDefinitionCatalog>("Assets/DeucarianDefinitions/Resources/Deucarian/Definitions/MovementPresetDefinitionCatalog.asset", "definitions", definitions, validateOnly);
        }
    }
}
