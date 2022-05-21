using MagicUI.Behaviours;
using MagicUI.Core;
using MagicUI.Graphics;
using System;
using UnityEngine;
using UnityEngine.UI;
using UButton = UnityEngine.UI.Button;
using UImage = UnityEngine.UI.Image;

namespace MagicUI.Elements
{
    /// <summary>
    /// A button element
    /// </summary>
    public sealed class Button : ArrangableElement, IGameObjectWrapper, IControllerInteractable
    {
        private readonly GameObject imgObj;
        private readonly GameObject textObj;
        private readonly RectTransform imgTx;
        private readonly RectTransform textTx;
        private readonly UButton btn;
        private readonly UImage borderImage;
        private readonly Text textComponent;

        private Sprite? borderlessSprite;
        private Sprite? borderSprite;

        /// <summary>
        /// Event that fires when the button is clicked
        /// </summary>
        public event Action<Button>? Click;

        private void InvokeClick()
        {
            Click?.Invoke(this);
        }

        private float margin = 10;
        /// <summary>
        /// The internal margin between the button's content and its border
        /// </summary>
        public float Margin
        {
            get => margin;
            set
            {
                if (margin != value)
                {
                    margin = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// The button's content
        /// </summary>
        public string Content
        {
            get => textComponent.text;
            set
            {
                if (textComponent.text != value)
                {
                    textComponent.text = value;
                    InvalidateMeasure();
                }
            }
        }

        private bool borderless = false;
        /// <summary>
        /// Whether the button should be displayed in a borderless style.
        /// </summary>
        public bool Borderless
        {
            get => borderless;
            set
            {
                if (borderless != value)
                {
                    borderless = value;
                    InvalidateArrange();
                }
            }
        }

        /// <summary>
        /// The border color of the button
        /// </summary>
        public Color BorderColor
        {
            get => borderImage.color;
            set 
            {
                if (borderImage.color != value)
                {
                    borderImage.color = value;
                    InvalidateArrange();
                }
            }
        }

        /// <summary>
        /// The color of the text in the button
        /// </summary>
        public Color ContentColor
        {
            get => textComponent.color;
            set
            {
                if (textComponent.color != value)
                {
                    textComponent.color = value;
                    InvalidateArrange();
                }
            }
        }

        private float minWidth = 10;
        /// <summary>
        /// The minimum width of the button
        /// </summary>
        public float MinWidth
        {
            get => minWidth;
            set
            {
                if (value != minWidth)
                {
                    minWidth = value;
                    InvalidateMeasure();
                }
            }
        }

        private float minHeight = 10;
        /// <summary>
        /// The minimum height of the button
        /// </summary>
        public float MinHeight
        {
            get => minHeight;
            set
            {
                if (value != minHeight)
                {
                    minHeight = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// The font to use to display the content
        /// </summary>
        public Font Font
        {
            get => textComponent.font;
            set
            {
                if (value != textComponent.font)
                {
                    textComponent.font = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// The font size of the content
        /// </summary>
        public int FontSize
        {
            get => textComponent.fontSize;
            set
            {
                if (value != textComponent.fontSize)
                {
                    textComponent.fontSize = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Whether the button is enabled
        /// </summary>
        public bool Enabled
        {
            get => btn.interactable;
            set
            {
                if (value != btn.interactable)
                {
                    btn.interactable = value;
                    InvalidateArrange();
                }
            }
        }

        /// <summary>
        /// Creates a button
        /// </summary>
        /// <param name="onLayout">The layout root to draw the button on</param>
        /// <param name="name">The name of the button</param>
        public Button(LayoutRoot onLayout, string name = "New Button") : base(onLayout, name)
        {
            //todo: this should get rebuilt as a composition of a TextObject and an image - but the API support for that is not amazing right
            //      now, so it'll happen in v2.
            imgObj = new GameObject(name + "-Border");
            imgObj.AddComponent<CanvasRenderer>();

            Sprite sprite = ChooseBorderStyle();

            Vector2 size = sprite.textureRect.size;
            Vector2 pos = UI.UnityScreenPosition(new Vector2(0, 0), size);
            imgTx = imgObj.AddComponent<RectTransform>();
            imgTx.sizeDelta = size;
            imgTx.anchorMin = pos;
            imgTx.anchorMax = pos;

            borderImage = imgObj.AddComponent<UImage>();
            borderImage.sprite = sprite;
            borderImage.type = UImage.Type.Sliced;
            btn = imgObj.AddComponent<MultiGraphicButton>();
            btn.onClick.AddListener(InvokeClick);

            imgObj.transform.SetParent(onLayout.Canvas.transform, false);

            textObj = new GameObject(name + "-Content");
            Vector2 textCenter = new(0.5f, 0.5f);
            textTx = textObj.AddComponent<RectTransform>();
            textTx.anchorMin = textCenter;
            textTx.anchorMax = textCenter;

            textComponent = textObj.AddComponent<Text>();
            textComponent.font = UI.TrajanNormal;
            textComponent.text = "";
            textComponent.fontSize = 12;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textTx.sizeDelta = MeasureText();

            textObj.transform.SetParent(imgObj.transform, false);

            // hide the GO until the first arrange cycle takes control
            imgObj.SetActive(false);
        }

        /// <inheritdoc/>
        public GameObject GameObject => imgObj;

        private Sprite ChooseBorderStyle() => Borderless 
            ? (borderlessSprite ??= BuiltInSprites.CreateSlicedTransparentRect())
            : (borderSprite ??= BuiltInSprites.CreateSlicedBorderRect());

        private Vector2 MeasureText()
        {
            TextGenerator textGen = new();
            // have as much space as the screen for the text; otherwise we risk unwanted clipping
            TextGenerationSettings settings = textComponent.GetGenerationSettings(UI.Screen.size);
            // by default, this will inherit the parent canvas's scale factor, which is set to scale with screen space.
            // however, since we're functioning in an unscaled coordinate system we should get the unscaled size to measure correctly.
            settings.scaleFactor = 1;
            float width = textGen.GetPreferredWidth(textComponent.text, settings);
            float height = textGen.GetPreferredHeight(textComponent.text, settings);
            return new Vector2(Mathf.Ceil(width) + 1, Mathf.Ceil(height) + 1);
        }

        /// <inheritdoc/>
        protected override Vector2 MeasureOverride()
        {
            Vector2 size = MeasureText() + new Vector2(margin, margin);
            size.x = Mathf.Max(size.x, minWidth);
            size.y = Mathf.Max(size.y, minHeight);

            return size;
        }

        /// <inheritdoc/>
        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            imgTx.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ContentSize.x);
            imgTx.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ContentSize.y);

            textTx.sizeDelta = MeasureText();

            Vector2 pos = UI.UnityScreenPosition(alignedTopLeftCorner, ContentSize);
            imgTx.anchorMin = pos;
            imgTx.anchorMax = pos;

            borderImage.sprite = ChooseBorderStyle();

            imgObj.SetActive(IsEffectivelyVisible);
        }

        /// <inheritdoc/>
        protected override void DestroyOverride()
        {
            UnityEngine.Object.Destroy(imgObj);
            UnityEngine.Object.Destroy(textObj);
        }

        /// <inheritdoc/>
        public Selectable GetSelectable()
        {
            return btn;
        }
    }
}
