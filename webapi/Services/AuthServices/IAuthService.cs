using URL_ShortenerAPI.Data.Auth;

namespace URL_ShortenerAPI.Services.AuthServices;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginModel model);
    Task<AuthResult> RegisterAsync(RegisterModel model);
    Task<TokenModel> GetTokenAsync();
    Task LogoutAsync();
}
