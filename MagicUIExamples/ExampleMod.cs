using MagicUI.Components;
using MagicUI.Core;
using Modding;
using System;
using UnityEngine;

namespace MagicUIExamples
{
    public class MagicUIExamples : Mod, ITogglableMod
    {
        internal static MagicUIExamples? Instance;

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        private LayoutRoot? layout;

        public override void Initialize()
        {
            Log("Initializing");

            Instance = this;

            On.HeroController.Awake += OnSaveOpened;

            Log("Initialized");
        }

        private void OnSaveOpened(On.HeroController.orig_Awake orig, HeroController self)
        {
            layout = new(true, "Persistent layout");

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

            StackLayout visibilityTest = new(layout, "Visibility Panel")
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 10,
                Padding = new Padding(0, 500, 10, 0)
            };
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
            layout.ListenForHotkey(KeyCode.V, () =>
            {
                foreach (Layout stack in visibilityTest.Children)
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
            }, ModifierKeys.Ctrl | ModifierKeys.Alt);

            MakeStackChildren(new StackLayout(layout)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Spacing = 2
            });

            orig(self);
        }

        private void MakeStackChildren(Layout stack)
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

        public void Unload()
        {
            On.HeroController.Awake -= OnSaveOpened;
            layout?.Destroy();
            layout = null;
        }
    }
}