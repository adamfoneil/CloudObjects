using System.Threading.Tasks;

namespace CloudObjects.Client.Interfaces
{
    public interface ITokenSaver
    {
        Task<string> GetAsync(string accountName);
        Task SaveAsync(string accountName, string token);
        Task DeleteAsync(string accountName);
    }
}
