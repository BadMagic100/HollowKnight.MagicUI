using System;

namespace MagicUI.Core
{
    /// <summary>
    /// Modifier keys for hotkey listeners
    /// </summary>
    [Flags]
    public enum ModifierKeys
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        None = 0,
        Ctrl = 1,
        Alt = 2,
        Shift = 4
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
