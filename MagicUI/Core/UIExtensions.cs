using UnityEngine;

namespace MagicUI.Core
{
    /// <summary>
    /// Static extensions for helping with UIs
    /// </summary>
    public static class UIExtensions
    {
        /// <summary>
        /// Desconstruction assignment for <see cref="Vector2"/>
        /// </summary>
        /// <param name="vec">The vector to deconstruct</param>
        /// <param name="x">The x component</param>
        /// <param name="y">The y component</param>
        public static void Deconstruct(this Vector2 vec, out float x, out float y)
        {
            x = vec.x;
            y = vec.y;
        }
    }
}
