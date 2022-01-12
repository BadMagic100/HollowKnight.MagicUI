using Modding;
using UnityEngine;

namespace MagicUI.Core
{
    /// <summary>
    /// Class containing various UI utilities and constants
    /// </summary>
    public static class UI
    {
        /// <summary>
        /// A rectangle representing the screen
        /// </summary>
        public static readonly Rect Screen = new(Vector2.zero, new Vector2(1920, 1080));

        public static Font TrajanNormal { get => CanvasUtil.TrajanNormal; }

        public static Font TrajanBold { get => CanvasUtil.TrajanBold; }

        public static Vector2 UnityScreenPosition(Vector2 pos, Vector2 elementSize)
        {
            return UnityParentRelativePosition(pos, elementSize, Screen.size);
        }

        public static Vector2 UnityParentRelativePosition(Vector2 pos, Vector2 elementSize, Vector2 parentSize)
        {
            return new((pos.x + elementSize.x / 2f) / parentSize.x,
                   (parentSize.y - (pos.y + elementSize.y / 2f)) / parentSize.y);
        }
    }
}
