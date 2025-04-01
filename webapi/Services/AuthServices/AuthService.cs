using Microsoft.AspNetCore.Identity;
using System.Data;
using URL_ShortenerAPI.Data;
using URL_ShortenerAPI.Data.Auth;
using URL_ShortenerAPI.Data.Models;
using URL_ShortenerAPI.Extentions;
using URL_ShortenerAPI.Repositories;
using URL_ShortenerAPI.Services.JwtServices;
using URL_ShortenerAPI.Validators;

namespace URL_ShortenerAPI.Services.AuthServices;

public class AuthService : IAuthService
{
    readonly SignInManager<User> signInManager;
    readonly UserRepository userRepository;
    readonly IHttpContextAccessor httpContextAccessor;
    readonly IJwtService jwtService;

    public AuthService(SignInManager<User> signInManager, UserRepository userRepository, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
    {
        this.signInManager = signInManager;
        this.userRepository = userRepository;
        this.jwtService = jwtService;
        this.httpContextAccessor = httpContextAccessor;
    }

    public virtual async Task<AuthResult> LoginAsync(LoginModel login)
    {
        ArgumentValidator.NotNull(nameof(login), login);

        var res = await signInManager.PasswordSignInAsync(login.Name, login.Password, login.RememberMe, false);

        if (!res.Succeeded)
            return AuthResult.Fail("Password or/and login invalid");

        try
        {
            var user = await userRepository.GetUserByUsernameAsync(login.Name);
            var token = await jwtService.GenerateJwtTokenAsync(user!);
            return AuthResult.Ok("Login successful", token);
        }
        catch
        {
            return AuthResult.Fail("Failed to log in...");
        }
    }


    public virtual async Task<AuthResult> RegisterAsync(RegisterModel register)
    {
        ArgumentValidator.NotNull(nameof(register), register);
 
        var existingUserByName = await userRepository.GetUserByUsernameAsync(register.Name);
        if (existingUserByName is not null)
            return AuthResult.Fail("Name already registered.");

        try
        {
            static string IdentityErrorsToString(IEnumerable<IdentityError> identityErrors)
                => string.Join("; ", identityErrors.Select(e => e.Description));

            User user = (User)register;
            user.Registered = DateTime.Now;

            IdentityResult result = await userRepository.CreateUserAsync(user, register.Password);
            if (!result.Succeeded)
                throw new Exception(IdentityErrorsToString(result.Errors));

            await signInManager.SignInAsync(user, register.RememberMe);

            string userRole = Roles.UserRole;

            var identityResult = await userRepository.AddRolesToUserAsync(user.Id, new[] { userRole });
            if (!identityResult.Succeeded)
                throw new Exception(IdentityErrorsToString(identityResult.Errors));

            var token = await jwtService.GenerateJwtTokenAsync(user);
            return AuthResult.Ok("Register successful", token);
        }
        catch
        {
            return AuthResult.Fail("Failed to register...");
        }
    }

    public virtual async Task LogoutAsync()
        => await signInManager.SignOutAsync();

    public virtual async Task<TokenModel> GetTokenAsync()
    {
        var userId = httpContextAccessor.GetUserId()!;
        var user = await userRepository.GetUserByIdAsync(userId);

        return await jwtService.GenerateJwtTokenAsync(user!);
    }
}
