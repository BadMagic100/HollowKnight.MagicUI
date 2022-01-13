using System;
using UnityEngine;

namespace MagicUI.Core
{
    public struct Padding
    {
        public static Padding Zero = new(0);

        public float Left { get; private set; }
        public float Top { get; private set; }
        public float Right { get; private set; }
        public float Bottom { get; private set; }

        public float AddedWidth { get => Left + Right; }

        public float AddedHeight { get => Top + Bottom; }

        private static void ValidateComponentPositive(float value, string name)
        {
            if (value < 0)
            {
                throw new ArgumentException("Margin component must be positive", name);
            }
        }

        /// <summary>
        /// Creates a uniform padding
        /// </summary>
        /// <param name="uniform">The padding to use on all sides</param>
        public Padding(float uniform)
        {
            ValidateComponentPositive(uniform, nameof(uniform));

            Left = uniform;
            Top = uniform;
            Right = uniform;
            Bottom = uniform;
        }

        /// <summary>
        /// Creates a padding with the same top/bottom and left/right components
        /// </summary>
        /// <param name="horizontal">The padding to use on the left and right sides</param>
        /// <param name="vertical">The padding to use on the top and bottom sides</param>
        public Padding(float horizontal, float vertical)
        {
            ValidateComponentPositive(horizontal, nameof(horizontal));
            ValidateComponentPositive(vertical, nameof(vertical));

            Left = horizontal;
            Right = horizontal;
            Top = vertical;
            Bottom = vertical;
        }

        /// <summary>
        /// Creates a padding with 4 custom sides
        /// </summary>
        /// <param name="left">The padding to use on the left side</param>
        /// <param name="top">The padding to use on the top side</param>
        /// <param name="right">The padding to use on the right side</param>
        /// <param name="bottom">The padding to use on the bottom side</param>
        public Padding(float left, float top, float right, float bottom)
        {
            ValidateComponentPositive(left, nameof(left));
            ValidateComponentPositive(top, nameof(top));
            ValidateComponentPositive(right, nameof(right));
            ValidateComponentPositive(bottom, nameof(bottom));

            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// Insets this padding by another padding. In other words, makes the padding smaller by the size of the other padding on each side, stopping at 0.
        /// </summary>
        /// <param name="other">The padding to inset by</param>
        /// <returns>A modified padding</returns>
        public Padding Inset(Padding other)
        {
            return new Padding(
                Mathf.Max(Left - other.Left, 0),
                Mathf.Max(Top - other.Top, 0), 
                Mathf.Max(Right - other.Right, 0), 
                Mathf.Max(Bottom - other.Bottom, 0)
            );
        }

        /// <summary>
        /// Outsets this padding by another padding. In other words, makes the padding larger by the size of the other padding on each side.
        /// </summary>
        /// <param name="other">The padding to outset by</param>
        /// <returns>A modified padding</returns>
        public Padding Outset(Padding other)
        {
            return new Padding(
                Left + other.Left, 
                Top + other.Top, 
                Right + other.Right, 
                Bottom + other.Bottom
            );
        }
    }
}
