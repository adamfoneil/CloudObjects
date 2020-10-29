using Microsoft.Extensions.Configuration;

namespace CloudObjects.App.Extensions
{
    public static class ConfigExtensions
    {
        /// <summary>
        /// Tries to return a setting called liveSetting. If not found, returns connection string named localSetting
        /// Hack because https://github.com/MicrosoftDocs/azure-docs/issues/65237
        /// </summary>
        public static string TryConnections(this IConfiguration config, string liveSetting, string localSetting) =>
            config[liveSetting] ?? config.GetConnectionString(localSetting);
    }
}
