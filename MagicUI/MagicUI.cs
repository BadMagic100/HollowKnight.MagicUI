using Modding;
using System;

namespace MagicUI
{
    /// <summary>
    /// Base mod class
    /// </summary>
    public class MagicUI : Mod, IGlobalSettings<MagicUIGlobalSettings>
    {
        private static MagicUI? instance;
        internal static MagicUI Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException($"{nameof(MagicUI)} was never initialized!");
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

        /// <inheritdoc/>
        public override void Initialize()
        {
            Instance = this;
        }

        /// <inheritdoc/>
        public void OnLoadGlobal(MagicUIGlobalSettings s) => GlobalSettings = s;

        /// <inheritdoc/>
        public MagicUIGlobalSettings OnSaveGlobal() => GlobalSettings;
    }
}