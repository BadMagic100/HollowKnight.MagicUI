using MagicUI.Core;
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
            if (layout == null)
            {
                layout = new(true, "Persistent layout");
                layout.RenderDebugLayoutBounds = true;

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
    }
}