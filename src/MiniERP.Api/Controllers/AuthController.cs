
using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.Authentication;

namespace MiniERP.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Kullanıcı adı ve şifre zorunludur." });
        }

        var token = await _authService.LoginAsync(request);

        if (token is null)
        {
            return Unauthorized(new
            {
                message = "Kullanıcı adı veya şifre hatalı."
            });
        }

        return Ok(new
        {
            accessToken = token,
            tokenType = "Bearer"
        });
    }
}
