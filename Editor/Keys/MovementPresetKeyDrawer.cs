using System;
using Deucarian.Editor;
using UnityEditor;

namespace Deucarian.WorldNavigation.Editor
{
    [CustomPropertyDrawer(typeof(MovementPresetKey), true)]
    public sealed class MovementPresetKeyDrawer : DeucarianKeyDrawer
    {
        public override Type KeyType => typeof(MovementPresetKey);
        public override Type DefinitionSetAttribute => typeof(MovementPresetKeySetAttribute);
        public override string SetupHint => "Select an existing MovementPresetKey; declare reusable keys once in a [MovementPresetKeySet] class.";
    }
}
