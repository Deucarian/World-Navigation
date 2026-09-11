using System;
using System.Linq;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.WorldNavigation.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.WorldNavigation.Editor.Definitions
{
    public sealed class MovementPresetDefinitionSchema : DeucarianSerializedDefinitionSchema<MovementPresetDefinitionAsset, MovementPresetDefinitionSpec>
    {
        public override string Id => "movement-presets";
        public override string DisplayName => "Movement presets";
        public override void ValidateAssetReady(ScriptableObject asset)
        {
            base.ValidateAssetReady(asset);
            ((MovementPresetDefinitionAsset)asset).Validate();
        }
        public override void RefreshCatalog(bool validateOnly = false) { ProjectDefinitionCatalogAuthoring.Refresh(validateOnly); }
        [MenuItem("Assets/Create/Deucarian/World Navigation/MovementPreset Definition")]
        private static void CreateDefinition() { Selection.activeObject = DeucarianDefinitionSync.Create(new MovementPresetDefinitionSchema(), "NewMovementPreset"); }
    }
}
