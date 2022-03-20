using UnityEngine;

namespace MagicUI.Graphics
{
    /// <summary>
    /// Extension methods for textures
    /// </summary>
    public static class TextureExtensions
    {
        /// <summary>
        /// Creates a basic full-size sprite from a texture
        /// </summary>
        public static Sprite ToSprite(this Texture2D tex)
        {
            return Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Creates a sliced sprite from a texture with the specified borders
        /// </summary>
        public static Sprite ToSlicedSprite(this Texture2D tex, float left, float top, float right, float bottom)
        {
            return Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100,
                0,
                SpriteMeshType.FullRect,
                new Vector4(left, bottom, right, top)
            );
        }
    }
}
