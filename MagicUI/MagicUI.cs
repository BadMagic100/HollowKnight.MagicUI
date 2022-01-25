using Modding;

namespace MagicUI
{
    /// <summary>
    /// Base mod class
    /// </summary>
    public class MagicUI : Mod, IGlobalSettings<MagicUIGlobalSettings>
    {
        internal static MagicUI? Instance { get; private set; }

        /// <summary>
        /// Global settings for the mod
        /// </summary>
        public MagicUIGlobalSettings GlobalSettings { get; private set; } = new();

        /// <inheritdoc/>
        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        /// <inheritdoc/>
        public override void Initialize()
        {
            Instance = this;
        }

        /// <inheritdoc/>
        public void OnLoadGlobal(MagicUIGlobalSettings s)
        {
            GlobalSettings = s;
        }

        /// <inheritdoc/>
        public MagicUIGlobalSettings OnSaveGlobal()
        {
            return GlobalSettings;
        }
    }
}