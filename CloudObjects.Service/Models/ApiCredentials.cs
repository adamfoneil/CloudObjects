namespace CloudObjects.Service.Models
{
    public class ApiCredentials
    {
        public ApiCredentials()
        {
        }

        public ApiCredentials(string accountName, string accountKey)
        {
            AccountName = accountName;
            AccountKey = accountKey;
        }

        public string AccountName { get; set; }
        public string AccountKey { get; set; }
    }
}
