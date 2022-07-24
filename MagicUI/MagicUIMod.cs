using Modding;
using System;

namespace MagicUI
{
    /// <summary>
    /// Base mod class
    /// </summary>
    public class MagicUIMod : Mod, IGlobalSettings<MagicUIGlobalSettings>
    {
        private static MagicUIMod? instance;
        internal static MagicUIMod Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException($"{nameof(MagicUIMod)} was never initialized!");
                }
                return instance;
            }
            private set => instance = value;
        }

        /// <summary>
        /// Global settings for the mod
        /// </summary>
        public MagicUIGlobalSettings GlobalSettings { get; private set; } = new();

        /// <inheritdoc/>
        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        /// <summary>
        /// Instantiates the mod instance
        /// </summary>
        public MagicUIMod() : base("MagicUI")
        {
            Instance = this;
        }

        /// <inheritdoc/>
        public void OnLoadGlobal(MagicUIGlobalSettings s) => GlobalSettings = s;

        /// <inheritdoc/>
        public MagicUIGlobalSettings OnSaveGlobal() => GlobalSettings;
    }
}