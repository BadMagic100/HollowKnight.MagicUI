using UnityEngine;

namespace MagicUI.Core
{
    public static class UIExtensions
    {
        public static void Deconstruct(this Vector2 vec, out float x, out float y)
        {
            x = vec.x;
            y = vec.y;
        }
    }
}
