using MagicUI.Core;
using MagicUI.Graphics;
using UnityEngine;
using UnityEngine.UI;
using UImage = UnityEngine.UI.Image;

namespace MagicUI.Components
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

        /// <inheritdoc/>
        public GameObject GameObject => underlineObj;

        private const int FONT_SIZE_DEFAULT = 12;
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
            textComponent.font = UI.TrajanNormal;
            textComponent.fontSize = FONT_SIZE_DEFAULT;
            textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            textComponent.verticalOverflow = VerticalWrapMode.Overflow;
            textComponent.alignment = TextAnchor.MiddleLeft;

            placeholderObj = new GameObject(name + "-Placeholder");
            placeholderTx = placeholderObj.AddComponent<RectTransform>();
            placeholderTx.anchorMin = textCenter;
            placeholderTx.anchorMax = textCenter;
            placeholderObj.transform.SetParent(underlineObj.transform, false);

            placeholder = placeholderObj.AddComponent<Text>();
            placeholder.font = UI.TrajanNormal;
            placeholder.fontSize = FONT_SIZE_DEFAULT;
            placeholder.fontStyle = FontStyle.Italic;
            placeholder.text = "Enter text...";
            placeholder.color = Color.grey;
            placeholder.horizontalOverflow = HorizontalWrapMode.Overflow;
            placeholder.verticalOverflow = VerticalWrapMode.Overflow;
            placeholder.alignment = TextAnchor.MiddleLeft;

            input = underlineObj.AddComponent<InputField>();
            input.targetGraphic = underlineImg;
            input.textComponent = textComponent;
            input.placeholder = placeholder;
        }

        private Vector2 MeasureText()
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
            Vector2 size = MeasureText();
            size += new Vector2(size.y + 5, 2);

            return size;
        }

        /// <inheritdoc/>
        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            underlineTx.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ContentSize.x);
            underlineTx.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ContentSize.y);

            Vector2 underlinePos = UI.UnityScreenPosition(alignedTopLeftCorner, ContentSize);
            underlineTx.anchorMin = underlinePos;
            underlineTx.anchorMax = underlinePos;

            Vector2 textSize = MeasureText();
            Vector2 textPos = UI.UnityParentRelativePosition(new Vector2(textSize.y + 5, 0), textSize, ContentSize);
            textTx.sizeDelta = textSize;
            textTx.anchorMin = textPos;
            textTx.anchorMax = textPos;
            placeholderTx.sizeDelta = textSize;
            placeholderTx.anchorMin = textPos;
            placeholderTx.anchorMax = textPos;

            Vector2 iconSize = new(textSize.y, textSize.y);
            Vector2 iconPos = UI.UnityParentRelativePosition(Vector2.zero, iconSize, ContentSize);
            iconTx.sizeDelta = iconSize;
            iconTx.anchorMin = iconPos;
            iconTx.anchorMax = iconPos;

            underlineObj.SetActive(IsEffectivelyVisible);
        }

        /// <inheritdoc/>
        protected override void DestroyOverride()
        {
            Object.Destroy(underlineObj);
            Object.Destroy(textObj);
            Object.Destroy(placeholderObj);
        }

        /// <inheritdoc/>
        public Selectable GetSelectable()
        {
            return input;
        }
    }
}
