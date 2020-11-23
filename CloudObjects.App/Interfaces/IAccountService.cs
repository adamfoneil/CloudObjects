using System.Threading.Tasks;
using CloudObjects.Models;

namespace CloudObjects.App.Interfaces
{
    public interface IAccountService : IRepositoryService<Account, long>
    {
        Task<Account> CreateAsync(string name);
        Task<Account> RenameAsync(long id, string newName);
    }
}
