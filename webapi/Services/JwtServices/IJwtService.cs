using URL_ShortenerAPI.Data.Auth;
using URL_ShortenerAPI.Data.Models;

namespace URL_ShortenerAPI.Services.JwtServices;

public interface IJwtService
{
    Task<TokenModel> GenerateJwtTokenAsync(User user);
}
