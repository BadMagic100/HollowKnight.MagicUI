using MagicUI.Core;
using UnityEngine;
using UImage = UnityEngine.UI.Image;

namespace MagicUI.Elements
{
    /// <summary>
    /// A horizontal progress bar. Note that this element only manages the filling image and value,
    /// for a more visually complex progress bar (e.g. with a border), use a <see cref="Panel"/> (recommended)
    /// or <see cref="GridLayout"/> to overlay the progress bar on a border image.
    /// </summary>
    public sealed class ProgressBar : ArrangableElement, IGameObjectWrapper
    {
        private readonly GameObject imgObj;
        private readonly UImage img;
        private readonly RectTransform tx;

        /// <inheritdoc/>
        public GameObject GameObject => imgObj;

        private float width;
        /// <summary>
        /// The desired width of the progress bar; it will be scaled as needed
        /// </summary>
        public float Width
        {
            get => width;
            set
            {
                if (width != value)
                {
                    width = value;
                    InvalidateMeasure();
                }
            }
        }

        private float height;
        /// <summary>
        /// The desired height of the progress bar; it will be scaled as needed
        /// </summary>
        public float Height
        {
            get => height;
            set
            {
                if (height != value)
                {
                    height = value;
                    InvalidateMeasure();
                }
            }
        }

        private float value;
        /// <summary>
        /// The value of the progress bar, as a percentage
        /// </summary>
        public float Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    InvalidateArrange();
                }
            }
        }

        private Color color = Color.white;
        /// <summary>
        /// A color to apply over top of the progress bar's image
        /// </summary>
        public Color Tint
        {
            get => color;
            set
            {
                if (color != value)
                {
                    color = value;
                    InvalidateArrange();
                }
            }
        }

        private Sprite sprite;
        private bool isSpriteValid = true;
        /// <summary>
        /// The current sprite underlying the progress bar
        /// </summary>
        public Sprite Sprite
        {
            get => sprite;
            set
            {
                sprite = value;
                isSpriteValid = false;
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Creates a progress bar
        /// </summary>
        /// <param name="onLayout">The layout root to draw the progress bar on</param>
        /// <param name="sprite">The sprite to use to render the progress bar</param>
        /// <param name="name">The name of the progress bar element</param>
        public ProgressBar(LayoutRoot onLayout, Sprite sprite, string name = "New ProgressBar") : base(onLayout, name)
        {
            imgObj = new GameObject(name);
            imgObj.AddComponent<CanvasRenderer>();

            Vector2 size = sprite.rect.size;
            Vector2 pos = UI.UnityScreenPosition(new Vector2(0, 0), size);
            tx = imgObj.AddComponent<RectTransform>();
            tx.sizeDelta = size;
            tx.anchorMin = pos;
            tx.anchorMax = pos;
            width = size.x;
            height = size.y;

            img = imgObj.AddComponent<UImage>();
            img.useSpriteMesh = true; // use a mesh material to respect packing rotation when applicable
            img.sprite = sprite;
            this.sprite = sprite;
            img.color = color;
            img.type = UImage.Type.Filled;
            img.fillAmount = 0;
            img.fillMethod = UImage.FillMethod.Horizontal;

            imgObj.transform.SetParent(onLayout.Canvas.transform, false);
            // hide the GO until the first arrange cycle takes control
            imgObj.SetActive(false);
        }

        /// <inheritdoc/>
        protected override Vector2 MeasureOverride()
        {
            return new Vector2(width, height);
        }

        /// <inheritdoc/>
        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            tx.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            tx.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            img.color = color;
            if (!isSpriteValid)
            {
                img.sprite = sprite;
                isSpriteValid = true;
            }
            img.fillAmount = value;

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
