using Modding;
using System.IO;
using System.Runtime.CompilerServices;

namespace MagicUI.Core.Internal
{
    internal static class LogHelper
    {
        public static Loggable GetLogger([CallerFilePath] string callingFile = "")
        {
            return new SimpleLogger($"MagicUI.{Path.GetFileNameWithoutExtension(callingFile)}");
        }
    }
}
