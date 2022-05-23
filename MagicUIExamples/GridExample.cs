using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;

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

            // more complex usage of grids, mix and match proportional and absolute columns
            new GridLayout(layout, "Complex Grid Example")
            {
                Padding = new Padding(10),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                RowDefinitions =
                {
                    new GridDimension(1, GridUnit.Proportional),
                    new GridDimension(50, GridUnit.AbsoluteMin),
                    new GridDimension(50, GridUnit.AbsoluteMin),
                    new GridDimension(3, GridUnit.Proportional),
                },
                ColumnDefinitions =
                {
                    new GridDimension(1.5f, GridUnit.Proportional),
                    new GridDimension(50, GridUnit.AbsoluteMin),
                    new GridDimension(50, GridUnit.AbsoluteMin),
                    new GridDimension(5, GridUnit.Proportional),
                },
                MinWidth = 400,
                MinHeight = 400,
                Children =
                {
                    new Image(layout, BuiltInSprites.CreateSlicedBorderRect())
                    {
                        Width = 90,
                        Height = 90,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    }.WithProp(GridLayout.Row, 1).WithProp(GridLayout.Column, 1)
                    .WithProp(GridLayout.RowSpan, 2).WithProp(GridLayout.ColumnSpan, 2),
                    new Image(layout, BuiltInSprites.CreateQuill())
                    {
                        Width = 25,
                        Height = 25,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tint = UnityEngine.Color.magenta,
                    }.WithProp(GridLayout.Row, 1).WithProp(GridLayout.Column, 1),
                    new Image(layout, BuiltInSprites.CreateQuill())
                    {
                        Width = 25,
                        Height = 25,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tint = UnityEngine.Color.green,
                    }.WithProp(GridLayout.Row, 2).WithProp(GridLayout.Column, 1),
                    new Image(layout, BuiltInSprites.CreateQuill())
                    {
                        Width = 25,
                        Height = 25,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tint = UnityEngine.Color.blue,
                    }.WithProp(GridLayout.Row, 1).WithProp(GridLayout.Column, 2),
                    new Image(layout, BuiltInSprites.CreateQuill())
                    {
                        Width = 25,
                        Height = 25,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tint = UnityEngine.Color.cyan,
                    }.WithProp(GridLayout.Row, 2).WithProp(GridLayout.Column, 2),
                    new TextObject(layout)
                    {
                        FontSize = 22,
                        Text = "This is in\nthe largest\ncell",
                        TextAlignment = HorizontalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                    }.WithProp(GridLayout.Row, 3).WithProp(GridLayout.Column, 3),
                    new TextObject(layout)
                    {
                        // using only the available space provided by min size this column should be
                        // ~70px wide; (400 - 50 - 50) / 6.5 * 1.5 to respect proportionality.
                        // note that by adding this longer text, the grid becomes much wider than 400px
                        // to retain the proportionality constraint as the cell grows.
                        Text = "Wider than 70px"
                    } // default row and column are 0
                }
            };
        }
    }
}
