using System.Collections.Generic;

namespace CloudObjects.App.ViewModels
{
    public class HomeView
    {
        public string DbServerName { get; set; }
        public bool HasValidDb { get; set; }
        public bool IsLocal { get; set; }
        public IEnumerable<KeyValuePair<string, string>> ConfigData { get; set; }
    }
}
