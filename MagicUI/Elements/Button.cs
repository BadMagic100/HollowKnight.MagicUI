using MagicUI.Behaviours;
using MagicUI.Core;
using MagicUI.Graphics;
using MagicUI.Styles;
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
    [Stylable]
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

        /// <summary>
        /// Event that fires when the button is hovered over, either by mouse or controller inputs.
        /// </summary>
        public event Action<Button>? OnHover;

        /// <summary>
        /// Event that fires when the button stops being hovered over, either by mouse or controller inputs.
        /// </summary>
        public event Action<Button>? OnUnhover;

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

        private string content = "";
        /// <summary>
        /// The button's content
        /// </summary>
        [StyleIgnore]
        public string Content
        {
            get => content;
            set
            {
                if (content != value)
                {
                    content = value;
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

        private Color borderColor = Color.white;
        /// <summary>
        /// The border color of the button
        /// </summary>
        public Color BorderColor
        {
            get => borderColor;
            set 
            {
                if (borderColor != value)
                {
                    borderColor = value;
                    InvalidateArrange();
                }
            }
        }

        private Color contentColor = Color.white;
        /// <summary>
        /// The color of the text in the button
        /// </summary>
        public Color ContentColor
        {
            get => contentColor;
            set
            {
                if (contentColor != value)
                {
                    contentColor = value;
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

        private Font font = UI.TrajanNormal;
        /// <summary>
        /// The font to use to display the content
        /// </summary>
        public Font Font
        {
            get => font;
            set
            {
                if (value != font)
                {
                    font = value;
                    InvalidateMeasure();
                }
            }
        }

        private int fontSize = 12;
        /// <summary>
        /// The font size of the content
        /// </summary>
        public int FontSize
        {
            get => fontSize;
            set
            {
                if (value != fontSize)
                {
                    fontSize = value;
                    InvalidateMeasure();
                }
            }
        }

        bool enabled = true;
        /// <summary>
        /// Whether the button is enabled
        /// </summary>
        [StyleIgnore]
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (value != enabled)
                {
                    enabled = value;
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
            borderImage.color = contentColor;
            borderImage.type = UImage.Type.Sliced;
            btn = imgObj.AddComponent<MultiGraphicButton>();
            btn.interactable = enabled;
            btn.onClick.AddListener(InvokeClick);

            SelectionEventProxy selectionProxy = imgObj.AddComponent<SelectionEventProxy>();
            selectionProxy.OnSelectProxy = () => OnHover?.Invoke(this);
            selectionProxy.OnDeselectProxy = () => OnUnhover?.Invoke(this);

            imgObj.transform.SetParent(onLayout.Canvas.transform, false);

            textObj = new GameObject(name + "-Content");
            Vector2 textCenter = new(0.5f, 0.5f);
            textTx = textObj.AddComponent<RectTransform>();
            textTx.anchorMin = textCenter;
            textTx.anchorMax = textCenter;

            textComponent = textObj.AddComponent<Text>();
            textComponent.font = font;
            textComponent.text = content;
            textComponent.fontSize = fontSize;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.color = contentColor;

            ContentSizeFitter fitter = textObj.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

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
            // use the staged backing fields instead of the actual current property of the text.
            // in other words, the value it will be after measure/arrange rather than the value it currently is.
            settings.font = font;
            settings.fontSize = fontSize;

            float width = textGen.GetPreferredWidth(content, settings);
            float height = textGen.GetPreferredHeight(content, settings);
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
            btn.interactable = enabled;

            textComponent.text = content;
            textComponent.font = font;
            textComponent.fontSize = fontSize;
            textComponent.color = contentColor;

            imgTx.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ContentSize.x);
            imgTx.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ContentSize.y);

            Vector2 pos = UI.UnityScreenPosition(alignedTopLeftCorner, ContentSize);
            imgTx.anchorMin = pos;
            imgTx.anchorMax = pos;

            borderImage.sprite = ChooseBorderStyle();
            borderImage.color = borderColor;

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
