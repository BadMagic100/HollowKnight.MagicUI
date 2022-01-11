using MagicUI.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MagicUI.Components
{
    public sealed class TextObject : ArrangableElement, IGameObjectWrapper
    {
        private readonly GameObject textObj;
        private readonly Text textComponent;
        private readonly RectTransform tx;

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
                    tx.sizeDelta = MeasureText();
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
                    tx.sizeDelta = MeasureText();
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
                    tx.sizeDelta = MeasureText();
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
                    tx.sizeDelta = MeasureText();
                    InvalidateMeasure();
                }
            }
        }

        public TextObject(LayoutRoot onLayout, string name = "New TextObject") : base(onLayout, name)
        {
            textObj = new GameObject(name);
            textObj.AddComponent<CanvasRenderer>();

            // note - it is (apparently) critically important to add the transform before the text.
            // Otherwise the text won't show (presumably because it's not transformed properly?)
            Vector2 pos = UI.ScreenPosition(new(0, 0), UI.Screen.size);
            tx = textObj.AddComponent<RectTransform>();
            tx.anchorMin = pos;
            tx.anchorMax = pos;

            textComponent = textObj.AddComponent<Text>();
            textComponent.font = UI.TrajanNormal;
            textComponent.text = "";
            textComponent.fontSize = 12;
            textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            textComponent.verticalOverflow = VerticalWrapMode.Overflow;
            textComponent.alignment = TextAnchor.MiddleLeft;
            tx.sizeDelta = MeasureText();

            textObj.transform.SetParent(onLayout.Canvas.transform, false);
        }

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
            return new Vector2(width, height);
        }

        protected override Vector2 MeasureOverride()
        {
            return MeasureText();
        }

        protected override void ArrangeOverride(Rect availableSpace)
        {
            Vector2 pos = UI.ScreenPosition(GetAlignedTopLeftCorner(availableSpace), DesiredSize);
            tx.anchorMax = pos;
            tx.anchorMin = pos;

            textObj.SetActive(IsEffectivelyVisible);
        }
    }
}
