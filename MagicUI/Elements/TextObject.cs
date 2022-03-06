using MagicUI.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MagicUI.Elements
{
    /// <summary>
    /// A text display element
    /// </summary>
    public sealed class TextObject : ArrangableElement, IGameObjectWrapper
    {
        private readonly GameObject textObj;
        private readonly Text textComponent;
        private readonly RectTransform tx;

        /// <inheritdoc/>
        public GameObject GameObject { get => textObj; }

        /// <summary>
        /// The text of this element
        /// </summary>
        public string Text
        {
            get => textComponent.text;
            set
            {
                if (value != textComponent.text)
                {
                    textComponent.text = value;
                    InvalidateMeasure();
                }
            }
        }

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
                    InvalidateMeasure();
                }
            }
        }

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
                    InvalidateMeasure();
                }
            }
        }

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
                    InvalidateMeasure();
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

        private float maxWidth = float.PositiveInfinity;
        /// <summary>
        /// The max width of the text. If text exceeds this width, it will roll to the next line.
        /// </summary>
        public float MaxWidth
        {
            get => maxWidth;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Value can't be negative", nameof(MaxWidth));
                }
                if (value != maxWidth)
                {
                    maxWidth = value;
                    InvalidateMeasure();
                }
            }
        }

        private float maxHeight = float.PositiveInfinity;
        /// <summary>
        /// The max height of the text. If text exceeds this height, it will be clipped.
        /// </summary>
        public float MaxHeight
        {
            get => maxHeight;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Value can't be negative", nameof(MaxHeight));
                }
                if (value != maxHeight)
                {
                    maxHeight = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Creates a text object
        /// </summary>
        /// <param name="onLayout">The layout to draw the text object on</param>
        /// <param name="name">The name of the text object</param>
        public TextObject(LayoutRoot onLayout, string name = "New TextObject") : base(onLayout, name)
        {
            textObj = new GameObject(name);
            textObj.AddComponent<CanvasRenderer>();

            // note - it is (apparently) critically important to add the transform before the text.
            // Otherwise the text won't show (presumably because it's not transformed properly?)
            Vector2 pos = UI.UnityScreenPosition(new(0, 0), UI.Screen.size);
            tx = textObj.AddComponent<RectTransform>();
            tx.anchorMin = pos;
            tx.anchorMax = pos;

            textComponent = textObj.AddComponent<Text>();
            textComponent.font = UI.TrajanNormal;
            textComponent.text = "";
            textComponent.fontSize = 12;
            textComponent.alignment = TextAnchor.MiddleLeft;
            tx.sizeDelta = MeasureText();

            textObj.transform.SetParent(onLayout.Canvas.transform, false);
        }

        private Vector2 MeasureText()
        {
            TextGenerator textGen = new();
            float availableWidth = Mathf.Min(UI.Screen.width, MaxWidth);
            float availableHeight = Mathf.Min(UI.Screen.height, MaxHeight);
            TextGenerationSettings settings = textComponent.GetGenerationSettings(new Vector2(availableWidth, availableHeight));
            // by default, this will inherit the parent canvas's scale factor, which is set to scale with screen space.
            // however, since we're functioning in an unscaled coordinate system we should get the unscaled size to measure correctly.
            settings.scaleFactor = 1;
            float preferredWidth = textGen.GetPreferredWidth(textComponent.text, settings);
            float preferredHeight = textGen.GetPreferredHeight(textComponent.text, settings);
            float width = Mathf.Min(preferredWidth, MaxWidth);
            float height = Mathf.Min(preferredHeight, MaxHeight);
            return new Vector2(Mathf.Ceil(width) + 1, Mathf.Ceil(height) + 1);
        }

        /// <inheritdoc/>
        protected override Vector2 MeasureOverride()
        {
            return MeasureText();
        }

        /// <inheritdoc/>
        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            // todo: this isn't parent-relative which could cause some problems using this as a building block
            Vector2 pos = UI.UnityScreenPosition(alignedTopLeftCorner, ContentSize);
            tx.sizeDelta = ContentSize;
            tx.anchorMax = pos;
            tx.anchorMin = pos;

            textObj.SetActive(IsEffectivelyVisible);
        }

        /// <inheritdoc/>
        protected override void DestroyOverride()
        {
            UnityEngine.Object.Destroy(textObj);
        }
    }
}
