using MagicUI.Core;
using MagicUI.Elements;
using System;
using UnityEngine;

namespace MagicUIExamples
{
    // A more complex layout example demonstrating the more advanced features of the layout system
    public static class ComplexLayoutExample
    {
        public static void Setup(LayoutRoot layout)
        {
            // center-right stack layout demonstrating visibility and padding
            StackLayout visibilityTest = new(layout, "Visibility Panel")
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 10,
                Padding = new Padding(0, 500, 15, 0)
            };
            // create a horizontal stack layout of children; make each second child have a different visibility
            foreach (Visibility viz in Enum.GetValues(typeof(Visibility)))
            {
                StackLayout hzTextStack = new(layout)
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 5
                };
                MakeStackChildren(hzTextStack);
                hzTextStack.Children[1].Visibility = viz;
                visibilityTest.Children.Add(hzTextStack);
            }

            // ctrl-alt-v swaps the visibility of each second child
            layout.ListenForHotkey(KeyCode.V, () =>
            {
                foreach (Layout stack in visibilityTest.Children)
                {
                    if (stack.Children.Count > 1)
                    {
                        Visibility currentViz = stack.Children[1].Visibility;
                        stack.Children[1].Visibility = currentViz switch
                        {
                            Visibility.Visible => Visibility.Hidden,
                            Visibility.Hidden => Visibility.Collapsed,
                            Visibility.Collapsed => Visibility.Visible,
                            _ => throw new NotImplementedException(),
                        };
                    }
                }
            }, ModifierKeys.Ctrl | ModifierKeys.Alt);

            // various ways to delete elements
            layout.ListenForHotkey(KeyCode.Delete, () =>
            {
                // children.removeat(0) and children[0].destroy do effectively the same thing, it's just a matter of preference
                (visibilityTest.Children[0] as Layout)?.Children.RemoveAt(0);
                (visibilityTest.Children[1] as Layout)?.Children[0].Destroy();
                (visibilityTest.Children[2] as Layout)?.Children.Clear();
            });
        }

        // helper function to add some example children to a layout
        private static void MakeStackChildren(Layout stack)
        {
            stack.Children.Add(new TextObject(stack.LayoutRoot)
            {
                Text = "top left text",
                FontSize = 15,
                Padding = new Padding(100, 0)
            });
            stack.Children.Add(new TextObject(stack.LayoutRoot)
            {
                Text = "center text",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 15
            });
            stack.Children.Add(new TextObject(stack.LayoutRoot)
            {
                Text = "bottom right\ntext",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                FontSize = 20
            });
        }
    }
}
