using MagicUI;
using MagicUI.Components;
using MagicUI.Layouts;
using Modding;

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
                Text = "This is center-aligned text in the\ntop-left"
            };
            new TextObject(layout)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                FontSize = 20,
                Font = UI.TrajanBold,
                Text = "This is a left-aligned text in the\nbottom center with big text"
            };

            MakeStackChildren(new StackLayout(layout)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Horizontal,
                Spacing = 5
            });

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
                FontSize = 15
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