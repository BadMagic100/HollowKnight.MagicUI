using Modding;

namespace MagicUI.Core.Internal
{
    internal class SettingsBoundLogger
    {
        private SimpleLogger logger;

        public SettingsBoundLogger(string name)
        {
            logger = new SimpleLogger(name);
        }

        public void Log(string message)
        {
            if (MagicUIMod.Instance != null && MagicUIMod.Instance.GlobalSettings.LogLayoutInformation)
            {
                switch (MagicUIMod.Instance.GlobalSettings.LogLevel)
                {
                    case LogLevel.Fine:
                        logger.LogFine(message);
                        break;
                    case LogLevel.Debug:
                        logger.LogDebug(message);
                        break;
                    case LogLevel.Info:
                        logger.Log(message);
                        break;
                    case LogLevel.Warn:
                        logger.LogWarn(message);
                        break;
                    case LogLevel.Error:
                        logger.LogError(message);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Log(object message)
        {
            Log(message.ToString());
        }
    }
}
