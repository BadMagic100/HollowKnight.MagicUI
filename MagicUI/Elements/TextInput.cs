using MagicUI.Core;
using MagicUI.Graphics;
using MagicUI.Styles;
using System;
using UnityEngine;
using UnityEngine.UI;
using UImage = UnityEngine.UI.Image;

namespace MagicUI.Elements
{
    /// <summary>
    /// A text input element
    /// </summary>
    [Stylable]
    public sealed class TextInput : ArrangableElement, IGameObjectWrapper, IControllerInteractable
    {
        private readonly GameObject underlineObj;
        private readonly GameObject iconObj;
        private readonly GameObject textObj;
        private readonly GameObject placeholderObj;
        private readonly RectTransform underlineTx;
        private readonly RectTransform iconTx;
        private readonly RectTransform textTx;
        private readonly RectTransform placeholderTx;
        private readonly UImage underline;
        private readonly UImage icon;
        private readonly InputField input;
        private readonly Text placeholder;
        private readonly Text textComponent;

        private Sprite? borderSprite;
        private Sprite? borderlessSprite;

        /// <summary>
        /// Event that fires when the edit is completed, e.g. by clicking off the element. Sends this input and its current text.
        /// </summary>
        public event Action<TextInput, string>? TextEditFinished;

        /// <summary>
        /// Event that fires when the text changes, e.g. when a key is entered. Sends this input and its current text.
        /// </summary>
        public event Action<TextInput, string>? TextChanged;

        private void InvokeEditFinished(string text)
        {
            TextEditFinished?.Invoke(this, text);
        }

        private void InvokeTextChanged(string text)
        {
            TextChanged?.Invoke(this, text);
            // don't go through the property - we don't need a rearrange for this, we are just syncing the value from the input
            this.text = text;
        }

        /// <inheritdoc/>
        public GameObject GameObject => underlineObj;

        private TextAnchor textAlignment = TextAnchor.MiddleLeft;
        /// <summary>
        /// The alignment of the text within this element
        /// </summary>
        public HorizontalAlignment TextAlignment
        {
            get => textAlignment switch
            {
                TextAnchor.MiddleLeft => HorizontalAlignment.Left,
                TextAnchor.MiddleCenter => HorizontalAlignment.Center,
                TextAnchor.MiddleRight => HorizontalAlignment.Right,
                _ => throw new NotImplementedException()
            };
            set
            {
                TextAnchor newAlignment = value switch
                {
                    HorizontalAlignment.Left => TextAnchor.MiddleLeft,
                    HorizontalAlignment.Right => TextAnchor.MiddleRight,
                    HorizontalAlignment.Center => TextAnchor.MiddleCenter,
                    _ => throw new NotImplementedException(),
                };
                if (newAlignment != textAlignment)
                {
                    textAlignment = newAlignment;
                    InvalidateMeasure();
                }
            }
        }

        private int fontSize = 12;
        /// <summary>
        /// The font size for the text
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

        private Font font = UI.TrajanNormal;
        /// <summary>
        /// The font to use to draw text
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

        private string placeholderText = "";
        /// <summary>
        /// The placeholder text shown when the input is empty
        /// </summary>
        public string Placeholder
        {
            get => placeholderText;
            set
            {
                if (value != placeholderText)
                {
                    placeholderText = value;
                    InvalidateMeasure();
                }
            }
        }

        private string text = "";
        /// <summary>
        /// The content of the text input
        /// </summary>
        public string Text
        {
            get => text;
            set
            {
                if (value != text)
                {
                    text = value;
                    InvalidateArrange();
                }
            }
        }

        private InputField.ContentType contentType = InputField.ContentType.Standard;
        /// <summary>
        /// The content type of the input. MagicUI does not provide API support for <see cref="InputField.ContentType.Custom"/>
        /// at this time.
        /// </summary>
        public InputField.ContentType ContentType
        {
            get => contentType;
            set
            {
                if (value != contentType)
                {
                    contentType = value;
                    InvalidateArrange();
                }
            }
        }

        private Color iconColor = Color.white;
        /// <summary>
        /// The color of the quill icon
        /// </summary>
        public Color IconColor
        {
            get => iconColor;
            set
            {
                if (value != iconColor)
                {
                    iconColor = value;
                    InvalidateArrange();
                }
            }
        }

        private Color underlineColor = Color.white;
        /// <summary>
        /// The color of the underline
        /// </summary>
        public Color UnderlineColor
        {
            get => underlineColor;
            set
            {
                if (value != underlineColor)
                {
                    underlineColor = value;
                    InvalidateArrange();
                }
            }
        }

        private bool borderless = false;
        /// <summary>
        /// Whether the text input should be displayed in a borderless style.
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

        private Color contentColor = Color.white;
        /// <summary>
        /// The color of the text
        /// </summary>
        public Color ContentColor
        {
            get => contentColor;
            set
            {
                if (value != contentColor)
                {
                    contentColor = value;
                    InvalidateArrange();
                }
            }
        }

        private Color placeholderColor = Color.gray;
        /// <summary>
        /// The color of the placeholder
        /// </summary>
        public Color PlaceholderColor
        {
            get => placeholderColor;
            set
            {
                if (value != placeholderColor)
                {
                    placeholderColor = value;
                    InvalidateArrange();
                }
            }
        }

        private float minWidth = 10;
        /// <summary>
        /// The minimum width of the text input
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

        private float iconSpacing = 5;
        /// <summary>
        /// The spacing between the quill icon
        /// </summary>
        public float IconSpacing
        {
            get => iconSpacing;
            set
            {
                if (value != iconSpacing)
                {
                    iconSpacing = value;
                    InvalidateMeasure();
                }
            }
        }

        private bool enabled = true;
        /// <summary>
        /// Whether the input is enabled
        /// </summary>
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
        /// Creates a text input
        /// </summary>
        /// <param name="onLayout">The layout to draw the input on</param>
        /// <param name="name">The name of the input</param>
        public TextInput(LayoutRoot onLayout, string name = "New TextInput") : base(onLayout, name)
        {
            underlineObj = new GameObject(name + "-Underline");
            underlineObj.AddComponent<CanvasRenderer>();

            Sprite underlineSprite = ChooseBorderStyle();

            Vector2 underlineSize = underlineSprite.textureRect.size;
            Vector2 underlinePos = UI.UnityScreenPosition(new Vector2(0, 0), underlineSize);
            underlineTx = underlineObj.AddComponent<RectTransform>();
            underlineTx.sizeDelta = underlineSize;
            underlineTx.anchorMin = underlinePos;
            underlineTx.anchorMax = underlinePos;

            underline = underlineObj.AddComponent<UImage>();
            underline.sprite = underlineSprite;
            underline.color = underlineColor;
            underline.type = UImage.Type.Sliced;

            underlineObj.transform.SetParent(onLayout.Canvas.transform, false);

            iconObj = new GameObject(name + "-Icon");

            Sprite iconSprite = BuiltInSprites.CreateQuill();

            Vector2 iconSize = iconSprite.textureRect.size;
            Vector2 iconPos = UI.UnityParentRelativePosition(Vector2.zero, iconSize, underlineSize);
            iconTx = iconObj.AddComponent<RectTransform>();
            iconTx.sizeDelta = iconSize;
            iconTx.anchorMin = iconPos;
            iconTx.anchorMax = iconPos;

            icon = iconObj.AddComponent<UImage>();
            icon.sprite = iconSprite;
            icon.color = iconColor;
            iconObj.transform.SetParent(underlineObj.transform, false);

            textObj = new GameObject(name + "-Content");
            Vector2 textCenter = new(0.5f, 0.5f);
            textTx = textObj.AddComponent<RectTransform>();
            textTx.anchorMin = textCenter;
            textTx.anchorMax = textCenter;
            textObj.transform.SetParent(underlineObj.transform, false);

            textComponent = textObj.AddComponent<Text>();
            textComponent.text = text;
            textComponent.font = font;
            textComponent.fontSize = fontSize;
            textComponent.alignment = textAlignment;
            textComponent.color = contentColor;
            textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            textComponent.verticalOverflow = VerticalWrapMode.Overflow;

            placeholderObj = new GameObject(name + "-Placeholder");
            placeholderTx = placeholderObj.AddComponent<RectTransform>();
            placeholderTx.anchorMin = textCenter;
            placeholderTx.anchorMax = textCenter;
            placeholderObj.transform.SetParent(underlineObj.transform, false);

            placeholder = placeholderObj.AddComponent<Text>();
            placeholder.text = placeholderText;
            placeholder.font = font;
            placeholder.fontSize = fontSize;
            placeholder.alignment = textAlignment;
            placeholder.color = placeholderColor;
            placeholder.fontStyle = FontStyle.Italic;
            placeholder.horizontalOverflow = HorizontalWrapMode.Overflow;
            placeholder.verticalOverflow = VerticalWrapMode.Overflow;

            input = underlineObj.AddComponent<InputField>();
            input.interactable = enabled;
            input.targetGraphic = underline;
            input.textComponent = textComponent;
            input.placeholder = placeholder;
            input.onEndEdit.AddListener(InvokeEditFinished);
            input.onValueChanged.AddListener(InvokeTextChanged);

            // hide the GO until the first arrange cycle takes control
            underlineObj.SetActive(false);
        }

        private Sprite ChooseBorderStyle() => Borderless
            ? (borderlessSprite ??= BuiltInSprites.CreateSlicedTransparentRect())
            : (borderSprite ??= BuiltInSprites.CreateSlicedUnderline());

        private Vector2 MeasurePlaceholder()
        {
            TextGenerator textGen = new();
            // have as much space as the screen for the text; otherwise we risk unwanted clipping
            TextGenerationSettings settings = placeholder.GetGenerationSettings(UI.Screen.size);
            // by default, this will inherit the parent canvas's scale factor, which is set to scale with screen space.
            // however, since we're functioning in an unscaled coordinate system we should get the unscaled size to measure correctly.
            settings.scaleFactor = 1;
            // use the staged backing fields instead of the actual current property of the text.
            // in other words, the value it will be after measure/arrange rather than the value it currently is.
            settings.textAnchor = textAlignment;
            settings.font = font;
            settings.fontSize = fontSize;

            float width = textGen.GetPreferredWidth(placeholderText, settings);
            float height = textGen.GetPreferredHeight(placeholderText, settings);
            return new Vector2(width, height);
        }

        /// <inheritdoc/>
        protected override Vector2 MeasureOverride()
        {
            Vector2 size = MeasurePlaceholder();
            size += new Vector2(size.y + IconSpacing, 2);
            size.x = Math.Max(size.x, minWidth);

            return size;
        }

        /// <inheritdoc/>
        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            icon.color = iconColor;

            input.contentType = contentType;
            input.interactable = enabled;

            textComponent.text = text;
            textComponent.alignment = textAlignment;
            textComponent.font = font;
            textComponent.fontSize = fontSize;
            textComponent.color = contentColor;

            placeholder.text = placeholderText;
            placeholder.alignment = textAlignment;
            placeholder.font = font;
            placeholder.fontSize = fontSize;
            placeholder.color = placeholderColor;

            underlineTx.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ContentSize.x);
            underlineTx.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ContentSize.y);

            Vector2 underlinePos = UI.UnityScreenPosition(alignedTopLeftCorner, ContentSize);
            underlineTx.anchorMin = underlinePos;
            underlineTx.anchorMax = underlinePos;

            Vector2 textSize = MeasurePlaceholder();
            Vector2 iconSize = new(textSize.y, textSize.y);
            textSize.x = ContentSize.x - iconSize.x - IconSpacing;

            Vector2 textPos = UI.UnityParentRelativePosition(new Vector2(textSize.y + IconSpacing, 0), textSize, ContentSize);
            textTx.sizeDelta = textSize;
            textTx.anchorMin = textPos;
            textTx.anchorMax = textPos;
            placeholderTx.sizeDelta = textSize;
            placeholderTx.anchorMin = textPos;
            placeholderTx.anchorMax = textPos;

            underline.sprite = ChooseBorderStyle();
            underline.color = underlineColor;

            Vector2 iconPos = UI.UnityParentRelativePosition(Vector2.zero, iconSize, ContentSize);
            iconTx.sizeDelta = iconSize;
            iconTx.anchorMin = iconPos;
            iconTx.anchorMax = iconPos;

            underlineObj.SetActive(IsEffectivelyVisible);
        }

        /// <inheritdoc/>
        protected override void DestroyOverride()
        {
            UnityEngine.Object.Destroy(underlineObj);
            UnityEngine.Object.Destroy(textObj);
            UnityEngine.Object.Destroy(placeholderObj);
        }

        /// <inheritdoc/>
        public Selectable GetSelectable()
        {
            return input;
        }
    }
}
