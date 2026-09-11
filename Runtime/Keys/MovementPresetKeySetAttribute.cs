using System;

namespace Deucarian.WorldNavigation
{
    /// <summary>Marks an authoritative set of named MovementPresetKey fields or properties for the Inspector.</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class MovementPresetKeySetAttribute : Attribute { }
}
