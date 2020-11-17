using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TicketNotifier.Contracts.Requests;
using TicketNotifier.Services.Interfaces;

namespace TicketNotifier.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpsertUser(UpsertUserRequest request)
        {
            var response = await _userService.UpsertUser(request);

            if (response.HasError)
            {
                return new BadRequestObjectResult(response);
            }

            return new OkObjectResult(response.Result);
        }
        
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteEvent(string id)
        {
            await _userService.DeleteUser(id);

            return new NoContentResult();
        }
    }
}