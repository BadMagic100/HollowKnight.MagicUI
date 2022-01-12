using System;

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

        public Padding(float uniform)
        {
            ValidateComponentPositive(uniform, nameof(uniform));

            Left = uniform;
            Top = uniform;
            Right = uniform;
            Bottom = uniform;
        }

        public Padding(float horizontal, float vertical)
        {
            ValidateComponentPositive(horizontal, nameof(horizontal));
            ValidateComponentPositive(vertical, nameof(vertical));

            Left = horizontal;
            Right = horizontal;
            Top = vertical;
            Bottom = vertical;
        }

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
    }
}
