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
            if (layout == null)
            {
                layout = new(true, false, "Persistent layout");

                StackLayout buttonLayout = new(layout)
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
                testButton.Click += TestButton_Click;
                Button toggleButton = new(layout)
                {
                    Content = "Toggle is on",
                    ContentColor = Color.green,
                    Margin = 20
                };
                toggleButton.Click += ToggleButton_Click;
                buttonLayout.Children.Add(testButton);
                buttonLayout.Children.Add(toggleButton);

                layout.ListenForHotkey(KeyCode.A, () => testButton.Content += "A");
                layout.ListenForHotkey(KeyCode.Return, () => testButton.Content += "\n");
                layout.ListenForHotkey(KeyCode.Equals, () => testButton.Margin += 5, ModifierKeys.Shift);

                Layout dynamicGrid = new DynamicUniformGrid(layout)
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalSpacing = 10,
                    HorizontalSpacing = 5,
                    ChildrenBeforeRollover = 2,
                    Orientation = Orientation.Vertical
                };
                dynamicGrid.Children.Add(new TextObject(layout)
                {
                    Text = "Test1"
                });
                dynamicGrid.Children.Add(new TextObject(layout)
                {
                    Text = "Test2"
                });
                dynamicGrid.Children.Add(new TextObject(layout)
                {
                    Text = "Beeeeeeg"
                });

                new TextObject(layout)
                {
                    TextAlignment = HorizontalAlignment.Center,
                    Text = "This is center-aligned text in the\ntop-left",
                    Padding = new(10)
                };
                ArrangableElement bottomText = new TextObject(layout)
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
                    Padding = new Padding(0, 500, 15, 0)
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

                MakeStackChildren(new StackLayout(layout)
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Spacing = 2
                });

                layout.ListenForHotkey(KeyCode.Delete, () =>
                {
                    // children.removeat(0) and children[0].destroy do effectively the same thing, it's just a matter of preference
                    (visibilityTest.Children[0] as Layout)?.Children.RemoveAt(0);
                    (visibilityTest.Children[1] as Layout)?.Children[0].Destroy();
                    (visibilityTest.Children[2] as Layout)?.Children.Clear();
                    bottomText.Destroy();
                });

                layout.ListenForHotkey(KeyCode.N, () =>
                {
                    // this particular hook happens before the scene actually begins - so be careful when you
                    // create non-persistent stuff as it may go away before you ever see it
                    LayoutRoot noPersist = new(false, false);
                    new TextObject(noPersist)
                    {
                        Text = "This text will go away when you leave the scene",
                        FontSize = 20,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                }, ModifierKeys.Ctrl);
            }

            orig(self);
        }

        private void ToggleButton_Click(Button sender)
        {
            if (sender.ContentColor == Color.green)
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

        private void TestButton_Click(Button sender)
        {
            sender.Content = "hivescream.";
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