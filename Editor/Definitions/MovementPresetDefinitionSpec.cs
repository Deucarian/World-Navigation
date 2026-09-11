using System;
using System.Linq;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.WorldNavigation.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.WorldNavigation.Editor.Definitions
{
    [Serializable]
    public sealed class MovementPresetDefinitionSpec : DeucarianDefinitionSpec
    {
        [DefinitionField("speed")] public float Speed = 3f;
    }
}
