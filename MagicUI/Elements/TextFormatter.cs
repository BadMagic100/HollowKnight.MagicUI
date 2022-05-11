using MagicUI.Core;
using System;
using UnityEngine;

namespace MagicUI.Elements
{
    /// <summary>
    /// An element that formats an underlying <see cref="TextObject"/> according to a formatter for an immutable data type
    /// </summary>
    /// <typeparam name="T">The type of data to format</typeparam>
    public sealed class TextFormatter<T> : ArrangableElement
    {
        private Func<T, string> formatter;

        private T data;
        /// <summary>
        /// The data to be formatted. This should be treated as immutable data to update properly. In other words,
        /// <code>Data.X = Y</code> will not trigger an update, but <code>Data = new(...)</code> will.
        /// </summary>
        public T Data
        {
            get => data;
            set
            {
                if (value?.Equals(data) == false)
                {
                    data = value;
                    if (Text != null)
                    {
                        Text.Text = formatter.Invoke(value) ?? string.Empty;
                    }
                }
            }
        }

        private TextObject? text;
        /// <summary>
        /// The underlying <see cref="TextObject"/> to be formatted. You control all other properties of the TextObject directly,
        /// this element will control the <see cref="TextObject.Text"/> property via <see cref="Data"/>
        /// </summary>
        public TextObject? Text
        {
            get => text;
            set
            {
                text?.Destroy();
                text = value;
                if (text != null)
                {
                    text.LogicalParent = this;
                    text.Text = formatter.Invoke(data) ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// Creates a text formatter
        /// </summary>
        /// <param name="onLayout">The layout to draw the text formatter on</param>
        /// <param name="initialValue">The initial data value</param>
        /// <param name="formatter">A function to use to format text</param>
        /// <param name="name">The name of the text formatter</param>
        public TextFormatter(LayoutRoot onLayout, T initialValue, Func<T, string> formatter, string name = "New TextFormatter") 
            : base(onLayout, name)
        {
            data = initialValue;
            this.formatter = formatter;
        }

        /// <inheritdoc/>
        protected override Vector2 MeasureOverride()
        {
            return Text?.Measure() ?? Vector2.zero;
        }

        /// <inheritdoc/>
        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            Text?.Arrange(new Rect(alignedTopLeftCorner, Text.EffectiveSize));
        }
        
        /// <inheritdoc/>
        protected override void DestroyOverride()
        {
            Text?.Destroy();
            Text = null;
        }
    }
}
