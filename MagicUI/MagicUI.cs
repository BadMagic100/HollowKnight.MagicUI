using Modding;

namespace MagicUI
{
    public class MagicUI : Mod
    {
        internal static MagicUI? Instance { get; private set; }

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public override void Initialize()
        {
            Instance = this;
        }
    }
}