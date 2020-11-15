using System.Threading.Tasks;
using TicketNotifier.Entities;

namespace TicketNotifier.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<T> GetByIdAsync<T>(string id) where T : class;
        Task UpsertUserAsync(User user);
    }
}