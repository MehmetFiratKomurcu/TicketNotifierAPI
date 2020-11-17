using System.Threading.Tasks;
using TicketNotifier.Contracts.Requests;
using TicketNotifier.Entities;
using TicketNotifier.Models;
using TicketNotifier.Repositories.Interfaces;
using TicketNotifier.Services.Interfaces;

namespace TicketNotifier.Services.Implementations
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _userRepository.GetByIdAsync<User>(id);
        }

        public async Task<BaseResponse<User>> UpsertUser(UpsertUserRequest request)
        {
            var response = new BaseResponse<User>();
            if (request == null)
            {
                response.Message = "Invalid Request";
                return response;
            }

            var user = await _userRepository.GetByIdAsync<User>(request.Email);
            
            if (user == null)
            {
                var newUser = CreateUserWithUpsertUserRequest(request);
                await _userRepository.UpsertUserAsync(newUser);
            }
            else
            {
                foreach (var requestEvent in request.Events)
                {
                    user.Events.Add(requestEvent);
                }
                await _userRepository.UpsertUserAsync(user);
            }

            return response;
        }

        public async Task DeleteUser(string id)
        {
            await _userRepository.DeleteUser(id);
        }

        private static User CreateUserWithUpsertUserRequest(UpsertUserRequest request)
        {
            return new User
            {
                Email = request.Email,
                Events = request.Events,
                Name = request.Name,
                Type = "user"
            };
        }
    }
}