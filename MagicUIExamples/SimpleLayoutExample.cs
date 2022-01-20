using MagicUI.Core;
using MagicUI.Elements;

namespace MagicUIExamples
{
    // demonstrates basic layout and element creation
    public static class SimpleLayoutExample
    {
        public static void Setup(LayoutRoot layout)
        {
            new TextObject(layout)
            {
                TextAlignment = HorizontalAlignment.Center,
                Text = "This is center-aligned text in the\ntop-left",
                Padding = new(10)
            };
            new TextObject(layout)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                FontSize = 20,
                Font = UI.TrajanBold,
                Text = "This is a left-aligned text in the\nbottom center with big text"
            };
        }
    }
}
