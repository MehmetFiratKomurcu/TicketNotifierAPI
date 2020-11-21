using System.Collections.Generic;
using System.Threading.Tasks;
using TicketNotifier.Entities;

namespace TicketNotifier.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<T> GetByIdAsync<T>(string id) where T : class;
        Task<List<User>> GetAllUsers();
        Task UpsertUserAsync(User user);
        Task DeleteUser(string id);
        Task<bool> UserExistsById(string id);
        Task AppendEventByUserId(string userId, List<Event> eventObject);
    }
}