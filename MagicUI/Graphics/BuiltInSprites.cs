using UnityEngine;

namespace MagicUI.Graphics
{
    /// <summary>
    /// A registry of available common built-in sprites
    /// </summary>
    public static class BuiltInSprites
    {
        private static TextureLoader? _loader;
        private static TextureLoader Loader
        {
            get
            {
                if (_loader == null)
                {
                    _loader = new TextureLoader(typeof(BuiltInSprites).Assembly, "MagicUI.Resources");
                }
                return _loader;
            }
        }

        /// <summary>
        /// Creates a sliced sprite for a scalable border with rounded corners
        /// </summary>
        public static Sprite CreateSlicedBorderRect()
        {
            Texture2D tex = Loader.GetTexture("RoundedCornerRect-2.png");
            return tex.ToSlicedSprite(6, 5, 6, 5);
        }

        /// <summary>
        /// Creates a sliced sprite for a scalable transparent border
        /// </summary>
        public static Sprite CreateSlicedTransparentRect()
        {
            Texture2D tex = Loader.GetTexture("BorderlessButton.png");
            return tex.ToSlicedSprite(6, 5, 6, 5);
        }

        /// <summary>
        /// Creates a sliced sprite for a scalable underline
        /// </summary>
        public static Sprite CreateSlicedUnderline()
        {
            Texture2D tex = Loader.GetTexture("Underline-2.png");
            return tex.ToSlicedSprite(6, 5, 6, 5);
        }

        /// <summary>
        /// Creates a standard sprite of the a white quill
        /// </summary>
        public static Sprite CreateQuill()
        {
            Texture2D tex = Loader.GetTexture("inv_item__0004_quill-white.png");
            return tex.ToSprite();
        }
    }
}
