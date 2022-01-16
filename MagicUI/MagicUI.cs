using Modding;

namespace MagicUI
{
    /// <summary>
    /// Base mod class
    /// </summary>
    public class MagicUI : Mod
    {
        internal static MagicUI? Instance { get; private set; }

        /// <inheritdoc/>
        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        /// <inheritdoc/>
        public override void Initialize()
        {
            Instance = this;
        }
    }
}