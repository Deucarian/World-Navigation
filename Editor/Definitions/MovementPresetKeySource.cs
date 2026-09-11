using System;
using System.Linq;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.WorldNavigation.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.WorldNavigation.Editor.Definitions
{
    public sealed class MovementPresetKeySource : DeucarianAssetKeySource<MovementPresetDefinitionAsset>
    {
        public override Type KeyType => typeof(MovementPresetKey);
        public override Type DefinitionSetAttribute => typeof(MovementPresetKeySetAttribute);
        public override string GeneratedClassName => "ProjectMovementPresets";
        protected override DeucarianKeyChoice ReadDefinition(MovementPresetDefinitionAsset asset) => new DeucarianKeyChoice(asset.Id, asset.DisplayName);
    }
}
