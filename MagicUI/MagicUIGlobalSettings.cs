using Modding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagicUI
{
    /// <summary>
    /// Global settings
    /// </summary>
    public class MagicUIGlobalSettings
    {
        /// <summary>
        /// Whether layout information should be logged (default false)
        /// </summary>
        public bool LogLayoutInformation { get; set; } = false;

        /// <summary>
        /// If layout information is to be logged (see <see cref="LogLayoutInformation"/>), the logging level that should be use (default <see cref="LogLevel.Fine"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel LogLevel { get; set; } = LogLevel.Fine;
    }
}
