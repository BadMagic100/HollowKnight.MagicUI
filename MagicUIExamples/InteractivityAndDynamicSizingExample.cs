using MagicUI.Core;
using MagicUI.Elements;
using UnityEngine;

namespace MagicUIExamples
{
    // example demonstrating the use of interactive components and dynamic layout behaviors
    public static class InteractivityAndDynamicSizingExample
    {
        private static LayoutRoot? layout;

        public static void Setup(LayoutRoot inputLayout)
        {
            layout = inputLayout;

            StackLayout interactionLayout = new(layout)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 10,
                Orientation = Orientation.Vertical,
                Padding = new(20)
            };
            Button testButton = new(layout)
            {
                MinWidth = 100
            };
            testButton.Click += ClearContent;
            testButton.OnHover += (_) => MagicUIExamples.Instance!.Log("Hovered!");
            testButton.OnUnhover += (_) => MagicUIExamples.Instance!.Log("Unhovered!");

            Button toggleButton = new(layout)
            {
                Content = "Toggle is on",
                BorderColor = Color.blue,
                ContentColor = Color.green,
                Margin = 20
            };
            toggleButton.Click += ToggleButtonState;
            
            TextInput input = new(layout, "TestInput")
            {
                FontSize = 22,
                Placeholder = "Enter text here...",
            };
            Button inputButton = new(layout)
            {
                Content = "Click to capture input",
                FontSize = 15,
                Margin = 20
            };
            inputButton.Click += CaptureInputContentForButton;

            interactionLayout.Children.Add(testButton);
            interactionLayout.Children.Add(toggleButton);
            interactionLayout.Children.Add(input);
            interactionLayout.Children.Add(inputButton);

            layout.ListenForHotkey(KeyCode.A, () => testButton.Content += "A");
            layout.ListenForHotkey(KeyCode.Return, () => testButton.Content += "\n");
            layout.ListenForHotkey(KeyCode.Equals, () => testButton.Margin += 5, ModifierKeys.Shift);
        }

        private static void CaptureInputContentForButton(Button sender)
        {
            TextInput? input = layout?.GetElement<TextInput>("TestInput");
            if (input != null)
            {
                sender.Content = input.Text;
            }
        }

        private static void ToggleButtonState(Button sender)
        {
            // realistally you would make a custom element to handle the toggle logic rather than managing state this way
            bool currentState = sender.ContentColor == Color.green;
            TextInput? input = layout?.GetElement<TextInput>("TestInput");
            if (input != null)
            {
                input.Borderless = currentState;
            }

            if (currentState)
            {
                sender.ContentColor = Color.red;
                sender.Content = "Toggle is off";
            }
            else
            {
                sender.ContentColor = Color.green;
                sender.Content = "Toggle is on";
            }
        }

        private static void ClearContent(Button sender)
        {
            sender.Content = "";
        }
    }
}
