using MagicUI.Core;
using MagicUI.Graphics;
using System;
using UnityEngine;
using UnityEngine.UI;
using UImage = UnityEngine.UI.Image;

namespace MagicUI.Elements
{
    /// <summary>
    /// A text input element
    /// </summary>
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
        private readonly InputField input;
        private readonly Text placeholder;
        private readonly Text textComponent;

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
        }

        /// <inheritdoc/>
        public GameObject GameObject => underlineObj;

        private const TextAnchor TEXT_ALIGN_DEFAULT = TextAnchor.MiddleLeft;
        /// <summary>
        /// The alignment of the text within this element
        /// </summary>
        public HorizontalAlignment TextAlignment
        {
            get => textComponent.alignment switch
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
                if (newAlignment != textComponent.alignment)
                {
                    textComponent.alignment = newAlignment;
                    placeholder.alignment = newAlignment;
                    InvalidateMeasure();
                }
            }
        }

        private const int FONT_SIZE_DEFAULT = 12;
        /// <summary>
        /// The font size for the text
        /// </summary>
        public int FontSize
        {
            get => textComponent.fontSize;
            set 
            {
                if (value != textComponent.fontSize)
                {
                    textComponent.fontSize = value;
                    placeholder.fontSize = value;
                    InvalidateMeasure();
                }
            }
        }

        private readonly Font FONT_DEFAULT = UI.TrajanNormal;
        /// <summary>
        /// The font to use to draw text
        /// </summary>
        public Font Font
        {
            get => textComponent.font;
            set
            {
                if (value != textComponent.font)
                {
                    textComponent.font = value;
                    placeholder.font = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// The placeholder text shown when the input is empty
        /// </summary>
        public string Placeholder
        {
            get => placeholder.text;
            set
            {
                if (value != placeholder.text)
                {
                    placeholder.text = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// The content of the text input
        /// </summary>
        public string Text
        {
            get => input.text;
            set
            {
                if (value != input.text)
                {
                    input.text = value;
                    InvalidateArrange();
                }
            }
        }

        /// <summary>
        /// The content type of the input. MagicUI does not provide API support for <see cref="InputField.ContentType.Custom"/>
        /// at this time.
        /// </summary>
        public InputField.ContentType ContentType
        {
            get => input.contentType;
            set
            {
                if (value != input.contentType)
                {
                    input.contentType = value;
                    InvalidateArrange();
                }
            }
        }

        /// <summary>
        /// The color of the text
        /// </summary>
        public Color ContentColor
        {
            get => textComponent.color;
            set
            {
                if (value != textComponent.color)
                {
                    textComponent.color = value;
                    InvalidateArrange();
                }
            }
        }

        /// <summary>
        /// The color of the placeholder
        /// </summary>
        public Color PlaceholderColor
        {
            get => placeholder.color;
            set
            {
                if (value != placeholder.color)
                {
                    placeholder.color = value;
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

        /// <summary>
        /// Creates a text input
        /// </summary>
        /// <param name="onLayout">The layout to draw the input on</param>
        /// <param name="name">The name of the input</param>
        public TextInput(LayoutRoot onLayout, string name = "New TextInput") : base(onLayout, name)
        {
            underlineObj = new GameObject(name + "-Underline");
            underlineObj.AddComponent<CanvasRenderer>();

            Sprite underlineSprite = BuiltInSprites.CreateSlicedUnderline();

            Vector2 underlineSize = underlineSprite.textureRect.size;
            Vector2 underlinePos = UI.UnityScreenPosition(new Vector2(0, 0), underlineSize);
            underlineTx = underlineObj.AddComponent<RectTransform>();
            underlineTx.sizeDelta = underlineSize;
            underlineTx.anchorMin = underlinePos;
            underlineTx.anchorMax = underlinePos;

            UImage underlineImg = underlineObj.AddComponent<UImage>();
            underlineImg.sprite = underlineSprite;
            underlineImg.type = UImage.Type.Sliced;

            underlineObj.transform.SetParent(onLayout.Canvas.transform, false);

            iconObj = new GameObject(name + "-Icon");

            Sprite iconSprite = BuiltInSprites.CreateQuill();

            Vector2 iconSize = iconSprite.textureRect.size;
            Vector2 iconPos = UI.UnityParentRelativePosition(Vector2.zero, iconSize, underlineSize);
            iconTx = iconObj.AddComponent<RectTransform>();
            iconTx.sizeDelta = iconSize;
            iconTx.anchorMin = iconPos;
            iconTx.anchorMax = iconPos;

            UImage iconImage = iconObj.AddComponent<UImage>();
            iconImage.sprite = iconSprite;
            iconObj.transform.SetParent(underlineObj.transform, false);

            textObj = new GameObject(name + "-Content");
            Vector2 textCenter = new(0.5f, 0.5f);
            textTx = textObj.AddComponent<RectTransform>();
            textTx.anchorMin = textCenter;
            textTx.anchorMax = textCenter;
            textObj.transform.SetParent(underlineObj.transform, false);

            textComponent = textObj.AddComponent<Text>();
            textComponent.font = FONT_DEFAULT;
            textComponent.fontSize = FONT_SIZE_DEFAULT;
            textComponent.alignment = TEXT_ALIGN_DEFAULT;
            textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            textComponent.verticalOverflow = VerticalWrapMode.Overflow;

            placeholderObj = new GameObject(name + "-Placeholder");
            placeholderTx = placeholderObj.AddComponent<RectTransform>();
            placeholderTx.anchorMin = textCenter;
            placeholderTx.anchorMax = textCenter;
            placeholderObj.transform.SetParent(underlineObj.transform, false);

            placeholder = placeholderObj.AddComponent<Text>();
            placeholder.font = FONT_DEFAULT;
            placeholder.fontSize = FONT_SIZE_DEFAULT;
            placeholder.alignment = TEXT_ALIGN_DEFAULT;
            placeholder.horizontalOverflow = HorizontalWrapMode.Overflow;
            placeholder.verticalOverflow = VerticalWrapMode.Overflow;
            placeholder.color = Color.grey;
            placeholder.fontStyle = FontStyle.Italic;

            input = underlineObj.AddComponent<InputField>();
            input.targetGraphic = underlineImg;
            input.textComponent = textComponent;
            input.placeholder = placeholder;
            input.onEndEdit.AddListener(InvokeEditFinished);
            input.onValueChanged.AddListener(InvokeTextChanged);
        }

        private Vector2 MeasurePlaceholder()
        {
            TextGenerator textGen = new();
            // have as much space as the screen for the text; otherwise we risk unwanted clipping
            TextGenerationSettings settings = placeholder.GetGenerationSettings(UI.Screen.size);
            // by default, this will inherit the parent canvas's scale factor, which is set to scale with screen space.
            // however, since we're functioning in an unscaled coordinate system we should get the unscaled size to measure correctly.
            settings.scaleFactor = 1;
            float width = textGen.GetPreferredWidth(placeholder.text, settings);
            float height = textGen.GetPreferredHeight(placeholder.text, settings);
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
            // hacky way to deal with the overline issue on half-pixel positions:
            // just make sure we don't ever go on a half-pixel - specifically an issue for the underline image
            // since it's not symmetric; the problem is not as noticable on other images
            alignedTopLeftCorner.y = Mathf.Round(alignedTopLeftCorner.y);

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
