using MagicUI.Core;
using UnityEngine;
using UImage = UnityEngine.UI.Image;

namespace MagicUI.Components
{
    /// <summary>
    /// A simple image element
    /// </summary>
    public sealed class Image : ArrangableElement, IGameObjectWrapper
    {
        private readonly GameObject imgObj;
        private readonly UImage img;
        private readonly RectTransform tx;

        /// <inheritdoc/>
        public GameObject GameObject => imgObj;

        /// <summary>
        /// The desired width of the image; it will be scaled as needed
        /// </summary>
        public float Width
        {
            get => tx.rect.width;
            set
            {
                if (tx.rect.width != value)
                {
                    tx.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// The desired height of the image; it will be scaled as needed
        /// </summary>
        public float Height
        {
            get => tx.rect.height;
            set
            {
                if (tx.rect.height != value)
                {
                    tx.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Creates an image
        /// </summary>
        /// <param name="onLayout">The layout root to draw the image on</param>
        /// <param name="sprite">The sprite to use to render the image</param>
        /// <param name="name">The name of the image element</param>
        public Image(LayoutRoot onLayout, Sprite sprite, string name = "New Image") : base(onLayout, name) 
        {
            imgObj = new GameObject(name);
            imgObj.AddComponent<CanvasRenderer>();

            Vector2 size = sprite.textureRect.size;
            Vector2 pos = UI.UnityScreenPosition(new Vector2(0, 0), size);
            tx = imgObj.AddComponent<RectTransform>();
            tx.sizeDelta = size;
            tx.anchorMin = pos;
            tx.anchorMax = pos;

            img = imgObj.AddComponent<UImage>();
            img.sprite = sprite;
            if (sprite.border != Vector4.zero)
            {
                img.type = UImage.Type.Sliced;
            }

            imgObj.transform.SetParent(onLayout.Canvas.transform, false);
        }

        /// <inheritdoc/>
        protected override Vector2 MeasureOverride()
        {
            return tx.rect.size;
        }

        /// <inheritdoc/>
        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            Vector2 pos = UI.UnityScreenPosition(alignedTopLeftCorner, ContentSize);
            tx.anchorMin = pos;
            tx.anchorMax = pos;

            imgObj.SetActive(IsEffectivelyVisible);
        }

        /// <inheritdoc/>
        protected override void DestroyOverride()
        {
            Object.Destroy(imgObj);
        }
    }
}
