using System;

namespace MagicUI.Core
{
    /// <summary>
    /// Modifier keys for hotkey listeners
    /// </summary>
    [Flags]
    public enum ModifierKeys
    {
        None = 0,
        Ctrl = 1,
        Alt = 2,
        Shift = 4
    }
}
