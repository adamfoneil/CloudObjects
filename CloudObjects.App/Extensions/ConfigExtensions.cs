using Microsoft.Extensions.Configuration;

namespace CloudObjects.App.Extensions
{
    public static class ConfigExtensions
    {
        public static string TryConnections(this IConfiguration config, string liveSetting, string localSetting) =>
            config[liveSetting] ?? config.GetConnectionString(localSetting);
    }
}
