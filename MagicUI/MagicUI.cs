using Modding;

namespace MagicUI
{
    public class MagicUI : Mod
    {
        internal static MagicUI? Instance { get; private set; }

        public override string GetVersion()
        {
            return GetType().Assembly.GetName().Version.ToString();
        }

        public MagicUI() : base()
        {
            Instance = this;
        }

        public override void Initialize()
        {
            Log("Initializing");

            //todo: make the magic happen (honestly may not even need stuff here lmao)

            Log("Initialized");
        }
    }
}