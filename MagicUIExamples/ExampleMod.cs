using MagicUI.Core;
using Modding;
using System.Collections.Generic;

namespace MagicUIExamples
{
    public class MagicUIExamples : Mod, ITogglableMod, IMenuMod
    {
        internal static MagicUIExamples? Instance;

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public bool ToggleButtonInsideMenu => true;

        private LayoutRoot? layout;

        private bool debugBoundStatePersistent = false;
        private void SetDebugBoundState(bool value)
        {
            debugBoundStatePersistent = value;
            if (layout != null)
            {
                layout.RenderDebugLayoutBounds = value;
            }
        }

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
                layout = new(true, "Persistent layout");
                layout.RenderDebugLayoutBounds = debugBoundStatePersistent;

                // comment out examples as needed to see only the specific ones you want
                SimpleLayoutExample.Setup(layout);
                InteractivityAndDynamicSizingExample.Setup(layout);
                ComplexLayoutExample.Setup(layout);
                NonPersistentLayoutExample.Setup(layout);
            }

            orig(self);
        }

        public void Unload()
        {
            On.HeroController.Awake -= OnSaveOpened;
            layout?.Destroy();
            layout = null;
        }

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            List<IMenuMod.MenuEntry> items = new()
            {
                new IMenuMod.MenuEntry {
                    Name = "Show Debug Bounds",
                    Values = new string[] {"On", "Off"},
                    Saver = opt => SetDebugBoundState(opt == 0),
                    Loader = () => debugBoundStatePersistent ? 0 : 1
                }
            };
            if (toggleButtonEntry != null)
            {
                items.Insert(0, (IMenuMod.MenuEntry)toggleButtonEntry);
            }
            return items;
        }
    }
}