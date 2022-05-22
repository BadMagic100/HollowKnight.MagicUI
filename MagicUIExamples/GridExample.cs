using MagicUI.Core;
using MagicUI.Elements;

namespace MagicUIExamples
{
    // An example that demonstrates the usage of the GridLayout, the most flexible layout option in MagicUI.
    // recommended to show this example on its own to avoid screen clutter
    public static class GridExample
    {
        public static void Setup(LayoutRoot layout)
        {
            // a grid demonstrating basic proportional control
            new GridLayout(layout, "Proportional Grid Example 1")
            {
                MinWidth = 1920, // divide the entire screen's width
                ColumnDefinitions =
                {
                    new GridDimension(2, GridUnit.Proportional),
                    new GridDimension(1, GridUnit.Proportional),
                    new GridDimension(1, GridUnit.Proportional),
                },
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Children =
                {
                    new TextObject(layout)
                    {
                        FontSize = 15,
                        Text = "This text uses 1 proportional column that is twice as large as the others",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    }.WithProp(GridLayout.Column, 0), // technically you don't need to set this
                    new TextObject(layout)
                    {
                        FontSize = 20,
                        Text = "This text spans only 1 proportional column\nand drives the height of the grid\nbecause it is tallest.",
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextAlignment = HorizontalAlignment.Center,
                    }.WithProp(GridLayout.Column, 1),
                    new TextObject(layout)
                    {
                        FontSize = 15,
                        Text = "Hope you're having a nice day :)",
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    }.WithProp(GridLayout.Column, 2),
                }
            };

            // another way to do the same thing, demonstrates columnspans
            new GridLayout(layout, "Proportional Grid Example 2")
            {
                MinWidth = 1920, // divide the entire screen's width
                ColumnDefinitions =
                {
                    new GridDimension(1, GridUnit.Proportional),
                    new GridDimension(1, GridUnit.Proportional),
                    new GridDimension(1, GridUnit.Proportional),
                    new GridDimension(1, GridUnit.Proportional),
                },
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Children =
                {
                    new TextObject(layout)
                    {
                        FontSize = 15,
                        Text = "This spans 2 proportional columns the same width as the others",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    }.WithProp(GridLayout.Column, 0).WithProp(GridLayout.ColumnSpan, 2),
                    new TextObject(layout)
                    {
                        FontSize = 20,
                        Text = "This text spans only 1 proportional column\nand drives the height of the grid\nbecause it is tallest.",
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextAlignment = HorizontalAlignment.Center,
                    }.WithProp(GridLayout.Column, 2),
                    new TextObject(layout)
                    {
                        FontSize = 15,
                        Text = "Hope you're having a nice day :)",
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    }.WithProp(GridLayout.Column, 3)
                }
            };
        }
    }
}
