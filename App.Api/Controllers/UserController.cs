using App.Application.Dto;
using App.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("/api/[controller]")]
    public class UserController(IUserService userService) : BaseController
    {
        private readonly IUserService _userService = userService;

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserDto userDto)
        {
            return Ok(await _userService.CreateUserAsync(userDto));
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            return Ok(await _userService.GetAllAsync());
        }
    }
}
