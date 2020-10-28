using System.Collections.Generic;

namespace CloudObjects.Client.Static
{
    public enum HostLocations
    {
        Local,
        Online
    }

    internal static class Host
    {
        internal static Dictionary<HostLocations, string> Urls => new Dictionary<HostLocations, string>()
        {
            [HostLocations.Local] = "https://localhost:44328",
            [HostLocations.Online] = "https://cloudobjects.azurewebsites.net"
        };
    }
}
