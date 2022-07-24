using MagicUI.Core;
using MagicUI.Styles;
using UnityEngine;
using UImage = UnityEngine.UI.Image;

namespace MagicUI.Elements
{
    /// <summary>
    /// A simple image element
    /// </summary>
    [Stylable]
    public sealed class Image : ArrangableElement, IGameObjectWrapper
    {
        private readonly GameObject imgObj;
        private readonly UImage img;
        private readonly RectTransform tx;

        /// <inheritdoc/>
        public GameObject GameObject => imgObj;

        private float width;
        /// <summary>
        /// The desired width of the image; it will be scaled as needed
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
        /// The desired height of the image; it will be scaled as needed
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

        private bool preserveAspect = false;
        /// <summary>
        /// Whether to preserve the aspect ratio of the image when scaling
        /// </summary>
        public bool PreserveAspectRatio
        {
            get => preserveAspect;
            set
            {
                if (preserveAspect != value)
                {
                    preserveAspect = value;
                    InvalidateMeasure();
                }
            }
        }

        private Color color = Color.white;
        /// <summary>
        /// A color to apply over top of the image
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
        private UImage.Type nextType;
        private bool isSpriteValid = true;
        /// <summary>
        /// The current sprite underlying the image
        /// </summary>
        [StyleIgnore]
        public Sprite Sprite
        {
            get => sprite;
            set
            {
                nextType = value.border != Vector4.zero ? UImage.Type.Sliced : UImage.Type.Simple;
                sprite = value;
                isSpriteValid = false;
                InvalidateArrange();
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
            if (sprite.border != Vector4.zero)
            {
                img.type = UImage.Type.Sliced;
            }

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
            img.preserveAspect = preserveAspect;
            if (!isSpriteValid)
            {
                img.sprite = sprite;
                img.type = nextType;
                isSpriteValid = true;
            }

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
