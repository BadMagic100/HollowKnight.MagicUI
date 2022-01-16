using MagicUI.Behaviours;
using MagicUI.Core;
using MagicUI.Graphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UButton = UnityEngine.UI.Button;
using UImage = UnityEngine.UI.Image;

namespace MagicUI.Components
{
    public sealed class Button : ArrangableElement, IGameObjectWrapper
    {
        private readonly GameObject imgObj;
        private readonly GameObject textObj;
        private readonly RectTransform imgTx;
        private readonly RectTransform textTx;
        private readonly UButton btn;
        private readonly Text textComponent;

        public event UnityAction<Button>? Click;

        private void InvokeClick()
        {
            Click?.Invoke(this);
        }

        private float margin = 10;
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

        public string Content
        {
            get => textComponent.text;
            set
            {
                if (textComponent.text != value)
                {
                    textComponent.text = value;
                    textTx.sizeDelta = MeasureText();
                    InvalidateMeasure();
                }
            }
        }

        private float minWidth = 10;
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

        public Button(LayoutRoot onLayout, string name = "New Button") : base(onLayout, name)
        {
            //todo: this should get rebuilt as a composition of a TextObject and an image - but the API support for that is not amazing right
            //      now, so it'll happen in v2.
            imgObj = new GameObject(name + "Border");
            imgObj.AddComponent<CanvasRenderer>();

            Sprite sprite = BuiltInSprites.CreateSlicedBorderRect();

            Vector2 size = sprite.textureRect.size;
            Vector2 pos = UI.UnityScreenPosition(new Vector2(0, 0), size);
            imgTx = imgObj.AddComponent<RectTransform>();
            imgTx.sizeDelta = size;
            imgTx.anchorMin = pos;
            imgTx.anchorMax = pos;

            UImage img = imgObj.AddComponent<UImage>();
            img.sprite = sprite;
            img.type = UImage.Type.Sliced;
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
            textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            textComponent.verticalOverflow = VerticalWrapMode.Overflow;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textTx.sizeDelta = MeasureText();

            textObj.transform.SetParent(imgObj.transform, false);
        }

        /// <inheritdoc/>
        public GameObject GameObject => imgObj;

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

            Vector2 pos = UI.UnityScreenPosition(alignedTopLeftCorner, ContentSize);
            imgTx.anchorMin = pos;
            imgTx.anchorMax = pos;

            imgObj.SetActive(IsEffectivelyVisible);
        }

        /// <inheritdoc/>
        protected override void DestroyOverride()
        {
            Object.Destroy(imgObj);
            Object.Destroy(textObj);
        }
    }
}
