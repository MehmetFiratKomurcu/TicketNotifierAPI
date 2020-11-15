using System.Threading.Tasks;
using TicketNotifier.Contracts.Requests;
using TicketNotifier.Entities;
using TicketNotifier.Models;

namespace TicketNotifier.Services.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<User>> UpsertUser(UpsertUserRequest request);
        Task<User> GetUserByIdAsync(string id);
    }
}