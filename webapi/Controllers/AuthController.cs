using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URL_ShortenerAPI.Data.Auth;
using URL_ShortenerAPI.Services.AuthServices;

namespace URL_ShortenerAPI.Controllers;

public class AuthController : APIController
{
    readonly IAuthService authService;
    public AuthController(IAuthService authService)
        => this.authService = authService;

    IActionResult HandleAuthResult(AuthResult authResult)
        => authResult.Success ? Ok(authResult) : BadRequest(authResult);

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync(RegisterModel register)
    {
        if (register is null)
            return EntryIsNull("Register");

        var result = await authService.RegisterAsync(register);
        return HandleAuthResult(result);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync(LoginModel login)
    {
        if (login is null)
            return EntryIsNull("Login");

        var result = await authService.LoginAsync(login);
        return HandleAuthResult(result);
    }

    [Authorize]
    [HttpGet("Token")]
    public async Task<IActionResult> GetTokenAsync()
    {
        try
        {
            var token = await authService.GetTokenAsync();
            return Ok(token);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [Authorize]
    [HttpPost("Logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        try
        {
            await authService.LogoutAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to log out: {ex.Message}.");
        }
    }
}