using Blog.Core.DTOs.Auth;
using Blog.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiFuncional.Controllers
{
    [ApiController]
    [Route("api/conta")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("registrar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Registrar(RegisterUserDto registerUser)
        {
            var result = await _userService.RegisterUserAsync(registerUser);
            if (result.Result.Succeeded)
            {
                return Ok(new {result.Token });
            }

            return BadRequest("Falha ao registrar o usuário");
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Login(LoginUserDto loginUser)
        {
            try
            {
                var result = await _userService.ValidateUserAsync(loginUser);
                return Ok(result);
            }
            catch
            {
                return BadRequest("Usuário ou senha incorretos");
            }
        }
    }
}
