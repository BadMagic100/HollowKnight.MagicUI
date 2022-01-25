using System.IO;
using System.Runtime.CompilerServices;

namespace MagicUI.Core.Internal
{
    internal static class LogHelper
    {
        public static SettingsBoundLogger GetLogger([CallerFilePath] string callingFile = "")
        {
            return new SettingsBoundLogger($"MagicUI.{Path.GetFileNameWithoutExtension(callingFile)}");
        }
    }
}
